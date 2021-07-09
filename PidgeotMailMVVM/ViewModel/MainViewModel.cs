using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using PidgeotMail.Lib;

namespace PidgeotMail.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </para>
	/// <para>
	/// You can also use Blend to data bind with the tool's support.
	/// </para>
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}
			Tilte = "Phần mềm gửi mail tự động PidgeotMail";
			AppName = "PidgeotMail";
			RootFolderCmd = new RelayCommand(() =>
				{
					Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				}
			);
			InfoCmd = new RelayCommand(() =>
				{
					MessageBox.Show("Võ Thanh Tú\nEmail: thanhtuvo135@gmail.com");
				}
			);
		}

		public ICommand RootFolderCmd { get; set; }
		public ICommand InfoCmd { get; set; }

		public static string TokenFolder => AppDomain.CurrentDomain.BaseDirectory + "\\4xR24anAtrw2ajpqW45SVB56saAfas";
		public static string TempFolder => AppDomain.CurrentDomain.BaseDirectory + "\\temp";
		public static string Tilte { get; set; }
		public static string AppName { get; set; }
	}
}