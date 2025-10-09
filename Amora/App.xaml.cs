using Amora.Resources;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Amora
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var systemTheme = ThemeManager.GetSystemTheme();
            ThemeManager.ApplyTheme(systemTheme);
        }
    }

}
