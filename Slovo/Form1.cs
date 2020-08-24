using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
		User clientUser;
		RSACng rsa;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			PortTextBox.Text = "8000";
			NameTextBox.Text = "user";
			IpTextBox.Text = "127.0.0.1";
			rsa = new RSACng(4096);
		}

		private async void JoinButton_Click(object sender, EventArgs e)
		{
			button1.Enabled = false;
			if (IPAddress.TryParse(IpTextBox.Text, out IPAddress ip))
			{
				clientUser = new User();
				clientUser.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				clientUser.socket.Connect(ip, int.Parse(PortTextBox.Text));

				string ServerResult = "";

				await Task.Run(() =>
				{
					using (Stream s = new NetworkStream(clientUser.socket))
					{
						StreamReader reader = new StreamReader(s);
						s.ReadTimeout = 60000;

						ServerResult = reader.ReadLine();
					}
				});

				if (ServerResult == "True")
				{
					UserLoginPacket ulp = new UserLoginPacket
					{
						Nick = NameTextBox.Text,
						OpenSecret = rsa.ToXmlString(false)
					};

					Task.Run(() => UserUpdate());
					clientUser.SendObject(ulp);
					ActiveControl = null;
					HostButton.Enabled = false;
					JoinButton.Enabled = false;
				}
				else
				{
					clientUser = null;
				}
			}
			else
			{
				await Task.Run(() =>
				{
					int i = 0;
					Action change = () =>
					{
						label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
						label1.ForeColor = Color.Red;
					};
					Action move = () =>
					{
						label1.Location = new Point(label1.Location.X + Convert.ToInt32(Math.Sin(i)), label1.Location.Y);
					};
					Action back = () =>
					{
						label1.Font = new Font("Microsoft Sans Serif", 8.25f);
						label1.Location = new Point(6, 60);
						label1.ForeColor = Color.Black;
					};
					Invoke(change);
					for (i = 0; i < 20; i++)
					{
						Invoke(move);
						Thread.Sleep(40);
					}
					Invoke(back);
				});
			}
		}

		private async Task UserUpdate()
		{
			while (true)
			{
				Thread.Sleep(10);
				await Task.Run(async () =>
				{
					object packet = await clientUser.RecieveObject();
					if (packet != null)
					{
						if (packet is MessagePacket mp)
						{
							Action action = () => label4.Text = Encoding.UTF8.GetString(mp.Message);
							Invoke(action);
						}
						if (packet is ServerSecretPacket ssp)
						{
							byte[] data = rsa.Decrypt(ssp.Secret, RSAEncryptionPadding.OaepSHA256);
							MemoryStream memory = new MemoryStream(data);
							memory.Position = 0;

							BinaryFormatter formatter = new BinaryFormatter();
							object serAes = formatter.Deserialize(memory);
							memory.Close();
							AesEncryption.Key = ((SerializableAes)serAes).GetAes();
						}
					}
				});
			}
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

			AesEncryption.Key = new AesCng();



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
				foreach (User user in users.ToList())
				{
					if (!user.ServerSocketConnected())
					{
						users.Remove(user);
						continue;
					}

					if (user.socket.Available != 0)
					{
						object obj = user.ServerReadObject();

						if (obj is UserLoginPacket ulp)
						{
							user.Nick = ulp.Nick;
							rsa.FromXmlString(ulp.OpenSecret);

							BinaryFormatter formatter = new BinaryFormatter();
							ServerSecretPacket ssp = new ServerSecretPacket();
							SerializableAes sa = new SerializableAes(AesEncryption.Key);
							using (MemoryStream stream = new MemoryStream())
							{
								formatter.Serialize(stream, sa);
								ssp.Secret = rsa.Encrypt(stream.ToArray(), RSAEncryptionPadding.OaepSHA256);
							}

							user.SendObject(ssp);
						}
					}
				}
			}
		}

		public async void ServerListen(Socket socket)
		{
			await Task.Run(() =>
			{
				while (true)
				{
					Thread.Sleep(10);
					if (socket.Poll(100000, SelectMode.SelectRead))
					{
						Socket newConnection = socket.Accept();
						if (newConnection != null)
						{
							DialogResult dr = MessageBox.Show
							(
								$"IP: {newConnection.RemoteEndPoint}",
								"New connection",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button2,
								MessageBoxOptions.DefaultDesktopOnly
							);
							User user = new User();
							user.ServerSetUserSocket(newConnection);
							if (dr == DialogResult.Yes)
							{
								user.ServerSendMessage("True");
								users.Add(user);
							}
							else
								user.ServerSendMessage("False");
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

		private void button1_Click(object sender, EventArgs e)
		{
			var h = Encoding.UTF8.GetBytes("Hello world!");
			MessagePacket mp = new MessagePacket(h);
			byte[] res = AesEncryption.Encryption(mp);
			label4.Text = res.Length.ToString();
			users[0].SendObject(res);
		}
	}
}
