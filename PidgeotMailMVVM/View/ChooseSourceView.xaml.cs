using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		private void UpdatePath (ChangePathMessage t)
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
