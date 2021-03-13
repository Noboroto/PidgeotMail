﻿using System;
using System.Windows;
using System.Windows.Threading;
using PidgeotMail.Lib;
using System.IO;

namespace PidgeotMail
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
			if (Directory.Exists(ViewModel.MainViewModel.TempFolder)) Directory.Delete(ViewModel.MainViewModel.TempFolder, true);
		}
	}
}
