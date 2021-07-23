using PidgeotMail.Lib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;

namespace PidgeotMail.ViewModel
{
	public class LoginViewModel : ViewModelBase
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private bool _NeedLog;
		public RelayCommand LoginCmd { get; set; }
		public bool NeedLog
		{
			get => _NeedLog;
			set
			{
				Set(ref _NeedLog, value);
			}
		}

		public LoginViewModel()
		{
			NeedLog = true;
			LoginCmd = new RelayCommand(() => ActiveAcount());
			try
			{
				if (!Directory.Exists(MainViewModel.TokenFolder)) Directory.CreateDirectory(MainViewModel.TokenFolder);
				if (Directory.GetFiles(MainViewModel.TokenFolder).Length > 0)
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
				log.Error(e.ToString());
				return;
			}
		}
	}
}
