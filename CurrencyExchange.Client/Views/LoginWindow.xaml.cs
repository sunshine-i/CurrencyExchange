using CurrencyExchange.Client.ViewModels;
using CurrencyExchange.Database.Models;
using System.Windows;

namespace CurrencyExchange.Client.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public User LoggedInUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _vm = new LoginViewModel();
            DataContext = _vm;

            _vm.LoginSucceeded += user =>
            {
                LoggedInUser = user;
                DialogResult = true;
            };

            _vm.NavigateToRegister += () =>
            {
                Hide();

                var registerWindow = new RegisterWindow();
                registerWindow.Show();

                registerWindow.RegisterSucceeded += user =>
                {
                    LoggedInUser = user;
                    registerWindow.Close();
                    DialogResult = true;
                };

                registerWindow.BackToLogin += () =>
                {
                    registerWindow.Close();
                    Show();
                };
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = PasswordBox.Password;
        }
    }
}
