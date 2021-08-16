using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.Lib;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace PidgeotMail.ViewModel
{
	public class AttachmentViewModel : ViewModelBase
	{
		private bool _Continue;
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

		public AttachmentViewModel()
		{
			Continue = true;
			Attachments = new ObservableCollection<AttachmentInfo>();
			Selection = new ObservableCollection<string>
			{
				"Tất cả"
			};
			log.Info("Choose Attachments");

			if (UserSettings.HeaderLocation != null)
			{
				foreach (var x in UserSettings.HeaderLocation)
				{
					Selection.Add(x.Key);
				}
			}

			DeleteCmd = new RelayCommand(() =>
				{
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsSelected)
						{
							log.Info("Xoá file: " + Attachments[i].AttachmentPath);
							Attachments.RemoveAt(i);
						}
					}
				}
			);

			FolderCmd = new RelayCommand(() =>
				{
					FolderBrowserDialog folderDlg = new FolderBrowserDialog
					{
						ShowNewFolderButton = true
					};
					if (folderDlg.ShowDialog() == DialogResult.OK)
					{
						log.Info("Chọn thư mục: " + folderDlg.SelectedPath);
						Attachments.Add(new AttachmentInfo(folderDlg.SelectedPath, false, UserSettings.KeyColumn + 1, true));
					}
				}
			);

			PDFCmd = new RelayCommand(() =>
				{
					OpenFileDialog openFileDialog = new OpenFileDialog
					{
						Filter = "PDF File|*.pdf",
						Multiselect = true
					};
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						foreach (var values in openFileDialog.FileNames)
						{
							log.Info("Chọn pdf: " + values);
							Attachments.Add(new AttachmentInfo(values, true, UserSettings.KeyColumn + 1));
						}
					}
				}
			);

			FileCmd = new RelayCommand(() =>
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Multiselect = true
				};
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					foreach (var values in openFileDialog.FileNames)
					{
						log.Info("Chọn file: " + values);
						Attachments.Add(new AttachmentInfo(values, false, 0));
					}
				}
			}
			);

			NextCmd = new RelayCommand(async () =>
				{
					Continue = false;
					bool temp = false;
					if (!GoogleService.StillAliveInMinutes(30))
					{
						MessageBox.Show("Đã quá hạn đăng nhập, vui lòng đăng nhập lại!");
						GoogleService.LogOut();
						return;
					}
					if (Directory.Exists(UserSettings.TempFolder)) Directory.Delete(UserSettings.TempFolder, true);
					UserSettings.Attachments = new List<AttachmentInfo>();
					for (int i = Attachments.Count - 1; i >= 0; --i)
					{
						if (Attachments[i].IsResultPDF)
						{
							Attachments[i].Error = !await PDFProcess.SplitPDFAsync(Attachments[i], UserSettings.Values, UserSettings.KeyColumn);
							temp = temp || Attachments[i].Error;
							log.Info("Chuyển đổi pdf: " + Attachments[i].AttachmentPath);
							UserSettings.Attachments.Add(new AttachmentInfo(PDFProcess.GetPDFPath(Attachments[i]), true, UserSettings.KeyColumn + 1, false, ".pdf"));
						}
						else UserSettings.Attachments.Add(Attachments[i]);
					}
					if (temp)
					{
						Continue = true;
						MessageBox.Show("Vui lòng xoá những file bị lỗi để tiếp tục");
						UserSettings.Attachments.Clear();
						return;
					}
					Messenger.Default.Send(new NavigateToMessage(new ResultView()));
				}
			);

			BackCmd = new RelayCommand(() =>
			{
				ViewModelLocator.CleanData<AttachmentInfo>();
				ViewModelLocator.CleanData<ChooseSourceView>();
				Messenger.Default.Send(new GoBackMessage());
			}
			);
		}
	}
}
