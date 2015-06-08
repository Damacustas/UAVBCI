using System;
using System.Windows.Forms;
using AR.Drone.Video;
using System.Drawing;
using AR.Drone.Data;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace UAV.Vision
{
    public partial class MainForm : Form
    {
        // Video related members.
        Bitmap frameBitmap;
        TcpClient videoClient;
        DateTime last;
        int bytes;
        bool running;
        VideoPacketDecoderWorker videoDecoder;
        uint lastFrameNumber;

        public MainForm()
        {
            InitializeComponent();

            last = DateTime.UtcNow;
            bytes = 0;
            running = true;

            // initialize video packet decoder.
            videoDecoder = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnFrameDecoded);
            videoDecoder.UnhandledException += (delegate(object sender, Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.InnerException.StackTrace);
                Process.GetCurrentProcess().Kill();
            });
            videoDecoder.Start();

            videoClient = new TcpClient(Environment.GetCommandLineArgs()[1], 1994);
            new Thread(VideoRecvLoop).Start();

            videoTimer.Enabled = true;
        }


        void OnFrameDecoded(VideoFrame frame)
        {
            //Console.WriteLine("On frame decoded.");

            if (frame == null || frame.Number == lastFrameNumber)
                return;

            lastFrameNumber = frame.Number;

            // If we already have a bitmap, update it, else, create new one.
            if (frameBitmap == null)
                frameBitmap = VideoHelper.CreateBitmap(ref frame);
            else
                VideoHelper.UpdateBitmap(ref frameBitmap, ref frame);
        }

        void VideoTimer_Tick(object sender, EventArgs e)
        {
            videoPictureBox.Image = frameBitmap;

            if (DateTime.UtcNow - last >= new TimeSpan(0, 0, 1)) // if time passed is >= 1 sec.
            {
                Console.WriteLine("Bitrate: {0} MB/s", (double)bytes / (1024.0 * 1024));
                last = DateTime.UtcNow;
                bytes = 0;
            }
        }

        void Form_Closing(object sender, EventArgs e)
        {
            running = false;
        }

        void VideoRecvLoop()
        {
            var stream = videoClient.GetStream();

            while (running)
            {
                try
                {
                    // Read packet size.
                    var szbuf = ReadBytes(stream, 4);
                    int total_size = BitConverter.ToInt32(szbuf, 0);
                    //Console.WriteLine("total_size={0}", total_size);
                    bytes += total_size;

                    // Read data.
                    var databuf = ReadBytes(stream, total_size);
                    var packet = ConvertVideoPacket(databuf);

                    videoDecoder.EnqueuePacket(packet);

//                    // Load image.
//                    using (var imgstream = new MemoryStream(databuf))
//                    {
//                        Image bmp = Image.FromStream(imgstream);
//                        frameBitmap = bmp;
//                        //Console.WriteLine("Received image ({0}x{1}) at {2}.", bmp.Width, bmp.Height, DateTime.UtcNow.Second);
//                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static byte[] ReadBytes(Stream stream, int count)
        {
            var buffer = new byte[count];

            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                    throw new EndOfStreamException();
                offset += read;
            }

            return buffer;
        }

        VideoPacket ConvertVideoPacket(byte[] data)
        {
            long timestamp = BitConverter.ToInt64(data, 0);
            uint framenumber = BitConverter.ToUInt32(data, 8);
            ushort height = BitConverter.ToUInt16(data, 8 + 4);
            ushort width = BitConverter.ToUInt16(data, 8 + 4 + 2);
            VideoFrameType ft = (VideoFrameType)BitConverter.ToInt32(data, 8 + 4 + 2 + 2);

            var viddata = new byte[data.Length - (8 + 4 + 2 + 2 + 4)];
            Array.Copy(data, 8 + 4 + 2 + 2 + 4, viddata, 0, viddata.Length);


            var packet = new VideoPacket
            {
                Timestamp = timestamp,
                FrameNumber = framenumber,
                Height = height,
                Width = width,
                FrameType = ft,
                Data = viddata
            };
            //Console.WriteLine("{0}: {1} viddata bytes.", packet.FrameNumber, packet.Data.Length);

            return packet;
        }
    }
}
