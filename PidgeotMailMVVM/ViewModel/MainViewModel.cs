using AutoUpdaterDotNET;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;

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
					MessageBox.Show("Phiên bản " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\nTác giả: Võ Thanh Tú\nEmail: thanhtuvo135@gmail.com");
				}
			);
			ReportCmd = new RelayCommand(() => Process.Start(@"https://forms.gle/hDTY1YLHQrihNfZr9"));
            CheckUpdateCmd = new RelayCommand(() =>
			{
				AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                AutoUpdater.Synchronous = true;
				AutoUpdater.Start(@"https://raw.githubusercontent.com/Noboroto/PidgeotMailWeb/main/PidgeotMailAutoUpdate.xml");
                AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
            });
		}

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    dialogResult =
                            MessageBox.Show(
                                $@"Có phiên bản mới {args.CurrentVersion}. Bạn đang dùng phiên bản {
                                        args.InstalledVersion
                                    }. Bạn có muốn cập nhật ngay bây giờ không?", @"Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Application.Exit();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(@"Bạn đã ở phiên bản mới nhất", @"No update available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        public ICommand RootFolderCmd { get; set; }
		public ICommand InfoCmd { get; set; }
		public ICommand ReportCmd { get; set; }
		public ICommand CheckUpdateCmd { get; set; }

		public static string Tilte { get; set; }
		public static string AppName { get; set; }
	}
}