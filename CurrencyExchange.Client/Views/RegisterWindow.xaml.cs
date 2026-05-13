using CurrencyExchange.Client.ViewModels;
using CurrencyExchange.Database.Models;
using System;
using System.Windows;

namespace CurrencyExchange.Client.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _vm;

        public event Action<User> RegisterSucceeded;
        public event Action BackToLogin;

        public RegisterWindow()
        {
            InitializeComponent();
            _vm = new RegisterViewModel();
            DataContext = _vm;

            _vm.RegisterSucceeded += user => RegisterSucceeded?.Invoke(user);
            _vm.NavigateToLogin += () => BackToLogin?.Invoke();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
}
