using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
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
		private string _Notice;
		private GMessage item;
		private int index;
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public string MyEmailContent => "Tài khoản hiện tại: " + GMService.UserEmail;
		public ObservableCollection<GMessage> ListSource { get; set; }
		public ICommand LogoutCmd { get; set; }
		public ICommand NextCmd { get; set; }
		public RelayCommand RefreshCmd { get; set; }
		public string Notice
		{
			get => _Notice;
			set
			{
				Set(ref _Notice, value);
			}
		}

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

		public ChooseDraftViewModel()
		{
			ListSource = new ObservableCollection<GMessage>();
			NextCmd = new RelayCommand(() =>
				{
					if (SelectedIndex < 0) return;
					UserSettings.ChoiceMailID = ListSource[SelectedIndex].MessageId;
					log.Info("Đã chọn draft " + UserSettings.ChoiceMailID);
					Messenger.Default.Send(new NavigateToMessage(new ChooseSourceView()));
				}, () => (SelectedIndex >= 0) && (ListSource.Count > 0)
			);
			LogoutCmd = new RelayCommand(() =>
				{
					UserSettings.Restart();
					Directory.Delete(MainViewModel.TokenFolder, true);
					log.Info("Logout");
					ViewModelLocator.CleanData<ChooseDraftView>();
					ViewModelLocator.CleanData<LoginView>();
					UserSettings.LogingOut = true;
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
					LoadDraftList();
				}, () => CanRefresh
			);
			CanRefresh = false;
			SelectedIndex = -1;
			SelectedItem = null;
			log.Info("Đã login bằng " + GMService.UserEmail);
			Notice = "Mail nháp";
			ListSource.Clear();
			LoadDraftList();
		}

		private Task LoadDraftList()
		{
			return Task.Run(() =>
			{
				var tmp = GMService.DraftsList;
				if (tmp != null)
				{
					foreach (var value in tmp)
					{
						try
						{
							var temp = new GMessage(value.Id, GMService.GetDraftByID(value.Id));
							App.Current.Dispatcher.Invoke(() =>
							{
								ListSource.Add(temp);
							});
						}
						catch (Exception e)
						{
							MessageBox.Show(e.ToString() + " " + value.Id);
							App.Current.Dispatcher.Invoke(() =>
							{
								log.Error(e.ToString() + " " + value.Id);
							});
							continue;
						}
					}
				}
				else
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						Notice = "Không có mail";
						log.Warn("Không có mail");
					});
				}
				App.Current.Dispatcher.Invoke(() =>
				{
					log.Info("Đã load mail");
					CanRefresh = true;
				});
			});
		}
	}
}
