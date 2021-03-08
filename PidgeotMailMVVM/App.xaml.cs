using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using System;
using System.Windows;
using System.Windows.Threading;
using PidgeotMailMVVM.Lib;

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
    }
}
