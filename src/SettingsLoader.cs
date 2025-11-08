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
using System.Threading.Tasks;

namespace TurnEdit
{
    public partial class MainWindow : Window
    {
        private bool? CreateFileFileNotExists;
        private string? DefaultDirectory;
		public string? TurnEditLanguage;
		public string[] msgboxStringsMain;
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
        public async void LoadTurnEditSettings()
        {
            if (!CheckSettingsFileExists())
            {
                return;
            }
            try
            {
				string AppDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TurnEdit");
				string SettingsPath = System.IO.Path.Combine(AppDataPath, "turnedit-settings.json");
                string json = await File.ReadAllTextAsync(SettingsPath);
                TurnEditSettings? obj = await Task.Run(() => 
				{
					return JsonSerializer.Deserialize<TurnEditSettings>(json);
				});
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
                if (obj.ThemeMode == "Light" || obj.ThemeMode == "ライト")
                {
                    this.AppTheme = "Light";
                } else if (obj.ThemeMode == "Dark" || obj.ThemeMode == "ダーク")
                {
                    this.AppTheme = "Dark";
                } else if (obj.ThemeMode == "Auto" || obj.ThemeMode == "自動")
                {
                    this.AppTheme = "Auto";
                }
				if (obj.EnableDeveloperFeature == true) {
					forDevelopers.Visibility = Visibility.Visible;
					this.DeveloperMode = true;
				} else {
					forDevelopers.Visibility = Visibility.Collapsed;
					this.DeveloperMode = false;
				}
				mainTextBox.ShowLineNumbers = obj.ShowLineNumber;
				mainTextBox.Options.EnableHyperlinks = obj.MakeLinkClickable;
				LoadTurnEditLanguage(obj?.language);
				this.TurnEditLanguage = obj!.language;
				InitlaizeMsgboxStrings(obj?.language);
            
            } catch (Exception ex)
            {
                MessageBox.Show("Failed to load settings. more information is available on debug console.\r\nTurnEdit is using default settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"TurnEdit: error: failed to load settings:\r\n{ex.Message}");
                mainTextBox.FontSize = 13;
                this.CreateFileFileNotExists = false;
                FontFamily fontDefault = new FontFamily("Segoe UI");
                mainTextBox.FontFamily = fontDefault;
                this.DefaultDirectory = null;
                this.AppTheme = "Light";
				this.TurnEditLanguage = "en-US";
				this.DeveloperMode = false;
				mainTextBox.ShowLineNumbers = true;
				mainTextBox.Options.EnableHyperlinks = true;
            }
        }
		/// <summary>
		/// This method loads TurnEdit language.
		/// </summary>
		// <param name="languageCode">Language code(example: en-US)</param>
		public void LoadTurnEditLanguage(string? languageCode) {
			if (languageCode == "ja-JP") {
				fileNav.Header = "ファイル";
				viewNav.Header = "表示";
				editNav.Header = "編集";
				windowNav.Header = "ウィンドウ";
				helpNav.Header = "ヘルプ";
				newFileNav.Header = "新規作成";
				openNav.Header = "開く";
				saveNav.Header = "上書き保存";
				saveAsNav.Header = "名前を付けて保存";
				//recentlyOpenedFile.Header = "最近開いたファイル";
				exitNav.Header = "終了";
				settingsNav.Header = "設定";
				searchNav.Header = "検索";
				replaceNav.Header = "置換";
				insertDateAndTimeNav.Header = "日付と時刻を挿入";
				undoNav.Header = "元に戻す";
				redoNav.Header = "やり直す";
				cutNav.Header = "切り取り";
				copyNav.Header = "コピー";
				pasteNav.Header = "貼り付け";
				pasteWithQuotes.Header = "引用符付きで貼り付け";
				deleteCurrentLine.Header = "現在の行を削除";
				selectAllNav.Header = "すべて選択";
				newWindowNav.Header = "新規ウィンドウ";
				helpNav.Header = "ヘルプ";
				helpOfflineNav.Header = "ヘルプ";
				aboutTurnEditNav.Header = "バージョン情報";
				updaterNav.Header = "TurnEdit をアップデート";
				searchOnGoogle.Header = "Googleで検索";
				searchOnBing.Header = "Bingで検索";
				searchEngineNav.Header = "検索";
				this.Title = "無題 - TurnEdit";
				lineStatus.Text = "行: 1";
				columnStatus.Text = "列: 1";
				totalTextCount.Text = "文字の総数: 0";
				pluginsNav.Header = "プラグイン";
				forDevelopers.Header = "開発者向け機能";
				occurExc.Header = "例外をスロー";
				scrollToEnd.Header = "末尾へ移動";
				moveLineNav.Header = "行を移動";
				pluginsTopNav.Header = "プラグイン";
				languagesNav.Header = "言語";
			} else if (languageCode == "en-US") {
				fileNav.Header = "File";
				viewNav.Header = "View";
				editNav.Header = "Edit";
				windowNav.Header = "Window";
				helpNav.Header = "Help";
				newFileNav.Header = "New";
				openNav.Header = "Open";
				saveNav.Header = "Save";
				saveAsNav.Header = "Save As";
				//recentlyOpenedFile.Header = "最近開いたファイル";
				exitNav.Header = "Exit";
				settingsNav.Header = "Settings";
				searchNav.Header = "Search";
				replaceNav.Header = "Replace";
				insertDateAndTimeNav.Header = "Insert date and time";
				undoNav.Header = "Undo";
				redoNav.Header = "Redo";
				cutNav.Header = "Cut";
				copyNav.Header = "Copy";
				pasteNav.Header = "Paste";
				pasteWithQuotes.Header = "Paste with quotes";
				deleteCurrentLine.Header = "Delete current line";
				selectAllNav.Header = "Select all";
				newWindowNav.Header = "New Window";
				helpNav.Header = "Help";
				helpOfflineNav.Header = "Help";
				aboutTurnEditNav.Header = "About TurnEdit";
				updaterNav.Header = "Update TurnEdit";
				searchOnGoogle.Header = "on Google";
				searchOnBing.Header = "on Bing";
				searchEngineNav.Header = "Search";
				this.Title = "Untitled - TurnEdit";
				lineStatus.Text = "Line: 1";
				columnStatus.Text = "Column: 1";
				totalTextCount.Text = "Length: 0";
				pluginsNav.Header = "Plugins";
				forDevelopers.Header = "For developers";
				occurExc.Header = "Throw a exception";
				scrollToEnd.Header = "Scroll to end";
				moveLineNav.Header = "Move line";
				pluginsTopNav.Header = "Plugins";
				languagesNav.Header = "Languages";
			}
		}
		private void InitlaizeMsgboxStrings(string? languageCode2) {
			if (languageCode2 == "ja-JP")
			{
				this.msgboxStringsMain[0] = "「path」は許可されていないパスです。";
				this.msgboxStringsMain[1] = "入出力エラーによりファイルを開く操作が失敗しました。";
				this.msgboxStringsMain[2] = "予期しないエラーによりファイルを開く操作が失敗しました。";
				this.msgboxStringsMain[3] = "ファイルを開いてください。";
				this.msgboxStringsMain[4] = "入出力エラーによりファイル保存が失敗しました。";
				this.msgboxStringsMain[5] = "予期しないエラーによりファイル保存が失敗しました。";
				this.msgboxStringsMain[6] = "変更が保存されていません。変更を保存しますか?";
				this.msgboxStringsMain[7] = "入出力エラーによりウィンドウを閉じる操作が失敗しました。保存されていない変更は失われます。TurnEditを強制終了してください。";
				this.msgboxStringsMain[8] = "セキュリティ違反によりウィンドウを閉じる操作が失敗しました。保存されていない変更は失われます。TurnEditを強制終了してください。";
				this.msgboxStringsMain[9] = "予期しないエラーによりウィンドウを閉じる操作が失敗しました。保存されていない変更は失われます。TurnEditを強制終了してください。";
				this.msgboxStringsMain[10] = "ヘルプファイルが見つかりません。";
				this.msgboxStringsMain[11] = "ヘルプを開けませんでした: exc";
				this.msgboxStringsMain[12] = "クリップボードのアクセス中にエラーが発生しました。";
				this.msgboxStringsMain[13] = "TurnEdit";
				this.msgboxStringsMain[14] = "TurnEdit";
				this.msgboxStringsMain[15] = "TurnEdit";
				this.msgboxStringsMain[16] = "アップデーターを開けませんでした: ";
				this.msgboxStringsMain[17] = "アップデーターの実行ファイルが見つかりません。";
				this.msgboxStringsMain[18] = "ファイルが大き過ぎます。最大のファイルサイズは50MBです。";
				this.msgboxStringsMain[19] = "プラグインを読み込めません。";
				this.msgboxStringsMain[20] = "情報";
				this.msgboxStringsMain[21] = "プラグイン名: plgname\r\n説明: desc\r\nバージョン: ver\r\n作者: autr";
				this.msgboxStringsMain[22] = "プラグインをインストールできませんでした。詳細情報はデバッグコンソールにあります。";
				this.msgboxStringsMain[23] = "不正なプラグインの形式です。";
				this.msgboxStringsMain[24] = "チェックサムの検証に失敗しました。セキュリティのため、プラグインのインストールを中止します。";
				this.msgboxStringsMain[25] = "本当に 「plg」プラグインを削除してよろしいですか ?";
				this.msgboxStringsMain[26] = "アプリケーションで重大なエラーが発生しました。\r\n技術情報: \r\n exc\r\n TurnEdit は終了します。";
				this.msgboxStringsMain[27] = "アップデートが利用可能です。更新しますか ?";
				this.msgboxStringsMain[28] = "アプリケーションで重大なエラーが発生しました。\r\n TurnEdit は終了します。";
			}
			else if (languageCode2 == "en-US")
			{
				this.msgboxStringsMain[0] = "path is not allowed path.";
				this.msgboxStringsMain[1] = "File opening failed because I/O error.";
				this.msgboxStringsMain[2] = "File opening failed because unexpected error.";
				this.msgboxStringsMain[3] = "Please open file.";
				this.msgboxStringsMain[4] = "Failed to save file because I/O error.";
				this.msgboxStringsMain[5] = "Failed to save file because unexpected error.";
				this.msgboxStringsMain[6] = "Changes are unsaved. do you want save a changes?";
				this.msgboxStringsMain[7] = "Window closing failed because I/O error. unsaved changes will be destroyed, please force exit TurnEdit.";
				this.msgboxStringsMain[8] = "Window closing failed because security violation. unsaved changes will be destroyed, please force exit TurnEdit.";
				this.msgboxStringsMain[9] = "Window closing failed because unexpected error. unsaved changes will be destroyed, please force exit TurnEdit.";
				this.msgboxStringsMain[10] = "Help file not found.";
				this.msgboxStringsMain[11] = "Failed to open help: ";
				this.msgboxStringsMain[12] = "Error accessing clipboard.";
				this.msgboxStringsMain[13] = "TurnEdit";
				this.msgboxStringsMain[14] = "TurnEdit";
				this.msgboxStringsMain[15] = "TurnEdit";
				this.msgboxStringsMain[16] = "Error opening updater: ";
				this.msgboxStringsMain[17] = "Updater executable file not found.";
				this.msgboxStringsMain[18] = "File is too big. maximum file size is 50MB.";
				this.msgboxStringsMain[19] = "Can't load plugins.";
				this.msgboxStringsMain[20] = "Information";
				this.msgboxStringsMain[21] = "Plugin name: plgname\r\nDescription: desc\r\nVersion: ver\r\nAuthor: autr";
				this.msgboxStringsMain[22] = "Failure installing plugin. More information is contains on debug console.";
				this.msgboxStringsMain[23] = "Invalid plugin format.";
				this.msgboxStringsMain[24] = "Failure verifying checksum. For a security, plugin installation has aborted.";
				this.msgboxStringsMain[25] = $"Are you sure want to delete the \"plg\" plugin?";
				this.msgboxStringsMain[26] = "There's critical error on this application.\r\nTechnical information: \r\n exc\r\n TurnEdit will terminating.";
				this.msgboxStringsMain[27] = "An update is available. Do you want to update now?";
				this.msgboxStringsMain[28] = "There's critical error on this application.\r\n TurnEdit will terminating.";
			}
		}
    }
}