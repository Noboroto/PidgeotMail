using Google.Apis.Gmail.v1.Data;
using MimeKit;
using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public ObservableCollection<Info> source;
        public static string HtmlEncode(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }
        public class Info
        {
            public int ID { get; set; }
            public string To { get; set; }
            public Info (int i = 0, string t = "")
            {
                ID = i;
                To = t;
            }
        }

        public ResultPage()
        {
            InitializeComponent();
            source = new ObservableCollection<Info>();
            table.ItemsSource = source;
            Logs.Write("Bắt đầu gửi mail");
            Task.Run(Process);
        }

        static string Base64UrlEncode(MimeMessage message)
        {
            using(var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length)
                    .Replace('+', '-').Replace('/', '_');
            }
        }

        static void GetDataFromBase64(out MimeMessage output, string input)
        {
            output = new MimeMessage();
            using(var stream = new MemoryStream(Convert.FromBase64String(input)))
            {
                output = MimeMessage.Load(stream);
            }
        }
        static MimeMessage GetDataFromBase64(string input)
        {
            var output = new MimeMessage();
            using(var stream = new MemoryStream(Convert.FromBase64String(input)))
            {
                output = MimeMessage.Load(stream);
            }
            return output;
        }

        private void Process()
        {   
            IList<IList<Object>> sheet = null;
            try
            {
                try
                {
                    sheet = App.sheetsService.Spreadsheets.Values.Get(App.ChoiceSheetID, App.ChoiceRange).Execute().Values;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Logs.Write(e.ToString());
                }
                Dictionary<string, int> header = new Dictionary<string, int>();
                string path = DateTime.Now.ToString().Replace("/", "-").Replace(":", ".");
                Directory.CreateDirectory(path);
                if(sheet == null || sheet.Count <= 0)
                {
                    Logs.Write("Sheet trống");
                    MessageBox.Show("Sheet trống!");
                    return;
                }
                Logs.Write("Template gửi thư: ");
                Logs.Add("Sheet: ");
                for(int i = 0; i < sheet[0].Count; ++i)
                {
                    header.Add(sheet[0][i].ToString(), i);
                    Logs.Add(sheet[0][i].ToString());
                }
                string htmlbody, plainbody, subject;
                string replacement, s;
                var request = App.MailService.Users.Drafts.Get("me", App.ChoiceMailID);
                request.Format = Google.Apis.Gmail.v1.UsersResource.DraftsResource.GetRequest.FormatEnum.Raw;
                MimeMessage ChoiceMail = GetDataFromBase64(request.Execute().Message.Raw.Replace('-', '+').Replace('_', '/'));
                Logs.Add("Origin: ");
                Logs.Add("ID: " + App.ChoiceMailID);
                Logs.Add("Subject: " + ChoiceMail.Subject);
                Logs.Add("Cc: " + App.Cc);
                Logs.Add("Bcc: " + App.Bcc);
                Logs.Add("Text: " + ChoiceMail.TextBody);
                Logs.Add("Html: " + ChoiceMail.HtmlBody);
                for(int i = 1; i < sheet.Count; ++i)
                {
                    htmlbody = ChoiceMail.HtmlBody;
                    plainbody = ChoiceMail.TextBody;
                    subject = ChoiceMail.Subject;
                    try
                    {
                        foreach(var value in header)
                        {
                            if(value.Value >= sheet[i].Count) continue;
                            replacement = (sheet[i][value.Value] != null) ? sheet[i][value.Value].ToString() : "";
                            s = HtmlEncode(App.L) + value.Key + HtmlEncode(App.R);
                            plainbody = plainbody.Replace(App.L + value.Key + App.R, replacement);
                            htmlbody = htmlbody.Replace(s, replacement);
                            subject = subject.Replace(App.L + value.Key + App.R, replacement);
                        }
                        MimeMessage t;
                        GetDataFromBase64(out t, Base64UrlEncode(ChoiceMail).Replace('-', '+').Replace('_', '/'));
                        foreach(var x in t.BodyParts.OfType<TextPart>())
                        {
                            if(x.IsHtml)
                            {
                                x.Text = htmlbody;
                            }
                            if(x.IsPlain)
                            {
                                x.Text = plainbody;
                            }
                        }
                        t.Subject = subject;
                        t.To.Add(new MailboxAddress("", sheet[i][header["Email"]].ToString()));
                        if(string.IsNullOrEmpty(App.Bcc)) t.Bcc.Add(new MailboxAddress("", App.Bcc));
                        if(string.IsNullOrEmpty(App.Cc)) t.Cc.Add(new MailboxAddress("", App.Cc));
                        App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            source.Add(new Info(i, t.To.ToString()));
                        });
                        File.WriteAllText(path + "/" + i + "-full.txt", t.ToString());
                        File.WriteAllText(path + "/" + i + "-html.txt", htmlbody);
                        File.WriteAllText(path + "/" + i + "-plain.txt", plainbody);
                        Message newMsg = new Message();
                        newMsg.Raw = Base64UrlEncode(t);
                        App.MailService.Users.Messages.Send(newMsg, "me").Execute();
                        App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            Logs.Write(i + ": " + t.To.ToString());
                        });
                    }
                    catch(Exception e)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            Warning.Content = "Có lỗi xảy ra tại mail thứ " + i.ToString() + "\n" +
                            "Nội dung: " + e.Message;
                            Logs.Write(e.ToString());
                            Home.IsEnabled = true;
                        });
                        return;
                    }
                    finally
                    {
                        Task.Delay(500).Wait();
                    }
                }
                App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    Warning.Content = "Hoàn thành gửi " + (sheet.Count - 1) + " email hợp lệ";
                    Home.IsEnabled = true;
                });
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new Uri("ChooseDraft.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
