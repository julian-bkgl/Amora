using Microsoft.Win32;
using System;
using System.Windows;

namespace Amora.Resources
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        /// <summary>
        /// Wendet das angegebene Theme an, indem die entsprechenden ResourceDictionaries geladen werden.
        /// </summary>
        /// <param name="theme">Das gewünschte Theme (Light oder Dark).</param>
        public static void ApplyTheme(AppTheme theme)
        {
            string themeFile = theme switch
            {
                AppTheme.Light => "LightTheme.xaml",
                AppTheme.Dark => "DarkTheme.xaml",
                _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
            };

            // Lade das Theme aus dem Resources-Ordner
            var themeDictionary = new ResourceDictionary
            {
                Source = new Uri($"Resources/{themeFile}", UriKind.Relative)
            };

            // Ressourcen neu setzen
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDictionary);

        }

        /// Liest das aktuelle Windows-Systemtheme aus der Registry aus.
        public static AppTheme GetSystemTheme()
        {
            const string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string valueName = "AppsUseLightTheme";

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(registryKey);
                if (key != null)
                {
                    var value = key.GetValue(valueName);
                    if (value is int intValue)
                    {
                        return intValue == 0 ? AppTheme.Dark : AppTheme.Light;
                    }
                }
            }
            catch
            {
                // Fallback bei Fehlern
            }
            return AppTheme.Light;
        }

    }
}