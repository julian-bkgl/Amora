using Amora.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Amora.Views
{
    public partial class MatchesPage : Page
    {
        private DBManager dbManager;
        private User currentUser;
        public ObservableCollection<User> Matches { get; set; }
        private List<User> AllMatches { get; set; } // Vollständige Liste

        public MatchesPage(User currentUser)
        {
            InitializeComponent();
            dbManager = new DBManager();
            this.currentUser = currentUser;
            Matches = new ObservableCollection<User>();
            DataContext = this;
            LoadMatches();
        }

        private void LoadMatches()
        {
            Matches.Clear();
            AllMatches = dbManager.GetMatchesForUser(currentUser.Id);

            foreach (var user in AllMatches)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string uploadsFolder = Path.Combine(documentsPath, "Amora", "Images", "UserUploads");
                string imagePath = Path.Combine(uploadsFolder, user.ProfilePictureUrl);

                if (!File.Exists(imagePath))
                {
                    imagePath = Path.Combine(documentsPath, "Amora", "Images", "default.png");
                }

                user.ProfilePictureUrl = imagePath;
                Matches.Add(user);
            }
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();
            Matches.Clear();

            var filtered = AllMatches.Where(u =>
                u.Username.ToLower().Contains(searchText) ||
                u.Bio.ToLower().Contains(searchText)).ToList();

            foreach (var user in filtered)
            {
                Matches.Add(user);
            }
        }

        // TODO : Button zum Chatten für jedes User-item hinzufügen
    }
}
