/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PidgeotMailMVVM"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;

using GalaSoft.MvvmLight.Ioc;

namespace PidgeotMail.ViewModel
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			////if (ViewModelBase.IsInDesignModeStatic)
			////{
			////    // Create design time view services and models
			////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
			////}
			////else
			////{
			////    // Create run time view services and models
			////    SimpleIoc.Default.Register<IDataService, DataService>();
			////}
			Register();
		}

		public static void Register ()
		{
			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<LoginViewModel>();
			SimpleIoc.Default.Register<ChooseDraftViewModel>();
			SimpleIoc.Default.Register<ChooseSourceViewModel>();
			SimpleIoc.Default.Register<AttachmentViewModel>();
			SimpleIoc.Default.Register<ResultViewModel>();
		}

		public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
		public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();
		public ChooseDraftViewModel ChooseDraft => ServiceLocator.Current.GetInstance<ChooseDraftViewModel>();
		public ChooseSourceViewModel ChooseSource => ServiceLocator.Current.GetInstance<ChooseSourceViewModel>();
		public AttachmentViewModel Attachment => ServiceLocator.Current.GetInstance<AttachmentViewModel>();
		public ResultViewModel Result => ServiceLocator.Current.GetInstance<ResultViewModel>();
		public static void Cleanup()
		{
			SimpleIoc.Default.Unregister<MainViewModel>();
			SimpleIoc.Default.Unregister<LoginViewModel>();
			SimpleIoc.Default.Unregister<ChooseDraftViewModel>();
			SimpleIoc.Default.Unregister<ChooseSourceViewModel>();
			SimpleIoc.Default.Unregister<AttachmentViewModel>();
			SimpleIoc.Default.Unregister<ResultViewModel>();
		}
	}
}