using MimeKit;

using System;
using System.IO;
using System.Windows;

namespace PidgeotMail.Lib
{
	public class GMessage
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public MimeMessage message { get; set; }
		public string MessageId { get; set; }
		public string HTMLContent
		{
			get
			{
				if (string.IsNullOrEmpty(message.HtmlBody)) return "";
				var visitor = new HtmlPreviewVisitor();
				message.Accept(visitor);
				return visitor.HtmlBody;
			}
		}
		public string Subject => (string.IsNullOrEmpty(message.Subject)) ? "None subject" : message.Subject;
		public string Date => message.Date.ToString();
		public string ShortContent => message.TextBody.Substring(0, Min(20, message.TextBody.Length)) + "...";
		public GMessage(string id = "", MimeMessage m = null)
		{
			message = m;
			MessageId = id;
		}

		private static int Min(int a, int b)
		{
			return (a < b) ? a : b;
		}

		public MimeMessage GenerateClone(int id, string subject, string plainbody, string htmlbody)
		{
			MimeMessage t = new MimeMessage();
			try
			{
				t = new MimeMessage();
				t.Subject = subject;				
				t.From.Add(new MailboxAddress("",GMService.UserEmail));
				t.To.Add(new MailboxAddress("", UserSettings.Values[id][UserSettings.KeyColumn].ToString()));
				if (!string.IsNullOrEmpty(UserSettings.Bcc)) t.Bcc.Add(new MailboxAddress("", UserSettings.Bcc));
				if (!string.IsNullOrEmpty(UserSettings.Cc)) t.Cc.Add(new MailboxAddress("", UserSettings.Cc));
				var builder = new BodyBuilder();
				builder.HtmlBody = htmlbody;
				builder.TextBody = plainbody;
				if (message.Attachments != null) foreach (var x in message.Attachments)
					{
						builder.Attachments.Add(x);
					}
				if (UserSettings.Attachments != null) 
				foreach (var x in UserSettings.Attachments)
				{
					string s, name = x.Name;
					if (!x.Enable && !x.IsResultPDF)
					{
						s = x.AttachmentPath;
					}
					else
					{
						s = UserSettings.Values[id][x.GroupIndex - 1].ToString();
						name = x.Name + "-" + s + "-" + id + x.OriginExt;
					}
					try
					{
						var info = x.GetFile(id, s);
						FileStream f = info.OpenRead();
						if (x.Enable) name = info.Name;
						if (f != null) builder.Attachments.Add(name, f); ;
					}
					catch (NullReferenceException)
					{
						continue;
					}
				}
				t.Body = builder.ToMessageBody();
			}
			catch (Exception e)
			{
				t = new MimeMessage();
				t.Subject = e.ToString();
				t.MessageId = "-1";
			}
			return t;
		}

		public static GMessage GetDataFromBase64(string input, string realID)
		{
			var output = new GMessage(realID);
			try
			{
				using (var stream = new MemoryStream(Convert.FromBase64String(input)))
				{
					output.message = MimeMessage.Load(stream);
				}
				return output;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString() + " " + realID + "\nBạn có thể bỏ qua thông báo này");
				log.Error(e.ToString() + realID);
				return output;
			}
		}
	}
}
