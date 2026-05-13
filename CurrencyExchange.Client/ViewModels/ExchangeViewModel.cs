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
                UpdatePreviewRateAsync();
            }
        }

        private string _toCurrency;
        public string ToCurrency
        {
            get => _toCurrency;
            set
            {
                SetProperty(ref _toCurrency, value);
                UpdatePreviewRateAsync();
            }
        }

        private string _amountText = "";
        public string AmountText
        {
            get => _amountText;
            set => SetProperty(ref _amountText, value);
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

        private string _previewRate;
        public string PreviewRate
        {
            get => _previewRate;
            set => SetProperty(ref _previewRate, value);
        }

        private bool _hasPreviewRate;
        public bool HasPreviewRate
        {
            get => _hasPreviewRate;
            set => SetProperty(ref _hasPreviewRate, value);
        }

        private bool _hasResult;
        public bool HasResult
        {
            get => _hasResult;
            set => SetProperty(ref _hasResult, value);
        }

        private double _resultAmount;
        public double ResultAmount
        {
            get => _resultAmount;
            set => SetProperty(ref _resultAmount, value);
        }

        private string _resultRate;
        public string ResultRate
        {
            get => _resultRate;
            set => SetProperty(ref _resultRate, value);
        }

        private DateTime _resultTimestamp;
        public DateTime ResultTimestamp
        {
            get => _resultTimestamp;
            set => SetProperty(ref _resultTimestamp, value);
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

        private async void UpdatePreviewRateAsync()
        {
            HasPreviewRate = false;
            PreviewRate = null;

            if (string.IsNullOrEmpty(FromCurrency) || string.IsNullOrEmpty(ToCurrency))
                return;
            if (FromCurrency == ToCurrency)
            {
                PreviewRate = $"1 {FromCurrency} = 1 {ToCurrency}";
                HasPreviewRate = true;
                return;
            }

            try
            {
                var request = new ExchangeRequestDto
                {
                    FromCurrency = FromCurrency,
                    ToCurrency = ToCurrency,
                    Amount = 1.0
                };
                var response = await _client.ExchangeCurrencyAsync(request);
                PreviewRate = $"1 {FromCurrency} = {response.Amount:N4} {ToCurrency}";
                HasPreviewRate = true;
            }
            catch
            {
                // Silently suppress
            }
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
                HasResult = false;

                if (string.IsNullOrEmpty(FromCurrency) || string.IsNullOrEmpty(ToCurrency))
                {
                    ErrorMessage = "Please select both currencies.";
                    return;
                }

                // Parse the string amount — handles "1.", "1.5", "1,5" etc.
                string normalised = AmountText?.Replace(',', '.') ?? "";
                if (!double.TryParse(normalised,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double amount) || amount <= 0)
                {
                    ErrorMessage = "Please enter a valid amount greater than zero.";
                    return;
                }

                double balance = _db.GetBalance(_user.UserId, FromCurrency);
                if (balance < amount)
                {
                    ErrorMessage = $"Insufficient balance. You have {balance:N4} {FromCurrency}, need {amount:N4}.";
                    return;
                }

                var request = new ExchangeRequestDto
                {
                    FromCurrency = FromCurrency,
                    ToCurrency = ToCurrency,
                    Amount = amount
                };

                var response = await _client.ExchangeCurrencyAsync(request);

                string rateString = $"1 {FromCurrency} = {response.Amount / amount:N4} {ToCurrency}";

                _db.RecordExchange(_user.UserId, FromCurrency, ToCurrency, amount, response.Amount, rateString);

                HasResult = true;
                ResultAmount = response.Amount;
                ResultRate = rateString;
                ResultTimestamp = response.Timestamp;

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
