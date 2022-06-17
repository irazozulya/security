using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SecurityLabs
{
	class Program
	{
		static void Main(string[] args)
		{
			var passwords = new Dictionary<string, string>
			{
				{ "ADMIN", "password1" },
				{ "LENNY", "1234567890" },
				{ "IRYNA", "qwerty" },
				{ "DAVID", "poiuytrewq" },
				{ "DMYTRO", "pass123" },
				{ "OLEGSANDR", "0leg" },
				{ "ZOZULIA", "student" },
				{ "UKRAINE", "Europe" },
				{ "PHONE", "0987654321" },
				{ "BRAVO", "zaqxsw" },
				{ "LANA", "edcvfr" },
				{ "PARTY", "tgbnhy" },
				{ "DIPLOMA", "ujm,ki" },
				{ "MOUSE", "ol./;p" },
				{ "COMPUTER", "okmnji" },
				{ "AUTOTEST", "uhbvgy" },
				{ "CHATBOT", "tfcxdr" },
				{ "MANAGER", "eszaqw" },
				{ "IVORY", "zxcvbnm" },
				{ "DANIEL", "asdfghjkl" }
			};

			var i = 0;
			foreach (var pair in passwords)
			{
				var password = pair.Value;
				byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
				byte[] salt = getSalt(16);

				var secretStr = "hgSf5IUPpuebIRdEeJy8wYcWgU/41Gx/u192JB4GZcaC5OFM3GcoC+JaZdoVV0ZVKVZQFLqLHExW3XHQzv45qCRY6UZY5NjO4yp78swCBD48yQonvBIuTf3UP15VNWBiTLCiZzhGKPHV1rkoiZfyw6KRx29BVF9uB6i3RZEQ4rrmEHIxCc+YfbwkIniRSPhFkD5ZGbYOs4s2Ky406FQDiU4ZJ7DXFarzUjx7Rmp1oYytvghR3hnSCI8aJ9DibvbQKJ4tuaLY/bPj7wtDRVYE1GBTRSRegeo4wGGo8+zBmeiXlF+fHUz9QEdbznuLEarh1ZY9ca/KWfbv8Hds5A4=";
				var secret = Convert.FromBase64String(secretStr);

				var config = new Argon2Config
				{
					Type = Argon2Type.DataIndependentAddressing,
					Version = Argon2Version.Nineteen,
					TimeCost = 10,
					MemoryCost = 32768,
					Lanes = 5,
					Threads = Environment.ProcessorCount,
					Password = passwordBytes,
					Salt = salt,
					Secret = secret,
					HashLength = 20
				};
				var argon2A = new Argon2(config);
				string hashString;
				using (SecureArray<byte> hashA = argon2A.Hash())
				{
					hashString = config.EncodeString(hashA.Buffer);
				}

				if (Argon2.Verify(hashString, config))
				{
					Console.WriteLine($@"{i}. {"{"} ""{pair.Key}"", ""{hashString}"" {"},"} {password} - {Convert.ToBase64String(salt)}");
				}
				i++;
			}
			//new Task3().Exec();
		}

		private static byte[] getSalt(int length)
		{
			var salt = new byte[length];
			using (var random = new RNGCryptoServiceProvider())
			{
				random.GetNonZeroBytes(salt);
			}

			return salt;
		}


		private static string ToHex(byte[] bytes)
		{
			return string.Join("", bytes.Select(x => string.Format("{0:x2}", x)).ToArray());
		}
	}
}
