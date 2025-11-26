using Amora.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Amora.Views
{
    public partial class LikedPage : Page
    {
        private DBManager dbManager; // Datenbankmanager-Instanz
        private User currentUser; // Aktuell eingeloggter User

        public ObservableCollection<User> LikedUsers { get; set; } // ObservableCollection für die gebundene Liste von gelikten Usern
        private List<User> AllLikedUsers { get; set; } // Liste von alle Usern die geliked wurden

        public LikedPage(User currentUser)
        {
            InitializeComponent();
            dbManager = new DBManager();
            this.currentUser = currentUser;
            LikedUsers = new ObservableCollection<User>();
            DataContext = this;

            LoadLikedUsers();
        }


        private void LoadLikedUsers()
        {
            LikedUsers.Clear();
            AllLikedUsers = dbManager.GetLikedUsers(currentUser.Id);  // Hole alle gelikten User aus der Datenbank

            foreach (var user in AllLikedUsers)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string uploadsFolder = Path.Combine(documentsPath, "Amora", "Images", "UserUploads");
                string imagePath = Path.Combine(uploadsFolder, user.ProfilePictureUrl);

                if (!File.Exists(imagePath))
                { // Lade Standardbild, wenn kein Bild gefunden wurde
                    imagePath = Path.Combine(documentsPath, "Amora", "Images", "default.png");
                }

                user.ProfilePictureUrl = imagePath;
                LikedUsers.Add(user);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();
            LikedUsers.Clear();

            // Filtere die Liste basierend auf dem Suchtext
            var filtered = AllLikedUsers.Where(u =>
                u.Username.ToLower().Contains(searchText) ||
                u.Bio.ToLower().Contains(searchText)).ToList();

            foreach (var user in filtered)
            { // Füge die gefilterten User zur ObservableCollection hinzu
                LikedUsers.Add(user);
            }
        }


    }
}
