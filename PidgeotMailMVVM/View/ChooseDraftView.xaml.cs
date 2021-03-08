using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.MessageForUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PidgeotMailMVVM.View
{
    /// <summary>
    /// Interaction logic for ChooseDraftView.xaml
    /// </summary>
    public partial class ChooseDraftView : Page
    {
        public ChooseDraftView()
        {
            InitializeComponent();
        }

        protected void OnNavigatedTo(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<ChangeHTMLContent>(this, t => HTMLToWeb(t));
        }

        protected void OnNavigatingFrom(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<ChangeHTMLContent>(this, t => HTMLToWeb(t));
        }

        public void HTMLToWeb (ChangeHTMLContent c)
        {
            wb.NavigateToString(c.content);
        }
    }
}
