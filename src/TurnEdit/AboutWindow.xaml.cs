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
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Interop;
using Wpf.Ui.Appearance;

namespace TurnEdit
{
    /// <summary>
    /// AboutWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutWindow
    {
		public MainWindow _mainWindow;
        private bool isBetaRelease;
        private string? betaReleaseID;
        public AboutWindow(MainWindow _mainWindow)
        {
			this._mainWindow = _mainWindow;
            InitializeComponent();
            this.isBetaRelease = true;
            this.betaReleaseID = "alpha.1";
            appdataForDebug.Text += System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
            installedDirectory.Text += AppDomain.CurrentDomain.BaseDirectory;
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			string? versionString = version!.ToString();
			versionText!.Text = $"TurnEdit version {versionString}";
            if (isBetaRelease)
            {
                versionText!.Text += $"-{betaReleaseID}";
                preReleaseWarning.IsOpen = true;
            }
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				installedDirectory.Text = $@"インストールディレクトリ: {AppDomain.CurrentDomain.BaseDirectory}";
				versionText!.Text = $"TurnEdit バージョン {version!.ToString()}";
				licenseText.Text = "このテキストエディタはGNU GPL 3.0に基づいてライセンスされています。";
				this.Title = "TurnEdit のバージョン情報";
			}
			clrVersion.Text += Environment.Version.ToString();
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				clrVersion.Text = "共通言語ランタイムのバージョン: " + Environment.Version.ToString();
			}
            AppIconImage.Source = GetAppIcon();
            DateTime buildedAt = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            buildTime.Text += buildedAt.ToString();
            if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
            {
                borderDebugInfo.Background = Brushes.DarkSlateGray;
                borderBuildInfo.Background = Brushes.DarkSlateGray;
            }
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private BitmapSource? GetAppIcon()
        {
            try
            {
                string executablePath = Assembly.GetExecutingAssembly().Location;
                System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(executablePath);
                if (appIcon != null)
                {
                    IntPtr hIcon = appIcon.Handle;
                    return Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            } catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
