using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMailMVVM.Lib;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;

namespace PidgeotMailMVVM.ViewModel
{
	public class AttachmentViewModel : ViewModelBase
	{
		private readonly string PDFMess = "Email";

		private static void Attachments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

		public IList<string> Selection { get; set; }
		public ICommand PDFCmd { get; set; }
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
					if (folderDlg.ShowDialog() == DialogResult.OK)
					{
						Attachments.Add(new AttachmentInfo(folderDlg.SelectedPath, false));
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
							Attachments.Add(new AttachmentInfo(values, true, PDFMess));
						}
					}
				}
			);

			NextCmd = new RelayCommand(() =>
				{
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsResultPDF)
						{
							PDFProcess.SplitPDF(Attachments[i], UserSettings.Values, UserSettings.HeaderLocation[Attachments[i].SenderGroup]);
							Attachments.Add(new AttachmentInfo(PDFProcess.GetPDFPath(Attachments[i]), false, PDFMess, ".pdf"));
							Attachments.RemoveAt(i);
						}
					}
					UserSettings.Attachments = Attachments;
					Messenger.Default.Send(new NavigateToMessage(new ResultView()));
				}
			);
		}
	}
}
