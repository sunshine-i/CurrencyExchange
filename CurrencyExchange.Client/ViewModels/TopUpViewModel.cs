using CurrencyExchange.Client.ExchangeServiceReference;
using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Client.Models;
using CurrencyExchange.Database.Models;
using CurrencyExchange.Database.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class TopUpViewModel : ViewModelBase
    {
        private readonly DatabaseService _db = new DatabaseService();
        private readonly ExchangeServiceClient _client = new ExchangeServiceClient();
        private readonly User _user;

        private ObservableCollection<string> _availableCurrencies = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableCurrencies
        {
            get => _availableCurrencies;
            set => SetProperty(ref _availableCurrencies, value);
        }

        private string _selectedCurrency;
        public string SelectedCurrency
        {
            get => _selectedCurrency;
            set => SetProperty(ref _selectedCurrency, value);
        }

        private string _amountText = "";
        public string AmountText
        {
            get => _amountText;
            set => SetProperty(ref _amountText, value);
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            private set => SetProperty(ref _amount, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private ObservableCollection<Balance> _balances = new ObservableCollection<Balance>();
        public ObservableCollection<Balance> Balances
        {
            get => _balances;
            set => SetProperty(ref _balances, value);
        }

        public ICommand TopUpCommand { get; }

        public TopUpViewModel(User user)
        {
            _user = user;
            TopUpCommand = new RelayCommand(async _ => await ExecuteTopUpAsync());
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var rates = await _client.GetExchangeRatesAsync();
                AvailableCurrencies.Clear();
                AvailableCurrencies.Add("PLN");
                foreach (var code in rates.Select(r => r.CurrencyCode))
                    AvailableCurrencies.Add(code);

                SelectedCurrency = "PLN";
                RefreshBalances();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load currencies: {ex.Message}";
            }
        }

        private void RefreshBalances()
        {
            Balances.Clear();
            foreach (var b in _db.GetBalances(_user.UserId))
                Balances.Add(b);
        }

        private async System.Threading.Tasks.Task ExecuteTopUpAsync()
        {
            ErrorMessage = null;
            SuccessMessage = null;

            if (string.IsNullOrEmpty(SelectedCurrency))
            {
                ErrorMessage = "Please select a currency.";
                return;
            }

            string normalised = AmountText?.Replace(',', '.') ?? "";
            if (!double.TryParse(normalised,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double amount) || amount <= 0)
            {
                ErrorMessage = "Please enter a valid amount greater than zero.";
                return;
            }

            try
            {
                _db.TopUp(_user.UserId, SelectedCurrency, amount);
                SuccessMessage = $"Successfully added {amount:N2} {SelectedCurrency} to your account.";
                AmountText = "";
                RefreshBalances();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Top-up failed: {ex.Message}";
            }
        }
    }
}
