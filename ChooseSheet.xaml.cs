using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for ChooseSheet.xaml
    /// </summary>
    public partial class ChooseSheet : Page
    {
        string RowCount;
        public ChooseSheet()
        {
            InitializeComponent();
            try
            {
                Left.Text = "{{";
                Right.Text = "}}";
                Warning.Content =
                    "Lưu ý!\n" +
                    "Nếu muốn định dạng chuỗi trong thư, phải định dạng \n" +
                    "cả tên chuỗi và dấu mở/đóng chuỗi. (ví dụ: Nếu muốn \n" +
                    "in đậm chuỗi Name phải định dạng là " +
                    ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "") + "Name" + ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "") +
                    "\n" +
                    "Tài khoản hiện tại phải có quyền xem sheet\n" +
                    "Các hàng và cột phải liền kề nhau\n" +
                    "Phải chứa cột \"Email\"";
                App.sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = App.credential,
                    ApplicationName = App.ApplicationName,
                });
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private int Min (int a, int b)
        {
            if (a < b) return a;
            return b;
        }

        private bool CheckAvailable ()
        {
            if (string.IsNullOrEmpty(Link.Text))
            {
                MessageBox.Show("Không được để trống!");
                Link.Text = "";
                return false;
            }
            if (!Link.Text.Contains(@"https://docs.google.com/spreadsheets/"))
            {
                MessageBox.Show("Đây không phải link Google Sheet!");
                Link.Text = "";
                return false;
            }
            var tmp = Link.Text.Split('/').ToList();
            for (int i = 0; i < tmp.Count; ++i)
            {
                if (tmp[i] == "d")
                {
                    try
                    {
                        App.ChoiceSheetID = App.sheetsService.Spreadsheets.Get(tmp[i + 1]).Execute().SpreadsheetId;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("403"))
                        {
                            MessageBox.Show("Bạn không có quyền xem!");
                        }
                        else if (ex.Message.Contains("404"))
                        {
                            MessageBox.Show("Không tìm thấy sheet!");
                            Link.Text = "";
                        }
                        else
                        {
                            MessageBox.Show(ex.Message);
                        }
                        return false;
                    }
                }
            }
            return true;
        }
        
        private bool CheckColRow ()
        {
            if (Regex.IsMatch(Row.Text, "^[0-9]+$"))
            {
                RowCount = (int.Parse(Row.Text)+1).ToString();
            }
            else
            {
                MessageBox.Show("Sai định dạng số lượng!");
                return false;
            }
            return true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Navigator.GoBack();
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!CheckAvailable()) return;
                if(!CheckColRow()) return;
                App.L = ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "");
                App.R = ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "");
                App.ChoiceRange = "1:" + RowCount.Trim();
                App.Cc = Cc.Text.Trim();
                App.Bcc = Bcc.Text.Trim();
                Logs.Write("Đã chọn sheet " + App.ChoiceSheetID);
                Navigator.Navigate(new Uri("ResultPage.xaml", UriKind.RelativeOrAbsolute));
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private void Left_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(Explain == null) return;
                Explain.Content = "Các cụm từ " + ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "") + "Name" + ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "") + " trong email sẽ được thay bằng các giá trị của cột Name";
                Warning.Content =
                    "Lưu ý!\n" +
                    "Nếu muốn định dạng chuỗi trong thư, phải định dạng \n" +
                    "cả tên chuỗi và dấu mở/đóng chuỗi. (ví dụ: Nếu muốn \n" +
                    "in đậm chuỗi Name phải định dạng là " +
                    ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "") + "Name" + ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "") +
                    "\n" +
                    "Tài khoản hiện tại phải có quyền xem sheet\n" +
                    "Các hàng và cột phải liền kề nhau\n" +
                    "Phải chứa cột \"Email\"";
            }
            catch(Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private void Right_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Explain == null) return;
            try
            {
                Explain.Content = "Các cụm từ " + ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "") + "Name" + ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "") + " trong email sẽ được thay bằng các giá trị của cột Name";
                Warning.Content =
                    "Lưu ý!\n" +
                    "Nếu muốn định dạng chuỗi trong thư, phải định dạng \n" +
                    "cả tên chuỗi và dấu mở/đóng chuỗi. (ví dụ: Nếu muốn \n" +
                    "in đậm chuỗi Name phải định dạng là " +
                    ((Left.Text.Length > 0) ? Left.Text.Substring(0, Min(30, Left.Text.Length)) : "") + "Name" + ((Right.Text.Length > 0) ? Right.Text.Substring(0, Min(30, Right.Text.Length)) : "") +
                    "\n" +
                    "Tài khoản hiện tại phải có quyền xem sheet\n" +
                    "Các hàng và cột phải liền kề nhau\n" +
                    "Phải chứa cột \"Email\"";
            }
            catch (Exception ex)
            {
                Logs.Write(ex.ToString());
            }
        }

        private void RefSheet_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(@"https://docs.google.com/spreadsheets/d/1Rg_rCJfClaGzBfgp3K3mWwEIYLCLRWf5ouGvh8O51Y4/edit?usp=sharing");
            Task.Run(() =>
                {
                    App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        refbut.IsEnabled = false;
                        announce.IsActive = true;
                    });
                    Task.Delay(1000).Wait();
                    App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        announce.IsActive = false;
                        refbut.IsEnabled = true;
                    });
                });
        }
    }
}