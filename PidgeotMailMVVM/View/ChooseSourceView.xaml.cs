using System.Windows;
using System.Windows.Controls;

using GalaSoft.MvvmLight.Messaging;

using PidgeotMailMVVM.MessageForUI;

namespace PidgeotMailMVVM.View
{
	/// <summary>
	/// Interaction logic for ChooseSourceView.xaml
	/// </summary>
	public partial class ChooseSourceView : Page
	{
		public ChooseSourceView()
		{
			InitializeComponent();
		}

		private void UpdatePath(ChangePathMessage t)
		{
			Ex.Text = t.Path;
			Ex.ToolTip = t.Path;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Register<ChangePathMessage>(this, t => UpdatePath(t));
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister<ChangePathMessage>(this, t => UpdatePath(t));
		}
	}
}
