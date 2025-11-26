using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Amora.Resources;

namespace Amora.Views
{
    public partial class SettingsPage : Page
    {
        private readonly DBManager dbManager = new DBManager();
        private User currentUser;
        private string newProfilePictureFileName = null;

        public SettingsPage(User user)
        {
            InitializeComponent();
            currentUser = user;

            // Felder mit aktuellen Werten füllen
            UsernameTextBox.Text = currentUser.Username;
            EmailTextBox.Text = currentUser.Email;
            LocationTextBox.Text = currentUser.Location;
            BioTextBox.Text = currentUser.Bio;
        }

        private void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Neues Profilbild auswählen",
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

                    newProfilePictureFileName = fileName; // Nur Dateiname für DB

                    MessageBox.Show($"Neues Profilbild gespeichert unter: {destinationPath}", "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Kopieren des Bildes: {ex.Message}", "Fehler");
                }
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text.Trim();
            string newEmail = EmailTextBox.Text.Trim();
            string newPassword = NewPasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();
            string newLocation = LocationTextBox.Text.Trim();
            string newBio = BioTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newEmail))
            {
                MessageBox.Show("Benutzername und E-Mail dürfen nicht leer sein.", "Fehler");
                return;
            }

            if (!string.IsNullOrEmpty(newPassword) && newPassword != confirmPassword)
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein.", "Fehler");
                return;
            }

            bool success = dbManager.UpdateUserSettings(
                currentUser.Id,
                newUsername,
                newEmail,
                string.IsNullOrEmpty(newPassword) ? null : newPassword,
                newLocation,
                newBio,
                newProfilePictureFileName
            );

            if (success)
            {
                MessageBox.Show("Einstellungen erfolgreich gespeichert!", "Info");

                // Aktualisiere currentUser
                currentUser.Username = newUsername;
                currentUser.Email = newEmail;
                currentUser.Location = newLocation;
                currentUser.Bio = newBio;
                if (!string.IsNullOrEmpty(newProfilePictureFileName))
                    currentUser.ProfilePictureUrl = newProfilePictureFileName;
            }
            else
            {
                MessageBox.Show("Fehler beim Speichern der Einstellungen.", "Fehler");
            }
        }
    }
}