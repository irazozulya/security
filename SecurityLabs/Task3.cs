using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SecurityLabs
{
	public static class Task3
	{
		private static int _accountCode = 5100;

		private static double _a, _c;

		public static void Exec()
		{
			getLcgNextNumber();
		}

		private static void getLcgNextNumber()
		{
			/*var account = JsonConvert.DeserializeObject<Account>(CreateNewAccount());

			Console.WriteLine($"{account.Money}");*/

			var response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));
			
			var firstElement = BitConverter.ToUInt32(BitConverter.GetBytes(response.RealNumber), 0); //Next(1);

			response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));

			var secondElement = BitConverter.ToUInt32(BitConverter.GetBytes(response.RealNumber), 0);

			response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));

			var thirdElement = BitConverter.ToUInt32(BitConverter.GetBytes(response.RealNumber), 0);

			setMultipliers(firstElement, secondElement, thirdElement);
			Console.WriteLine($"{_a} {_c}");

			/*var last = thirdElement;
			for (int i = 0; i < 3; i++)
			{
				last = Next(last);
				response = JsonConvert.DeserializeObject<Response>(PostNewNumber(last));

				Console.WriteLine($"{response.Account.Money} {response.RealNumber} {last}");
			}*/
		}

		private static void setMultipliers(uint firstElement, uint secondElement, uint thirdElement)
		{
			_a = (thirdElement - secondElement) * moduleInversion(secondElement - firstElement) % Math.Pow(2, 32);

			_c = (secondElement - firstElement * _a) % Math.Pow(2, 32);
		}

		private static double moduleInversion(uint k)
		{
			long a = k;
			var n = (int)Math.Pow(2, 32);
			long i = n, v = 0, d = 1;
			while (a > 0)
			{
				long t = i / a, x = a;
				a = i % x;
				i = x;
				x = d;
				d = v - t * x;
				v = x;
			}
			v %= n;
			if (v < 0) v = (v + n) % n;
			return v;
		}

		public static uint Next(uint last)
		{
			var next = (_a * (double)last + _c) % Math.Pow(2, 32); // m is 2^32
			return (uint)next;
		}

		private static string CreateNewAccount()
		{
			var url = $"http://95.217.177.249/casino/createacc?id={_accountCode}";
			return post(url);
		}

		private static string PostNewNumber(uint number)
		{
			var url = $"http://95.217.177.249/casino/playLcg?id={_accountCode}&bet=1&number={number}";
			return post(url);
		}

		private static string post(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return string.Empty;
			}

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

			HttpWebResponse response;

			response = (HttpWebResponse)request.GetResponse();

			string jsonResponse;
			using (var dataStream = response.GetResponseStream())
			{
				using (var reader = new StreamReader(dataStream, Encoding.UTF8))
				{
					jsonResponse = reader.ReadToEnd();
				}
			}

			return jsonResponse;
		}

		private class Response
		{
			[JsonProperty("realNumber")]
			public int RealNumber { get; set; }

			[JsonProperty("account")]
			public Account Account { get; set; }
		}

		private class Account
		{
			[JsonProperty("money")]
			public int Money { get; set; }
		}

		private struct ModuleInversionData
		{
			public long A { get; set; }

			public long B { get; set; }
		}
	}
}
