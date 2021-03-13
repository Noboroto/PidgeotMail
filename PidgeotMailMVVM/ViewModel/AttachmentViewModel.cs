using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.Lib;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;

namespace PidgeotMail.ViewModel
{
	public class AttachmentViewModel : ViewModelBase
	{
		private readonly string PDFMess = "Email";

		private static void Attachments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			throw new System.NotImplementedException();
		}

		public ICommand PDFCmd { get; set; }
		public ICommand FolderCmd { get; set; }
		public ICommand NextCmd { get; set; }
		public ICommand DeleteCmd { get; set; }
		public ObservableCollection<string> Selection { get; set; }
		public ObservableCollection<AttachmentInfo> Attachments { get; set; }

		public void Start (StartMessage s)
		{
			if (s.CurrentView != StartMessage.View.Attachments) return;
			Attachments.Clear();
			Selection.Clear();
			if (UserSettings.HeaderLocation != null)
			{
				foreach (var x in UserSettings.HeaderLocation)
				{
					Selection.Add(x.Key);
				}
			}
		}

		public AttachmentViewModel()
		{
			Attachments = new ObservableCollection<AttachmentInfo>();
			Selection = new ObservableCollection<string>();
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

			NextCmd = new RelayCommand(async () =>
				{
					if (Directory.Exists(MainViewModel.TempFolder)) Directory.Delete(MainViewModel.TempFolder, true);
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsResultPDF)
						{
							await PDFProcess.SplitPDF(Attachments[i], UserSettings.Values, UserSettings.HeaderLocation[Attachments[i].SenderGroup]);
							Attachments.Add(new AttachmentInfo(PDFProcess.GetPDFPath(Attachments[i]), false, PDFMess, ".pdf"));
							Attachments.RemoveAt(i);
						}
					}
					UserSettings.Attachments = Attachments;
					Messenger.Default.Send(new NavigateToMessage(new ResultView()));
					Messenger.Default.Send(new StartMessage(StartMessage.View.Result));
				}
			);
			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
		}
	}
}
