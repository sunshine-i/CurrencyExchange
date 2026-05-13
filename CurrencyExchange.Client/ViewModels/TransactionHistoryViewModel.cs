using CurrencyExchange.Client.Helpers;
using CurrencyExchange.Database.Models;
using CurrencyExchange.Database.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CurrencyExchange.Client.ViewModels
{
    public class TransactionHistoryViewModel : ViewModelBase
    {
        private readonly DatabaseService _db = new DatabaseService();
        private readonly User _user;

        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
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

        public ICommand RefreshCommand { get; }

        public TransactionHistoryViewModel(User user)
        {
            _user = user;
            RefreshCommand = new RelayCommand(_ => LoadTransactions());
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                Transactions.Clear();
                foreach (var t in _db.GetTransactions(_user.UserId))
                    Transactions.Add(t);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load transactions: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
