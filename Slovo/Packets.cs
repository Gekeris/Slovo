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
		public bool GameRun;
	}
	
	[Serializable]
	class QuestionPacket
	{
		public string Word;
		public bool BColor;

		public QuestionPacket(string word)
		{
			Word = word;
		}

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
		public string Res;
		public string Referee;

		public MessagePacket(string word, bool res)
		{
			Word = word;
			Res = res ? "Accepted" : "Declined";
		}
		public string ToAutoComplete()
		{
			return $"{Word} - {Nick} [{Res} by {Referee}]";
		}
		public override string ToString()
		{
			return $"{Nick}: {Word} [{Res} by {Referee}]";
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
		public string Template;

		public StartPacket(int gamemode, string template)
		{
			GameMode = gamemode;
			Template = template;
		}
	}

	[Serializable]
	class HistoryPacket
	{
		public MessagePacket[] messagePackets;

		public HistoryPacket()
		{
		}

		public HistoryPacket(ListBox.ObjectCollection items)
		{
			messagePackets = new MessagePacket[items.Count];
			for(int i = 0; i < items.Count; i++)
				messagePackets[i] = (MessagePacket)items[i];
		}
	}
}
