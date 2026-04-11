using CurrencyExchange.Client.ExchangeServiceReference;
using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class ExchangeViewModel : ViewModelBase
    {
        private readonly ExchangeServiceClient _client = new ExchangeServiceClient();

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
            set => SetProperty(ref _fromCurrency, value);
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

        public ExchangeViewModel()
        {
            LoadCurrenciesCommand = new RelayCommand(async _ => await LoadCurrenciesAsync());
            PerformExchangeCommand = new RelayCommand(async _ => await PerformExchangeAsync());
            LoadCurrenciesCommand.Execute(null);
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

                var request = new ExchangeRequestDto
                {
                    FromCurrency = FromCurrency,
                    ToCurrency = ToCurrency,
                    Amount = Amount
                };

                var response = await _client.ExchangeCurrencyAsync(request);

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
