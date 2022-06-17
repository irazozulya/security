using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SecurityLabs
{
	public class Task3
	{
		private int _accountCode = 5100;

		private double _a, _c;

		public void Exec()
		{
			getLcgNextNumber();
		}

		private void getLcgNextNumber()
		{
			/*var account = JsonConvert.DeserializeObject<Account>(CreateNewAccount());

			Console.WriteLine($"{account.Money}");*/

			/*var response = JsonConvert.DeserializeObject<Response>(PostLcgNewNumber(0));
			
			var firstElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0); //Next(1);

			response = JsonConvert.DeserializeObject<Response>(PostLcgNewNumber(0));

			var secondElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0);

			response = JsonConvert.DeserializeObject<Response>(PostLcgNewNumber(0));

			var thirdElement = BitConverter.ToInt32(BitConverter.GetBytes(response.RealNumber), 0);

			setMultipliers(firstElement, secondElement, thirdElement);
			Console.WriteLine($"{_a} {_c}");

			var last = thirdElement;
			for (int i = 0; i < 1000; i++)
			{
				last = Next(last);
				PostLcgNewNumber(last);
			}*/

			var response = JsonConvert.DeserializeObject<Response>(PostMersenneTwisterNewNumber(0));

			var firstElement = response.RealNumber;
			var currTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

			_elements = new List<long>();

			for (int i = 0; i < 624; i++)
			{
				_elements.Add(0);
			}

			for (int i = -2000; i < 2000; i++)
			{
				seed(currTime + i);

				if (firstElement != getElement())
				{
					continue;
				}

				PostMersenneTwisterNewNumber(getElement(), 750);

				PostMersenneTwisterNewNumber(getElement(), 1000);
			}
		}

		#region Common

		private string CreateNewAccount()
		{
			var url = $"http://95.217.177.249/casino/createacc?id={_accountCode}";
			return post(url);
		}

		private string PostMersenneTwisterNewNumber(long number, int bet = 1)
		{
			var url = $"http://95.217.177.249/casino/playMt?id={_accountCode}&bet={bet}&number={number}";
			return post(url);
		}

		private string PostLcgNewNumber(int number, int bet = 1)
		{
			var url = $"http://95.217.177.249/casino/playLcg?id={_accountCode}&bet={bet}&number={number}";
			return post(url);
		}

		private string post(string url)
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
			public long RealNumber { get; set; }

			[JsonProperty("account")]
			public Account Account { get; set; }
		}

		private class Account
		{
			[JsonProperty("money")]
			public int Money { get; set; }
		}

		#endregion

		#region Task 1

		private void setMultipliers(int firstElement, int secondElement, int thirdElement)
		{
			var mod = Math.Pow(2, 32);

			_a = (thirdElement - secondElement) * moduleInversion(secondElement - firstElement) % mod;

			_c = (secondElement - firstElement * _a) % mod;
		}

		private double moduleInversion(int k)
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

		public int Next(int last)
		{
			var next = (_a * last + _c) % Math.Pow(2, 32); // m is 2^32
			return (int)next;
		}

		#endregion

		#region Task 2

		private const int _n = 624;

		private const int _m = 397;

		private const int _u = 11;

		private const int _s = 7;

		private const int _t = 15;

		private const int _l = 18;

		private const long _A = 0x9908B0DF;

		private const long _B = 0x9D2C5680;

		private const long _C = 0xEFC60000;

		private List<long> _elements;

		private int _index;

		private void seed(long seed)
		{
			_elements[0] = seed;
			for (_index = 1; _index < _n; _index++)
			{
				_elements[_index] = (1812433253U * (_elements[_index - 1] ^ (_elements[_index - 1] >> 30)) + _index);
			}
		}

		private long getElement()
		{
			long y;

			if (_index >= _n)
			{
				const long _lowerMask = 2147483647;
				const long _upperMask = 0x80000000;
				long[] mag01 = { 0, _A };

				for (int i = 0; i < _n - _m; i++)
				{
					y = (_elements[i] & _upperMask) | (_elements[i + 1] & _lowerMask);
					_elements[i] = _elements[i + _m] ^ (y >> 1) ^ mag01[y & 1];
				}

				for (int i = _n - _m; i < _n - 1; i++)
				{
					y = (_elements[i] & _upperMask) | (_elements[i + 1] & _lowerMask);
					_elements[i] = _elements[i + (_m - _n)] ^ (y >> 1) ^ mag01[y & 1];
				}

				y = (_elements[_n - 1] & _upperMask) | (_elements[0] & _lowerMask);
				_elements[_n - 1] = _elements[_m - 1] ^ (y >> 1) ^ mag01[y & 1];
				_index = 0;
			}

			y = _elements[_index++];

			y ^= y >> _u;
			y ^= (y << _s) & _B;
			y ^= (y << _t) & _C;
			y ^= y >> _l;
			return y;
		}

		#endregion
	}
}
