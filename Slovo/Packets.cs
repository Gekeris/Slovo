using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}

	[Serializable]
	class MessagePacket
	{
		public byte[] Message;
		public MessagePacket(byte[] message)
		{
			Message = message;
		}
	}
}
