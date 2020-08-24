using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slovo
{
	[Serializable]
	class UserLoginPacket
	{
		public string Nick;
		public string OpenSecret;
	}

	[Serializable]
	class ServerSecretPacket
	{
		public byte[] Secret;
		public Guid guid;
	}
	
	[Serializable]
	class QuestionPacket
	{
		public string Word;
		public bool BColor;

		public QuestionPacket(string word, bool bColor)
		{
			Word = word;
			BColor = bColor;
		}
	}
	

	[Serializable]
	class MessagePacket
	{
		public string Nick;
		public string Word;
		private string res;
		public string Res
		{
			get
			{
				return res;
			}
			set
			{
				res = bool.Parse(value) ? "Accepted" : "Declined";
			}
		}
		public string User;

		public MessagePacket(string word)
		{
			Word = word;
		}

		public MessagePacket(string nick, string word, bool res, string user)
		{
			Nick = nick;
			Word = word;
			Res = res.ToString();
			User = user;
		}

		public override string ToString()
		{
			return $"{Nick}: {Word} [{Res} by {User}]";
		}
	}

	[Serializable]
	class QueuePacket
	{
		public User[] Queue;

		public QueuePacket(ListBox.ObjectCollection queue)
		{
			Queue = new User[queue.Count];
			for(int i = 0; i < queue.Count; i++)
				Queue[i] = (User)queue[i];
		}
	}

	[Serializable]
	class StartPacket
	{
		public int GameMode;

		public StartPacket(int gamemode)
		{
			GameMode = gamemode;
		}
	}
}
