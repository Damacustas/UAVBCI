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

        public MainForm()
        {
            InitializeComponent();

            videoClient = new TcpClient("145.116.165.243", 1994);
            new Thread(VideoRecvLoop).Start();

            videoTimer.Enabled = true;
        }

        void VideoTimer_Tick(object sender, EventArgs e)
        {
            videoPictureBox.Image = frameBitmap;
        }

        void VideoRecvLoop()
        {
            var stream = videoClient.GetStream();
            var memstream = new MemoryStream();

            while (true)
            {
                try
                {
                    // Read packet size.
                    var szbuf = new byte[4];
                    int read = stream.Read(szbuf, 0, 4);

                    if (read == 0)
                        continue;

                    int total_size = BitConverter.ToInt32(szbuf, 0);

                    // Read data.
                    read = (int)memstream.Length;
                    var buff = new byte[4096];
                    while (true)
                    {
                        int just_read = stream.Read(buff, 0, 4096);
                        memstream.Write(buff, 0, just_read);
                        read += just_read;
                        if (read >= total_size)
                            break;
                    }

                    // Copy back data we don't need.
                    var buff_bytes = memstream.ToArray();
                    memstream = new MemoryStream();
                    memstream.Write(buff_bytes, total_size, buff_bytes.Length - total_size);

                    // Save data we're gonna use.
                    var databuf = new byte[total_size];
                    Array.Copy(buff_bytes, databuf, total_size);

                    // Load image.
                    using (var imgstream = new MemoryStream(databuf))
                    {
                        Image bmp = Image.FromStream(imgstream);
                        frameBitmap = bmp;
                        Console.WriteLine("Received image ({0}x{1}) at {2}.", bmp.Width, bmp.Height, DateTime.UtcNow.Second);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
