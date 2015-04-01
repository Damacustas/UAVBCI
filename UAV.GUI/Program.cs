using System;
using System.Windows.Forms;

namespace UAV.GUI
{
	class MainClass
	{
		[STAThread]
		public static void Main (string[] args)
		{
			Application.Run(new MainForm());
		}
	}
}
