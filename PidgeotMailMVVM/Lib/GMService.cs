using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

using MailKit.Net.Smtp;
using MailKit.Security;

using MimeKit;

using PidgeotMail.ViewModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PidgeotMail.Lib
{
	public static class GMService
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static int MaxLoop = 5;
		private static GmailService gs;
		private static SmtpClient client;
		private static string _UserEmail;
		public static IList<Draft> DraftsList => gs.Users.Drafts.List("me").Execute().Drafts;
		public static string UserEmail => _UserEmail;

		static CancellationTokenSource source = new CancellationTokenSource();

		public static Task InitAsync()
		{
			return Task.Run(() =>
			{
				client = new SmtpClient();
				if (gs != null && !UserSettings.LogingOut) return;
				gs = new GmailService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = GoogleService.Credential,
					ApplicationName = MainViewModel.AppName,
				});
				_UserEmail = gs.Users.GetProfile("me").Execute().EmailAddress;
			});
		}

		public static async Task Connect(CancellationToken token)
		{
			try
			{
				await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto, token);
				var oauth2 = new SaslMechanismOAuth2(UserEmail, GoogleService.Credential.Token.AccessToken);
				if (client.IsConnected)
				{
					client.Authenticate(oauth2);
				}
			}
			catch (AggregateException ae)
			{
				throw ae.Flatten();
			}
			catch (Exception e)
			{
				throw e;
			}
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

		public static async Task SendAsync(MimeMessage m, CancellationToken cancellation)
		{
			try
			{
				await client.SendAsync(m, cancellation);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static MimeMessage GetDraftByID(string id)
		{
			var request = gs.Users.Drafts.Get("me", id);
			request.Format = UsersResource.DraftsResource.GetRequest.FormatEnum.Raw;
			return GetDataFromBase64(request.Execute().Message.Raw.Replace('-', '+').Replace('_', '/'));
		}
		public static Task<MimeMessage> GetDraftByIDAsync(string id)
		{
			return Task.Run(() => GetDraftByID(id));
		}
		public static Task<MimeMessage> GetDraftByIDAsync(string id, CancellationToken cancellationToken)
		{
			return Task.Run(() => GetDraftByID(id), cancellationToken);
		}
	}
}
