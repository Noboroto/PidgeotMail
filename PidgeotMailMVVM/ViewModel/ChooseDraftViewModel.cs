using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.Lib;
using PidgeotMailMVVM.MessageForUI;
using PidgeotMailMVVM.View;

namespace PidgeotMailMVVM.ViewModel
{
    /// <summary>
    /// Interaction logic for ChooseDraft.xaml
    /// </summary>
    public class ChooseDraftViewModel : ViewModelBase
    {
        private readonly string header = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>";
        private bool RefreshEnable;
        private GMessage item;
        private int index;

        public string MyEmailContent => "Tài khoản hiện tại: " + GMService.UserEmail;
        public ObservableCollection<GMessage> ListSource { get; set; }
        

        public ICommand LogoutCmd { get; set; }
        public ICommand RefreshCmd { get; set; }
        public ICommand NextCmd { get; set; }
        public bool CanRefresh
        {
            get
            {
                return RefreshEnable;
            }
            set
            {
                Set(nameof(RefreshEnable), ref RefreshEnable, value);
            }
        }
        public GMessage SelectedItem
        {
            get
            {
                return item;
            }
            set
            {
                Set(nameof(item), ref item, value);
                Messenger.Default.Send(new ChangeHTMLContent(header + ((item == null) ? "" : item.HTMLContent)));
            }
        }

        public int SelectedIndex
        {
            get
            {
                return index;
            }
            set
            {
                Set(nameof(index), ref index, value);
            }
        }

        public ChooseDraftViewModel()
        {
            Logs.Write("Đã login bằng " + GMService.UserEmail);
            ListSource = new ObservableCollection<GMessage>();
            Task.Run(new Action(Process));
            RefreshEnable = true;
            NextCmd = new RelayCommand(() =>
                {
                    if (SelectedIndex < 0) return;
                    UserSettings.ChoiceMailID = ListSource[SelectedIndex].MessageId;
                    Logs.Write("Đã chọn draft " + UserSettings.ChoiceMailID);
                    Messenger.Default.Send(new NavigateToMessage(null));
                }
            );

            LogoutCmd = new RelayCommand(() =>
                {
                    var tmp = Directory.GetFiles("4xR24anAtrw2ajpqW45SVB56saAfas");
                    foreach (var value in tmp)
                    {
                        File.Delete(value);
                    }
                    Logs.Write("Logout");
                    Messenger.Default.Send(new NavigateToMessage(new LoginView()));
                }
            );

            RefreshCmd = new RelayCommand(() =>
                {
                    if (!RefreshEnable) return;
                    RefreshEnable = false;
                    ListSource.Clear();
                    Messenger.Default.Send(new ChangeHTMLContent(header));
                    SelectedIndex = -1;
                    Logs.Write("Đã refresh");
                    Task.Run(new Action(Process));
                }
            );
        }

        private void Process()
        {
            var tmp = GMService.GetDraft();
            if (tmp != null) foreach (var value in tmp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        ListSource.Add(value);
                    });
                }
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Logs.Write("Đã load mail");
                RefreshEnable = true;
            });
        }

        private void Choose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (index < 0) return;
            UserSettings.ChoiceMailID = ListSource[SelectedIndex].MessageId;
            Logs.Write("Đã chọn draft " + UserSettings.ChoiceMailID);
            //Navigator.Navigate(new Uri("ChooseSheet.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var tmp = Directory.GetFiles("token");
            foreach (var value in tmp)
            {
                File.Delete(value);
            }
            Logs.Write("Logout");
            //Navigator.Navigate("Login.xaml");
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshEnable = false;
            ListSource.Clear();
            SelectedIndex = -1;
            Logs.Write("Đã refresh");
            Task.Run(new Action(Process));
        }
    }
}
