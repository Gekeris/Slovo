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
		Dictionary<string, MessagePacket> HistoryDict = new Dictionary<string, MessagePacket>();
		User localUser;
		RSACng rsa;
		bool IAmServer;
		bool GameStarted;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			WordTextBox.BackColor = Color.White;
			GMComboBox.SelectedIndex = 0;
			PortTextBox.Text = "8000";
			NameTextBox.Text = "user";
			IpTextBox.Text = "127.0.0.1";
			rsa = new RSACng(4096);
		}

		private async void JoinButton_Click(object sender, EventArgs e)
		{
			IAmServer = false;
			if (IPAddress.TryParse(IpTextBox.Text, out IPAddress ip))
			{
				localUser = new User();
				localUser.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				localUser.socket.Connect(ip, int.Parse(PortTextBox.Text));

				string ServerResult = "";

				await Task.Run(() =>
				{
					using (Stream s = new NetworkStream(localUser.socket))
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

					#pragma warning disable CS4014
					Task.Run(() => UserUpdate());
					#pragma warning restore CS4014
					localUser.SendObject(ulp);
					ActiveControl = null;
					HostButton.Enabled = false;
					JoinButton.Enabled = false;
				}
				else
				{
					localUser = null;
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
					object packet = await localUser.RecieveObject();
					if (packet != null)
					{
						if (packet is MessagePacket mp)
						{
							HistoryListBox.Items.Add(mp);
							MessageTextBox.AutoCompleteCustomSource.Add($"{mp.Word} [{mp.Res} by {mp.User}]");
						}
						else if (packet is QuestionPacket qup)
						{
							WordTextBox.Text = qup.Word;
							Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
							Graphics g = Graphics.FromImage(bmp);
							if (qup.BColor)
								g.Clear(Color.FromArgb(0, 255, 0));
							else
								g.Clear(Color.FromArgb(255, 0, 0));

							pictureBox1.Image = bmp;
						}
						else if (packet is QueuePacket qp)
						{
							Action action = () =>
							{
								ClientsList.Items.Clear();
								ClientsList.Items.AddRange(qp.Queue);
							};
							Invoke(action);
							await QueueId();
						}
						else if(packet is StartPacket sp)
						{
							GameStarted = true;
							Action action = () =>
							{
								StartButton_Click(null, null);
								GMComboBox.SelectedIndex = sp.GameMode;
								GMComboBox.Enabled = false;
								StartButton.Enabled = false;
							};
							Invoke(action);
							SendButtonCheck();
						}
						else if (packet is ServerSecretPacket ssp)
						{
							localUser.guid = ssp.guid;
							byte[] data = rsa.Decrypt(ssp.Secret, RSAEncryptionPadding.OaepSHA256);
							MemoryStream memory = new MemoryStream(data);
							memory.Position = 0;

							BinaryFormatter formatter = new BinaryFormatter();
							object serAes = formatter.Deserialize(memory);
							memory.Close();
							AesEncryption.Key = ((SerializableAes)serAes).GetAes();
						}
						else
						{
							MessageBox.Show
							(
								"Client New Packet " + packet.GetType(),
								"New Packet",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning,
								MessageBoxDefaultButton.Button1,
								MessageBoxOptions.DefaultDesktopOnly
							);
						}
					}
				});
			}
		}

		private Task QueueId()
		{
			return Task.Run(() => 
			{
				SendButtonCheck();
				foreach (User u in ClientsList.Items)
				{
					if (u.guid == localUser.guid)
					{
						Action action = () =>
						{
							ClientsList.SelectedItem = u;
							int index = ClientsList.SelectedIndex;
							ClientsList.Items.RemoveAt(index);
							u.Nick += "@YOU";
							ClientsList.Items.Insert(index, u);
							ClientsList.SelectedItem = u;
						};
						Invoke(action);
						break;
					}
				}
			});
		}

		private async void HostButton_Click(object sender, EventArgs e)
		{
			IAmServer = true;
			ActiveControl = null;
			HostButton.Enabled = false;
			JoinButton.Enabled = false;
			label4.Enabled = true;
			GMComboBox.Enabled = true;
			StartButton.Enabled = true;

			NameTextBox.ReadOnly = true;
			NameTextBox.BackColor = Color.White;
			localUser = new User();
			localUser.Nick = NameTextBox.Text + "@HOST";
			ClientsList.Items.Add(localUser);

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
						Action action = () =>
						{
							ClientsList.Items.Remove(user);
						};
						Invoke(action);
						continue;
					}

					if (user.socket.Available != 0)
					{
						object obj = user.ServerReadObject();

						if (obj is MessagePacket mp)
						{
							mp.Nick = user.Nick;
							mp.User = ((User) ClientsList.Items[1]).Nick;
							((User) ClientsList.Items[1]).SendObject(AesEncryption.Encryption(new QuestionPacket(mp.Word, HistoryDict.ContainsKey(mp.Word))));
							string asd = ((User) ClientsList.Items[1]).RecieveObject().ToString();
							mp.Res = ((User) ClientsList.Items[1]).RecieveObject().ToString();
							var a = 1;
						}
						else if (obj is UserLoginPacket ulp)
						{
							user.Nick = ulp.Nick;
							user.guid = Guid.NewGuid();
							rsa.FromXmlString(ulp.OpenSecret);

							BinaryFormatter formatter = new BinaryFormatter();
							ServerSecretPacket ssp = new ServerSecretPacket();
							SerializableAes sa = new SerializableAes(AesEncryption.Key);
							using (MemoryStream stream = new MemoryStream())
							{
								formatter.Serialize(stream, sa);
								ssp.Secret = rsa.Encrypt(stream.ToArray(), RSAEncryptionPadding.OaepSHA256);
								ssp.guid = user.guid;
							}
							Action action = () =>
							{
								ClientsList.Items.Add(user);
							};
							Invoke(action);
							user.SendObject(ssp);
							Thread.Sleep(500);
							ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
						}
						else
						{
							throw new Exception("Server: New Packet " + obj.GetType());
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

		private async void ServerSendAllUsersPacket(object obj)
		{
			byte[] encrypted = AesEncryption.Encryption(obj);
			await Task.Run(() => 
			{
				foreach (User user in users)
				{
					user.SendObject(encrypted);
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

		private void GMComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void Form1_Click(object sender, EventArgs e)
		{
			ActiveControl = null;
		}

		private void ClientsList_KeyDown(object sender, KeyEventArgs e)
		{
			if (ClientsList.SelectedIndex == -1)
				return;
			e.Handled = true;
			if ((ClientsList.SelectedIndex > 0) && (e.KeyData.ToString() == "Up") && IAmServer)
			{
				User user = (User) ClientsList.Items[ClientsList.SelectedIndex - 1];
				ClientsList.Items.RemoveAt(ClientsList.SelectedIndex - 1);
				ClientsList.Items.Insert(ClientsList.SelectedIndex + 1, user);
				SendButtonCheck();
				ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
				Thread.Sleep(10);
			}
			else if ((ClientsList.SelectedIndex < ClientsList.Items.Count - 1) && (e.KeyData.ToString() == "Down") && IAmServer)
			{
				User user = (User) ClientsList.Items[ClientsList.SelectedIndex + 1];
				ClientsList.Items.RemoveAt(ClientsList.SelectedIndex + 1);
				ClientsList.Items.Insert(ClientsList.SelectedIndex, user);
				SendButtonCheck();
				ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
				Thread.Sleep(10);
			}
			else if (e.KeyData.ToString() == "Escape")
				ClientsList.SelectedIndex = -1;
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			ClientsList.Height = Height - 194;
		}



		private void StartButton_Click(object sender, EventArgs e)
		{
			HistoryListBox.Items.Clear();
			MessageTextBox.Text = "";
			WordTextBox.Text = "";

			if (IAmServer)
			{
				StartButton.Text = "Restart";

				ServerSendAllUsersPacket(new StartPacket(GMComboBox.SelectedIndex));
			}
		}

		private void SendButtonCheck()
		{
			Action action = () =>
			{
				if ((ClientsList.Items.Count > 0) && (((User) ClientsList.Items[0]).guid == localUser.guid) && GameStarted)
					SendButton.Enabled = true;
				else
					SendButton.Enabled = false;
			};
			Invoke(action);
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			localUser.SendObject(AesEncryption.Encryption(new MessagePacket(MessageTextBox.Text)));
		}
	}
}
