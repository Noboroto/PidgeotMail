using PidgeotMail.Lib;
using System;
using System.IO;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using System.Windows;
using AutoUpdaterDotNET;

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
			AutoUpdater.Mandatory = true;
			AutoUpdater.UpdateMode = Mode.Forced;
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
				MessageBox.Show("Có lỗi, vui lòng thử lại");
				log.Error(e.ToString());
				Directory.Delete(UserSettings.TokenFolder, true);
			}
		}
	}
}
