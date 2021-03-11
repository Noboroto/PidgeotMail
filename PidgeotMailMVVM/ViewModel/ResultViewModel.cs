using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PidgeotMailMVVM.Lib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;

namespace PidgeotMailMVVM.ViewModel
{
	/// <summary>
	/// Interaction logic for ResultPage.xaml
	/// </summary>
	public class ResultViewModel : ViewModelBase
	{
		private bool _Home;
		private string _Warning;

		public string Warning
		{
			get
			{
				return _Warning;
			}
			set
			{
				Set(nameof(_Warning), ref _Warning, value);
			}
		}
		public ObservableCollection<SenderInfo> source { get; set; }

		public ICommand HomeCmd { get; set; }
		public ICommand CloseCmd { get; set; }

		public static string HtmlEncode(string text)
		{
			return HttpUtility.HtmlEncode(text);
		}

		public ResultViewModel()
		{
			source = new ObservableCollection<SenderInfo>();
			Logs.Write("Bắt đầu gửi mail");
			Task.Run(Process);

			CloseCmd = new RelayCommand(() =>
				{
					App.Current.Shutdown();	
				}
			);

			HomeCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				}, () => _Home
			);
		}

		private void Process()
		{
			IList<IList<Object>> sheet = null;
			try
			{
				sheet = UserSettings.Values;
				var header = UserSettings.HeaderLocation;
				if (sheet == null || sheet.Count <= 0)
				{
					Logs.Write("Sheet trống");
					MessageBox.Show("Sheet trống!");
					return;
				}
				Logs.Write("Template gửi thư: ");
				Logs.Add("Sheet: ");
				for (int i = 0; i < sheet[0].Count; ++i)
				{
					header.Add(sheet[0][i].ToString(), i);
					Logs.Add(sheet[0][i].ToString());
				}
				string htmlbody, plainbody, subject;
				string replacement, s;
				GMessage ChoiceMail = new GMessage(UserSettings.ChoiceMailID, GMService.GetDraftByID(UserSettings.ChoiceMailID));
				foreach (var value in UserSettings.Attachments)
				{
					if (value.IsFile && !value.IsResultPDF)
					{
						ChoiceMail.AddAttachment(value);
					}
				}
				Logs.Add("Origin: ");
				Logs.Add("ID: " + UserSettings.ChoiceMailID);
				Logs.Add("Subject: " + ChoiceMail.Subject);
				Logs.Add("Cc: " + UserSettings.Cc);
				Logs.Add("Bcc: " + UserSettings.Bcc);
				Logs.Add("Text: " + ChoiceMail.message.TextBody);
				Logs.Add("Html: " + ChoiceMail.message.HtmlBody);
				for (int i = 1; i < sheet.Count; ++i)
				{
					htmlbody = ChoiceMail.message.HtmlBody;
					plainbody = ChoiceMail.message.TextBody;
					subject = ChoiceMail.Subject;
					try
					{
						foreach (var value in header)
						{
							if (value.Value >= sheet[i].Count) continue;
							replacement = (sheet[i][value.Value] != null) ? sheet[i][value.Value].ToString() : "";
							s = HtmlEncode(UserSettings.L) + value.Key + HtmlEncode(UserSettings.R);
							plainbody = plainbody.Replace(UserSettings.L + value.Key + UserSettings.R, replacement);
							htmlbody = htmlbody.Replace(s, replacement);
							subject = subject.Replace(UserSettings.L + value.Key + UserSettings.R, replacement);
						}
						App.Current.Dispatcher.BeginInvoke((Action)delegate ()
						{
							source.Add(new SenderInfo(i, sheet[i][header["Email"]].ToString()));
						});
						GMService.Send(ChoiceMail.GenerateClone(sheet[i][header["Email"]].ToString(), UserSettings.Bcc, UserSettings.Cc, subject,plainbody, htmlbody));
						App.Current.Dispatcher.BeginInvoke((Action)delegate ()
						{
							Logs.Write(i + ": " + sheet[i][header["Email"]].ToString());
						});
					}
					catch (Exception e)
					{
						App.Current.Dispatcher.BeginInvoke((Action)delegate ()
						{
							Warning = "Có lỗi xảy ra tại mail thứ " + i.ToString() + "\n" +
							"Nội dung: " + e.Message;
							Logs.Write(e.ToString());
							_Home = true;
						});
						return;
					}
					finally
					{
						Task.Delay(500).Wait();
					}
				}
				App.Current.Dispatcher.BeginInvoke((Action)delegate ()
				{
					Warning = "Hoàn thành gửi " + (sheet.Count - 1) + " email hợp lệ";
					_Home = true;
				});
			}
			catch (Exception ex)
			{
				Logs.Write(ex.ToString());
			}
		}
	}
}
