using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Amora.Resources;

namespace Amora.Views
{

    public partial class RegisterPage : Page
    {
        private MainWindow _mainWindow;
        private readonly DBManager dbManager = new DBManager();

        private string profilePicturePath = null;

        public RegisterPage(MainWindow mainWindow, String username, String password)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            UsernameTextBox.Text = username; // Username direkt setzen weil ja bereits in der LoginPage angegeben und überprüft

            Loaded += (s, e) => RegisterPage_Loaded(s, e, password);
        }

        private void RegisterPage_Loaded(object sender, RoutedEventArgs e, string password)
        {
            PasswordBox.Password = password; // Password nach dem Laden der Seite setzen, damit es nicht zu problemen führt!
        }

        private void UploadProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Profilbild auswählen",
                Filter = "Bilddateien|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string uploadsFolder = Path.Combine(documentsPath, "Amora", "Images", "UserUploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string destinationPath = Path.Combine(uploadsFolder, fileName);

                    File.Copy(openFileDialog.FileName, destinationPath, true);

                    profilePicturePath = fileName; // Nur Dateiname für DB

                    MessageBox.Show($"Bild erfolgreich kopiert nach: {destinationPath}", "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Kopieren: {ex.Message}");
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Eingaben auslesen
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Geschlecht auslesen
            string gender = null;
            foreach (var radioButton in GenderPanel.Children.OfType<RadioButton>())
            {
                if (radioButton.IsChecked == true)
                {
                    gender = radioButton.Tag.ToString();
                    break;
                }
            }

            DateTime? birthdate = BirthDatePicker.SelectedDate;
            string location = LocationTextBox.Text.Trim();
            string bio = BioTextBox.Text.Trim();

            // Validierung
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Bitte füllen Sie alle Pflichtfelder aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!birthdate.HasValue)
            {
                MessageBox.Show("Bitte wählen Sie ein Geburtsdatum.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!email.Contains("@"))
            {
                MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string mysqlBirthdate = birthdate.Value.ToString("yyyy-MM-dd") ?? "NULL";

            bool registerd = dbManager.RegisterUser(username, password, email, gender, mysqlBirthdate, location, bio, profilePicturePath); // User in der Datenbank registrieren
            if (registerd) // Wenn TRUE dann ist alles gut gelaufen !
            {
                User newUser = dbManager.LoginUser(username, password);
                if (newUser != null)
                {
                    _mainWindow.NavigateToHome(newUser); // Weiter zur HomePage mit dem entsprechendem Login
                }else
                {
                    MessageBox.Show("User ist null", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string HashPassword(string password) // Theoretisch könnte man hiermit noch verschlüsseln ( will ich später noch einbauen) 
        {
            // Einfacher Hash (nur Beispiel, besser: BCrypt oder PBKDF2 verwenden)
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

    }
}
