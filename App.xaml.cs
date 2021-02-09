using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using MimeKit;
using System;
using System.Windows;
using System.Windows.Threading;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserCredential credential;
        static public readonly string ApplicationName = "PidgeotMail";
        static public readonly string TextCredential = "{\"installed\":{\"client_id\":\"155023122563-9qnq9022jb9189377rjt98k91s393pen.apps.googleusercontent.com\",\"project_id\":\"PidgeotMail\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"ztuBZE_qYRBEJbYSAgbPimu4\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";
        static public SheetsService sheetsService;
        public static GmailService MailService;
        public static string ChoiceMailID;
        public static string ChoiceSheetID;
        public static string L;
        public static string R;
        public static string ChoiceRange;
        public static string Bcc;
        public static string Cc;
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if(e != null)
                Logs.Write (e.Exception.ToString());
            e.Handled = true;
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if(e != null)
                Logs.Write(ex.ToString());
        }
    }
}
