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
							Action action = () =>
							{
								HistoryListBox.Items.Add(mp);
								MessageTextBox.AutoCompleteCustomSource.Add(mp.ToAutoComplete());
								if (!HistoryDict.ContainsKey(mp.Word))
									HistoryDict.Add(mp.Word, mp);
							};
							Invoke(action);
							SendButtonCheck();
						}
						else if (packet is QuestionPacket qup)
						{
							Action action = () =>
							{
								AcceptButton.Enabled = true;
								DeclineButton.Enabled = true;
								WordTextBox.Text = qup.Word;
								Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
								Graphics g = Graphics.FromImage(bmp);
								if (qup.BColor)
									g.Clear(Color.FromArgb(0, 255, 0));
								else
									g.Clear(Color.FromArgb(255, 0, 0));

								pictureBox1.Image = bmp;
								if (AutoAcceptCheckBox.Checked)
									AcceptButton_Click(null, null);
							};
							Invoke(action);
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
							Action action = () =>
							{
								StartButton_Click(null, null);
								GMComboBox.SelectedIndex = sp.GameMode;
								GMComboBox.Enabled = false;
								TemplateTextBox.Text = sp.Template;
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

							if (ssp.GameRun)
							{
								Action action = () =>
								{
									GMComboBox.Enabled = false;
									StartButton.Enabled = false;
									TemplateTextBox.ReadOnly = true;
									TemplateTextBox.BackColor = Color.White;
									AutoAcceptCheckBox.Enabled = true;
									GameStarted = true;
									SyncHistoryButton.Enabled = true;
									SendButtonCheck();
								};
								Invoke(action);
							}
						}
						else if (packet is HistoryPacket hp)
						{
							Action action = () =>
							{
								HistoryDict = new Dictionary<string, MessagePacket>();
								MessageTextBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
								HistoryListBox.Items.Clear();
								foreach (MessagePacket mpa in hp.messagePackets)
								{
									HistoryDict.Add(mpa.Word, mpa);
									MessageTextBox.AutoCompleteCustomSource.Add(mpa.ToAutoComplete());
									HistoryListBox.Items.Add(mpa);
								}
							};
							Invoke(action);
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
						ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
						continue;
					}

					if (user.socket.Available != 0)
					{
						object obj = user.ServerReadObject();

						if (obj is MessagePacket mp)
						{
							mp.Nick = ((User) ClientsList.Items[0]).Nick;
							mp.Referee = ((User) ClientsList.Items[1]).Nick;

							ServerSendAllUsersMessage(mp, mp.Res == "Accepted");
							SendButtonCheck();
						}
						else if (obj is QuestionPacket qp)
						{
							qp.BColor = !HistoryDict.ContainsKey(qp.Word);
							if (((User) ClientsList.Items[1]).guid == localUser.guid)
							{
								Action action = () =>
								{
									AcceptButton.Enabled = true;
									DeclineButton.Enabled = true;
									WordTextBox.Text = qp.Word;

									Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
									Graphics g = Graphics.FromImage(bmp);
									if (qp.BColor)
										g.Clear(Color.FromArgb(0, 255, 0));
									else
										g.Clear(Color.FromArgb(255, 0, 0));
									pictureBox1.Image = bmp;

									if (AutoAcceptCheckBox.Checked)
										AcceptButton_Click(null, null);
								};
								Invoke(action);
							}
							else
								((User) ClientsList.Items[1]).SendObject(AesEncryption.Encryption(qp));
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
								ssp.GameRun = GameStarted;
							}
							Action action = () =>
							{
								ClientsList.Items.Add(user);
							};
							Invoke(action);
							user.SendObject(ssp);
							Thread.Sleep(10);
							ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
						}
						else if (obj is HistoryPacket hp)
							user.SendObject(AesEncryption.Encryption(new HistoryPacket(HistoryListBox.Items)));
						else
						{
							throw new Exception("Server: New Packet " + obj.GetType());
						}
					}
				}
			}
		}

		private void ServerSendAllUsersMessage(MessagePacket mp, bool append)
		{
			ServerSendAllUsersPacket(mp);

			Action action = () =>
			{
				if (!HistoryDict.ContainsKey(mp.Word))
					HistoryDict.Add(mp.Word, mp);
				MessageTextBox.AutoCompleteCustomSource.Add(mp.ToAutoComplete());
				HistoryListBox.Items.Add(mp);
				if (append)
				{
					ClientsList.Items.Add(ClientsList.Items[0]);
					ClientsList.Items.RemoveAt(0);
				}
			};
			Invoke(action);
			if (append)
			{
				Thread.Sleep(100);
				ServerSendAllUsersPacket(new QueuePacket(ClientsList.Items));
				SendButtonCheck();
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

		private async void JoinButton_Click(object sender, EventArgs e)
		{
			IAmServer = false;
			HostButton.Enabled = false;
			JoinButton.Enabled = false;
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

					Task.Run(() => UserUpdate());

					localUser.SendObject(ulp);
					ActiveControl = null;
				}
				else
				{
					localUser = null;
					HostButton.Enabled = true;
					JoinButton.Enabled = true;
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

		private async void SendButton_Click(object sender, EventArgs e)
		{
			bool sent = false;
			bool find = false;
			bool correct = false;
			if ((GMComboBox.SelectedIndex == 0) && (HistoryListBox.Items.Count > 0))
			{
				for (int i = HistoryListBox.Items.Count - 1; i >= 0; i--)
				{
					if (((MessagePacket) HistoryListBox.Items[i]).Res == "Accepted")
					{
						find = true;
						string word = ((MessagePacket) HistoryListBox.Items[i]).Word;
						if (char.ToLower(MessageTextBox.Text.ToCharArray()[0]) == char.ToLower(word.ToCharArray()[word.Length - 1]))
							correct = true;
						break;
					}
				}
			}
			else if (GMComboBox.SelectedIndex == 1)
				correct = MessageTextBox.Text.StartsWith(TemplateTextBox.Text);
			else if (GMComboBox.SelectedIndex == 2)
				correct = MessageTextBox.Text.EndsWith(TemplateTextBox.Text);


			if ((MessageTextBox.TextLength > 0) && (correct || (!find && (GMComboBox.SelectedIndex == 0))))
			{
				if (!IAmServer)
					localUser.SendObject(AesEncryption.Encryption(new QuestionPacket(MessageTextBox.Text)));
				else
					((User) ClientsList.Items[1]).SendObject(AesEncryption.Encryption(new QuestionPacket(MessageTextBox.Text, !HistoryDict.ContainsKey(MessageTextBox.Text))));
				MessageTextBox.Text = "";
				sent = true;
			}
			if (!sent)
			{
				await Task.Run(() =>
				{
					int i = 0;
					Point p = new Point(0, 0);
					Action change = () =>
					{
						p = SendButton.Location;
						SendButton.BackColor = Color.Red;
						SendButton.FlatStyle = FlatStyle.Popup;
					};
					Action move = () =>
					{
						SendButton.Location = new Point(SendButton.Location.X + Convert.ToInt32(Math.Sin(i)), SendButton.Location.Y);
					};
					Action back = () =>
					{
						SendButton.BackColor = Color.FromArgb(225, 225, 225);
						SendButton.FlatStyle = FlatStyle.Standard;
						SendButton.Location = p;
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

		private void AcceptButton_Click(object sender, EventArgs e)
		{
			if (!IAmServer)
				localUser.SendObject(AesEncryption.Encryption(new MessagePacket(WordTextBox.Text, true)));
			else
			{
				MessagePacket mp = new MessagePacket(WordTextBox.Text, true);
				mp.Nick = ((User) ClientsList.Items[0]).Nick;
				mp.Referee = localUser.Nick;
				ServerSendAllUsersMessage(mp, true);
			}
			AcceptButton.Enabled = false;
			DeclineButton.Enabled = false;
			WordTextBox.Text = "";
			pictureBox1.Image = null;
		}

		private void DeclineButton_Click(object sender, EventArgs e)
		{
			if (!IAmServer)
				localUser.SendObject(AesEncryption.Encryption(new MessagePacket(WordTextBox.Text, false)));
			else
			{
				MessagePacket mp = new MessagePacket(WordTextBox.Text, false);
				mp.Nick = ((User) ClientsList.Items[0]).Nick;
				mp.Referee = localUser.Nick;
				ServerSendAllUsersMessage(mp, false);
			}
			AcceptButton.Enabled = false;
			DeclineButton.Enabled = false;
			WordTextBox.Text = "";
			pictureBox1.Image = null;
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

		private void StartButton_Click(object sender, EventArgs e)
		{
			if (ClientsList.Items.Count > 1)
			{
				GMComboBox.Enabled = false;
				TemplateTextBox.ReadOnly = true;
				TemplateTextBox.BackColor = Color.White;
				AutoAcceptCheckBox.Enabled = true;
				GameStarted = true;
				SyncHistoryButton.Enabled = true;
				SendButtonCheck();

				if (IAmServer)
				{
					StartButton.Enabled = false;
					ServerSendAllUsersPacket(new StartPacket(GMComboBox.SelectedIndex, TemplateTextBox.Text));
				}
			}
		}

		private void SyncHistoryButton_Click(object sender, EventArgs e)
		{
			if (IAmServer)
				ServerSendAllUsersPacket(new HistoryPacket(HistoryListBox.Items));
			else
				localUser.SendObject(new HistoryPacket());
		}

		private void RetryButton_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
		}

		private void GMComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			TemplateTextBox.Enabled = GMComboBox.SelectedIndex != 0;
		}

		private void MessageTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsLetter(e.KeyChar) && (e.KeyChar != 8))
				e.Handled = true;
		}

		private void NameTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (((((TextBox) sender).TextLength > 16) || (e.KeyChar == '@')) && (e.KeyChar != 8))
				e.Handled = true;
		}

		private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyValue == 13) && SendButton.Enabled)
				SendButton_Click(null, null);
		}
	}
}
