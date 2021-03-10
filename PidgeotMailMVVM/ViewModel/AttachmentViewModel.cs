using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;

using PidgeotMailMVVM.Lib;

namespace PidgeotMailMVVM.ViewModel
{
	public class AttachmentViewModel : ViewModelBase
	{
		public List<string> Selection { get; set; }
		public ObservableCollection<AttachmentInfo> Attachments { get; set; }

		public AttachmentViewModel()
		{
			Attachments = new ObservableCollection<AttachmentInfo>();
			Attachments.Add(new AttachmentInfo(@"E:\GitHub\DemoApp", false));
			Attachments.Add(new AttachmentInfo(@"E:\GitHub\DemoApp\MaterialDesignDemo.exe", false, false, "a"));
			Selection = new List<string>();
			Selection.Add("aasdasd");
		}
	}
}
