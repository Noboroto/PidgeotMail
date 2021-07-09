using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using PidgeotMail.ViewModel;
using System;
using System.Collections.Generic;
using MimeKit;
using System.IO;
using Google.Apis.Gmail.v1.Data;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading;

namespace PidgeotMail.Lib
{
	public class GMService
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static GmailService gs;
		private static SmtpClient client;
		private static string _UserEmail;
		public static Task<IList<Draft>> DraftsList => Task.Run(() => gs.Users.Drafts.List("me").Execute().Drafts);
		public static string UserEmail => _UserEmail;

		static CancellationTokenSource source = new CancellationTokenSource();

		public static Task Init()
		{		
			return Task.Run(() =>
			{
				client = new SmtpClient();
				if (gs != null) return;
				gs = new GmailService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = GoogleService.Credential,
					ApplicationName = MainViewModel.AppName,
				});
				_UserEmail = gs.Users.GetProfile("me").Execute().EmailAddress;
				connect:
				client.Connect("smtp.gmail.com", 587);
				var oauth2 = new SaslMechanismOAuth2(UserEmail, GoogleService.Credential.Token.AccessToken);
				if (!client.IsConnected) goto connect;
				client.Authenticate(oauth2);
			});
		}

		public static Task VerifyEmail (string email)
		{
			return client.VerifyAsync(email);
		}

		public static async void Disconnect()
		{
			await client.DisconnectAsync(true);
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
				log.Info("Start send");
				client.Send(m);
			}
			catch (Exception e)
			{
				result = e.ToString();
			}
			finally
			{
				log.Info("End send");
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
