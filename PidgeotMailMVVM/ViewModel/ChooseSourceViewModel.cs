using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.Lib;
using System.Windows.Input;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;
using System.Windows;
using Microsoft.Win32;

namespace PidgeotMailMVVM.ViewModel
{
	public class ChooseSourceViewModel : ViewModelBase
	{
		private string link;
		private bool _SelectEx;
		private bool _SelectGs;
		private int _Row;
		private int _Column;
		private string _ExPath;

		public string Left
		{
			get
			{
				return UserSettings.L;
			}
			set
			{
				Set(nameof(UserSettings.L), ref UserSettings.L, value);
			}
		}
		public string Right
		{
			get
			{
				return UserSettings.R;
			}
			set
			{
				Set(nameof(UserSettings.R), ref UserSettings.R, value);
			}
		}
		public string Bcc
		{
			get
			{
				return UserSettings.Bcc;
			}
			set
			{
				Set(nameof(UserSettings.Bcc), ref UserSettings.Bcc, value);
			}
		}
		public string Cc
		{
			get
			{
				return UserSettings.Cc;
			}
			set
			{
				Set(nameof(UserSettings.Cc), ref UserSettings.Cc, value);
			}
		}
		public string Link
		{
			get
			{
				return link;
			}
			set
			{
				Set(nameof(link), ref link, value);
			}
		}
		public bool SelectEx
		{
			get
			{
				return _SelectEx;
			}
			set
			{
				Set(nameof(_SelectEx), ref _SelectEx, value);
			}
		}
		public bool SelectGs
		{
			get
			{
				return _SelectGs;
			}
			set
			{
				Set(nameof(_SelectGs), ref _SelectGs, value);
			}
		}
		public int Row
		{
			get
			{
				return _Row;
			}
			set
			{
				Set(nameof(_Row), ref _Row, value);
			}
		}
		public int Column
		{
			get
			{
				return _Column;
			}
			set
			{
				Set(nameof(_Column), ref _Column, value);
			}
		}
		public string ExPath
		{
			get
			{
				return _ExPath;
			}
			set
			{
				Set(nameof(_ExPath), ref _ExPath, value);
			}
		}

		public ICommand NextCmd { get; set; }
		public ICommand BackCmd { get; set; }
		public ICommand GSCmd { get; set; }
		public ICommand ExCmd { get; set; }
		public ICommand BrowseCmd { get; set; }

		public ChooseSourceViewModel()
		{
			SelectEx = true;
			Left = "{{";
			Right = "}}";
			ExPath = "Chưa chọn";

			BrowseCmd = new RelayCommand(() =>
			   {
				   var OpenFileBox = new OpenFileDialog();
				   OpenFileBox.Filter = "Excel Files|*.xls;*.xlsx";
				   OpenFileBox.Multiselect = false;
				   if (OpenFileBox.ShowDialog() == true)
				   {
					   ExPath = OpenFileBox.FileName;
					   Messenger.Default.Send(new ChangePathMessage(ExPath));
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
						string result = GSheetService.CheckAvailable(link);
						if (result != "OK")
						{
							MessageBox.Show(result);
							Logs.Write(result);
							return;
						}
						result = await GSheetService.InitValue(Row);
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
				}
				, () =>
				{
					if (SelectEx) return ExPath != "Chưa chọn";
					if (SelectGs) return !string.IsNullOrEmpty(link);
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
		}
	}
}
