using System.Collections.Generic;
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
		private bool _Continue;
		public bool Continue
		{
			get => _Continue;
			set => Set(ref _Continue, value);
		}
		public ICommand PDFCmd { get; set; }
		public ICommand FolderCmd { get; set; }
		public ICommand FileCmd { get; set; }
		public ICommand NextCmd { get; set; }
		public ICommand DeleteCmd { get; set; }
		public ICommand BackCmd { get; set; }
		public ObservableCollection<string> Selection { get; set; }
		public ObservableCollection<AttachmentInfo> Attachments { get; set; }

		public void Start(StartMessage s)
		{
			Continue = true;
			if (s.CurrentView != StartMessage.View.Attachments) return;
			Attachments.Clear();
			Selection.Clear();
			Selection.Add("Tất cả");
			Logs.Add("Chọn tệp đính kèm");
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
						if (Attachments[i].IsSelected)
						{
							Logs.Add("Xoá file: " + Attachments[i].AttachmentPath);
							Attachments.RemoveAt(i);
						}
					}
				}
			);

			FolderCmd = new RelayCommand(() =>
				{
					FolderBrowserDialog folderDlg = new FolderBrowserDialog();
					folderDlg.ShowNewFolderButton = true;
					if (folderDlg.ShowDialog() == DialogResult.OK)
					{
						Logs.Add("Chọn thư mục: " + folderDlg.SelectedPath);
						Attachments.Add(new AttachmentInfo(folderDlg.SelectedPath, false, UserSettings.KeyColumn + 1, true));
					}
				}
			);

			PDFCmd = new RelayCommand(() =>
				{
					OpenFileDialog openFileDialog = new OpenFileDialog();
					openFileDialog.Filter = "PDF File|*.pdf";
					openFileDialog.Multiselect = true;
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						foreach (var values in openFileDialog.FileNames)
						{
							Logs.Add("Chọn pdf: " + values);
							Attachments.Add(new AttachmentInfo(values, true, UserSettings.KeyColumn + 1));
						}
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
						Logs.Add("Chọn file: " + values);
						Attachments.Add(new AttachmentInfo(values, false, 0));
					}
				}
			}
   );

			NextCmd = new RelayCommand(async () =>
				{
					Continue = false;
					if (Directory.Exists(MainViewModel.TempFolder)) Directory.Delete(MainViewModel.TempFolder, true);
					UserSettings.Attachments = new List<AttachmentInfo>();
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsResultPDF)
						{
							await PDFProcess.SplitPDF(Attachments[i], UserSettings.Values, UserSettings.KeyColumn);
							Logs.Add("Chuyển đổi pdf: " + Attachments[i].AttachmentPath);
							UserSettings.Attachments.Add(new AttachmentInfo(PDFProcess.GetPDFPath(Attachments[i]), true, UserSettings.KeyColumn + 1, false, ".pdf"));
						}
						else UserSettings.Attachments.Add(Attachments[i]);
					}
					Messenger.Default.Send(new NavigateToMessage(new ResultView()));
					Messenger.Default.Send(new StartMessage(StartMessage.View.Result));
				}
			);

			BackCmd = new RelayCommand(() =>
			{
				Messenger.Default.Send(new NavigateToMessage(new ChooseSourceView()));
			}
			);

			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
		}
	}
}
