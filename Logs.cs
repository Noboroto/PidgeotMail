using System.IO;
using System;

namespace PidgeotMail
{
    class Logs
    {
        public static string path = "History.log";

        public static void Write (string message)
        {
            File.AppendAllText(path, DateTime.Now.ToString() + ": " + message + "\n");
        }

        public static string Get ()
        {
            return File.ReadAllText(path);
        }
    }
}
