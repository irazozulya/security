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
			var account = JsonConvert.DeserializeObject<Account>(CreateNewAccount());

			Console.WriteLine($"{account.Money}");

			var response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));
			
			var firstElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0); //Next(1);

			response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));

			var secondElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0);

			response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));

			var thirdElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0);

			setMultipliers(firstElement, secondElement, thirdElement);
			Console.WriteLine($"{_a} {_c}");

			var last = thirdElement;
			for (int i = 0; i < 1000; i++)
			{
				last = Next(last);
				PostNewNumber(last);
			}
		}

		private static void setMultipliers(int firstElement, int secondElement, int thirdElement)
		{
			var mod = Math.Pow(2, 32);

			_a = (thirdElement - secondElement) * moduleInversion(secondElement - firstElement) % mod;

			_c = (secondElement - firstElement * _a) % mod;
		}

		private static double moduleInversion(int k)
		{
			var n = (long)Math.Pow(2, 32);
			long a = (k + n) % n;
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

		public static int Next(int last)
		{
			var next = (_a * last + _c) % Math.Pow(2, 32); // m is 2^32
			return (int)next;
		}

		private static string CreateNewAccount()
		{
			var url = $"http://95.217.177.249/casino/createacc?id={_accountCode}";
			return post(url);
		}

		private static string PostNewNumber(int number, int bet = 1)
		{
			var url = $"http://95.217.177.249/casino/playLcg?id={_accountCode}&bet={bet}&number={number}";
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
