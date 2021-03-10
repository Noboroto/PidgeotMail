using System.IO;

namespace PidgeotMailMVVM.Lib
{
	public class AttachmentInfo
	{
		private DirectoryInfo Dinfo;
		private FileInfo Finfo;

		public string Path => (IsFile) ? Finfo.FullName : Dinfo.FullName;
		public string Name => (IsFile) ? Finfo.Name : Dinfo.Name;
		public bool IsResultPDF { get; set; }
		public bool IsFile { get; set; }
		public string SenderGroup { get; set; }
		public bool ReadOnly => !(IsFile && !IsResultPDF);
		public bool IsSelected { get; set; }

		public AttachmentInfo(string path, bool isfile = true, bool ispdf = false,string group = "")
		{
			if (isfile) Finfo = new FileInfo(path);
			else Dinfo = new DirectoryInfo(path);
			ispdf = isfile;
			IsResultPDF = ispdf;
			SenderGroup = group;
		}
	}
}
