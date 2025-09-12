using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

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
			if (e.Args.Length > 0) {
				string path = e.Args[0];
				var mainWindow = new MainWindow(path);
				mainWindow.Show();
			} else {
				var mainWindow = new MainWindow();
				mainWindow.Show();
			}
        }
    }

}
