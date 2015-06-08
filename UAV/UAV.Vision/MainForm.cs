using System;
using System.Windows.Forms;
using AR.Drone.Video;
using System.Drawing;
using AR.Drone.Data;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace UAV.Vision
{
    public partial class MainForm : Form
    {
        // Video related members.
        Image frameBitmap;
        TcpClient videoClient;
        DateTime last;
        int bytes;
        bool running;

        public MainForm()
        {
            InitializeComponent();

            last = DateTime.UtcNow;
            bytes = 0;
            running = true;

            videoClient = new TcpClient("localhost", 1994);
            new Thread(VideoRecvLoop).Start();

            videoTimer.Enabled = true;
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
                    //Console.WriteLine("Read {0} bytes", total_size);
                    bytes += total_size + 4;

                    // Read data.
                    var databuf = ReadBytes(stream, total_size);

                    // Load image.
                    using (var imgstream = new MemoryStream(databuf))
                    {
                        Image bmp = Image.FromStream(imgstream);
                        frameBitmap = bmp;
                        //Console.WriteLine("Received image ({0}x{1}) at {2}.", bmp.Width, bmp.Height, DateTime.UtcNow.Second);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex.StackTrace);
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
    }
}
