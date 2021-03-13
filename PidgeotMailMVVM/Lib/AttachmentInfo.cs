using System;
using System.IO;

using GalaSoft.MvvmLight;

namespace PidgeotMailMVVM.Lib
{
	public class AttachmentInfo : ObservableObject
	{
		private DirectoryInfo Dinfo;
		private bool _IsSelected;
		private string _SenderGroup;

		public string OriginExt { get; set; }
		public string AttachmentPath => Dinfo.FullName;
		public string Name => Dinfo.Name;
		public bool IsResultPDF { get; set; }
		public bool Enable => !IsResultPDF;
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
		public FileStream Stream(string matcher = "")
		{
			try
			{
				if (!string.IsNullOrEmpty(matcher))
				{
					return Dinfo.GetFiles("*" + matcher + "*")[0].OpenRead();
				}
				return null;
			}
			catch (Exception e)
			{
				Logs.Write(e.Message);
				return null;
			}
		}

		public AttachmentInfo(string path, bool ispdf = false, string group = "", string ext = "")
		{
			Dinfo = new DirectoryInfo(path);
			IsResultPDF = ispdf;
			SenderGroup = group;
			OriginExt = ext;
		}
	}
}
