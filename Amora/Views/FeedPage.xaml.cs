using Amora.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace Amora.Views
{

    public partial class FeedPage : Page
    {
        private DBManager dbManager;
        private Queue<User> userQueue;
        private User currentQueuedUser; // aktueller User der auf dem Feed steht
        private User currentUser; // aktueller eingeloggter Nutzer

        public FeedPage(User currentUser)
        {
            InitializeComponent();
            dbManager = new DBManager();

            this.currentUser = currentUser;

            LoadUsers();
            ShowNextUser();
        }



        private void LoadUsers()
        {
            userQueue = new Queue<User>(); // User-Warteschlange erstellen 
            string query = "SELECT user_id, username, bio, profile_picture_url FROM users WHERE user_id != @currentUserId"; // Alle User bekommen außer die ID des gerade eingeloggten Users

            using (var conn = new MySqlConnection("Server=localhost;Database=dating_app;Uid=root;Pwd=test123#"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUser.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userQueue.Enqueue(new User
                            {
                                Id = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : 0,
                                Username = reader["username"] as string ?? string.Empty,
                                Bio = reader["bio"] as string ?? string.Empty,
                                ProfilePictureUrl = reader["profile_picture_url"] as string ?? "/Images/default.png"
                            });
                        }
                    }
                }
            }
        }

        private void ShowNextUser()
        {
            if (LikeButton.Visibility != Visibility.Visible || DislikeButton.Visibility != Visibility.Visible)
            { // WEnn Funktion nochmals ausgeführt wird und die beiden Buttons nicht Sichtbar sind, vorsichtshalber einfach wieder Sichtbar machen.
                LikeButton.Visibility = Visibility.Visible; 
                DislikeButton.Visibility = Visibility.Visible;
            }

            if (userQueue.Count > 0)
            {
                currentQueuedUser = userQueue.Dequeue();
                UsernameText.Text = currentQueuedUser.Username;
                BioText.Text = currentQueuedUser.Bio;



                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string uploadsFolder = System.IO.Path.Combine(documentsPath, "Amora", "Images", "UserUploads");
                string imagePath = System.IO.Path.Combine(uploadsFolder, currentQueuedUser.ProfilePictureUrl);

                if (!File.Exists(imagePath))
                {
                    imagePath = System.IO.Path.Combine(documentsPath, "Amora", "Images", "default.png");
                }

                ProfileImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));



            }
            else
            {
                // Wenn kein User mehr in der Queue ist
                UsernameText.Text = "Keine weiteren Nutzer";
                BioText.Text = "";
                ProfileImage.Source = null;

                // Buttons unsichtbar machen
                LikeButton.Visibility = Visibility.Hidden;
                DislikeButton.Visibility = Visibility.Hidden;

            }
        }

        private void Like_Click(object sender, RoutedEventArgs e)
        {
            if (currentQueuedUser != null)
            {
                dbManager.SaveAction(currentUser.Id, currentQueuedUser.Id, "like");
                if (dbManager.CheckForMatch(currentUser.Id, currentQueuedUser.Id))
                {
                    // Nachricht das ein Match geschehen ist
                    dbManager.SaveMatch(currentUser.Id, currentQueuedUser.Id);
                }
                ShowNextUser();
            }
        }

        private void Dislike_Click(object sender, RoutedEventArgs e)
        {
            if (currentQueuedUser != null)
            {
                dbManager.SaveAction(currentUser.Id, currentQueuedUser.Id, "dislike");
                ShowNextUser();
            }
        }


    }
}
