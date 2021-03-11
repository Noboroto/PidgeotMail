﻿using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PidgeotMailMVVM.Lib
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
			if (_Values == null) _Values = new List<IList<Object>>();
			if (_Header == null) _Header = new Dictionary<string, int>();
		}
		public static string CheckAvailable(string path)
		{
			if (path == "Chưa chọn")
				return "Chưa chọn danh sách Excel!";
			if (!File.Exists(path))
				return "File không tồn tại!";
			_path = path;
			return "OK";
		}
		public static Task<string> InitValue(int Col, int Row)
		{
			Init();
			Row++;
			return Task.Run(() =>
			{
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
								_Values[i].Add(worksheet.Cells[i + 1, j + 1].Value.ToString());
								if (i != 0) continue;
								_Header.Add(_Values[i][j].ToString(), x);
								if (_Values[i][j].ToString().Trim().ToUpper() == "EMAIL") UserSettings.KeyColumn = i;
								x++;
							}
						}
					}
					catch (Exception e)
					{
						return e.Message;
					}
				}
				if (_Values == null || _Values.Count == 0) return "Danh sách trống!";
				if (UserSettings.KeyColumn == -1) return "Không tìm thấy cột Email";
				return "OK";
			});
		}
	}
}