using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.IO;
using System.Diagnostics;

namespace TurnEdit
{
    /// <summary>
    /// TurnEdit settings base class
    /// </summary>
    public class TurnEditSettings
    {
        public bool DenyFileDoubleOpen { get; set; }
        public bool CreateFileWhenFileNotExists { get; set; }
        public string? DefaultDirectoryWhenFileOpen { get; set; }
        public string? ThemeMode { get; set; }
        public string? TextFont { get; set; }
        public double TextFontSize { get; set; }
    }
    public class TurnEditLanguage
    {
        public string[] menuTexts { get; set; }
        public string[] searchWindowTexts { get; set; }
        public string[] replaceWindowTexts { get; set; }
        public string[] settingsWindowTexts { get; set; }
        public string language { get; set; }
    }
    public partial class SettingsWindow : Window
    {
        public MainWindow _mainwindow;
        public SettingsWindow(MainWindow _mainwindow)
        {
            InitializeComponent();
            ViewTurnEditSettings();
            this._mainwindow = _mainwindow;
        }
        /// <summary>
        /// This method loads and views TurnEdit settings.
        /// Settings are saved in C:\Users\%USERNAME%\AppData\Roaming\TurnEdit\turnedit-settings.json
        /// </summary>
        private void ViewTurnEditSettings()
        {
            if (!File.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\turnedit-settings.json"))
            {
                return;
            }
            try
            {
                string json = File.ReadAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\turnedit-settings.json");
                var obj = JsonSerializer.Deserialize<TurnEditSettings>(json);
                createFileWhenFNotExists.IsEnabled = obj.CreateFileWhenFileNotExists;
                defaultDirectory.Text = obj.DefaultDirectoryWhenFileOpen;
                thememd.Text = obj.ThemeMode;
                txtfnt.Text = obj.TextFont;
                fontSize.Text = obj.TextFontSize.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load settings. more information is available on debug console.\r\nTurnEdit is using default settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"TurnEdit: error: failed to load settings:\r\n{ex.Message}");
            }
        }

        /// <summary>
        /// This method handles when clicked "Default Directory" check box.
        /// This method have bug, please fix it
        /// </summary>
        private void defaultDirectoryEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (defaultDirectoryEnabled.IsEnabled == true)
            {
                defaultDirectory.IsEnabled = true;
            } else if (defaultDirectoryEnabled.IsEnabled == false)
            {
                defaultDirectory.IsEnabled = false;
            }
        }

        /// <summary>
        /// This method handles when clicked "Apply" button.
        /// How this works:
        /// 1. User clicks button.
        /// 2. Get settings and save as variable as json.
        /// 3. Use System.Text.Json, and serialize json.
        /// 4. Save serialized JSON to C:\Users\%USERNAME%\AppData\Roaming\TurnEdit\turnedit-settings.json.
        /// </summary>
        private void applySettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = new TurnEditSettings
                {
                    DenyFileDoubleOpen = denyFiledblOpen.IsEnabled,
                    CreateFileWhenFileNotExists = createFileWhenFNotExists.IsEnabled,
                    DefaultDirectoryWhenFileOpen = defaultDirectory.Text,
                    ThemeMode = thememd.Text,
                    TextFont = txtfnt.Text,
                    TextFontSize = double.Parse(fontSize.Text)
                };
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string settingsJson = JsonSerializer.Serialize(settings, options);
                string appDataDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\";
                if (!Directory.Exists(appDataDirectory))
                {
                    Directory.CreateDirectory(appDataDirectory);
                }
                File.WriteAllText($@"{appDataDirectory}turnedit-settings.json", settingsJson);
                this._mainwindow.LoadTurnEditSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void okSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = new TurnEditSettings
                {
                    DenyFileDoubleOpen = denyFiledblOpen.IsEnabled,
                    CreateFileWhenFileNotExists = createFileWhenFNotExists.IsEnabled,
                    DefaultDirectoryWhenFileOpen = defaultDirectory.Text,
                    ThemeMode = thememd.Text,
                    TextFont = txtfnt.Text,
                    TextFontSize = double.Parse(fontSize.Text)
                };
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string settingsJson = JsonSerializer.Serialize(settings, options);
                string appDataDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\TurnEdit\";
                if (!Directory.Exists(appDataDirectory))
                {
                    Directory.CreateDirectory(appDataDirectory);
                }
                File.WriteAllText($@"{appDataDirectory}turnedit-settings.json", settingsJson);
                this._mainwindow.LoadTurnEditSettings();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void fontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex($@"[^0-9.]+");
            var text = fontSize.Text + e.Text;
            e.Handled = regex.IsMatch(text);
        }
        /// <summary>
        /// This method save or overwrite language file.
        /// </summary>
        /// <param name="language">Target language</param>
        public void SaveTurnEditLanguages(string language)
        {
            if (language == "ja-jp")
            {
                var japaneseStrings = new TurnEditLanguage();
                japaneseStrings.menuTexts[0] = "ファイル";
                japaneseStrings.menuTexts[1] = "表示";
                japaneseStrings.menuTexts[2] = "編集";
                japaneseStrings.menuTexts[3] = "ヘルプ";
                japaneseStrings.searchWindowTexts[0] = "検索";
                japaneseStrings.searchWindowTexts[1] = "検索するテキスト";
                japaneseStrings.searchWindowTexts[2] = "キャンセル";
                japaneseStrings.replaceWindowTexts[0] = "置き換え";
                japaneseStrings.replaceWindowTexts[1] = "置き換え前";
                japaneseStrings.replaceWindowTexts[2] = "置き換え後";
                japaneseStrings.replaceWindowTexts[3] = "キャンセル";
                japaneseStrings.settingsWindowTexts[0] = "設定";
                japaneseStrings.settingsWindowTexts[1] = "外観";
                japaneseStrings.settingsWindowTexts[2] = "ファイル/フォルダ";
                japaneseStrings.settingsWindowTexts[3] = "フォント";
                japaneseStrings.settingsWindowTexts[4] = "モード";
                japaneseStrings.settingsWindowTexts[5] = "フォントサイズ";
                japaneseStrings.settingsWindowTexts[6] = "ファイルを重複で開く操作を拒否する(開発中)";
                japaneseStrings.settingsWindowTexts[7] = "ファイルが存在しない場合に作成する";
                japaneseStrings.settingsWindowTexts[8] = "デフォルトのディレクトリ(フォルダ)";
                japaneseStrings.settingsWindowTexts[9] = "適用";
                japaneseStrings.settingsWindowTexts[10] = "OK";
                japaneseStrings.settingsWindowTexts[11] = "キャンセル";
                japaneseStrings.language = "ja-JP";
                var optionsJapanese = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var jsonJapanese = JsonSerializer.Serialize(japaneseStrings, optionsJapanese);
                string languagedirectoryJapanese = $@"{AppDomain.CurrentDomain.BaseDirectory}languages\";
                string languageFilePathJapanese = $@"{languagedirectoryJapanese}turnedit-language.json";
                if (!Directory.Exists(languagedirectoryJapanese))
                {
                    Directory.CreateDirectory(languagedirectoryJapanese);
                }
                File.WriteAllText(languageFilePathJapanese, jsonJapanese);
                MessageBox.Show("表示言語を変更するにはTurnEditを再起動する必要があります。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            } else if (Regex.IsMatch(language, "/en-[a-zA-Z]{2}/"))
            {
                var englishStrings = new TurnEditLanguage();
                englishStrings.menuTexts[0] = "File";
                englishStrings.menuTexts[1] = "View";
                englishStrings.menuTexts[2] = "Edit";
                englishStrings.menuTexts[3] = "Help";
                englishStrings.searchWindowTexts[0] = "Search";
                englishStrings.searchWindowTexts[1] = "Search text";
                englishStrings.searchWindowTexts[2] = "Cancel";
                englishStrings.replaceWindowTexts[0] = "Replace";
                englishStrings.replaceWindowTexts[1] = "What to replace";
                englishStrings.replaceWindowTexts[2] = "Replace destination";
                englishStrings.replaceWindowTexts[3] = "Cancel";
                englishStrings.settingsWindowTexts[0] = "Settings";
                englishStrings.settingsWindowTexts[1] = "Design";
                englishStrings.settingsWindowTexts[2] = "File/Folder";
                englishStrings.settingsWindowTexts[3] = "Font";
                englishStrings.settingsWindowTexts[4] = "Mode";
                englishStrings.settingsWindowTexts[5] = "Font Size";
                englishStrings.settingsWindowTexts[6] = "Deny file double open(Under construction)";
                englishStrings.settingsWindowTexts[7] = "Create file when file not exists";
                englishStrings.settingsWindowTexts[8] = "Default directory";
                englishStrings.settingsWindowTexts[9] = "Apply";
                englishStrings.settingsWindowTexts[10] = "OK";
                englishStrings.settingsWindowTexts[11] = "Cancel";
                var optionsEnglish = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var jsonEnglish = JsonSerializer.Serialize(englishStrings, optionsEnglish);
                string languageDirectoryEnglish = $@"{AppDomain.CurrentDomain.BaseDirectory}languages\";
                string languageFilePathEnglish = $@"{languageDirectoryEnglish}turnedit-language.json";
                if (!Directory.Exists(languageDirectoryEnglish))
                {
                    Directory.CreateDirectory(languageDirectoryEnglish);
                }
                File.WriteAllText(languageFilePathEnglish, jsonEnglish);
                MessageBox.Show("TurnEdit needs restart for changing language.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
