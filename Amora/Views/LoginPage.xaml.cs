using Amora.Resources;
using Amora.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Amora.Pages
{
    public partial class LoginPage : Page
    {
        private readonly DBManager dbManager = new DBManager(); 
        private MainWindow _mainWindow;

        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateAndLogin();
        }
        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateAndLogin();
            }
        }


        private void ValidateAndLogin()
        {
            var input = UsernameTextBox.Text?.Trim(); // Kann Username oder Email sein
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Bitte Benutzername/E-Mail und Passwort eingeben.",
                                "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dbManager.ValidateUser(input, password))
            {
                User user = dbManager.LoginUser(input, password);
                if (user != null)
                {
                    _mainWindow.NavigateToHome(user);
                }
                else
                {
                    MessageBox.Show("User ist null", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (!dbManager.ValidateUserName(input)) // Prüft nur Username, nicht Email
                {
                    _mainWindow.NavigateToRegister(input, password);
                }
                else
                {
                    MessageBox.Show("Dein Benutzername/E-Mail oder Passwort war leider falsch.", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
