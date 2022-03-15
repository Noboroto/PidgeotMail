using GalaSoft.MvvmLight;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PidgeotMail.Lib
{
	public class AttachmentInfo : ObservableObject
	{
		private DirectoryInfo Dinfo;
		private bool _IsSelected;
		private int _GroupIndex;
		private bool _Enable;
		private bool _Error;

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public string OriginExt { get; set; }
		public string AttachmentPath => Dinfo.FullName;
		public string Name => Dinfo.Name;
		public bool IsResultPDF { get; set; }
		public bool Enable => _Enable;
		public bool Error { get => _Error; set => Set(ref _Error, value); }
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
				IEnumerable<FileInfo> res;
				if (GroupIndex == 0) return new FileInfo(Dinfo.FullName);
				if (IsResultPDF)
				{
					s = matcher + "-" + id.ToString();
					res = from file in Dinfo.GetFiles()
						  where file.Name.Contains(s)
						  select file;

				}
				else
				{
					res = from file in Dinfo.GetFiles()
						  where file.Name == matcher
						  select file;
				}
				var a = res.ToArray();
				if (a.Length > 0) return a[0];
				return null;
			}
			catch (Exception e)
			{
				log.Error(e.ToString());
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
