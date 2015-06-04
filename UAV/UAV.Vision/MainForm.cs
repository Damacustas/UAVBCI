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
        private uint frameNumber;
        private Bitmap frameBitmap;
        private VideoPacketDecoderWorker videoDecoder;
        private VideoFrame frame;
        private TcpClient videoClient;

        public MainForm()
        {
            InitializeComponent();

            // innitialize video packet decoder.
            videoDecoder = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnFrameDecoded);
            videoDecoder.UnhandledException += VideoDecoder_UnhandledException;
            videoDecoder.Start();

            videoClient = new TcpClient("145.116.156.243", 1994);
            new Thread(VideoRecvLoop).Start();

            videoTimer.Enabled = true;
        }

        private void OnFrameDecoded(VideoFrame frame)
        {
            this.frame = frame;
        }

        private void VideoDecoder_UnhandledException(object arg1, Exception arg2)
        {
            Console.WriteLine(arg2);
        }

        private void OnVideoPacketAqcuired(VideoPacket packet)
        {
            // If the video decoder is alive, enqueue the packet to be decoded.
            if (videoDecoder.IsAlive)
            {
                videoDecoder.EnqueuePacket(packet);
            }
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            // If there is no (new) frame, ignore.
            if (frame == null || frameNumber == frame.Number)
                return;

            frameNumber = frame.Number;

            // If we already have a bitmap, update it, else, create new one.
            if (frameBitmap == null)
                frameBitmap = VideoHelper.CreateBitmap(ref frame);
            else
                VideoHelper.UpdateBitmap(ref frameBitmap, ref frame);

            // Set image.
            videoPictureBox.Image = frameBitmap;
        }

        void VideoRecvLoop()
        {
            var stream = videoClient.GetStream();
            var memstream = new MemoryStream();

            while (true)
            {
				try{
                // Read packet size.
                var szbuf = new byte[4];
                int read = stream.Read(szbuf, 0, 4);

                if (read == 0)
                    continue;

                int total_size = BitConverter.ToInt32(szbuf, 0);

//                // Read total_size bytes.
//                read = 0;
//                var databuf = new byte[total_size];
//                var to_read_left = total_size;
//
//                while ((read += stream.Read(databuf, read, to_read_left < 4096 ? to_read_left : 4096)) < total_size)
//                {
//                    to_read_left -= read;
//                }
//
//                // Extract video data.
//                var videodata = new byte[total_size - 8 - 4 - 2 - 2 - 4];
//                Array.Copy(databuf, 8 + 4 + 2 + 2 + 4, videodata, 0, total_size - 8 - 4 - 2 - 2 - 4);

                // Read data.
                read = (int)memstream.Length;
                var buff = new byte[4096];
                while (true)
                {
					int just_read = stream.Read (buff, 0, 4096);
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

                // Extract video data.
                var videodata = new byte[total_size - 8 - 4 - 2 - 2 - 4];
                Array.Copy(databuf, 8 + 4 + 2 + 2 + 4, videodata, 0, total_size - 8 - 4 - 2 - 2 - 4);

                // Create videopacket.
                var packet = new VideoPacket()
                {
                    Timestamp = BitConverter.ToInt64(databuf, 0),
                    FrameNumber = BitConverter.ToUInt32(databuf, 8),
                    Height = BitConverter.ToUInt16(databuf, 12),
                    Width = BitConverter.ToUInt16(databuf, 14),
                    FrameType = (VideoFrameType)BitConverter.ToInt32(databuf, 16),
                    Data = videodata
                };

				Console.WriteLine ("OnVideoPacketAqcuired()");
                // Submit packet.
					OnVideoPacketAqcuired(packet);
				}
				catch(Exception)
				{
				}
            }
        }
    }
}
