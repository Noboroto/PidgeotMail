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

namespace PidgeotMail.ViewModel
{
	/// <summary>
	/// Interaction logic for ResultPage.xaml
	/// </summary>
	public class ResultViewModel : ViewModelBase
	{
		private string _Warning;
		private bool _HomeEnabled;
		private int _Done;
		private int _Linmit;
		private IList<IList<Object>> sheet = null;

		public string Warning { get => _Warning; set => Set(ref _Warning, value); }
		public bool HomeEnabled
		{
			get => _HomeEnabled;
			set 
			{
				Set(ref _HomeEnabled, value);
				HomeCmd.RaiseCanExecuteChanged();
			}
		}
		public int Done { get => _Done; set => Set(ref _Done, value); }
		public int Limit { get => _Linmit; set => Set(ref _Linmit, value); }

		public ObservableCollection<SenderInfo> source { get; set; }

		public RelayCommand HomeCmd { get; set; }
		public ICommand CloseCmd { get; set; }

		public static string HtmlEncode(string text)
		{
			return HttpUtility.HtmlEncode(text);
		}

		public ResultViewModel()
		{
			Messenger.Default.Register<StartMessage>(this, (t) => Start(t));
			source = new ObservableCollection<SenderInfo>();
			CloseCmd = new RelayCommand(() =>
				App.Current.Shutdown()
			); ;
			HomeCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				},() => HomeEnabled
			);
		}

		private static void AddLogs(GMessage ChoiceMail)
		{
			Logs.Add("Origin: ");
			Logs.Add("ID: " + ChoiceMail.MessageId);
			Logs.Add("Subject: " + ChoiceMail.Subject);
			Logs.Add("Cc: " + UserSettings.Cc);
			Logs.Add("Bcc: " + UserSettings.Bcc);
			Logs.Add("Text: " + ChoiceMail.message.TextBody);
			Logs.Add("Html: " + ChoiceMail.message.HtmlBody);
		}

		private void Start (StartMessage s)
		{
			if (s.CurrentView != StartMessage.View.Result) return;
			Logs.Write("Bắt đầu gửi mail");
			sheet = UserSettings.Values;
			Limit = sheet.Count - 1;
			Done = 0;
			HomeEnabled = false;
			Process();
			Warning = "Đang thực hiện ...";
		}

		private async void Process()
		{
			source.Clear();
			string htmlbody, plainbody, subject;
			string replacement, s, result;
			try
			{
				var header = UserSettings.HeaderLocation;
				Logs.Write("Template gửi thư: ");
				Logs.Add("Sheet: ");
				foreach (var value in header)
				{
					Logs.Add(value.Key);
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
					result = await GMService.Send(ChoiceMail.GenerateClone(i, subject, plainbody, htmlbody));
					Logs.Write(i + ": " + sheet[i][header["Email"]].ToString());
					if (result != "OK")
					{
						Logs.Write(result);
					}		
					source.Add(new SenderInfo(i, sheet[i][header["Email"]].ToString(), result));
					Done++;
					Warning = "Đã hoàn thành " + source.Count + " email";
				}
			}
			catch (Exception ex)
			{
				Logs.Write(ex.ToString());
			}
			finally
			{
				HomeEnabled = true;
				if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/temp")) Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "/temp", true);
			}
		}
	}
}
