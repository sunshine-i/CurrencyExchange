using CurrencyExchange.Client.ViewModels;
using CurrencyExchange.Database.Models;
using System.Windows;

namespace CurrencyExchange.Client.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _vm;

        public User RegisteredUser { get; private set; }

        public RegisterWindow()
        {
            InitializeComponent();
            _vm = new RegisterViewModel();
            DataContext = _vm;

            _vm.RegisterSucceeded += user =>
            {
                RegisteredUser = user;
                DialogResult = true;
            };

            _vm.NavigateToLogin += () =>
            {
                DialogResult = false;
                Close();
            };
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
