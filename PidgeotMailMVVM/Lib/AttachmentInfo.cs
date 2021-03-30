using System;
using System.IO;

using GalaSoft.MvvmLight;

namespace PidgeotMail.Lib
{
	public class AttachmentInfo : ObservableObject
	{
		private DirectoryInfo Dinfo;
		private bool _IsSelected;
		private int _GroupIndex;
		private bool _Enable;

		public string OriginExt { get; set; }
		public string AttachmentPath => Dinfo.FullName;
		public string Name => Dinfo.Name;
		public bool IsResultPDF { get; set; }
		public bool Enable => _Enable;
		public int GroupIndex
		{
			get => _GroupIndex; set
			{
				if (Enable && value == 0) value = 1;
				Set(ref _GroupIndex, value);
			}
		}
		public bool IsSelected { get => _IsSelected; set => Set(ref _IsSelected, value); }
		public FileInfo GetFile(int id, string matcher = "")
		{
			try
			{
				string s;
				if (GroupIndex == 0) return new FileInfo(Dinfo.FullName);
				if (IsResultPDF) s = "*" + matcher + "-" + id.ToString() + "*";
				else s = "*" + matcher + "*";
				var a = Dinfo.GetFiles(s);
				if (a.Length > 0) return a[0];
				return null;
			}
			catch (Exception e)
			{
				Logs.Write(e.ToString());
				return null;
			}
		}

		public AttachmentInfo(string path, bool ispdf = false, int group = 0, bool enab = false, string ext = "")
		{
			Dinfo = new DirectoryInfo(path);
			IsResultPDF = ispdf;
			GroupIndex = group;
			OriginExt = ext;
			_Enable = enab;
		}
	}
}
