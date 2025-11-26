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

namespace Amora.Views
{
    public partial class HomePage : Page
    {
        // --------------------------------------------------
        // Wird eigentlich nicht mehr benötigt aber als Erinnerung mal hier lassen
        // private MainWindow _mainWindow; 
        // private User currentUser;
        // --------------------------------------------------

        // Seiten einmal Rendern lassen, dann wird Seite nicht immer neu geladen wenn sie per Button geöffnet wird
        private SettingsPage settingsPage;
        private LikedPage likedPage;
        private MatchesPage matchesPage;
        private FeedPage feedPage;

        public HomePage(MainWindow mainWindow, User user)
        {
            InitializeComponent();

            feedPage = new FeedPage(user);
            likedPage = new LikedPage(user);
            matchesPage = new MatchesPage(user);
            settingsPage = new SettingsPage(user);

            ContentFrame.Navigate(feedPage); // Sobald die HomePage geöffnet wird, soll automatisch der Feed aufgemacht werden!
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(settingsPage);
        }

        private void Like_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(likedPage);
        }

        private void Matches_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(matchesPage);
        }


        private void GoToFeed_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(feedPage);
        }

       
    }
}
