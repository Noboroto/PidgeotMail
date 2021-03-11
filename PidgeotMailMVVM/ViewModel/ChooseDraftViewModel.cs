using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMailMVVM.Lib;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;

namespace PidgeotMailMVVM.ViewModel
{
	/// <summary>
	/// Interaction logic for ChooseDraft.xaml
	/// </summary>
	public class ChooseDraftViewModel : ViewModelBase
	{
		private readonly string header = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>";
		private bool _CanRefresh;
		private GMessage item;
		private int index;

		public string MyEmailContent => "Tài khoản hiện tại: " + GMService.UserEmail;
		public ObservableCollection<GMessage> ListSource { get; set; }

		public ICommand LogoutCmd { get; set; }
		public RelayCommand RefreshCmd { get; set; }
		public ICommand NextCmd { get; set; }

		public bool CanRefresh
		{
			get
			{
				return _CanRefresh;
			}
			set
			{
				Set(nameof(_CanRefresh), ref _CanRefresh, value);
			}
		}
		public GMessage SelectedItem
		{
			get
			{
				return item;
			}
			set
			{
				Set(nameof(item), ref item, value);
				Messenger.Default.Send(new ChangeHTMLContent(header + ((item == null) ? "" : item.HTMLContent)));
			}
		}

		public int SelectedIndex
		{
			get
			{
				return index;
			}
			set
			{
				Set(nameof(index), ref index, value);
			}
		}

		public ChooseDraftViewModel()
		{
			Logs.Write("Đã login bằng " + GMService.UserEmail);
			ListSource = new ObservableCollection<GMessage>();
			Process();
			NextCmd = new RelayCommand(() =>
				{
					if (SelectedIndex < 0) return;
					UserSettings.ChoiceMailID = ListSource[SelectedIndex].MessageId;
					Logs.Write("Đã chọn draft " + UserSettings.ChoiceMailID);
					Messenger.Default.Send(new NavigateToMessage(new ChooseSourceView()));
				}, () => (SelectedIndex >= 0) && CanRefresh
			); 

			LogoutCmd = new RelayCommand(() =>
				{
					var tmp = Directory.GetFiles("4xR24anAtrw2ajpqW45SVB56saAfas");
					foreach (var value in tmp)
					{
						File.Delete(value);
					}
					Logs.Write("Logout");
					Messenger.Default.Send(new NavigateToMessage(new LoginView()));
				}
			);

			RefreshCmd = new RelayCommand(() =>
				{
					CanRefresh = false;
					RefreshCmd.RaiseCanExecuteChanged();
					ListSource.Clear();
					Messenger.Default.Send(new ChangeHTMLContent(header));
					SelectedIndex = -1;
					Logs.Write("Đã refresh");
					Process();
				}, () => CanRefresh
			);
		}

		private async void Process()
		{
			var tmp = await GMService.DraftsList;
			if (tmp != null) foreach (var value in tmp)
				{
					try
					{
						ListSource.Add(new GMessage(value.Id, await GMService.GetDraftByID(value.Id)));
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message + " " + value.Id);
						Logs.Write(e.ToString() + " " + value.Id);
						continue;
					}

				}
			Logs.Write("Đã load mail");
			CanRefresh = true;
		}
	}
}
