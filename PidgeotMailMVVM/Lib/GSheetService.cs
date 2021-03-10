using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PidgeotMailMVVM.Lib
{
	public class GSheetService
	{
		private static SheetsService sheetsService;
		private static IList<IList<Object>> _Values;
		private static string _ChoiceSheetID;

		public static IList<IList<Object>> Values => Values;
		public static string ChoiceSheetID => _ChoiceSheetID;
		private static void Init()
		{
			if (sheetsService != null) return;
			sheetsService = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = GoogleService.Credential,
				ApplicationName = PidgeotMailMVVM.ViewModel.MainViewModel.AppName,
			});
		}
		public static string InitValue(int Amount)
		{
			try
			{
				_Values = sheetsService.Spreadsheets.Values.Get(ChoiceSheetID, "1:" + Amount).Execute().Values;
			}
			catch (Exception e)
			{
				return e.Message;
			}
			return "OK";
		}
		public static string CheckAvailable(string Link)
		{
			Init();
			if (string.IsNullOrEmpty(Link))
			{
				return "Không được để trống!";
			}
			if (!Link.Contains(@"https://docs.google.com/spreadsheets/"))
			{
				return "Đây không phải link Google Sheet!";
			}
			var tmp = Link.Split('/').ToList();
			for (int i = 0; i < tmp.Count; ++i)
			{
				if (tmp[i] == "d")
				{
					try
					{
						_ChoiceSheetID = sheetsService.Spreadsheets.Get(tmp[i + 1]).Execute().SpreadsheetId;
					}
					catch (Exception ex)
					{
						if (ex.Message.Contains("403"))
						{
							return "Bạn không có quyền xem!";
						}
						else if (ex.Message.Contains("404"))
						{
							return "Không tìm thấy sheet!";
						}
						else
						{
							return ex.Message;
						}
					}
				}
			}
			return "OK";
		}

	}
}
