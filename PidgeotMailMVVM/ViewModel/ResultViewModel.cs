﻿using System;
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
			Messenger.Default.Send<ResultMessage>(new ResultMessage("Đang thực hiện..."));
			CloseCmd = new RelayCommand(() =>
				{
					App.Current.Shutdown();	
				}
			);

			HomeCmd = new RelayCommand(() =>
				{
					Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				}
			);
		}

		private static void AddLogs (GMessage ChoiceMail)
		{
			Logs.Add("Origin: ");
			Logs.Add("ID: " + ChoiceMail.MessageId);
			Logs.Add("Subject: " + ChoiceMail.Subject);
			Logs.Add("Cc: " + UserSettings.Cc);
			Logs.Add("Bcc: " + UserSettings.Bcc);
			Logs.Add("Text: " + ChoiceMail.message.TextBody);
			Logs.Add("Html: " + ChoiceMail.message.HtmlBody);
		}

		private void Process()
		{
			IList<IList<Object>> sheet = null;
			try
			{
				sheet = UserSettings.Values;
				var header = UserSettings.HeaderLocation;
				Logs.Write("Template gửi thư: ");
				Logs.Add("Sheet: ");
				foreach (var value in header)
				{
					Logs.Add(value.Key);
				}
				string htmlbody, plainbody, subject;
				string replacement, s;
				GMessage ChoiceMail = new GMessage(UserSettings.ChoiceMailID, GMService.GetDraftByID(UserSettings.ChoiceMailID).Result);
				AddLogs(ChoiceMail);
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
						GMService.Send(ChoiceMail.GenerateClone(i, subject, plainbody, htmlbody));
						Logs.Write(i + ": " + sheet[i][header["Email"]].ToString());
					}
					catch (Exception e)
					{
						Messenger.Default.Send<ResultMessage>(new ResultMessage("Có lỗi xảy ra tại mail thứ " + i.ToString() + "\n" +
						"Nội dung: " + e.Message, true));
						Logs.Write(e.ToString());
						return;
					}

				}
				Messenger.Default.Send<ResultMessage>(new ResultMessage("Hoàn thành gửi " + (sheet.Count - 1) + " email hợp lệ",true));
			}
			catch (Exception ex)
			{
				Logs.Write(ex.ToString());
			}					
			finally
			{
				if(Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/temp")) Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "/temp", true);
			}
		}
	}
}