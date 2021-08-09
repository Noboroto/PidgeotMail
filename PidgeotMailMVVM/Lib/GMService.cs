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
	public static class GMService
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static int MaxLoop = 5;
		private static GmailService gs;
		private static SmtpClient client;
		private static string _UserEmail;
		public static IList<Draft> DraftsList =>  gs.Users.Drafts.List("me").Execute().Drafts;
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

		public static void Connect(int loop = 0)
		{
			if (loop > MaxLoop)
			{
				throw new TimeoutException();
			}
			client.Connect("smtp.gmail.com", 587);
			var oauth2 = new SaslMechanismOAuth2(UserEmail, GoogleService.Credential.Token.AccessToken);
			if (!client.IsConnected) Connect(loop + 1);
			else
			{
				client.Authenticate(oauth2);
				client.Timeout = 10000;
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
		
		public static async Task<string> Send(MimeMessage m, CancellationToken cancellation)
		{
			string result = "OK";
			try
			{
				log.Info("Start send");
				await client.SendAsync(m, cancellation);
			}
			catch (OperationCanceledException)
			{
				result = "Bị dừng";
				log.Error(result);
			}
			catch (MailKit.ServiceNotAuthenticatedException e)
			{
				result = "Có lỗi xác thực, bạn vui lòng đợi 5 phút rồi hãy thử lại";
				log.Error(e.ToString());
			}
			catch (Exception e)
			{
				result = "Có lỗi xảy ra, vui lòng đăng nhập lại hoặc báo lỗi!";
				if (e is AuthenticationException) result = "Có lỗi xác thực, bạn vui lòng đợi 5 phút rồi hãy thử lại";
				log.Error(e.ToString());
			}
			finally
			{
				log.Info("End send");
			}
			return result;
		}
		public static Task<string> SendAsync (MimeMessage m)
		{
			return Task.Run(() => Send(m, CancellationToken.None));
		}
		public static Task<string> SendAsync(MimeMessage m, CancellationToken cancellationToken)
		{
			return Task.Run(() => Send(m, cancellationToken), cancellationToken);
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
