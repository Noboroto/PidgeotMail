using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using PidgeotMail.Lib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;
using System.Threading.Tasks;
using System.Threading;

namespace PidgeotMail.ViewModel
{
	/// <summary>
	/// Interaction logic for ResultPage.xaml
	/// </summary>
	public class ResultViewModel : ViewModelBase
	{
		private string _Warning;
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private bool _HomeEnabled;
		private int _Done;
		private int _Linmit;
		private bool _Cancel = false;
		private IList<IList<Object>> sheet = null;
		private List<GMessage> messages = new List<GMessage>();

		public string Warning { get => _Warning; set => Set(ref _Warning, value); }
		public bool HomeEnabled
		{
			get => _HomeEnabled;
			set
			{
				Set(ref _HomeEnabled, value);
				CloseCmd.RaiseCanExecuteChanged();
			}
		}
		public int Done { get => _Done; set => Set(ref _Done, value); }
		public int Limit { get => _Linmit; set => Set(ref _Linmit, value); }

		public ObservableCollection<SenderInfo> source { get; set; }

		public RelayCommand HomeCmd { get; set; }
		public RelayCommand CloseCmd { get; set; }

		public static string HtmlEncode(string text)
		{
			return HttpUtility.HtmlEncode(text);
		}

		public ResultViewModel()
		{
			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
			source = new ObservableCollection<SenderInfo>();
			CloseCmd = new RelayCommand(() =>
			{
				App.Current.Shutdown();
			}, () => HomeEnabled
			);
			HomeCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				}
			);
		}

		private static void AddLogs(GMessage ChoiceMail)
		{
			log.Info("Origin: ");
			log.Info("ID: " + ChoiceMail.MessageId);
			log.Info("Subject: " + ChoiceMail.Subject);
			log.Info("Text: " + ChoiceMail.message.TextBody);
			log.Info("Html: " + ChoiceMail.message.HtmlBody);
		}

		private async void Start(StartMessage s)
		{
			if (s.CurrentView != StartMessage.View.Result) return;
			log.Info("Bắt đầu gửi mail");
			sheet = UserSettings.Values;
			Limit = sheet.Count - 1;
			Done = 0;
			HomeEnabled = false;			
			Warning = "Đang thực hiện ...";
			await Process();
			await Task.Run(Loop);
		}

		private async void Loop()
		{
			string result;
			while (!_Cancel)
			{
				if (messages.Count > 0)
				{
					result = GMService.Send(messages[0].message);					
					Done++;
					if (result != "OK")
					{
						log.Error(result);
						App.Current.Dispatcher.Invoke(() =>
						{
							source[int.Parse(messages[0].MessageId)-1].Status = result;
							Warning = "Đã hoàn thành " + Done + " email";
						});
					}
					else
					{
						log.Info(messages[0].message.To.ToString());
						App.Current.Dispatcher.Invoke(() =>
						{
							source[int.Parse(messages[0].MessageId) - 1].Status = "Đã gửi";
							Warning = "Đã hoàn thành " + Done + " email";
						});
					}							
					messages.RemoveAt(0);
					await Task.Delay(1000);
				}
				if (Done >= UserSettings.Values.Count - 1)
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						HomeEnabled = true;
					});
					break;
				}
			}
		}

		private Task Process()
		{
			source.Clear();
			string htmlbody, plainbody, subject;
			string replacement, s;
			return Task.Run(() =>
			{
				var header = UserSettings.HeaderLocation;
				log.Info("Sheet: ");
				foreach (var value in header)
				{
					log.Info(value.Key);
				}
				GMessage ChoiceMail = new GMessage(UserSettings.ChoiceMailID, GMService.GetDraftByID(UserSettings.ChoiceMailID).Result);
				AddLogs(ChoiceMail);
				for (int i = 1; i < sheet.Count; ++i)
				{
					htmlbody = ChoiceMail.message.HtmlBody;
					plainbody = ChoiceMail.message.TextBody;
					subject = ChoiceMail.Subject;
					foreach (var value in header)
					{
						if (value.Value >= sheet[i].Count) continue;
						replacement = (sheet[i][value.Value] != null) ? sheet[i][value.Value].ToString() : "";
						s = HtmlEncode(UserSettings.L) + value.Key + HtmlEncode(UserSettings.R);
						plainbody = plainbody.Replace(UserSettings.L + value.Key + UserSettings.R, replacement);
						htmlbody = htmlbody.Replace(s, replacement);
						subject = subject.Replace(UserSettings.L + value.Key + UserSettings.R, replacement);
					}
					try
					{
						messages.Add(new GMessage(i.ToString(), ChoiceMail.GenerateClone(i, subject, plainbody, htmlbody)));
						App.Current.Dispatcher.Invoke(() =>
						{
							source.Add(new SenderInfo(i, sheet[i][header["Email"]].ToString(), "Đợi gửi"));
						});
					}
					catch (Exception ex)
					{
						log.Error(ex.ToString());
						Done++;
					}
				}
			});
		}
	}
}
