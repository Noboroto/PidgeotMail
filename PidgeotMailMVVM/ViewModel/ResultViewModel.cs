using MimeKit;
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



		static void GetDataFromBase64(out MimeMessage output, string input)
		{
			output = new MimeMessage();
			using (var stream = new MemoryStream(Convert.FromBase64String(input)))
			{
				output = MimeMessage.Load(stream);
			}
		}

		static string Base64UrlEncode(MimeMessage message)
		{
			using (var stream = new MemoryStream())
			{
				message.WriteTo(stream);
				return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length)
					.Replace('+', '-').Replace('/', '_');
			}
		}


		private void Process()
		{
			IList<IList<Object>> sheet = null;
			try
			{
				sheet = UserSettings.Values;
				Dictionary<string, int> header = UserSettings.HeaderLocation;
				string path = DateTime.Now.ToString().Replace("/", "-").Replace(":", ".");
				Directory.CreateDirectory(path);
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
				MimeMessage ChoiceMail = GMService.GetDraftByID(UserSettings.ChoiceMailID);
				Logs.Add("Origin: ");
				Logs.Add("ID: " + UserSettings.ChoiceMailID);
				Logs.Add("Subject: " + ChoiceMail.Subject);
				Logs.Add("Cc: " + UserSettings.Cc);
				Logs.Add("Bcc: " + UserSettings.Bcc);
				Logs.Add("Text: " + ChoiceMail.TextBody);
				Logs.Add("Html: " + ChoiceMail.HtmlBody);
				for (int i = 1; i < sheet.Count; ++i)
				{
					htmlbody = ChoiceMail.HtmlBody;
					plainbody = ChoiceMail.TextBody;
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
						MimeMessage t;
						GetDataFromBase64(out t, Base64UrlEncode(ChoiceMail).Replace('-', '+').Replace('_', '/'));
						foreach (var x in t.BodyParts.OfType<TextPart>())
						{
							if (x.IsHtml)
							{
								x.Text = htmlbody;
							}
							if (x.IsPlain)
							{
								x.Text = plainbody;
							}
						}
						t.Subject = subject;
						t.To.Add(new MailboxAddress("", sheet[i][header["Email"]].ToString()));
						if (string.IsNullOrEmpty(UserSettings.Bcc)) t.Bcc.Add(new MailboxAddress("", UserSettings.Bcc));
						if (string.IsNullOrEmpty(UserSettings.Cc)) t.Cc.Add(new MailboxAddress("", UserSettings.Cc));
						App.Current.Dispatcher.BeginInvoke((Action)delegate ()
						{
							source.Add(new SenderInfo(i, t.To.ToString()));
						});
						File.WriteAllText(path + "/" + i + "-full.txt", t.ToString());
						File.WriteAllText(path + "/" + i + "-html.txt", htmlbody);
						File.WriteAllText(path + "/" + i + "-plain.txt", plainbody);
						GMService.Send(t);
						App.Current.Dispatcher.BeginInvoke((Action)delegate ()
						{
							Logs.Write(i + ": " + t.To.ToString());
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
