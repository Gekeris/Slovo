using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slovo
{
	public partial class Form1 : Form
	{
		List<User> users;
		static RSACng rsa;
		public static string XmlRsaParam;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			PortTextBox.Text = "8000";
			NameTextBox.Text = "user";
			rsa = new RSACng(4096);
		}

		private async void HostButton_Click(object sender, EventArgs e)
		{
			ActiveControl = null;
			HostButton.Enabled = false;
			JoinButton.Enabled = false;
			
			NameTextBox.Text += "@HOST";
			NameTextBox.ReadOnly = true;
			NameTextBox.BackColor = Color.White;

			using (WebClient wc = new WebClient())
				IpTextBox.Text = wc.DownloadString("https://api.myip.com/").Split('\"')[3];
			IpTextBox.ReadOnly = true;
			IpTextBox.BackColor = Color.White;

			if (PortTextBox.Text == "")
				PortTextBox.Text = "8000";
			PortTextBox.ReadOnly = true;
			PortTextBox.BackColor = Color.White;

			XmlRsaParam = rsa.ToXmlString(false);



			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(PortTextBox.Text)));
			socket.Listen(10);
			socket.ReceiveTimeout = 10000;
			users = new List<User>();

			ServerListen(socket);
			await Task.Run(() => ServerMonitor());
			socket.Close();
		}

		private Task ServerMonitor()
		{
			while (true)
			{
				Thread.Sleep(10);
				foreach (User user in users)
				{
					if (!user.ServerSocketConnected())
					{
						users.Remove(user);
						continue;
					}

					if (user.socket.Available != 0)
					{
						///////////////////////////////////////////////////////////////////////////////////
					}
				}
			}
		}

		public async void ServerListen(Socket socket)
		{
			await Task.Run(async () =>
			{
				while (true)
				{
					Thread.Sleep(10);
					if (socket.Poll(100000, SelectMode.SelectRead))
					{
						Socket newConnection = socket.Accept();
						if (newConnection != null)
						{
							User user = new User();
							string newGuid = await user.ServerCreateGuidParam(newConnection);
							user.ServerSendMessage(newGuid);
							users.Add(user);
						}
					}
				}
			});
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

		private void Form1_Click(object sender, EventArgs e)
		{
			ActiveControl = null;
		}
	}
}
