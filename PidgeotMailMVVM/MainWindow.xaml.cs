using PidgeotMailMVVM.View;
using System.Windows;


namespace PidgeotMailMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyFrame.Navigate(new LoginView());
        }
    }
}
