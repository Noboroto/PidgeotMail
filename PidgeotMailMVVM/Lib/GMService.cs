using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using PidgeotMailMVVM.ViewModel;
using System;
using System.Windows;
using System.Collections.Generic;

namespace PidgeotMailMVVM.Lib
{
	public class GMService
	{
		private static GmailService gs;
		private static int Size = 30;

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
			if (gs != null) ;
			gs = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = GoogleService.Credential,
				ApplicationName = MainViewModel.AppName,
			});
		}

		public static List<GMessage> GetDraft()
		{
			Init();
			var tmp = gs.Users.Drafts.List("me").Execute().Drafts;
			UsersResource.DraftsResource.GetRequest request;
			string raw;
			int count = 0;
			List<GMessage> Result = new List<GMessage>();
			if (tmp != null)
				foreach (var value in tmp)
				{
					if (count > Size) break;
					try
					{
						request = gs.Users.Drafts.Get("me", value.Id);
						request.Format = UsersResource.DraftsResource.GetRequest.FormatEnum.Raw;
						raw = request.Execute().Message.Raw;
						Result.Add(GMessage.GetDataFromBase64(raw.Replace('-', '+').Replace('_', '/'), value.Id));
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message + " " + value.Id);
						Logs.Write(e.ToString() + " " + value.Id);
						continue;
					}
					count++;
				}
			return Result;
		}
	}
}
