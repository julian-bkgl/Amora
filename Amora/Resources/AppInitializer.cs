using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amora.Resources
{
    public static class AppInitializer
    {
        public static void EnsureAppFolders()
        { // Stellt sicher das die Ordner mit User-Images vorhanden sind
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string baseFolder = System.IO.Path.Combine(documentsPath, "Amora", "Images");
            string uploadsFolder = System.IO.Path.Combine(baseFolder, "UserUploads");

            // Ordner erstellen ( falls nicht vorhanden )
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // default.png prüfen
            string defaultImagePath = System.IO.Path.Combine(baseFolder, "default.png");
            if (!File.Exists(defaultImagePath))
            {
                // Kopiere eine vorhandene default.png aus dem Projektverzeichnis
                string sourceDefault = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "default.png");
                if (File.Exists(sourceDefault))
                {
                    File.Copy(sourceDefault, defaultImagePath);
                }
            }
        }
    }
}
