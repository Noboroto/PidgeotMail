using PidgeotMail.Lib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PidgeotMail.MessageForUI;
using PidgeotMail.View;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace PidgeotMail.ViewModel
{
	public class LoginViewModel
	{
		public ICommand LoginCmd { get; set; }

		public LoginViewModel()
		{
			LoginCmd = new RelayCommand(() => ActiveAcount());
			try
			{
				if (!Directory.Exists(MainViewModel.TokenFolder)) Directory.CreateDirectory(MainViewModel.TokenFolder);
				if (Directory.GetFiles(MainViewModel.TokenFolder).Length > 0) ActiveAcount();
			}
			catch (Exception e)
			{
				Logs.Write(e.ToString());
			}
		}
		public async void ActiveAcount()
		{
			try
			{
				Task InitTask = GoogleService.Init();
				await InitTask;
				Messenger.Default.Send(new NavigateToMessage(new ChooseDraftView()));
				Messenger.Default.Send(new StartMessage(StartMessage.View.ChooseDraft));
			}
			catch (Exception e)
			{
				Logs.Write(e.ToString());
				return;
			}
		}
	}
}
