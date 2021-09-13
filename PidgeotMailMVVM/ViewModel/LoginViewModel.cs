using AutoUpdaterDotNET;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.Lib;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;

using System;
using System.IO;
using System.Windows;

namespace PidgeotMail.ViewModel
{
	public class LoginViewModel : ViewModelBase
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private bool _NeedLog;
		public RelayCommand LoginCmd { get; set; }
		public bool NeedLog { get => _NeedLog; set => Set(ref _NeedLog, value); }

		public LoginViewModel()
		{
			AutoUpdater.Start(@"https://raw.githubusercontent.com/Noboroto/PidgeotMailWeb/main/PidgeotMailAutoUpdate.xml");
            NeedLog = true;
			LoginCmd = new RelayCommand(() => ActiveAcount());
			try
			{
                if (!GoogleService.StillAliveInMinutes(5)) Directory.Delete(UserSettings.TokenFolder, true);
				if (!Directory.Exists(UserSettings.TokenFolder)) Directory.CreateDirectory(UserSettings.TokenFolder);
				if (Directory.GetFiles(UserSettings.TokenFolder).Length > 0)
				{
					NeedLog = false;
					ActiveAcount();
				}
			}
			catch (AggregateException ae)
			{
				var x = ae.Flatten();
				foreach (var e in x.InnerExceptions)
				{
					log.Error(e.ToString());
				}
			}
			catch (Exception e)
			{
				log.Error(e.ToString());
			}
			finally
			{
				UserSettings.Restart();
			}
		}

        public async void ActiveAcount()
		{
			try
			{
				await GoogleService.InitAsync();
				await GMService.InitAsync();
				UserSettings.LogingOut = false;
				Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
			}
			catch (Exception e)
			{
				MessageBox.Show(HandleException.CorrectErrorMessage(e));
				log.Error(e.ToString());
				if (Directory.Exists(UserSettings.TokenFolder)) Directory.Delete(UserSettings.TokenFolder, true);
			}
		}
	}
}
