using CurrencyExchange.Client.ExchangeServiceReference;
using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Client.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class ExchangeRatesViewModel : ViewModelBase
    {
        private readonly ExchangeServiceClient _client = new ExchangeServiceClient();

        private ObservableCollection<ExchangeRate> _exchangeRates = new ObservableCollection<ExchangeRate>();
        public ObservableCollection<ExchangeRate> ExchangeRates
        {
            get => _exchangeRates;
            set => SetProperty(ref _exchangeRates, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoadRatesCommand { get; }

        public ExchangeRatesViewModel()
        {
            LoadRatesCommand = new RelayCommand(async _ => await LoadRatesAsync());
            // Auto-load on startup
            LoadRatesCommand.Execute(null);
        }

        private async System.Threading.Tasks.Task LoadRatesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                ExchangeRates.Clear();

                var rates = await _client.GetExchangeRatesAsync();

                foreach (var r in rates.Select(r => new ExchangeRate
                {
                    CurrencyCode = r.CurrencyCode,
                    CurrencyName = r.CurrencyName,
                    Mid = r.Mid
                }))
                {
                    ExchangeRates.Add(r);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load rates: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
