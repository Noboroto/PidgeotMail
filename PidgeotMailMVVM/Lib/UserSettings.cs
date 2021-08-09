using System;
using System.Collections.Generic;

namespace PidgeotMail.Lib
{
	public class UserSettings
	{
		public static string ChoiceMailID;
		public static string L = "{{";
		public static string R = "}}";
		public static IList<IList<Object>> Values;
		public static Dictionary<string, int> HeaderLocation;
		public static IList<AttachmentInfo> Attachments;
		public static int KeyColumn = -1;
		public static int BccColumn = -1;
		public static int CcColumn = -1;
		public static bool SelectEx = true;
		public static bool SelectGs = false;
		public static bool LogingOut = false;
		public static string TokenFolder => AppDomain.CurrentDomain.BaseDirectory + "\\4xR24anAtrw2ajpqW45SVB56saAfas";
		public static string TempFolder => AppDomain.CurrentDomain.BaseDirectory + "\\temp";

		public static void Restart ()
		{
			ChoiceMailID = "";
			L = "{{";
			R = "}}";
			Values = null;
			HeaderLocation = new Dictionary<string, int>();
			Attachments = new List<AttachmentInfo>();
			KeyColumn = -1;
			BccColumn = -1;
			CcColumn = -1;
			SelectEx = true;
			SelectGs = false;
			LogingOut = false;
		}
	}
}
