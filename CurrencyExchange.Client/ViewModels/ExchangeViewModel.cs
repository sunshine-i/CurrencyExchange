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
    public class ExchangeViewModel : ViewModelBase
    {
        private readonly ExchangeServiceClient _client = new ExchangeServiceClient();
        private readonly DatabaseService _db = new DatabaseService();
        private readonly User _user;

        private ObservableCollection<string> _availableCurrencies = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableCurrencies
        {
            get => _availableCurrencies;
            set => SetProperty(ref _availableCurrencies, value);
        }

        private string _fromCurrency;
        public string FromCurrency
        {
            get => _fromCurrency;
            set
            {
                SetProperty(ref _fromCurrency, value);
                UpdateFromBalance();
            }
        }

        private string _toCurrency;
        public string ToCurrency
        {
            get => _toCurrency;
            set => SetProperty(ref _toCurrency, value);
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private double _fromBalance;
        public double FromBalance
        {
            get => _fromBalance;
            set => SetProperty(ref _fromBalance, value);
        }

        private bool _hasFromBalance;
        public bool HasFromBalance
        {
            get => _hasFromBalance;
            set => SetProperty(ref _hasFromBalance, value);
        }

        private ExchangeResult _result;
        public ExchangeResult Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoadCurrenciesCommand { get; }
        public ICommand PerformExchangeCommand { get; }

        public ExchangeViewModel(User user)
        {
            _user = user;
            LoadCurrenciesCommand = new RelayCommand(async _ => await LoadCurrenciesAsync());
            PerformExchangeCommand = new RelayCommand(async _ => await PerformExchangeAsync());
            LoadCurrenciesCommand.Execute(null);
        }

        private void UpdateFromBalance()
        {
            if (_user == null || string.IsNullOrEmpty(FromCurrency))
            {
                HasFromBalance = false;
                return;
            }
            FromBalance = _db.GetBalance(_user.UserId, FromCurrency);
            HasFromBalance = true;
        }

        private async System.Threading.Tasks.Task LoadCurrenciesAsync()
        {
            try
            {
                ErrorMessage = null;
                var rates = await _client.GetExchangeRatesAsync();
                AvailableCurrencies.Clear();
                AvailableCurrencies.Add("PLN");
                foreach (var code in rates.Select(r => r.CurrencyCode))
                    AvailableCurrencies.Add(code);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load currencies: {ex.Message}";
            }
        }

        private async System.Threading.Tasks.Task PerformExchangeAsync()
        {
            try
            {
                ErrorMessage = null;
                Result = null;

                if (string.IsNullOrEmpty(FromCurrency) || string.IsNullOrEmpty(ToCurrency))
                {
                    ErrorMessage = "Please select both currencies.";
                    return;
                }

                if (Amount <= 0)
                {
                    ErrorMessage = "Amount must be greater than zero.";
                    return;
                }

                double balance = _db.GetBalance(_user.UserId, FromCurrency);
                if (balance < Amount)
                {
                    ErrorMessage = $"Insufficient balance. You have {balance:N2} {FromCurrency}, need {Amount:N2}.";
                    return;
                }

                var request = new ExchangeRequestDto
                {
                    FromCurrency = FromCurrency,
                    ToCurrency = ToCurrency,
                    Amount = Amount
                };

                var response = await _client.ExchangeCurrencyAsync(request);

                double rate = response.Rate?.FirstOrDefault()?.Mid ?? 0.0;

                _db.RecordExchange(_user.UserId, FromCurrency, ToCurrency, Amount, response.Amount, rate);

                Result = new ExchangeResult
                {
                    Amount = response.Amount,
                    Timestamp = response.Timestamp,
                    Rate = response.Rate?.Select(r => new RateInfo
                    {
                        Currency = r.Currency,
                        Code = r.Code,
                        Mid = r.Mid
                    }).ToList()
                };

                UpdateFromBalance();
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (System.ServiceModel.FaultException ex)
            {
                ErrorMessage = $"Exchange error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
        }
    }
}
