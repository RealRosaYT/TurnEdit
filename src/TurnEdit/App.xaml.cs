using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;

namespace TurnEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EnsureDefaultBrushes();
            if (e.Args.Length > 0) {
				string path = e.Args[0];
				var mainWindow = new MainWindow(path);
				mainWindow.Show();
			} else {
				var mainWindow = new MainWindow();
				mainWindow.Show();
			}
        }
        private void EnsureDefaultBrushes()
        {
            if (Resources["WindowBackgroundBrush"] == null)
                Resources["WindowBackgroundBrush"] = new SolidColorBrush(Colors.White);
            if (Resources["WindowForegroundBrush"] == null)
                Resources["WindowForegroundBrush"] = new SolidColorBrush(Colors.Black);
            if (Resources["TitleBarBackgroundBrush"] == null)
                Resources["TitleBarBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            if (Resources["TitleBarForegroundBrush"] == null)
                Resources["TitleBarForegroundBrush"] = new SolidColorBrush(Colors.Black);
            if (Resources["MenuBackgroundBrush"] == null)
                Resources["MenuBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            if (Resources["MenuForegroundBrush"] == null)
                Resources["MenuForegroundBrush"] = new SolidColorBrush(Colors.Black);
            if (Resources["EditorBackgroundBrush"] == null)
                Resources["EditorBackgroundBrush"] = new SolidColorBrush(Colors.White);
            if (Resources["EditorForegroundBrush"] == null)
                Resources["EditorForegroundBrush"] = new SolidColorBrush(Colors.Black);
        }
        // theme: "Light", "Dark", "System"
        public void ApplyAppTheme(string theme)
        {
            bool isSystem = string.Equals(theme, "System", StringComparison.OrdinalIgnoreCase);
            bool isLight = string.Equals(theme, "Light", StringComparison.OrdinalIgnoreCase);

            if (isSystem)
            {
                // システムに追従
                bool sysLight = IsSystemLightTheme();
                ApplicationThemeManager.Apply(sysLight ? ApplicationTheme.Light : ApplicationTheme.Dark);
                // システムアクセントを適用（必要なら）
                ApplicationAccentColorManager.ApplySystemAccent();

                // システム準拠の色（控えめな暗めライト系）
                SetBrushes(
                    sysLight ? Colors.White : Color.FromRgb(30, 30, 30),
                    sysLight ? Colors.Black : Colors.White,
                    sysLight ? Color.FromRgb(240, 240, 240) : Color.FromRgb(40, 40, 40),
                    sysLight ? Colors.Black : Colors.White);
            }
            else if (isLight)
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                // カスタムライト色（システムの影響を受けない）
                SetBrushes(Colors.White, Colors.Black, Color.FromRgb(240, 240, 240), Colors.Black);
                // アクセントは任意：SystemAccent を呼ばない（システムに戻らない）
            }
            else // Dark
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                SetBrushes(Color.FromRgb(30, 30, 30), Colors.White, Color.FromRgb(40, 40, 40), Colors.White);
            }
        }
        private void SetBrushes(Color windowBg, Color windowFg, Color titleBg, Color titleFg)
        {
            Resources["WindowBackgroundBrush"] = new SolidColorBrush(windowBg);
            Resources["WindowForegroundBrush"] = new SolidColorBrush(windowFg);
            Resources["TitleBarBackgroundBrush"] = new SolidColorBrush(titleBg);
            Resources["TitleBarForegroundBrush"] = new SolidColorBrush(titleFg);
            Resources["MenuBackgroundBrush"] = Resources["TitleBarBackgroundBrush"];
            Resources["MenuForegroundBrush"] = Resources["TitleBarForegroundBrush"];
            Resources["EditorBackgroundBrush"] = new SolidColorBrush(windowBg);
            Resources["EditorForegroundBrush"] = new SolidColorBrush(windowFg);
        }

        private bool IsSystemLightTheme()
        {
            const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string valueName = "AppsUseLightTheme";
            using (var key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                var keyvalue = key?.GetValue(valueName);
                return keyvalue is int i && i > 0;
            }
        }
    }
}
