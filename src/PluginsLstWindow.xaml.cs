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
using System.Diagnostics;
using Microsoft.Win32;

namespace TurnEdit
{
    public partial class PluginsLstWindow : Window
    {
		public MainWindow _mainWindow;
        public PluginsLstWindow(MainWindow _mainWindow)
        {
            InitializeComponent();
			this._mainWindow = _mainWindow;
            var plugins = PluginLoader.LoadPlugins();
            pluginsLstView.ItemsSource = plugins;
			InitializePluginListWindowI18n();
        }
		public void InitializePluginListWindowI18n() {
			switch (this._mainWindow.TurnEditLanguage) {
				case "en-US":
					installPlugin.Content = "Install";
					propertyPlugin.Content = "Property";
					this.Title = "Plugins";
					break;
				case "ja-JP":
					installPlugin.Content = "インストール";
					propertyPlugin.Content = "プロパティ";
					this.Title = "プラグイン";
					break;
			}
		}
		public void propertyPlugin_Click(object? sender, RoutedEventArgs e) {
			if (pluginsLstView != null) {
				var selectedItem = pluginsLstView.SelectedItem as PluginInformationsTemplate;
				if (selectedItem != null) {
					string? pluginDesc = null;
					if (selectedItem.PluginDescription == null) {
						switch (this._mainWindow.TurnEditLanguage) {
							case "en-US":
								pluginDesc = "No";
								break;
							case "ja-JP":
								pluginDesc = "なし";
								break;
						}
					} else {
						pluginDesc = selectedItem.PluginDescription;
					}
					string? pluginAutr = null;
					if (selectedItem.PluginAuthor == null) {
						switch (this._mainWindow.TurnEditLanguage) {
							case "en-US":
								pluginAutr = "Anonymous";
								break;
							case "ja-JP":
								pluginAutr = "匿名";
								break;
						}
					} else {
						pluginAutr = selectedItem.PluginAuthor;
					}
					MessageBox.Show(this._mainWindow.msgboxStringsMain[21].Replace("plgname", selectedItem.PluginName).Replace("desc", pluginDesc).Replace("ver", selectedItem.PluginVersion).Replace("autr", pluginAutr), this._mainWindow.msgboxStringsMain[20], MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}
		public bool IsValidDotNetAssembly(string fileName) {
			try {
				System.Reflection.Assembly.LoadFrom(fileName);
				return true;
			} catch (BadImageFormatException) {
				return false;
			}
		}
		
		/// <summary>
		/// Calculates SHA256 hash from file.
		/// </summary>
		/// <param name="fileName">File to calculate.</param>
		/// <returns>Calculated SHA256 hash.</returns>
		public string CalculateSha256(string fileName) {
			System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create();
			System.IO.FileStream stream = System.IO.File.OpenRead(fileName);
			byte[] bytes = hash.ComputeHash(stream);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (byte b in bytes) {
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}
		/// <summary>
		/// Installs plugin.
		/// </summary>
		public async void installPlugin_Click(object? sender, RoutedEventArgs e) {
			try {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Plugin File(*.dll)|*.dll";
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			if (ofd.ShowDialog() == true) {
				string pluginsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
				if (!System.IO.Directory.Exists(pluginsPath)) {
					System.IO.Directory.CreateDirectory(pluginsPath);
				}
				string fullPluginPath = System.IO.Path.Combine(pluginsPath, ofd.SafeFileName);
				bool isValid = IsValidDotNetAssembly(ofd.FileName);
				if (!isValid) {
					MessageBox.Show(this._mainWindow.msgboxStringsMain[23], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ofd.FileName)!, "sha256.txt"))) {
					MessageBox.Show(this._mainWindow.msgboxStringsMain[24], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				} else {
					string expectedSha256 = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ofd.FileName)!, "sha256.txt"));
					string calculatedSha256 = CalculateSha256(ofd.FileName);
					if (!calculatedSha256.Equals(expectedSha256, StringComparison.OrdinalIgnoreCase)) {
						MessageBox.Show(this._mainWindow.msgboxStringsMain[24], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					string checksumDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "checksums");
					System.IO.File.WriteAllText(System.IO.Path.Combine(checksumDirectory!, System.IO.Path.GetFileNameWithoutExtension(ofd.FileName) + "-sha256.txt"), expectedSha256);
					string checksumFileName = System.IO.Path.Combine(checksumDirectory!, System.IO.Path.GetFileNameWithoutExtension(ofd.FileName)) + "-sha256.txt";
					System.IO.File.SetAttributes(checksumFileName, System.IO.FileAttributes.ReadOnly);
				}
				System.IO.File.Copy(ofd.FileName, fullPluginPath);
			}
			} catch (System.IO.IOException ex) {
				MessageBox.Show(this._mainWindow.msgboxStringsMain[22], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				Debug.WriteLine("TurnEdit: error: failure installing plugin: " + ex.Message);
				MessageBox.Show(ex.Message);
			} catch (Exception ex) {
				MessageBox.Show(this._mainWindow.msgboxStringsMain[22], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				Debug.WriteLine("TurnEdit: error: failure installing plugin: " + ex.Message);
				MessageBox.Show(ex.Message);
			}
		}
    }
}