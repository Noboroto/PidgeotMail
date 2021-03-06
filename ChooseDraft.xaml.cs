using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using MimeKit;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for ChooseDraft.xaml
    /// </summary>
    public partial class ChooseDraft : Page
    {
        public ObservableCollection<GMessage> source;
        string header = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>";
        public ChooseDraft()
        {
            InitializeComponent();
            App.MailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = App.credential,
                ApplicationName = App.ApplicationName,
            });
            Refresh.IsEnabled = false;
            Logs.Write("Đã login bằng " + App.MailService.Users.GetProfile("me").Execute().EmailAddress);
            source = new ObservableCollection<GMessage>();
            table.ItemsSource = source;
            MyEmail.Content = "Tài khoản hiện tại: " + App.MailService.Users.GetProfile("me").Execute().EmailAddress;
            Task.Run(new Action(Process));
        }

        private void Process()
        {
            GMessage output;
            var tmp = App.MailService.Users.Drafts.List("me").Execute().Drafts;
            UsersResource.DraftsResource.GetRequest request;
            string raw;
            if (tmp != null) foreach (var value in tmp)
            {
                try
                {
                    request = App.MailService.Users.Drafts.Get("me", value.Id);
                    request.Format = UsersResource.DraftsResource.GetRequest.FormatEnum.Raw;
                    raw = request.Execute().Message.Raw;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message + " " + value.Id);
                    Logs.Write(e.ToString() + " " + value.Id);
                    continue;
                }
                App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    output = GetDataFromBase64(raw.Replace('-', '+').Replace('_', '/'), value.Id);
                    source.Add(output);
                });
            }
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Logs.Write("Đã load mail");
                Refresh.IsEnabled = true;
            });
        }

        public static string HtmlEncode(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        private void table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(table.SelectedIndex < 0 || table.SelectedIndex >= source.Count) return;
                var visitor = new HtmlPreviewVisitor();
                var gMessage = source[table.SelectedIndex];
                gMessage.message.Accept(visitor);
                wb.NavigateToString(header + "<h3>ID: " + gMessage.MessageId + "</h3><h2>Subject: " + HtmlEncode(gMessage.Subject) + "</h2><br/>" + visitor.HtmlBody);
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void Choose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(table.SelectedIndex < 0) return;
            App.ChoiceMailID = source[table.SelectedIndex].MessageId;
            Logs.Write("Đã chọn draft " + App.ChoiceMailID);
            Navigator.Navigate(new Uri("ChooseSheet.xaml", UriKind.RelativeOrAbsolute));
        }
        static GMessage GetDataFromBase64(string input, string realID)
        {
            var output = new GMessage(realID);
            try
            {
                using(var stream = new MemoryStream(Convert.FromBase64String(input)))
                {
                    output.message = MimeMessage.Load(stream);
                }
                return output;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + " " + realID + "\nBạn có thể bỏ qua thông báo này");
                Logs.Write(e.ToString() + realID);
                return output;
            }
        }
        private void Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var tmp = Directory.GetFiles("token");
            foreach(var value in tmp)
            {
                File.Delete(value);
            }
            Logs.Write("Logout");
            Navigator.Navigate("Login.xaml");
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            Refresh.IsEnabled = false;
            source.Clear();
            wb.NavigateToString(header);
            table.SelectedIndex = -1;
            Logs.Write("Đã refresh");
            Task.Run(new Action(Process));
        }
    }
}
