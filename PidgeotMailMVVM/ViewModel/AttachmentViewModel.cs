using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using PidgeotMailMVVM.Lib;

namespace PidgeotMailMVVM.ViewModel
{
	public class AttachmentViewModel : ViewModelBase
	{
		private readonly string SendAll = "Gửi cho tất cả";
		private readonly string PDFMess = "Email";

		private static void Attachments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

		public IList<string> Selection { get; set; }
		public ICommand PDFCmd { get; set; }
		public ICommand FileCmd { get; set; }
		public ICommand FolderCmd { get; set; }
		public ICommand NextCmd { get; set; }
		public ICommand DeleteCmd { get; set; }

		public ObservableCollection<AttachmentInfo> Attachments { get; set; }

		public AttachmentViewModel()
		{
			Attachments = new ObservableCollection<AttachmentInfo>();
			Selection = new ObservableCollection<string>();
			if (UserSettings.HeaderLocation != null) Selection = UserSettings.HeaderLocation.Keys.ToList();

			DeleteCmd = new RelayCommand(() =>
				{
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsSelected) Attachments.RemoveAt(i);
					}
				}
			);

			FolderCmd = new RelayCommand(() =>
				{
					FolderBrowserDialog folderDlg = new FolderBrowserDialog();
					folderDlg.ShowNewFolderButton = true;
					// Show the FolderBrowserDialog.  
					if (folderDlg.ShowDialog() == DialogResult.OK)
					{
						Attachments.Add(new AttachmentInfo(folderDlg.SelectedPath, false, false));
					}
				}
			);

			FileCmd = new RelayCommand(() =>
				{
					OpenFileDialog openFileDialog = new OpenFileDialog();
					openFileDialog.Multiselect = true;
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						foreach (var values in openFileDialog.FileNames)
						{
							Attachments.Add(new AttachmentInfo(values, true, false, SendAll));
						}
					}
				}
			);

			PDFCmd = new RelayCommand(() =>
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "PDF File|*.pdf";
				openFileDialog.Multiselect = false;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					foreach (var values in openFileDialog.FileNames)
					{
						Attachments.Add(new AttachmentInfo(values, true, true, PDFMess));
					}
				}
			}
			);
			Selection.Add(SendAll);
		}
	}
}
