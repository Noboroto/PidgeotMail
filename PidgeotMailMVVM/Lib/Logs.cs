using System.IO;
using System;
using System.Text;

namespace PidgeotMail.Lib
{
	class Logs
	{
		public static string path = "History.log";

		public static void Write(string message)
		{
			File.AppendAllText(path, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss: ") + message + "\n");
		}

		public static void Add(string message)
		{
			File.AppendAllText(path, message + "\n");
		}

		public static string Get()
		{
			return File.ReadAllText(path);
		}
	}
}
