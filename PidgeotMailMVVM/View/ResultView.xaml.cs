using System;
using System.Windows;
using System.Windows.Controls;

using GalaSoft.MvvmLight.Messaging;

using PidgeotMailMVVM.MessageForUI;

namespace PidgeotMailMVVM.View
{
	/// <summary>
	/// Interaction logic for ResultView.xaml
	/// </summary>
	public partial class ResultView : Page
	{
		public ResultView()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Register<ResultMessage>(this, t => Update(t));
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister<ResultMessage>(this, t => Update(t));
		}

		private void Update (ResultMessage r)
		{
			App.Current.Dispatcher.BeginInvoke((Action)delegate ()
			{
				Warning.Text = r.Message;
				Home.IsEnabled = r.IsEnable;
			});
		}
	}
}
