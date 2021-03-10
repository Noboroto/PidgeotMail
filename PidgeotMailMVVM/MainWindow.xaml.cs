using GalaSoft.MvvmLight.Messaging;

using PidgeotMailMVVM.MessageForUI;
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

		protected void OnNavigatedTo(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Register<NavigateToMessage>(this, t => NavigateTo(t));
		}

		protected void OnNavigatingFrom(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister<NavigateToMessage>(this, t => NavigateTo(t));
		}

		public void NavigateTo(NavigateToMessage t)
		{
			MyFrame.Navigate(t.Target);
		}
	}
}
