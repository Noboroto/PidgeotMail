using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PidgeotMail.Lib
{
	public class ExService
	{
		private static IList<IList<Object>> _Values;
		private static string _path;
		private static Dictionary<string, int> _Header;

		public static string Path => _path;
		public static Dictionary<string, int> Header => _Header;
		public static IList<IList<Object>> Value => _Values;

		private static void Init()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			_Values = new List<IList<Object>>();
			_Header = new Dictionary<string, int>();
		}

		public static string CheckAvailable(string path)
		{
			Init();
			if (path == "Chưa chọn")
				return "Chưa chọn danh sách Excel!";
			if (!File.Exists(path))
				return "File không tồn tại!";
			_path = path;
			return "OK";
		}

		public static Task<string> InitValueAsync(int Col, int Row)
		{
			Row++;
			return Task.Run(() =>
			{
				UserSettings.KeyColumn = -1;
				using (ExcelPackage package = new ExcelPackage(new FileInfo(_path)))
				{
					try
					{
						int x = 0;
						var worksheet = package.Workbook.Worksheets[0];
						for (int i = 0; i < Row; ++i)
						{
							_Values.Add(new List<Object>());
							for (int j = 0; j < Col; ++j)
							{
								string s = (worksheet.Cells[i + 1, j + 1].Value == null) ? "" : worksheet.Cells[i + 1, j + 1].Value.ToString();
								_Values[i].Add(s);
								if (i != 0) continue;
								if (s == "") return "Danh sách không đủ số cột";
								_Header.Add(s.ToString(), x);
								if (s.ToString().Trim().ToUpper() == "EMAIL") UserSettings.KeyColumn = j;
								if (UserSettings.TurnOnBcc && s.ToString().Trim().ToUpper() == "BCC") UserSettings.BccColumn = j;
								if (UserSettings.TurnOnCc && s.ToString().Trim().ToUpper() == "CC") UserSettings.CcColumn = j;
								x++;
							}
						}
					}
					catch (Exception e)
					{
						return e.ToString();
					}
				}
				if (_Values == null || _Values.Count == 0) return "Danh sách trống!";
				if (UserSettings.KeyColumn == -1) return "Không tìm thấy cột Email";
				return "OK";
			});
		}
	}
}
