using System;
using System.Security.Cryptography;

namespace Slovo
{
	[Serializable]
	class SerializableAes
	{
		byte[] IV;
		byte[] Key;
		int KeySize;
		CipherMode Mode;
		PaddingMode Padding;

		public SerializableAes(AesCng aes)
		{
			IV = aes.IV;
			Key = aes.Key;
			KeySize = aes.KeySize;
			Mode = aes.Mode;
			Padding = aes.Padding;
		}

		public AesCng GetAes()
		{
			AesCng aes = new AesCng();
			aes.KeySize = KeySize;
			aes.Mode = Mode;
			aes.Padding = Padding;
			aes.IV = IV;
			aes.Key = Key;
			return aes;
		}
	}
}