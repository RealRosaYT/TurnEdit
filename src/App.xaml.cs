using System.Configuration;
using System.Data;
using System.Windows;

namespace TurnEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			MainWindow mainWindow = new MainWindow();
			if (e.Args.Length > 0) {
				string filePath = e.Args[0];
				mainWindow.OpenInCommandLineArgument(filePath);
			}
		}
    }

}
