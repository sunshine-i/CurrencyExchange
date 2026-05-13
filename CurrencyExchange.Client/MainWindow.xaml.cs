using CurrencyExchange.Client.ViewModels;
using CurrencyExchange.Database.Models;
using System.Windows;

namespace CurrencyExchange.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();
            var vm = new MainViewModel(user);
            vm.LogoutRequested += () =>
            {
                Close();
                App.ShowLogin();
            };
            DataContext = vm;
        }
    }
}