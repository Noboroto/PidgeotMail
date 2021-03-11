using PidgeotMailMVVM.Lib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace PidgeotMailMVVM.ViewModel
{
	public class LoginViewModel
	{
		public ICommand LoginCmd { get; set; }

		public LoginViewModel()
		{
			LoginCmd = new RelayCommand(() => ActiveAcount());
			try
			{
				if (!Directory.Exists("4xR24anAtrw2ajpqW45SVB56saAfas")) Directory.CreateDirectory("token");
				if (Directory.GetFiles("4xR24anAtrw2ajpqW45SVB56saAfas").Length > 0) ActiveAcount();
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
			}
			catch (Exception e)
			{
				Logs.Write(e.ToString());
				return;
			}
		}
	}
}
