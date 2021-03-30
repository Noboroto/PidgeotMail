using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using PidgeotMail.ViewModel;
using System;
using System.Collections.Generic;
using MimeKit;
using System.IO;
using Google.Apis.Gmail.v1.Data;
using System.Threading.Tasks;

namespace PidgeotMail.Lib
{
	public class GMService
	{
		private static GmailService gs;

		public static Task<IList<Draft>> DraftsList => Task.Run(() => gs.Users.Drafts.List("me").Execute().Drafts);
		public static string UserEmail
		{
			get
			{
				Init();
				return gs.Users.GetProfile("me").Execute().EmailAddress;
			}
		}

		public static void Init()
		{
			if (gs != null) return;
			gs = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = GoogleService.Credential,
				ApplicationName = MainViewModel.AppName,
			});
		}

		private static MimeMessage GetDataFromBase64(string input)
		{
			var output = new MimeMessage();
			using (var stream = new MemoryStream(Convert.FromBase64String(input)))
			{
				output = MimeMessage.Load(stream);
			}
			return output;
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

		public static string Send(MimeMessage m)
		{
			string result = "OK";
			try
			{
				Logs.Write("Start send");
				if (m.MessageId == "-1") return m.Subject;
				Message newMsg = new Message();
				newMsg.Raw = Base64UrlEncode(m);
				gs.Users.Messages.Send(newMsg, "me").Execute();
			}
			catch (Exception e)
			{
				result = e.ToString();
			}
			finally
			{
				Logs.Write("End send");
			}
			return result;
		}

		public static Task<MimeMessage> GetDraftByID(string id)
		{
			return Task.Run(() =>
				{
					var request = gs.Users.Drafts.Get("me", id);
					request.Format = UsersResource.DraftsResource.GetRequest.FormatEnum.Raw;
					return GetDataFromBase64(request.Execute().Message.Raw.Replace('-', '+').Replace('_', '/'));
				});
		}
	}
}
