using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Database.Models;
using System;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly User _user;

        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string Username => _user?.Username;

        public ICommand ShowRatesCommand { get; }
        public ICommand ShowExchangeCommand { get; }
        public ICommand ShowTopUpCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand LogoutCommand { get; }

        // Raised when the user clicks Logout — App.xaml.cs / MainWindow handles it
        public event Action LogoutRequested;

        public MainViewModel(User user)
        {
            _user = user;

            ShowRatesCommand = new RelayCommand(_ => CurrentView = new ExchangeRatesViewModel());
            ShowExchangeCommand = new RelayCommand(_ => CurrentView = new ExchangeViewModel(_user));
            ShowTopUpCommand = new RelayCommand(_ => CurrentView = new TopUpViewModel(_user));
            ShowHistoryCommand = new RelayCommand(_ => CurrentView = new TransactionHistoryViewModel(_user));
            LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());

            // Default view on startup
            CurrentView = new ExchangeRatesViewModel();
        }
    }
}
