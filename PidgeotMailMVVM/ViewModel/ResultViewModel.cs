using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.Lib;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

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
		private CancellationTokenSource cancellation = new CancellationTokenSource();

		public string Warning { get => _Warning; set => Set(ref _Warning, value); }
		public bool HomeEnabled
		{
			get => _HomeEnabled;
			set
			{
				Set(ref _HomeEnabled, value);
				HomeCmd.RaiseCanExecuteChanged();
				CancelCmd.RaiseCanExecuteChanged();
			}
		}
		public int Done { get => _Done; set => Set(ref _Done, value); }
		public int Limit { get => _Linmit; set => Set(ref _Linmit, value); }

		public ObservableCollection<ReceiverInfo> source { get; set; }
		private List<string> FailEmail = new List<string>();

		public RelayCommand HomeCmd { get; set; }
		public RelayCommand SaveFailedCmd { get; set; }
		public RelayCommand CancelCmd { get; set; }

		public static string HtmlEncode(string text)
		{
			return HttpUtility.HtmlEncode(text);
		}

		private async void Start()
		{
			await Task.WhenAll(LoadResult(), LoadMail()).ConfigureAwait(false);
		}
		public ResultViewModel()
		{
			source = new ObservableCollection<ReceiverInfo>();
			HomeCmd = new RelayCommand(() =>
			{
				ViewModelLocator.Cleanup();
				ViewModelLocator.Register();
				Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
			}, () => HomeEnabled
			);

			CancelCmd = new RelayCommand(() =>
			{
				cancellation.Cancel();
				HomeEnabled = true;
			}, () => !HomeEnabled
			);

			SaveFailedCmd = new RelayCommand(() =>
			{
				if (FailEmail.Count <= 0) MessageBox.Show("Không có lỗi");
				else
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						Filter = "Text file (*.txt)|*.txt"
					};
					if (saveFileDialog.ShowDialog() == DialogResult.OK)
						foreach (var s in FailEmail)
							File.WriteAllText(saveFileDialog.FileName, s);
				}
			}
			);
			log.Info("Bắt đầu gửi mail");
			sheet = UserSettings.Values;
			Limit = sheet.Count - 1;
			Done = 0;
			HomeEnabled = false;
			Warning = "Đang thực hiện ...";
			Start();
		}

		private static void AddLogs(GMessage ChoiceMail)
		{
			log.Info("Origin: ");
			log.Info("ID: " + ChoiceMail.MessageId);
			log.Info("Subject: " + ChoiceMail.Subject);
			log.Info("Text: " + ChoiceMail.message.TextBody);
			log.Info("Html: " + ChoiceMail.message.HtmlBody);
		}

		private Task LoadResult()
		{
			return Task.Run(async () =>
			{
				string result = "";
				try
				{
					await GMService.Connect(cancellation.Token);
				}
				catch (Exception e)
				{
					MessageBox.Show(HandleException.CorrectErrorMessage(e), "Không thể kết nối máy chủ gửi mail");
					log.Fatal(e.ToString());
					foreach (var mess in messages)
					{
						App.Current.Dispatcher.Invoke(() =>
						{
							source[int.Parse(mess.MessageId) - 1].Status = "Không thể gửi";
							Warning = "Đã hoàn thành " + Done + " email";
						});
						FailEmail.Add(source[int.Parse(mess.MessageId) - 1].ToString());
					}
					App.Current.Dispatcher.Invoke(() =>
					{
						HomeEnabled = true;
					});
					_Cancel = true;
					return;
				}
				while (!_Cancel)
				{
					if (messages.Count > 0)
					{
						try
						{
							log.Info("StartSend " + messages[0].message.To);
							result = "";
							await GMService.SendAsync(messages[0].message, cancellation.Token);
							result = "OK";
						}
						catch (AggregateException ae)
						{
							foreach (var e in ae.InnerExceptions)
							{
								result = HandleException.CorrectErrorMessage(e);
								log.Error(e.ToString());
							}
						}
						catch (Exception e)
						{
							result = HandleException.CorrectErrorMessage(e);
							log.Error(e.ToString());
						}
						finally
						{
							log.Info("EndSend");
						}
						Done++;
						if (result != "OK")
						{
							App.Current.Dispatcher.Invoke(() =>
							{
								source[int.Parse(messages[0].MessageId) - 1].Status = result;
								Warning = "Đã hoàn thành " + Done + " email";
							});
							FailEmail.Add(source[int.Parse(messages[0].MessageId) - 1].ToString());
						}
						else
						{
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
			});
		}

		private Task LoadMail()
		{
			source.Clear();
			string htmlbody, plainbody, subject;
			string replacement, s;
			return Task.Run(async () =>
			{
				var header = UserSettings.HeaderLocation;
				log.Info("Sheet: ");
				int counting = 1;
				foreach (var value in header)
				{
					log.Info(counting + " " + value.Key);
					counting++;
				}
				GMessage ChoiceMail = new GMessage(UserSettings.ChoiceMailID, await GMService.GetDraftByIDAsync(UserSettings.ChoiceMailID));
				AddLogs(ChoiceMail);
				for (int i = 1; i < sheet.Count; ++i)
				{
					htmlbody = (ChoiceMail.message.HtmlBody == null) ? "" : ChoiceMail.message.HtmlBody;
					plainbody = (ChoiceMail.message.TextBody == null) ? "" : ChoiceMail.message.TextBody;
					subject = (ChoiceMail.Subject == null) ? "" : ChoiceMail.Subject;
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
							source.Add(new ReceiverInfo(i, sheet[i][UserSettings.KeyColumn].ToString(), "Đợi gửi"));
						});
					}
					catch (Exception ex)
					{
						log.Error(ex.ToString());
						Done++;
					}
				}
			}, cancellation.Token);
		}
	}
}
