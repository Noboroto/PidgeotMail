using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.Lib;
using System.Windows.Input;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;
using System.Windows;
using System;

namespace PidgeotMailMVVM.ViewModel
{
	public class ChooseSourceViewModel : ViewModelBase
	{
		private string link;
		private bool _SelectEx;
		private bool _SelectGs;
		private int _Amount;
		private string path;

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
		public int Amount
		{
			get
			{
				return _Amount;
			}
			set
			{
				Set(nameof(_Amount), ref _Amount, value);
			}
		}

		public ICommand NextCmd { get; set; }
		public ICommand BackCmd { get; set; }
		public ICommand GSCmd { get; set; }
		public ICommand ExCmd { get; set; }

		public ChooseSourceViewModel()
		{
			SelectEx = true;
			Left = "{{";
			Right = "}}";

			BackCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				}
			);

			NextCmd = new RelayCommand(() =>
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
						result = GSheetService.InitValue(Amount);
						if (result != "OK")
						{
							MessageBox.Show(result);
							Logs.Write(result);
							return;
						}
						Logs.Write("Đã chọn sheet " + GSheetService.ChoiceSheetID);
					}
					else if (SelectEx)
					{

					}
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
