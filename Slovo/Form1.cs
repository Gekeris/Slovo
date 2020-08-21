using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slovo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			PortTextBox.Text = "8000";
			NameTextBox.Text = "user";
			//RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(384);
			//var open = rsa.ExportParameters(true);
			//var close = rsa.ExportParameters(false);

		}

		private void HostButton_Click(object sender, EventArgs e)
		{
			NameTextBox.Text += "@HOST";
			NameTextBox.ReadOnly = true;
			NameTextBox.BackColor = Color.White;

			using (WebClient wc = new WebClient())
			{
				IpTextBox.Text = wc.DownloadString("https://api.myip.com/").Split('\"')[3];
			}
			IpTextBox.ReadOnly = true;
			IpTextBox.BackColor = Color.White;

			if (PortTextBox.Text == "")
				PortTextBox.Text = "8000";
			PortTextBox.ReadOnly = true;
			PortTextBox.BackColor = Color.White;

			JoinButton.Enabled = false;
			HostButton.Enabled = false;
		}

		private void PortTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && (e.KeyChar != 8))
				e.Handled = true;
			else
			{
				if ((e.KeyChar != 8) && (PortTextBox.Text.Length > 0) && (int.Parse(PortTextBox.Text + e.KeyChar) > 65535))
				{
					PortTextBox.Text = "65535";
					e.Handled = true;
					PortTextBox.SelectionStart = PortTextBox.Text.Length;
				}
			}
		}

		private void NameTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((((TextBox) sender).TextLength > 16) || (e.KeyChar == '@'))
				e.Handled = true;
		}
	}
}
