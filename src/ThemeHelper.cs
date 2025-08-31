using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Media.TextFormatting;
using System.Diagnostics;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TurnEdit
{
    using Microsoft.Win32;
    public partial class MainWindow : Window
    {
        public bool GetWindowsTheme()
        {
            const string WindowsThemeRegistory = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string WindowsThemeRegistoryName = @"AppsUseLightTheme";
            var key =  Registry.CurrentUser.OpenSubKey(WindowsThemeRegistory);
            if (key == null) return true;
            var v = (int)key.GetValue(WindowsThemeRegistoryName);
            return v > 0;
        }
        public void SetAppTheme()
        {
            if (this.AppTheme == "Light")
            {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    Wpf.Ui.Appearance.ApplicationTheme.Light,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    false
                );
            } else if (this.AppTheme == "Dark")
            {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    Wpf.Ui.Appearance.ApplicationTheme.Dark,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    false
                );
            } else if (this.AppTheme == "Auto")
            {
                var currentWindowsTheme = GetWindowsTheme() ? Wpf.Ui.Appearance.ApplicationTheme.Light : Wpf.Ui.Appearance.ApplicationTheme.Dark;
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    currentWindowsTheme,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    false
                );

            } else
            {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    Wpf.Ui.Appearance.ApplicationTheme.Light,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    false
                );
            }
        }
    }
}