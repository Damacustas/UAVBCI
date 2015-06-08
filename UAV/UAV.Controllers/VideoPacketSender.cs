using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

using AR.Drone.Data;
using System.IO;
using System;
using AR.Drone.Video;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace UAV.Controllers
{
    public class VideoPacketSender
    {
        Thread sendthread;
        bool running;
        readonly ConcurrentQueue<byte[]> packetQueue;
        readonly List<TcpClient> clients;
        TcpListener server;
        VideoPacketDecoderWorker videoDecoder;

        uint lastFrameNumber = 0;
        Bitmap bitmap;

        public VideoPacketSender()
        {
            packetQueue = new ConcurrentQueue<byte[]>();
            clients = new List<TcpClient>();
            server = new TcpListener(
                IPAddress.Parse("0.0.0.0"),
                1994
            );
        }

        public void Start()
        {
            Stop();


            // initialize video packet decoder.
            videoDecoder = new VideoPacketDecoderWorker(AR.Drone.Video.PixelFormat.BGR24, true, OnFrameDecoded);
            videoDecoder.UnhandledException += (delegate(object sender, Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.InnerException.StackTrace);
                Process.GetCurrentProcess().Kill();
            });
            videoDecoder.Start();

            running = true;
            sendthread = new Thread(MainLoop);
            sendthread.Start();

            new Thread(ListenLoop).Start();
        }

        public void EnqueuePacket(VideoPacket packet)
        {
            videoDecoder.EnqueuePacket(packet);
        }

        void OnFrameDecoded(VideoFrame frame)
        {
            if (frame == null || frame.Number == lastFrameNumber)
                return;

            if (frame.Number % 3 != 0)
                return;
            
            lastFrameNumber = frame.Number;

            // If we already have a bitmap, update it, else, create new one.
            if (bitmap == null)
                bitmap = VideoHelper.CreateBitmap(ref frame);
            else
                VideoHelper.UpdateBitmap(ref bitmap, ref frame);

            // Write frame to stream.
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                var data = stream.ToArray();
                packetQueue.Enqueue(data);
                Console.WriteLine("Enqueued frame {0} for sending.", frame.Number);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                running = false;
                server.Stop();
                sendthread.Join();
            }
        }

        public bool IsRunning
        {
            get
            {
                return running;
            }
        }

        void ListenLoop()
        {
            server.Start();

            while (running)
            {
                var client = server.AcceptTcpClient();
                lock (clients)
                {
                    clients.Add(client);
                }
                Console.WriteLine("Client connected from {0}.", client.Client.RemoteEndPoint);
            }
        }

        void MainLoop()
        {
            while (running)
            {
                byte[] packet;
                if (packetQueue.TryDequeue(out packet))
                {
                    if (clients.Count > 0)
                    {
                        //Console.WriteLine("Dequeued packet.");

//                    var stream = new MemoryStream();
//                    using (var writer = new BinaryWriter(stream))
//                    {
//                        writer.Write(packet.Data.Length + 8 + 4 + 2 + 2 + 4);
//                        writer.Write(packet.Timestamp); // 8
//                        writer.Write(packet.FrameNumber); // 4
//                        writer.Write(packet.Height); // 2
//                        writer.Write(packet.Width); // 2
//                        writer.Write((int)packet.FrameType); // 4
//                        writer.Write(packet.Data); // rest
//                    }
//
//                    byte[] data = stream.ToArray();

                        foreach (var client in clients)
                        {
                            try
                            {
                                byte[] len = BitConverter.GetBytes((int)packet.Length);
                                client.GetStream().Write(len, 0, len.Length);
                                client.GetStream().Write(packet, 0, packet.Length);
                            }
                            catch (Exception)
                            {
                                client.Close();
                                clients.Remove(client);
                            }
                        }

                        //Console.WriteLine("Packet sent. :D");
                    }
                }
            }
        }
    }
}

