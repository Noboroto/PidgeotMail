using System.IO;
using System;
using System.Text;

namespace PidgeotMailMVVM
{
    class Logs
    {
        public static string path = "History.log";

        private static string Encode(string rawdata)
        {
            rawdata = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawdata));
            string text = "";
            foreach(char c in rawdata)
            {
                text += ((char)(ushort)(c + 1372)).ToString();
            }
            return text;
        }

        public static void Write (string message)
        {
            File.AppendAllText(path, Encode (DateTime.Now.ToString() + ": " + message) + "\n");
        }

        public static void Add(string message)
        {
            File.AppendAllText(path, Encode(message) + "\n");
        }

        public static string Get ()
        {
            return File.ReadAllText(path);
        }
    }
}
