using System;
using System.Windows.Forms;
using AR.Drone.Client;
using AR.Drone.Video;
using AR.Drone.Data;
using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace UAV.GUI
{
	public class MainForm : Form
	{
		private PictureBox videoBox;
		private System.Windows.Forms.Timer videoTimer;
		private uint frameNumber;
		private Bitmap frameBitmap;
		private VideoFrame frame;
		private DroneClient drone;
		private VideoPacketDecoderWorker videoDecoder;

		public MainForm ()
		{
			InitializeComponent();

			videoDecoder = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnFrameDecoded);
			videoDecoder.UnhandledException += HandleUnhandledException;
			videoDecoder.Start();

			drone = new DroneClient();
			drone.VideoPacketAcquired += OnVideoPacketAcquired;
			drone.Start();
			drone.FlatTrim();

			Thread.Sleep(1000);

			SimpleFlightController controller = new SimpleFlightController();
			controller.Client = drone;
			controller.Start();

			videoTimer.Enabled = true;
		}

		void HandleUnhandledException (object arg1, Exception arg2)
		{
			Console.WriteLine(arg2);
		}

		private void OnVideoPacketAcquired(VideoPacket packet)
		{
			if (videoDecoder.IsAlive)
			{
				videoDecoder.EnqueuePacket(packet);
			}
		}

		private void OnFrameDecoded(VideoFrame frame)
		{
			this.frame = frame;
		}

		private void InitializeComponent()
		{
			this.Text = "UAV Control";
			this.KeyUp += MainForm_KeyUp;
			this.Size = new Size(800, 600);
			this.WindowState = FormWindowState.Maximized;

			videoBox = new PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
			videoBox.BackColor = System.Drawing.SystemColors.ControlDark;
			videoBox.SizeMode = PictureBoxSizeMode.Zoom;
			videoBox.Dock = DockStyle.Fill;
			((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();

			videoTimer = new System.Windows.Forms.Timer();
			videoTimer.Interval = 20;
			videoTimer.Tick += VideoTimer_Tick;

			this.Controls.Add(videoBox);
		}

		void VideoTimer_Tick (object sender, EventArgs e)
		{
			if (frame == null || frameNumber == frame.Number)
				return;
			frameNumber = frame.Number;

			if (frameBitmap == null)
				frameBitmap = VideoHelper.CreateBitmap(ref frame);
			else
				VideoHelper.UpdateBitmap(ref frameBitmap, ref frame);

			videoBox.Image = frameBitmap;
		}

		void MainForm_KeyUp (object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				drone.Land();
				drone.Stop();

				Application.Exit();
			}
		}
	}
}

