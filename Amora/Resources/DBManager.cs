using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Amora.Resources
{
    public class DBManager
    {
        private readonly string connectionString =
        "Server=localhost;Database=dating_app;Uid=root;Pwd=test123#";

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // prüft Benutzername + Passwort in der Datenbank
        public bool ValidateUser(string usernameOrEmail, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE (username = @input OR email = @input) AND password = @password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@input", usernameOrEmail);
                        cmd.Parameters.AddWithValue("@password", password);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei ValidateUser(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        // Prüft nur Benutzername ohne Passwort in der Datenbank
        public bool ValidateUserName(string username)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0; // Wenn es bereits Einträge gibt:  return = True
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show($"Fehler bei ValidateUserName(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Fügt einen neuen Benutzer hinzu (Registrierung)
        public bool RegisterUser(string username, string password, string email, string gender, string birthdate, string location, string bio, string profilePicturePath)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // prüfen, ob Benutzer schon existiert
                    if (ValidateUserName(username)) { return false; }


                    string fileName = string.IsNullOrEmpty(profilePicturePath) ? "default.png" : System.IO.Path.GetFileName(profilePicturePath);

                    string insertQuery = "INSERT INTO users (username, password, email, gender, birthdate, location, bio, profile_picture_url) VALUES (@username, @password, @email, @gender, @birthdate, @location, @bio, @profile_picture_url)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@gender", gender);
                        cmd.Parameters.AddWithValue("@birthdate", birthdate);
                        cmd.Parameters.AddWithValue("@location", location);
                        cmd.Parameters.AddWithValue("@bio", bio);
                        cmd.Parameters.AddWithValue("@profile_picture_url", fileName);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei RegisterUser(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Methode um User-Objekt bei Login zu bekommen
        public User LoginUser(string usernameOrEmail, string password)
        {
            string query = "SELECT * FROM users WHERE (username = @input OR email = @input) AND password = @password";
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@input", usernameOrEmail);
                    command.Parameters.AddWithValue("@password", password);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : 0,
                                Username = reader["username"] as string ?? string.Empty,
                                Email = reader["email"] as string ?? string.Empty,
                                Password = reader["password"] as string ?? string.Empty,
                                Gender = reader["gender"] as string ?? string.Empty,
                                Birthdate = reader["birthdate"] != DBNull.Value ? Convert.ToDateTime(reader["birthdate"]) : DateTime.MinValue,
                                Location = reader["location"] as string ?? string.Empty,
                                Bio = reader["bio"] as string ?? string.Empty,
                                ProfilePictureUrl = reader["profile_picture_url"] as string ?? string.Empty
                            };
                        }
                    }
                }
            }
            return null;
        }


        public void SaveAction(int from_user_id, int to_user_id, string action)
        { // Trägt in interactions-Tabelle ein, ob ein User einen anderen User geliket oder gedisliket hat
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO interactions (from_user_id, to_user_id ,action) VALUES (@from_user_id, @to_user_id ,@action)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@from_user_id", from_user_id); // nimmt Klassen-Variable vom eingeloggten User
                        cmd.Parameters.AddWithValue("@to_user_id", to_user_id);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei SaveAction(): {ex.Message}", "[FeedPage]", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool UpdateUserSettings(int userId, string username, string email, string password, string location, string bio, string profilePictureFileName)
        { // Ändert DB-Einträge für einen User ( sprich Name,Email,Password,Ort,Bio,Profilbild )
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Dynamisches Update-Statement
                    string updateQuery = @"UPDATE users SET username = @username, email = @email, location = @location, bio = @bio";

                    if (!string.IsNullOrEmpty(password))
                        updateQuery += ", password = @password";

                    if (!string.IsNullOrEmpty(profilePictureFileName))
                        updateQuery += ", profile_picture_url = @profile_picture_url";

                    updateQuery += " WHERE user_id = @user_id";

                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@location", location);
                        cmd.Parameters.AddWithValue("@bio", bio);
                        cmd.Parameters.AddWithValue("@user_id", userId);

                        if (!string.IsNullOrEmpty(password))
                            cmd.Parameters.AddWithValue("@password", password);

                        if (!string.IsNullOrEmpty(profilePictureFileName))
                            cmd.Parameters.AddWithValue("@profile_picture_url", profilePictureFileName);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei UpdateUserSettings(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        public bool CheckForMatch(int fromUserId, int toUserId)
        { // Überprüft ob sich zwei User gegenseitig geliket haben
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM interactions WHERE from_user_id = @toUserId AND to_user_id = @fromUserId AND action = 'like'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // true = Match
                }
            }
        }
        public void SaveMatch(int userId1, int userId2)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Prüfen, ob der Match bereits existiert (damit keine Duplikate entstehen)
                    string checkQuery = "SELECT COUNT(*) FROM matches WHERE " +
                                        "(user1_id = @userId1 AND user2_id = @userId2) OR " +
                                        "(user1_id = @userId2 AND user2_id = @userId1)";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId1", userId1);
                        checkCmd.Parameters.AddWithValue("@userId2", userId2);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0) return; // Match existiert schon
                    }

                    // Wenn nicht vorhanden, neuen Match eintragen
                    string insertQuery = "INSERT INTO matches (user1_id, user2_id) VALUES (@userId1, @userId2)";
                    using (var insertCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@userId1", userId1);
                        insertCmd.Parameters.AddWithValue("@userId2", userId2);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei SaveMatch(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<User> GetMatchesForUser(int userId)
        { // Methode um die gematchten User zu bekommen ( für MatchesPage )
            var matchedUsers = new List<User>();

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Alle Matches für den angegebenen User finden
                    string matchQuery = "SELECT user1_id, user2_id FROM matches WHERE user1_id = @userId OR user2_id = @userId";
                    using (var matchCmd = new MySqlCommand(matchQuery, conn))
                    {
                        matchCmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = matchCmd.ExecuteReader())
                        {
                            var otherUserIds = new List<int>();

                            while (reader.Read())
                            {
                                int user1 = reader["user1_id"] != DBNull.Value ? Convert.ToInt32(reader["user1_id"]) : 0;
                                int user2 = reader["user2_id"] != DBNull.Value ? Convert.ToInt32(reader["user2_id"]) : 0;

                                // Finde die andere ID
                                if (user1 == userId && user2 != 0)
                                    otherUserIds.Add(user2);
                                else if (user2 == userId && user1 != 0)
                                    otherUserIds.Add(user1);
                            }

                            reader.Close();

                            // Hole die User-Daten für alle anderen IDs
                            foreach (var id in otherUserIds.Distinct())
                            {
                                string userQuery = "SELECT * FROM users WHERE user_id = @id";
                                using (var userCmd = new MySqlCommand(userQuery, conn))
                                {
                                    userCmd.Parameters.AddWithValue("@id", id);
                                    using (var userReader = userCmd.ExecuteReader())
                                    {
                                        if (userReader.Read())
                                        {
                                            matchedUsers.Add(new User
                                            {
                                                Id = userReader["user_id"] != DBNull.Value ? Convert.ToInt32(userReader["user_id"]) : 0,
                                                Username = userReader["username"] as string ?? string.Empty,
                                                Email = userReader["email"] as string ?? string.Empty,
                                                Password = userReader["password"] as string ?? string.Empty,
                                                Gender = userReader["gender"] as string ?? string.Empty,
                                                Birthdate = userReader["birthdate"] != DBNull.Value ? Convert.ToDateTime(userReader["birthdate"]) : DateTime.MinValue,
                                                Location = userReader["location"] as string ?? string.Empty,
                                                Bio = userReader["bio"] as string ?? string.Empty,
                                                ProfilePictureUrl = userReader["profile_picture_url"] as string ?? string.Empty
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei GetMatchesForUser(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return matchedUsers;
        }

        public List<User> GetLikedUsers(int userId)
        { // Methode um die Geliketen User zu bekommen ( für LikedPage )
            var likedUsers = new List<User>();

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Hole alle User, die vom aktuellen User geliked wurden
                    string query = @"
                        SELECT u.* FROM users u
                        INNER JOIN interactions i ON u.user_id = i.to_user_id
                        WHERE i.from_user_id = @userId AND i.action = 'like'
                        AND u.user_id NOT IN (
                            SELECT user2_id FROM matches WHERE user1_id = @userId
                            UNION
                            SELECT user1_id FROM matches WHERE user2_id = @userId
                        )";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                likedUsers.Add(new User
                                {
                                    Id = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : 0,
                                    Username = reader["username"] as string ?? string.Empty,
                                    Email = reader["email"] as string ?? string.Empty,
                                    Password = reader["password"] as string ?? string.Empty,
                                    Gender = reader["gender"] as string ?? string.Empty,
                                    Birthdate = reader["birthdate"] != DBNull.Value ? Convert.ToDateTime(reader["birthdate"]) : DateTime.MinValue,
                                    Location = reader["location"] as string ?? string.Empty,
                                    Bio = reader["bio"] as string ?? string.Empty,
                                    ProfilePictureUrl = reader["profile_picture_url"] as string ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei GetLikedUsers(): {ex.Message}", "[DBManager]", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return likedUsers;
        }

    }
}
