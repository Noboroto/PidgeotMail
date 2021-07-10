using GalaSoft.MvvmLight.Messaging;

using PidgeotMail.MessageForUI;
using PidgeotMail.View;

using System.Windows;


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
			MyFrame.Navigate(new LoginView());
		}

		protected void OnNavigatedTo(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Register<NavigateToMessage>(this, t => NavigateTo(t));
			Messenger.Default.Register<GoBackMessage>(this, (t) => GoBack(t));

		}

		protected void OnNavigatingFrom(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister<NavigateToMessage>(this, t => NavigateTo(t));
		}

		public void NavigateTo(NavigateToMessage t)
		{
			MyFrame.Navigate(t.Target);
		}
		public void GoBack(GoBackMessage t)
		{
			MyFrame.GoBack();
		}
	}
}
