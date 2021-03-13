using System;
using System.Windows;
using System.Windows.Threading;
using PidgeotMailMVVM.Lib;
using System.IO;

namespace PidgeotMailMVVM
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if (e != null)
				Logs.Write(e.Exception.ToString());
			e.Handled = true;
		}
		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;
			if (e != null)
				Logs.Write(ex.ToString());
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (Directory.Exists(ViewModel.MainViewModel.TokenFolder)) Directory.Delete(ViewModel.MainViewModel.TokenFolder, true);
			if (Directory.Exists(ViewModel.MainViewModel.TokenFolder)) Directory.Delete(ViewModel.MainViewModel.TempFolder, true);
		}
	}
}
