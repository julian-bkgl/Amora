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
        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0; // Wenn es bereits Einträge gibt:  return = True
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

        // Fügt einen neuen Benutzer hinzu (z. B. Registrierung)
        public bool RegisterUser(string username, string password)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // prüfen, ob Benutzer schon existiert
                    //string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    //using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    //{
                    //    checkCmd.Parameters.AddWithValue("@username", username);
                    //    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    //    if (count > 0)
                    //        return false;
                    //}

                    if (ValidateUserName(username)) { return false; }

                    string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";

                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
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
    }
}
