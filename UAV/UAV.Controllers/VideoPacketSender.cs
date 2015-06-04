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


            // innitialize video packet decoder.
            videoDecoder = new VideoPacketDecoderWorker(AR.Drone.Video.PixelFormat.BGR24, true, OnFrameDecoded);
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
            if (frame.Number == lastFrameNumber)
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
                bitmap.Save(stream, ImageFormat.Png);
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
                clients.Add(client);
                Console.WriteLine("Client connected.");
            }
        }

        void MainLoop()
        {
            while (running)
            {
                byte[] packet;
                if (packetQueue.TryDequeue(out packet))
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
                        byte[] len = BitConverter.GetBytes((int)packet.Length);
                        client.GetStream().Write(len, 0, len.Length);
                        client.GetStream().Write(packet, 0, packet.Length);
                    }

                    //Console.WriteLine("Packet sent. :D");
                }
            }
        }
    }
}

