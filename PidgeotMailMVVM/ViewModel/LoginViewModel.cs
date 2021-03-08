using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace PidgeotMailMVVM.ViewModel
{
    public class LoginViewModel
    {
        public ICommand LoginCmd { get; set; }

        static string[] Scopes = { GmailService.Scope.GmailReadonly, SheetsService.Scope.Spreadsheets, GmailService.Scope.GmailSend };

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public LoginViewModel()
        {
            LoginCmd = new RelayCommand(() => Task.Run(ActiveAcount));
            try
            {
                if (!Directory.Exists("4xR24anAtrw2ajpqW45SVB56saAfas")) Directory.CreateDirectory("token");
                if (Directory.GetFiles("4xR24anAtrw2ajpqW45SVB56saAfas").Length > 0) Task.Run(ActiveAcount);
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
                using (var stream = GenerateStreamFromString(App.TextCredential))
                {
                    string path = "4xR24anAtrw2ajpqW45SVB56saAfas";
                    App.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None, new FileDataStore(path, true)).Result;
                    File.Delete("credentials.json");
                    App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        //Navigator.Navigate(new Uri("ChooseDraft.xaml", UriKind.RelativeOrAbsolute));
                    });
                }
            }
            catch (Exception e)
            {
                Logs.Write(e.ToString());
                return;
            }
        }
    }
}
