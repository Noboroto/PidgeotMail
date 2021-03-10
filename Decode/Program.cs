using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Decode
{
	class Program
	{
		static void Main(string[] args)
		{
			var path = Console.ReadLine();
			;
			File.WriteAllText(Path.GetFileName(path) + "-decode.log", Decode(File.ReadAllText(path)));
		}

		public static string Decode(string raw)
		{
			string result = "";
			foreach (char c in raw)
			{
				result += ((char)(ushort)(c - 1372)).ToString();
			}
			return Encoding.UTF8.GetString(Convert.FromBase64String(result));
		}
	}
}
