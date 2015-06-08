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
        readonly ConcurrentQueue<VideoPacket> packetQueue;
        readonly List<TcpClient> clients;
        TcpListener server;

        public VideoPacketSender()
        {
            packetQueue = new ConcurrentQueue<VideoPacket>();
            clients = new List<TcpClient>();
            server = new TcpListener(
                IPAddress.Parse("0.0.0.0"),
                1994
            );
        }

        public void Start()
        {
            Stop();

            running = true;
            sendthread = new Thread(MainLoop);
            sendthread.Start();

            new Thread(ListenLoop).Start();
        }

        public void EnqueuePacket(VideoPacket packet)
        {
            packetQueue.Enqueue(packet);
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
                VideoPacket packet;
                if (packetQueue.TryDequeue(out packet))
                {
                    //Console.WriteLine("Dequeued");
                    if (clients.Count > 0)
                    {

                        foreach (var client in clients)
                        {
                            var client_stream = client.GetStream();
                            using (var stream = new MemoryStream(packet.Data.Length + 8 + 4 + 2 + 2 + 4))
                            {
                                byte[] temp;

                                // Write members
                                temp = BitConverter.GetBytes(packet.Timestamp);
                                stream.Write(temp, 0, 8);

                                temp = BitConverter.GetBytes(packet.FrameNumber);
                                stream.Write(temp, 0, 4);

                                temp = BitConverter.GetBytes(packet.Height);
                                stream.Write(temp, 0, 2);

                                temp = BitConverter.GetBytes(packet.Width);
                                stream.Write(temp, 0, 2);

                                temp = BitConverter.GetBytes((int)packet.FrameType);
                                stream.Write(temp, 0, 4);

                                // Write video data.
                                stream.Write(packet.Data, 0, packet.Data.Length);

                                // Extract data to send.
                                var data = stream.ToArray();
                                if (data.Length != packet.Data.Length + 8 + 4 + 2 + 2 + 4)
                                    Debugger.Break();

                                // Write total length.
                                temp = BitConverter.GetBytes((int)data.Length);
                                client_stream.Write(temp, 0, 4);

                                // Transmit data.
                                client_stream.Write(data, 0, data.Length);
                            }
                        }
                    }
                }
            }
        }
    }
}

