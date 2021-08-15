using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PidgeotMail
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if (e != null)
				log.Error(e.ToString());
			e.Handled = true;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
		}
		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;
			if (e != null)
				log.Error(ex.InnerException.ToString());
		}

		static void MyHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			log.Error(e.ToString());
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			try
			{
				if (Directory.Exists(Lib.UserSettings.TempFolder)) Directory.Delete(Lib.UserSettings.TempFolder, true);
			}
			catch (Exception ex)
			{
				log.Error(ex.ToString());
			}
		}
	}
}
