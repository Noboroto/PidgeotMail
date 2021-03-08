using MimeKit;
using System;
using System.IO;
using System.Windows;

namespace PidgeotMailMVVM.Lib
{
    public class GMessage
    {
        public MimeMessage message { get; set; }
        public string MessageId { get; set; }

		public string HTMLContent => message.HtmlBody;
        public string Subject => (string.IsNullOrEmpty(message.Subject)) ? "None subject" : message.Subject;
        public string Date => message.Date.ToString();
        public string ShortContent => message.TextBody.Substring(0, 20) + "...";
        public GMessage(string id = "", MimeMessage m = null)
        {
            message = m;
            MessageId = id;
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
