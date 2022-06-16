using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SecurityLabs
{
	public static class Task3
	{
		public static void Exec()
		{
			getLcgNextNumber();
		}

		private static void getLcgNextNumber()
		{
			var account = JsonConvert.DeserializeObject<Account>(CreateNewAccount());

			Console.WriteLine($"{account.Money}");

			var response = JsonConvert.DeserializeObject<Response>(PostNewNumber(0));

			Console.WriteLine($"{response.RealNumber} {response.Account.Money}");

			/*var firstElement = -660036803; //Next(1);
			var sec = 1593740152;
			var secondElement = firstElement < sec ? sec : sec + Math.Pow(2, 32);//Next(firstElement);
			var th = 430299255;
			var thirdElement = sec < th ? th : th + Math.Pow(2, 32); //Next(secondElement);


			var c = (secondElement - firstElement * 0) % Math.Pow(2, 32);
			Console.WriteLine($"{a} {c}");

			Console.WriteLine($"{Next(th, 0, c)}");*/
		}

		private static double getMultiplier(int firstElement, int secondElement, int thirdElement)
		{
			var a = ((double)(thirdElement - secondElement) / (secondElement - firstElement)) % Math.Pow(2, 32);
			return 0;
		}

		public static int Next(double _last, double a, double c)
		{
			_last = (a * _last + c) % Math.Pow(2, 32); // m is 2^32
			Console.WriteLine(_last);
			return (int)_last;
		}

		private static string CreateNewAccount()
		{
			var url = $"http://95.217.177.249/casino/createacc?id=55555";
			return post(url);
		}

		private static string PostNewNumber(int number)
		{
			var url = $"http://95.217.177.249/casino/playLcg?id=55555&bet=1&number={number}";
			return post(url);
		}

		private static string post(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return string.Empty;
			}

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
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
	}
}
