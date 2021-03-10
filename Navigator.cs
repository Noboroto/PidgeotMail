using System;
using System.Windows;
using System.Windows.Navigation;

namespace PidgeotMail
{
	public static class Navigator
	{
		private static NavigationService NavigationService
		{
			get
			{
				return (Application.Current.MainWindow as MainWindow).f.NavigationService;
			}
		}

		public static void Navigate(string path, object param = null)
		{
			NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute), param);
		}

		public static void Navigate(Uri obj)
		{
			NavigationService.Navigate(obj);
		}

		public static void GoBack()
		{
			NavigationService.GoBack();
		}

		public static void GoForward()
		{
			NavigationService.GoForward();
		}
	}
}
