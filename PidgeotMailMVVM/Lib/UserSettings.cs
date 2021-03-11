using System;
using System.Collections.Generic;

namespace PidgeotMailMVVM.Lib
{
	public class UserSettings
	{
		public static string ChoiceMailID;
		public static string L;
		public static string R;
		public static string Bcc;
		public static string Cc;
		public static IList<IList<Object>> Values;
		public static Dictionary<string, int> HeaderLocation;
		public static IList<AttachmentInfo> Attachments;
		public static int KeyColumn = -1;
	}
}
