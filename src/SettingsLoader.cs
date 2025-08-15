using System;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Configuration;
using System.Windows.Media.TextFormatting;
using System.Diagnostics;

namespace TurnEdit
{
    public partial class MainWindow : Window
    {
        private bool? CreateFileFileNotExists;
        private string? DefaultDirectory;
        public class TurnEditSettings
        {
            public bool DenyFileDoubleOpen { get; set; }
            public bool CreateFileWhenFileNotExists { get; set; }
            public string? DefaultDirectoryWhenFileOpen { get; set; }
            public string? ThemeMode { get; set; }
            public string? TextFont { get; set; }
            public double TextFontSize { get; set; }
        }
        public bool CheckSettingsFileExists()
        {
            if (File.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\turnedit-settings.json"))
            {
                return true;
            } else
            {
                return false;
            }
        }
        public void LoadTurnEditSettings()
        {
            if (!CheckSettingsFileExists())
            {
                return;
            }
            try
            {
                string json = File.ReadAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\turnedit-settings.json");
                var obj = JsonSerializer.Deserialize<TurnEditSettings>(json);
                mainTextBox.FontSize = obj!.TextFontSize;
                this.CreateFileFileNotExists = obj.CreateFileWhenFileNotExists;
                FontFamily font = new FontFamily(obj.TextFont);
                mainTextBox.FontFamily = font;
                if (obj.DefaultDirectoryWhenFileOpen is null || string.IsNullOrEmpty(obj.DefaultDirectoryWhenFileOpen))
                {
                    this.DefaultDirectory = null;
                } else if (obj.DefaultDirectoryWhenFileOpen is not null)
                {
                    this.DefaultDirectory = obj.DefaultDirectoryWhenFileOpen;
                } else
                {
                    this.DefaultDirectory = null;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Failed to load settings. more information is available on debug console.\r\nTurnEdit is using default settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"TurnEdit: error: failed to load settings:\r\n{ex.Message}");
            }
        }
    }
}