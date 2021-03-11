using System.IO;
using System.Collections.Generic;

using GalaSoft.MvvmLight;

namespace PidgeotMailMVVM.Lib
{
	public class AttachmentInfo : ViewModelBase
	{
		private DirectoryInfo Dinfo;
		private FileInfo Finfo;
		private bool _IsSelected;
		private string _SenderGroup;

		public string AttachmentPath => (IsFile) ? Finfo.FullName : Dinfo.FullName;
		public string Name => (IsFile) ? Finfo.Name : Dinfo.Name;
		public bool IsResultPDF { get; set; }
		public bool IsFile { get; set; }
		public string SenderGroup
		{
			get
			{
				return _SenderGroup;
			}
			set
			{
				Set(nameof(_SenderGroup), ref _SenderGroup, value);
			}
		}
		public bool Enable => (IsFile && !IsResultPDF);
		public bool IsSelected
		{
			get
			{
				return _IsSelected;
			}
			set
			{
				Set(nameof(_IsSelected), ref _IsSelected, value);
			}
		}
		public FileStream Stream (string matcher = "")
		{
			if (IsFile) return Finfo.OpenRead();
			else if (!string.IsNullOrEmpty(matcher))
			{
				return Dinfo.GetFiles(matcher)[0].OpenRead();
			}
			return null;
		}

		public AttachmentInfo(string path, bool isfile = true, bool ispdf = false,string group = "")
		{
			if (isfile) Finfo = new FileInfo(path);
			else Dinfo = new DirectoryInfo(path);
			IsFile = isfile;
			IsResultPDF = ispdf;
			SenderGroup = group;
		}
	}
}
