using Amora.Pages;
using Amora.Resources;
using Amora.Views;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Amora
{

    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent(); // UI wird geladen


            // System-Theme erkennen
            var systemTheme = ThemeManager.GetSystemTheme();
            ThemeManager.ApplyTheme(systemTheme);

            // Status setzen
            isDarkTheme = systemTheme == AppTheme.Dark;
            ThemeSwitchButton.Content = isDarkTheme ? "☀️" : "🌙";



            MainFrame.Navigate(new LoginPage(this));
        }

        public void NavigateToHome(User user) 
        {
            MainFrame.Navigate(new HomePage(this, user));
        }

        public void NavigateToRegister(String username, String password)
        {
            MainFrame.Navigate(new RegisterPage(this, username, password));
        }


        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            var theme = isDarkTheme ? AppTheme.Dark : AppTheme.Light;
            ThemeManager.ApplyTheme(theme);

            // Icon aktualisieren
            ThemeSwitchButton.Content = isDarkTheme ? "☀️" : "🌙";
        }

    }
}