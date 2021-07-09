using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.Lib;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;

namespace PidgeotMail.ViewModel
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
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		public string MyEmailContent => "Tài khoản hiện tại: " + GMService.UserEmail;
		public ObservableCollection<GMessage> ListSource { get; set; }

		public ICommand LogoutCmd { get; set; }
		public ICommand NextCmd { get; set; }
		public RelayCommand RefreshCmd { get; set; }

		public bool CanRefresh { get => _CanRefresh; set => Set(ref _CanRefresh, value); }
		public GMessage SelectedItem
		{
			get => item;
			set
			{
				Set(ref item, value);
				Messenger.Default.Send(new ChangeHTMLContent(header + ((item == null) ? "" : item.HTMLContent)));
			}
		}

		public int SelectedIndex { get => index; set => Set(ref index, value); }

		private void Start(StartMessage s)
		{
			if (s.CurrentView != StartMessage.View.ChooseDraft) return;
			CanRefresh = false;
			SelectedIndex = -1;
			SelectedItem = null;
			log.Info("Đã login bằng " + GMService.UserEmail);
			ListSource.Clear();
			Process();
		}

		public ChooseDraftViewModel()
		{
			ListSource = new ObservableCollection<GMessage>();
			NextCmd = new RelayCommand(() =>
				{
					if (SelectedIndex < 0) return;
					UserSettings.ChoiceMailID = ListSource[SelectedIndex].MessageId;
					log.Info("Đã chọn draft " + UserSettings.ChoiceMailID);
					Messenger.Default.Send(new NavigateToMessage(new ChooseSourceView()));
					Messenger.Default.Send(new StartMessage(StartMessage.View.ChooseSource));
				}, () => (SelectedIndex >= 0) && (ListSource.Count > 0)
			);

			LogoutCmd = new RelayCommand(() =>
				{
					Directory.Delete(MainViewModel.TokenFolder, true);
					log.Info("Logout");
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
					log.Info("Đã refresh");
					Process();
				}, () => CanRefresh
			);
			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
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
						MessageBox.Show(e.ToString() + " " + value.Id);
						log.Error(e.ToString() + " " + value.Id);
						continue;
					}

				}
			log.Info("Đã load mail");
			CanRefresh = true;
		}
	}
}
