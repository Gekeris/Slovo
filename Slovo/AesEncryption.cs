using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slovo
{
	static class AesEncryption
	{
		static private AesCng key;
		static private ICryptoTransform Encr;
		static private ICryptoTransform Decr;
		static public AesCng Key 
		{ 
			get
			{
				return key;
			}
			set
			{
				key = value;
				Encr = key.CreateEncryptor();
				Decr = key.CreateDecryptor();
			}
		}

		static public byte[] Encryption(object obj)
		{
			if (Key == null)
				throw new Exception("AesEncryption key is null");

			byte[] ret;
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, obj);
				ret = Encr.TransformFinalBlock(stream.ToArray(), 0, stream.ToArray().Length);
			}
			return ret;
		}

		static public object Decryption(byte[] data)
		{
			if (Key == null)
				throw new Exception("AesEncryption key is null");

			MemoryStream memory = new MemoryStream(Decr.TransformFinalBlock(data, 0, data.Length));
			memory.Position = 0;

			BinaryFormatter formatter = new BinaryFormatter();
			object obj = formatter.Deserialize(memory);
			memory.Close();
			return obj;
		}
	}
}
