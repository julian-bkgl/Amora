using Amora.Resources;
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

        private void ValidateAndLogin() // Funktion überprüft ob der Username und das Passwort eingetragen wurden
        {
            var username = UsernameTextBox.Text?.Trim();
            var password = PasswordBox.Password;
            MessageBox.Show(username, password);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            { // Gucken ob auch tatsächlich was geschrieben wurde
                MessageBox.Show("Bitte Benutzername und Passwort eingeben.",
                                "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dbManager.ValidateUser(username, password)) { // Abfrage : Gibt es die Userdaten in der DB | Wenn es den Usernamen und Passwort in der DB gibt dann gehts weiter
                _mainWindow.NavigateToHome(); // Normaler Login
            }
            else
            {
                if (!dbManager.ValidateUserName(username)) 
                {
                    // TODO : Registrierungsseite öffnen
                    dbManager.RegisterUser(username, password); // Registrieren der Daten wenn nicht bereits vorhanden sind !
                    MessageBox.Show("Du hast ein Konto erstellt!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }else
                {
                    MessageBox.Show("Dein Benutzername oder Passwort war leider Falsch.", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
