using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Database.Models;
using CurrencyExchange.Database.Services;
using System;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly DatabaseService _db = new DatabaseService();

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public event Action<User> RegisterSucceeded;
        public event Action NavigateToLogin;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(_ => ExecuteRegister());
            GoToLoginCommand = new RelayCommand(_ => NavigateToLogin?.Invoke());
        }

        private void ExecuteRegister()
        {
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username cannot be empty.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 4)
            {
                ErrorMessage = "Password must be at least 4 characters.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            try
            {
                var user = _db.RegisterUser(Username, Password);
                if (user == null)
                {
                    ErrorMessage = "Username already taken. Please choose another.";
                    return;
                }

                RegisterSucceeded?.Invoke(user);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Registration failed: {ex.Message}";
            }
        }
    }
}
