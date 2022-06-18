using PhoneNumbers;
using System;

namespace SecurityLabs
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(PhoneNumbersManager.GetPhoneNumber("admin@gmail.com"));
			//new Task3().Exec();
		}
	}
}
