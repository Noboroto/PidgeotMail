using MimeKit;
using MimeKit.Utils;

using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace PidgeotMailMVVM.Lib
{
	public class GMessage
	{
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

		private static int Min (int a, int b)
		{
			return (a < b) ? a : b;
		}

		private static string Base64UrlEncode(MimeMessage message)
		{
			using (var stream = new MemoryStream())
			{
				message.WriteTo(stream);
				return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length)
					.Replace('+', '-').Replace('/', '_');
			}
		}
		private static void GetDataFromBase64(out MimeMessage output, string input)
		{
			output = new MimeMessage();
			using (var stream = new MemoryStream(Convert.FromBase64String(input)))
			{
				output = MimeMessage.Load(stream);
			}
		}

		public MimeMessage GenerateClone (string email, string bcc, string cc, string subject, string plainbody, string htmlbody)
		{
			MimeMessage t;
			GetDataFromBase64(out t, Base64UrlEncode(message).Replace('-', '+').Replace('_', '/'));
			t.Subject = subject; 
			t.To.Add(new MailboxAddress("", email));
			if (string.IsNullOrEmpty(bcc)) t.Bcc.Add(new MailboxAddress("", bcc));
			if (string.IsNullOrEmpty(cc)) t.Cc.Add(new MailboxAddress("", cc));
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
			return t;
		}

		public void AddAttachment (AttachmentInfo info)
		{
			var builder = new BodyBuilder();
			builder.Attachments.Add(message.Body);
			builder.Attachments.Add(info.Name, info.Stream(info.SenderGroup));
			message.Body = builder.ToMessageBody();
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
				MessageBox.Show(e.Message + " " + realID + "\nBạn có thể bỏ qua thông báo này");
				Logs.Write(e.ToString() + realID);
				return output;
			}
		}
	}
}
