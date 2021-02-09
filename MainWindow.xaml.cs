using System.Windows;
using System;

namespace PidgeotMail
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigator.Navigate(new Uri("Login.xaml", UriKind.RelativeOrAbsolute));
            Title = App.ApplicationName;
        }
    }
}
