using System.Windows;
using System;
using Microsoft.Win32;
using System.IO;

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if(saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, Logs.Get());
        }
    }
}
