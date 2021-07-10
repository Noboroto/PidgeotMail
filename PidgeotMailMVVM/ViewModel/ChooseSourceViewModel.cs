using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMail.Lib;
using System.Windows.Input;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;
using System.Windows;
using Microsoft.Win32;

namespace PidgeotMail.ViewModel
{
	public class ChooseSourceViewModel : ViewModelBase
	{
		private string _Link;
		private int _Row;
		private int _Column;
		private string _ExPath;
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		public string Left { get => UserSettings.L; set => Set(nameof(UserSettings.L), ref UserSettings.L, value); }
		public string Right { get => UserSettings.R; set => Set(nameof(UserSettings.R), ref UserSettings.R, value); }
		public string Link { get => _Link; set => Set(ref _Link, value); }
		public bool SelectEx
		{
			get => UserSettings.SelectEx;
			set
			{
				Set(ref UserSettings.SelectEx, value);
			}
		}
		public bool SelectGs
		{
			get => UserSettings.SelectGs;
			set
			{
				Set(ref UserSettings.SelectGs, value);
			}
		}
		public int Row { get => _Row; set => Set(ref _Row, value); }
		public int Column { get => _Column; set => Set(ref _Column, value); }
		public string ExPath { get => _ExPath; set => Set(ref _ExPath, value); }

		public ICommand NextCmd { get; set; }
		public ICommand BackCmd { get; set; }
		public ICommand GSCmd { get; set; }
		public ICommand ExCmd { get; set; }
		public ICommand BrowseCmd { get; set; }

		private void Start(StartMessage s)
		{
			if (s.CurrentView != StartMessage.View.ChooseSource) return;
			Left = "{{";
			Right = "}}";
			ExPath = "Chưa chọn";
			Link = "";
			Row = 0;
			Column = 0;
		}

		public ChooseSourceViewModel()
		{
			BrowseCmd = new RelayCommand(() =>
			   {
				   var OpenFileBox = new OpenFileDialog();
				   OpenFileBox.Filter = "Excel Files|*.xls;*.xlsx";
				   OpenFileBox.Multiselect = false;
				   if (OpenFileBox.ShowDialog() == true)
				   {
					   ExPath = OpenFileBox.FileName;
				   }
			   }
			);

			BackCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new GoBackMessage());
				}
			);

			NextCmd = new RelayCommand(async () =>
				{
					if (SelectGs)
					{
						string result = GSheetService.CheckAvailable(_Link);
						if (result != "OK")
						{
							MessageBox.Show(result);
							log.Error(result);
							return;
						}
						result = await GSheetService.InitValue(Column, Row);
						if (result != "OK")
						{
							MessageBox.Show(result);
							log.Error(result);
							return;
						}
						log.Info("Choose " + GSheetService.ChoiceSheetID);
						UserSettings.HeaderLocation = GSheetService.Header;
						UserSettings.Values = GSheetService.Values;
					}
					else if (SelectEx)
					{
						string result = ExService.CheckAvailable(ExPath);
						if (result != "OK")
						{
							MessageBox.Show(result);
							log.Error(result);
							return;
						}
						result = await ExService.InitValue(Column, Row);
						if (result != "OK")
						{
							MessageBox.Show(result);
							log.Error(result);
							return;
						}
						UserSettings.HeaderLocation = ExService.Header;
						UserSettings.Values = ExService.Value;
						log.Info("Đã chọn ExcelWorkbook " + ExService.Path);
					}
					log.Info("Gửi " + Column + " cột và " + Row + " email");
					Messenger.Default.Send(new NavigateToMessage(new AttachmentView()));
					Messenger.Default.Send(new StartMessage(StartMessage.View.Attachments));
				}
				, () =>
				{
					if (SelectEx) return ExPath != "Chưa chọn";
					if (SelectGs) return !string.IsNullOrEmpty(_Link);
					return false;
				}
			);

			ExCmd = new RelayCommand(() =>
				{
					System.Diagnostics.Process.Start(@"https://drive.google.com/file/d/1zRLzX_kxGnDrqkEw_gUdFKalpi-4KmIS/view?usp=sharing");
				}
			);

			GSCmd = new RelayCommand(() =>
				{
					System.Diagnostics.Process.Start(@"https://docs.google.com/spreadsheets/d/1MLF5S_CsSFZa2fNVqyjSHOYwYWOlULftFyF5rFQzUKk/edit?usp=sharing");
				}
			);
			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
		}
	}
}
