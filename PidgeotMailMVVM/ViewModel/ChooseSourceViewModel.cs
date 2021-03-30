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
		private bool _SelectEx;
		private bool _SelectGs;
		private int _Row;
		private int _Column;
		private string _ExPath;

		public string Left { get => UserSettings.L; set => Set(nameof(UserSettings.L), ref UserSettings.L, value); }
		public string Right { get => UserSettings.R; set => Set(nameof(UserSettings.R), ref UserSettings.R, value); }
		public string Bcc { get => UserSettings.Bcc; set => Set(nameof(UserSettings.Bcc), ref UserSettings.Bcc, value); }
		public string Cc { get => UserSettings.Cc; set => Set(nameof(UserSettings.Cc), ref UserSettings.Cc, value); }
		public string Link { get => _Link; set => Set(ref _Link, value); }
		public bool SelectEx
		{
			get => _SelectEx;
			set
			{
				Set(ref _SelectEx, value);
				RaisePropertyChanged(nameof(SelectEx));
			}
		}
		public bool SelectGs
		{
			get => _SelectGs; set
			{
				Set(ref _SelectGs, value);
				RaisePropertyChanged(nameof(SelectGs));
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
			_SelectEx = true;
			RaisePropertyChanged(nameof(SelectEx));
			_SelectGs = false;
			RaisePropertyChanged(nameof(SelectGs));
			Left = "{{";
			Right = "}}";
			ExPath = "Chưa chọn";
			Bcc = "";
			Cc = "";
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
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
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
							Logs.Write(result);
							return;
						}
						result = await GSheetService.InitValue(Column, Row);
						if (result != "OK")
						{
							MessageBox.Show(result);
							Logs.Write(result);
							return;
						}
						Logs.Write("Đã chọn sheet " + GSheetService.ChoiceSheetID);
						UserSettings.HeaderLocation = GSheetService.Header;
						UserSettings.Values = GSheetService.Values;
					}
					else if (SelectEx)
					{
						string result = ExService.CheckAvailable(ExPath);
						if (result != "OK")
						{
							MessageBox.Show(result);
							Logs.Write(result);
							return;
						}
						result = await ExService.InitValue(Column, Row);
						if (result != "OK")
						{
							MessageBox.Show(result);
							Logs.Write(result);
							return;
						}
						UserSettings.HeaderLocation = ExService.Header;
						UserSettings.Values = ExService.Value;
						Logs.Write("Đã chọn ExcelWorkbook " + ExService.Path);
					}
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
