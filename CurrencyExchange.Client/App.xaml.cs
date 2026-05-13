using CurrencyExchange.Client.Views;
using System.Windows;

namespace CurrencyExchange.Client
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Prevent the default MainWindow from opening automatically
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ShowLogin();
        }

        public static void ShowLogin()
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = new MainWindow(loginWindow.LoggedInUser);
                Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Current.Shutdown();
            }
        }
    }
}