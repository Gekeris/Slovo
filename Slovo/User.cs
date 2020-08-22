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

namespace Slovo
{
	public class User
	{
		public Guid guid;
		public Socket socket;

		public void ServerSendMessage(string message)
		{
			using (Stream s = new NetworkStream(socket))
			{
				StreamWriter writer = new StreamWriter(s);
				writer.AutoFlush = true;

				writer.WriteLine(message);
			}
		}

		public Task<string> ServerCreateGuidParam(Socket s)
		{
			socket = s;
			return Task.Run(() =>
			{
				guid = Guid.NewGuid();
				return $"{guid}@{Form1.XmlRsaParam}";
			});
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
	}
}
