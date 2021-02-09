using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        static string[] Scopes = { GmailService.Scope.GmailReadonly, SheetsService.Scope.Spreadsheets, GmailService.Scope.GmailSend };

        public Login()
        {
            InitializeComponent();
            try
            {
                File.WriteAllText("credentials.json", App.TextCredential);
                if(!Directory.Exists("token")) Directory.CreateDirectory("token");
                if(Directory.GetFiles("token").Length > 0) Task.Run(ActiveAcount);
            }
            catch (Exception e)
            {
                Logs.Write(e.ToString());
            }
        }
        public void ActiveAcount()
        {
            try
            {
                using(var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string path = "token";
                    App.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None, new FileDataStore(path, true)).Result;
                    File.Delete("credentials.json");
                    App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {                        
                        Navigator.Navigate(new Uri("ChooseDraft.xaml", UriKind.RelativeOrAbsolute));
                    });
                }
            }
            catch (Exception e)
            {
                Logs.Write(e.ToString());
                return;
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(ActiveAcount);
        }
    }
}
