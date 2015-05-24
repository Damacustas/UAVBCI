using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

using AR.Drone.Data;
using System.IO;

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

        public void EnqueuePacket(VideoPacket packet)
        {
            packetQueue.Enqueue(packet);
        }

        void ListenLoop()
        {
            server.Start();

            while (running)
            {
                var client = server.AcceptTcpClient();
                clients.Add(client);
            }
        }

        void MainLoop()
        {
            while (running)
            {
                VideoPacket packet;
                if (packetQueue.TryDequeue(out packet))
                {
                    var stream = new MemoryStream();
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(packet.Data.Length + 8 + 4 + 2 + 2 + 4);
                        writer.Write(packet.Timestamp); // 8
                        writer.Write(packet.FrameNumber); // 4
                        writer.Write(packet.Height); // 2
                        writer.Write(packet.Width); // 2
                        writer.Write((int)packet.FrameType); // 4
                        writer.Write(packet.Data); // rest
                    }

                    byte[] data = stream.ToArray();

                    foreach (var client in clients)
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                }
            }
        }
    }
}

