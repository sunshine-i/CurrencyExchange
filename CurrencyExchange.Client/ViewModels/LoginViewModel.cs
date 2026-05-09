using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Database.Models;
using CurrencyExchange.Database.Services;
using System;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DatabaseService _db = new DatabaseService();

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        public event Action<User> LoginSucceeded;
        public event Action NavigateToRegister;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => ExecuteLogin());
            GoToRegisterCommand = new RelayCommand(_ => NavigateToRegister?.Invoke());
        }

        private void ExecuteLogin()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your username and password.";
                return;
            }

            try
            {
                var user = _db.Login(Username, Password);
                if (user == null)
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                LoginSucceeded?.Invoke(user);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
        }
    }
}
