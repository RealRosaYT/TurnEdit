using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TurnEdit.AI;
using Wpf.Ui.Controls;

namespace TurnEdit
{
    public partial class SettingsView : UserControl
    {
        public MainWindow _mainwindow;
        private string[] SettingsWindowMsgboxStrings;
        public SettingsView(MainWindow _mainwindow)
        {
            InitializeComponent();
            ViewTurnEditSettings();
            this._mainwindow = _mainwindow;
            this.SettingsWindowMsgboxStrings = new string[2];
            LocalizeTurnEditSettingsWindow();
        }

        private void NavView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (NavView.SelectedItem is NavigationViewItem item && item.Tag is string tag)
            {
                DesignPanel.Visibility = tag == "Design" ? Visibility.Visible : Visibility.Collapsed;
                FileFolderPanel.Visibility = tag == "FileFolder" ? Visibility.Visible : Visibility.Collapsed;
                AIPanel.Visibility = tag == "AI" ? Visibility.Visible : Visibility.Collapsed;
                OthersPanel.Visibility = tag == "Others" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void defaultDirectoryEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (defaultDirectoryEnabled.IsChecked.HasValue)
            {
                defaultDirectory.IsEnabled = defaultDirectoryEnabled.IsChecked.Value;
            }
        }

        private void isAiEnabled_Checked(object sender, RoutedEventArgs e)
        {
            aiSelectModel.IsEnabled = isAiEnabled.IsChecked == true;
            aiApiKey.IsEnabled = isAiEnabled.IsChecked == true;
            isAiContextMenuEnabled.IsEnabled = isAiEnabled.IsChecked == true;
        }

        private void isAiEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            aiSelectModel.IsEnabled = isAiEnabled.IsChecked == true;
            aiApiKey.IsEnabled = isAiEnabled.IsChecked == true;
            isAiContextMenuEnabled.IsEnabled = isAiEnabled.IsChecked == true;
        }

        private void fontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex($@"[^0-9.]+");
            var text = fontSize.Text + e.Text;
            e.Handled = regex.IsMatch(text);
        }
        private void LocalizeTurnEditSettingsWindow()
        {
            if (this._mainwindow.TurnEditLanguage == "ja-JP")
            {
                navDesign.Content = "外観";
                navFileFolder.Content = "ファイル/フォルダ";
                navOthers.Content = "その他";
                fontTextblk.Text = "フォント";
                fontSizeTextblk.Text = "フォントサイズ";
                modeTextblk.Text = "モード";
                denyFiledblOpen.Content = "ファイルを重複で開く操作を拒否(開発中)";
                createFileWhenFNotExists.Content = "ファイルが見つからない場合は作成";
                defaultDirectoryTextblk.Text = "デフォルトのディレクトリ";
                languageTextblk.Text = "言語";
                applySettingsBtn.Content = "適用";
                showLineNumber.Content = "行番号を表示する";
                makeURLClickable.Content = "URLをクリック可能にする";
                this.SettingsWindowMsgboxStrings[0] = "設定を保存できませんでした: exc";
                this.SettingsWindowMsgboxStrings[1] = "TurnEdit";
                developerFeatureTextBlk.Text = "開発者向け機能を有効にする";
            }
            else if (this._mainwindow.TurnEditLanguage == "en-US")
            {
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
                if (obj.IsAIEnabled != null && obj.AIModel != null && obj.AIApiKey != null && obj.IsAiContextMenuEnabled != null)
                {
                    isAiEnabled.IsChecked = obj!.IsAIEnabled;
                    aiSelectModel.IsEnabled = obj!.IsAIEnabled;
                    aiApiKey.Text = obj!.AIApiKey;
                    aiApiKey.IsEnabled = obj!.IsAIEnabled;
                    isAiContextMenuEnabled.IsEnabled = obj!.IsAIEnabled;
                    isAiContextMenuEnabled.IsChecked = obj!.IsAiContextMenuEnabled;
                }
                if (obj.language == "ja-JP")
                {
                    languageCmbBox.SelectedItem = japaneseLanguage;
                }
                else if (obj.language == "en-US")
                {
                    languageCmbBox.SelectedItem = englishLanguage;
                }
                if (obj.EnableDeveloperFeature == true)
                {
                    enableDeveloperFeature.IsChecked = true;
                }
                else
                {
                    enableDeveloperFeature.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to load settings. more information is available on debug console.\r\nTurnEdit is using default settings.", "Error", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"TurnEdit: error: failed to load settings:\r\n{ex.Message}");
            }
        }

        private AIModels GetAIModel(string ModelID)
        {
            switch (ModelID)
            {
                case "gemini-2.5-flash":
                    return AIModels.Gemini25Flash;
                case "gemini-2.5-flash-lite":
                    return AIModels.Gemini25FlashLite;
                case "gemini-2.5-pro":
                    return AIModels.Gemini25Pro;
                case "gemini-3-pro-preview":
                    return AIModels.Gemini3Pro;
                case "gpt-5.1-2025-11-13":
                    return AIModels.GPT51;
                case "gpt-5-mini-2025-08-07":
                    return AIModels.GPT5Mini;
                case "grok-4-1-fast-reasoning":
                    return AIModels.Grok41;
                case "grok-4-fast-reasoning":
                    return AIModels.Grok4;
                case "grok-3":
                    return AIModels.Grok3;
                case "grok-3-mini":
                    return AIModels.Grok3Mini;
                case "grok-2-vision-1212":
                    return AIModels.Grok2;
                default:
                    return AIModels.Gemini25Flash;
            }
        }
        private async void applySettings(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] themeModeLocalizedToJapanese = { thememd.Text.Replace("ライト", "Light"), thememd.Text.Replace("ダーク", "Dark") };

                string? languageCode = null;
                if (languageCmbBox.Text == "日本語")
                {
                    languageCode = "ja-JP";
                }
                else if (languageCmbBox.Text == "English")
                {
                    languageCode = "en-US";
                }
                var selectedAIModelItem = aiSelectModel.SelectedItem as ComboBoxItem;
                var selectedAiModel = selectedAIModelItem == null ? "" : selectedAIModelItem.Tag?.ToString();
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
                    MakeLinkClickable = (bool)makeURLClickable.IsChecked!,
                    IsAIEnabled = (bool)isAiEnabled.IsChecked!,
                    IsAiContextMenuEnabled = (bool)isAiContextMenuEnabled.IsChecked!,
                    AIModel = GetAIModel(selectedAiModel == null ? "" : selectedAiModel),
                    AIApiKey = aiApiKey == null ? "" : aiApiKey.Text
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
                System.Windows.MessageBox.Show(this.SettingsWindowMsgboxStrings[0].Replace("exc", ex.Message), this.SettingsWindowMsgboxStrings[1], System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
