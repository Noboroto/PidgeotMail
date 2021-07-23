using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PidgeotMail.Lib
{
	public static class GSheetService
	{
		private static SheetsService sheetsService;
		private static IList<IList<Object>> _Values;
		private static string _ChoiceSheetID;
		private static Dictionary<string, int> _Header;

		public static Dictionary<string, int> Header => _Header;
		public static IList<IList<Object>> Values => _Values;
		public static string ChoiceSheetID => _ChoiceSheetID;

		public static void Init()
		{
			_Values = new List<IList<Object>>();
			_Header = new Dictionary<string, int>();
			if (sheetsService != null && !UserSettings.LogingOut) return;
			sheetsService = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = GoogleService.Credential,
				ApplicationName = PidgeotMail.ViewModel.MainViewModel.AppName,
			});
		}

		public static Task<string> InitValueAsync(int Col, int Row)
		{
			return Task.Run(() =>
			{
				UserSettings.KeyColumn = -1;
				Row++;
				try
				{
					_Values = sheetsService.Spreadsheets.Values.Get(ChoiceSheetID, "1:" + Row).Execute().Values;
					int i = 0;
					foreach (var value in _Values[0])
					{
						if (_Header.ContainsKey(value.ToString())) _Header[value.ToString()] = i;
						else _Header.Add(value.ToString(), i);
						if (value.ToString().Trim().ToUpper() == "EMAIL") UserSettings.KeyColumn = i;
						if (value.ToString().Trim().ToUpper() == "BCC") UserSettings.BccColumn = i;
						if (value.ToString().Trim().ToUpper() == "CC") UserSettings.CcColumn = i;
						i++;
					}
					if (_Header.Count < Col) return "Danh sách không đủ số cột";
				}
				catch (Exception e)
				{
					return e.ToString();
				}
				if (_Values == null || _Values.Count == 0) return "Danh sách trống!";
				if (UserSettings.KeyColumn == -1) return "Không tìm thấy cột Email";
				return "OK";
			});
		}
		public static string CheckAvailable(string Link)
		{
			Init();
			if (string.IsNullOrEmpty(Link))
			{
				return "Chưa chọn danh sách Google Sheet!";
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
							return ex.ToString();
						}
					}
				}
			}
			return "OK";
		}
	}
}
