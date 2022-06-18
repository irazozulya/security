using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityLabs
{
	class Program
	{
		private static readonly Dictionary<string, string> _passwordSalts = new Dictionary<string, string>
		{
			{ "ADMIN@GMAIL.COM", "0639311110" }, //0639311110 ADMIN@GMAIL.COM
			{ "LENNY@GMAIL.COM", "0639311111" }, //0639311111 LENNY@GMAIL.COM
			{ "IRYNA@GMAIL.COM", "0639311112" }, //0639311112 IRYNA@GMAIL.COM
			{ "DAVID@GMAIL.COM", "0639311113" }, //0639311113 DAVID@GMAIL.COM
			{ "DMYTRO@GMAIL.COM", "0639311114" }, //0639311114 DMYTRO@GMAIL.COM
			{ "OLEGSANDR@GMAIL.COM", "0639311115" }, //0639311115 OLEGSANDR@GMAIL.COM
			{ "ZOZULIA@GMAIL.COM", "0639311116" }, //0639311116 ZOZULIA@GMAIL.COM
			{ "UKRAINE@GMAIL.COM", "0639311117" }, //0639311117 UKRAINE@GMAIL.COM
			{ "PHONE@GMAIL.COM", "0639311118" }, //0639311118 PHONE@GMAIL.COM
			{ "BRAVO@GMAIL.COM", "0639311119" }, //0639311119 BRAVO@GMAIL.COM
			{ "LANA@GMAIL.COM", "0639311120" }, //0639311120 LANA@GMAIL.COM
			{ "PARTY@GMAIL.COM", "0639311121" }, //0639311121 PARTY@GMAIL.COM
			{ "DIPLOMA@GMAIL.COM", "0639311122" }, //0639311122 DIPLOMA@GMAIL.COM
			{ "MOUSE@GMAIL.COM", "0639311123" }, //0639311123 MOUSE@GMAIL.COM
			{ "COMPUTER@GMAIL.COM", "0639311124" }, //0639311124 COMPUTER@GMAIL.COM
			{ "AUTOTEST@GMAIL.COM", "0639311125" }, //0639311125 AUTOTEST@GMAIL.COM
			{ "CHATBOT@GMAIL.COM", "0639311126" }, //0639311126 CHATBOT@GMAIL.COM
			{ "MANAGER@GMAIL.COM", "0639311127" }, //0639311127 MANAGER@GMAIL.COM
			{ "IVORY@GMAIL.COM", "0639311128" }, //0639311128 IVORY@GMAIL.COM
			{ "DANIEL@GMAIL.COM", "0639311129" }, //0639311129 DANIEL@GMAIL.COM
		};
		static void Main(string[] args)
		{
			Chilkat.Crypt2 crypt = new Chilkat.Crypt2
			{
				CryptAlgorithm = "aes",
				CipherMode = "gcm",
				KeyLength = 128,
				EncodingMode = "hex"
			};
			
			string hexKey = "feffe9928665731c6d6a8f9467308308";// 128-bit secret key

			string initializationVector = "cafebabefacedbaddecaf888";

			// Set the secret key and IV
			crypt.SetEncodedIV(initializationVector, "hex");
			crypt.SetEncodedKey(hexKey, "hex");

			foreach (var pair in _passwordSalts)
			{
				string plainText = BitConverter.ToString(Encoding.UTF8.GetBytes("Hello world")).Replace("-", "");
				string cipherText = crypt.EncryptEncoded(plainText);
				if (crypt.LastMethodSuccess != true)
				{
					return;
				}

				string authenticationTag = crypt.GetEncodedAuthTag("hex");

				Console.WriteLine($@"{"{ "} ""{pair.Key.GetHashCode()}"", ""{cipherText}"" {"}"}, //{pair.Value} {pair.Key}");
				Console.WriteLine($@"{"{ "} ""{pair.Key.GetHashCode()}"", ""{authenticationTag}"" {"}"}, //{pair.Value} {pair.Key}");
			}

			/*string plainText = BitConverter.ToString(Encoding.UTF8.GetBytes("Hello world")).Replace("-", "");
			string cipherText = crypt.EncryptEncoded(plainText);
			if (crypt.LastMethodSuccess != true)
			{
				return;
			}

			string authenticationTag = crypt.GetEncodedAuthTag("hex");
			crypt.SetEncodedAuthTag(authenticationTag, "hex");

			string decodedPlainText = Task1.ConvertHexToAscii(crypt.DecryptEncoded(cipherText));*/

			//new Task3().Exec();
		}
	}
}
