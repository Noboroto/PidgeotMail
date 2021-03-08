﻿using GalaSoft.MvvmLight.Messaging;
using PidgeotMailMVVM.MessageForUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PidgeotMailMVVM.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
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
            NavigationService.Navigate(t.Target);
        }
    }
}