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

namespace TurnEdit
{
    /// <summary>
    /// AboutWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutWindow : Window
    {
		public MainWindow _mainWindow;
        public AboutWindow(MainWindow _mainWindow)
        {
			this._mainWindow = _mainWindow;
            InitializeComponent();
            appdataForDebug.Text = "AppData: " + System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
            installedDirectory.Text = $@"Installed directory: {AppDomain.CurrentDomain.BaseDirectory}";
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			string? versionString = version!.ToString();
			versionText!.Text = $"TurnEdit version {versionString}";
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				installedDirectory.Text = $@"インストールディレクトリ: {AppDomain.CurrentDomain.BaseDirectory}";
				versionText!.Text = $"TurnEdit バージョン {version!.ToString()}";
				licenseText.Text = "このテキストエディタはGNU GPL 3.0に基づいてライセンスされています。";
				this.Title = "TurnEdit のバージョン情報";
			}
        }
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
