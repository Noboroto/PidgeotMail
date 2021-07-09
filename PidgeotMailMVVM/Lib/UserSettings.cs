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
	}
}
