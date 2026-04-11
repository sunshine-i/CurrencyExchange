using CurrencyExchange.Client.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand ShowRatesCommand { get; }
        public ICommand ShowExchangeCommand { get; }

        public MainViewModel()
        {
            ShowRatesCommand = new RelayCommand(_ => CurrentView = new ExchangeRatesViewModel());
            ShowExchangeCommand = new RelayCommand(_ => CurrentView = new ExchangeViewModel());

            // Default view on startup
            CurrentView = new ExchangeRatesViewModel();
        }
    }
}
