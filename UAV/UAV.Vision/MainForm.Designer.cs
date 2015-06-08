using System.ComponentModel;

namespace UAV.Vision
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.videoPictureBox = new System.Windows.Forms.PictureBox();
            this.videoTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // videoPictureBox
            // 
            this.videoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoPictureBox.Location = new System.Drawing.Point(0, 0);
            this.videoPictureBox.Name = "videoPictureBox";
            this.videoPictureBox.Size = new System.Drawing.Size(519, 561);
            this.videoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.videoPictureBox.TabIndex = 0;
            this.videoPictureBox.TabStop = false;
            // 
            // videoTimer
            // 
            this.videoTimer.Interval = 20;
            this.videoTimer.Tick += new System.EventHandler(this.VideoTimer_Tick);
            // 
            // MainForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 520);
            this.Controls.Add(this.videoPictureBox);
            this.Name = "MainForm2";
            this.Text = "UAV.Vision";
            this.FormClosing += Form_Closing;
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox videoPictureBox;
        private System.Windows.Forms.Timer videoTimer;
    }
}