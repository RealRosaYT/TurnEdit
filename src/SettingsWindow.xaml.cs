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
		public string? language { get; set; }
		public bool EnableDeveloperFeature { get; set; }
		public bool ShowLineNumber { get; set; }
		public bool MakeLinkClickable { get; set; }
    }
    public class TurnEditLanguage
    {
        public required string[] menuTexts { get; set; }
        public required string[] searchWindowTexts { get; set; }
        public required string[] replaceWindowTexts { get; set; }
        public required string[] settingsWindowTexts { get; set; }
        public required string language { get; set; }
    }
    public partial class SettingsWindow : Window
    {
        public MainWindow _mainwindow;
		private string[] SettingsWindowMsgboxStrings;
        public SettingsWindow(MainWindow _mainwindow)
        {
            InitializeComponent();
            ViewTurnEditSettings();
            this._mainwindow = _mainwindow;
			this.SettingsWindowMsgboxStrings = new string[2];
			LocalizeTurnEditSettingsWindow();
        }
		private void LocalizeTurnEditSettingsWindow() {
			if (this._mainwindow.TurnEditLanguage == "ja-JP") {
				designTab.Header = "外観";
				fileAndFolderTab.Header = "ファイル/フォルダ";
				othersTab.Header = "その他";
				fontTextblk.Text = "フォント";
				fontSizeTextblk.Text = "フォントサイズ";
				modeTextblk.Text = "モード";
				denyFiledblOpen.Content = "ファイルを重複で開く操作を拒否(開発中)";
				createFileWhenFNotExists.Content = "ファイルが見つからない場合は作成";
				defaultDirectoryTextblk.Text = "デフォルトのディレクトリ";
				languageTextblk.Text = "言語";
				applySettings.Content = "適用";
				okSettings.Content = "OK";
				cancelSettings.Content = "キャンセル";
				showLineNumber.Content = "行番号を表示する";
				makeURLClickable.Content = "URLをクリック可能にする";
				this.Title = "設定";
				this.SettingsWindowMsgboxStrings[0] = "設定を保存できませんでした: exc";
				this.SettingsWindowMsgboxStrings[1] = "TurnEdit";
				developerFeatureTextBlk.Text = "開発者向け機能を有効にする";
			} else if (this._mainwindow.TurnEditLanguage == "en-US") {
				this.SettingsWindowMsgboxStrings[0] = "Failed to save settings: exc";
				this.SettingsWindowMsgboxStrings[1] = "TurnEdit";
			}
		}
        /// <summary>
        /// This method loads and views TurnEdit settings.
        /// Settings are saved in C:\Users\%USERNAME%\AppData\Roaming\TurnEdit\turnedit-settings.json
        /// </summary>
        private async void ViewTurnEditSettings()
        {
			string AppData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
			string SettingsPath = System.IO.Path.Combine(AppData, "turnedit-settings.json");
            if (!File.Exists(SettingsPath))
            {
                return;
            }
            try
            {
                string json = await File.ReadAllTextAsync(SettingsPath);
                var obj = await Task.Run(() => {
					return JsonSerializer.Deserialize<TurnEditSettings>(json);
				});
                createFileWhenFNotExists!.IsChecked = obj!.CreateFileWhenFileNotExists;
				denyFiledblOpen!.IsChecked = obj!.DenyFileDoubleOpen;
                defaultDirectory!.Text = obj!.DefaultDirectoryWhenFileOpen;
                thememd.Text = obj.ThemeMode;
                txtfnt.Text = obj.TextFont;
                fontSize.Text = obj.TextFontSize.ToString();
				showLineNumber!.IsChecked = obj!.ShowLineNumber;
				makeURLClickable!.IsChecked = obj!.MakeLinkClickable;
				if (obj.language == "ja-JP") {
					languageCmbBox.SelectedItem = japaneseLanguage;
				} else if (obj.language == "en-US") {
					languageCmbBox.SelectedItem = englishLanguage;
				}
				if (obj.EnableDeveloperFeature == true) {
					enableDeveloperFeature.IsChecked = true;
				} else {
					enableDeveloperFeature.IsChecked = false;
				}
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
            if (defaultDirectoryEnabled.IsChecked.HasValue)
            {
                defaultDirectory.IsEnabled = defaultDirectoryEnabled.IsChecked.Value;
            }
        }

        /// <summary>
        /// This method handles when clicked "Apply" button.
        /// Here's How this works:
        /// 1. User clicks button.
        /// 2. Get settings and save as variable as json.
        /// 3. Use System.Text.Json, and serialize json.
        /// 4. Save serialized JSON to C:\Users\%USERNAME%\AppData\Roaming\TurnEdit\turnedit-settings.json.
        /// </summary>
        private async void applySettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] themeModeLocalizedToJapanese = { thememd.Text.Replace("ライト", "Light"), thememd.Text.Replace("ダーク", "Dark") };
                
				string? languageCode = null;
				if (languageCmbBox.Text == "日本語") {
					languageCode = "ja-JP";
				} else if (languageCmbBox.Text == "English") {
					languageCode = "en-US";
				}
                var settings = new TurnEditSettings
                {
                    DenyFileDoubleOpen = denyFiledblOpen.IsEnabled,
                    CreateFileWhenFileNotExists = createFileWhenFNotExists.IsEnabled,
                    DefaultDirectoryWhenFileOpen = defaultDirectory.Text,
                    ThemeMode = thememd.Text,
                    TextFont = txtfnt.Text,
                    TextFontSize = double.Parse(fontSize.Text),
					language = languageCode,
					EnableDeveloperFeature = (bool)enableDeveloperFeature.IsChecked!,
					ShowLineNumber = (bool)showLineNumber.IsChecked!,
					MakeLinkClickable = (bool)makeURLClickable.IsChecked!
                };
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string settingsJson = await Task.Run(() => {
					return JsonSerializer.Serialize(settings, options);
				});
                string appDataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
                if (!Directory.Exists(appDataDirectory))
                {
                    Directory.CreateDirectory(appDataDirectory);
                }
                await File.WriteAllTextAsync(System.IO.Path.Combine(appDataDirectory, "turnedit-settings.json"), settingsJson);
				//SaveTurnEditLanguages(languageCode);
                this._mainwindow.LoadTurnEditSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.SettingsWindowMsgboxStrings[0].Replace("exc", ex.Message), this.SettingsWindowMsgboxStrings[1], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void okSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
				string? languageCode2 = null;
				if (languageCmbBox.Text == "日本語") {
					languageCode2 = "ja-JP";
				} else if (languageCmbBox.Text == "English") {
					languageCode2 = "en-US";
				}
                var settings = new TurnEditSettings
                {
                    DenyFileDoubleOpen = denyFiledblOpen.IsEnabled,
                    CreateFileWhenFileNotExists = createFileWhenFNotExists.IsEnabled,
                    DefaultDirectoryWhenFileOpen = defaultDirectory.Text,
                    ThemeMode = thememd.Text,
                    TextFont = txtfnt.Text,
                    TextFontSize = double.Parse(fontSize.Text),
					language = languageCode2,
					EnableDeveloperFeature = (bool)enableDeveloperFeature.IsChecked!,
					ShowLineNumber = (bool)showLineNumber.IsChecked!,
					MakeLinkClickable = (bool)makeURLClickable.IsChecked!
                };
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string settingsJson = await Task.Run(() => {
					return JsonSerializer.Serialize(settings, options);
				});
                string appDataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
                if (!Directory.Exists(appDataDirectory))
                {
                    Directory.CreateDirectory(appDataDirectory);
                }
                await File.WriteAllTextAsync(System.IO.Path.Combine(appDataDirectory, "turnedit-settings.json"), settingsJson);
				//SaveTurnEditLanguages(languageCode2);
                this._mainwindow.LoadTurnEditSettings();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.SettingsWindowMsgboxStrings[0].Replace("exc", ex.Message), this.SettingsWindowMsgboxStrings[1], MessageBoxButton.OK, MessageBoxImage.Error);
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
        /*
        public void SaveTurnEditLanguages(string language)
        {
            if (language == "ja-JP")
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
                string languagedirectoryJapanese = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
                string languageFilePathJapanese = System.IO.Path.Combine(languagedirectoryJapanese, "turnedit-language.json");
                if (!Directory.Exists(languagedirectoryJapanese))
                {
                    Directory.CreateDirectory(languagedirectoryJapanese);
                }
                File.WriteAllText(languageFilePathJapanese, jsonJapanese);
                //MessageBox.Show("表示言語を変更するにはTurnEditを再起動する必要があります。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
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
				englishStrings.language = "en-US";
                var optionsEnglish = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var jsonEnglish = JsonSerializer.Serialize(englishStrings, optionsEnglish);
                string languageDirectoryEnglish = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages");
                string languageFilePathEnglish = System.IO.Path.Combine(languageDirectoryEnglish, "turnedit-language.json");
                if (!Directory.Exists(languageDirectoryEnglish))
                {
                    Directory.CreateDirectory(languageDirectoryEnglish);
                }
                File.WriteAllText(languageFilePathEnglish, jsonEnglish);
                //MessageBox.Show("TurnEdit needs restart for changing language.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
		*/
    }
}
