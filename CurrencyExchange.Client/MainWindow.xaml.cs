using CurrencyExchange.Client.ViewModels;
using CurrencyExchange.Client.Views;
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
                // Show login window again; if successful open a new MainWindow
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == true)
                {
                    var newMain = new MainWindow(loginWindow.LoggedInUser);
                    Application.Current.MainWindow = newMain;
                    newMain.Show();
                }
                Close();
            };
            DataContext = vm;
        }
    }
}
