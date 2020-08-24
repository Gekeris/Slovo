using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Slovo
{
	public class User
	{
		public string Nick;
		private Socket s;
		public Socket socket
		{
			get
			{
				return s;
			}
			set
			{
				s = value;
				s.SendBufferSize = 1000;
				s.ReceiveBufferSize = 1000;
			}
		}

		public void ServerSendMessage(string message)
		{
			using (Stream s = new NetworkStream(socket))
			{
				StreamWriter writer = new StreamWriter(s);
				writer.AutoFlush = true;

				writer.WriteLine(message);
			}
		}

		public void ServerSetUserSocket(Socket s)
		{
			socket = s;
		}

		// https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
		public bool ServerSocketConnected()
		{
			bool part1 = socket.Poll(1000, SelectMode.SelectRead);
			bool part2 = (socket.Available == 0);
			if (part1 && part2)
				return false;
			else
				return true;
		}

		public void SendObject(object obj)
		{
			using (Stream s = new NetworkStream(socket))
			{
				MemoryStream memory = new MemoryStream();
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memory, obj);
				byte[] newObj = memory.ToArray();
				memory.Position = 0;
				memory.Close();
				s.Write(newObj, 0, newObj.Length);
			}
		}

		public Task<object> RecieveObject()
		{
			return Task.Run(() =>
			{
				if (socket.Available == 0)
					return null;

				byte[] data = new byte[socket.ReceiveBufferSize];

				using (Stream s = new NetworkStream(socket))
					s.Read(data, 0, data.Length);
				MemoryStream memory = new MemoryStream(data);
				memory.Position = 0;

				BinaryFormatter formatter = new BinaryFormatter();
				object obj = formatter.Deserialize(memory);
				memory.Close();

				if (obj is byte[] byteObj)
					obj = AesEncryption.Decryption(byteObj);

				return obj;
			});
		}

		internal object ServerReadObject()
		{
			byte[] data = new byte[socket.ReceiveBufferSize];

			using (Stream s = new NetworkStream(socket))
			{
				s.Read(data, 0, data.Length);
				MemoryStream memory = new MemoryStream(data);
				memory.Position = 0;

				BinaryFormatter formatter = new BinaryFormatter();
				object obj = formatter.Deserialize(memory);
				memory.Close();
				return obj;
			}
		}
	}
}
