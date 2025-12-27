#pragma warning disable IDE0031
#pragma warning disable IDE1006
#pragma warning disable IDE0090
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TurnEdit
{
	using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
	using ICSharpCode.AvalonEdit.Editing;
	using ICSharpCode.AvalonEdit.Folding;
	using ICSharpCode.AvalonEdit.Highlighting;
	using ICSharpCode.AvalonEdit.Search;
	using System.Collections.Generic;
	using System.Linq;
    using System.Printing.IndexedProperties;
    using System.Runtime.InteropServices;
    using System.Text.Json;
	using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using TurnEdit.AI;
    using Wpf.Ui.Appearance;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    // If this line is uncommented occurs compile error
    //using Wpf.Ui.Controls;

    public class GitHubRelease {
		[JsonPropertyName("tag_name")]
		public string? TagName {get; set;}
	}
    /// <summary>
    /// The TurnEdit main window
    /// </summary>
    public partial class MainWindow
    {
        public bool IsFileOpened;
        public string? currentFileName;
        public bool ChangesUnsaved;
        public string? AppTheme;
        public string? CommandLineArgumentFileName;
		public bool? AcssFromApp;
		public bool DeveloperMode;
        private bool settingsVisible = false;
        private SearchPanel searchPanel;
        //private CompletionWindow? completionWindow;
		private FoldingManager foldingManager;
		private XmlFoldingStrategy foldingStrategy;
        // public  List<string>? recentFiles;
        public MainWindow()
        {
            //this.CommandLineArgumentFileName = null;
            InitializeComponent();
            //ApplySystemTheme();
            ApplicationAccentColorManager.ApplySystemAccent();
            this.IsFileOpened = false;
            this.currentFileName = null;
            this.msgboxStringsMain = new string[29];
            mainTextBox.Width = this.Width;
            mainTextBox.Height = this.Height - mainMenu.Height;
            this.CreateFileFileNotExists = null;
            this.SizeChanged += new SizeChangedEventHandler(sizeChangedEvent);
            this.ChangesUnsaved = false;
            this.TurnEditLanguage = "en-US";
            LoadTurnEditSettings();
            string themetoApply = this.theme ?? "System";
            ((App)Application.Current).ApplyAppTheme(themetoApply);
            try
            {
                PluginLoader.InitPluginAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.msgboxStringsMain[19], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine("TurnEdit: error: error loading plugins: " + ex.Message);
            }
            this.MouseLeftButtonDown += new MouseButtonEventHandler(this.MainWindow_MouseLeftButtonDown);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
			InitializeLanguagesList();
            /*
			if (this.AppTheme == "Light") {
				mainTextBox.Background = Brushes.White;
				mainTextBox.Foreground = Brushes.Black;
			} else {
				mainTextBox.Background = GetBrushFromHex("#101010");
				mainTextBox.Foreground = Brushes.White;
			}
            if (this.AppTheme == "Light")
            {
                mainMenu.Background = Brushes.White;
                mainTitleBar.Background = Brushes.White;
            }
            */
            //this.foldingManager = FoldingManager.Install(mainTextBox.TextArea);
			//this.foldingStrategy = new XmlFoldingStrategy();
			//this.foldingStrategy.UpdateFoldings(this.foldingManager, mainTextBox.Document);
            this.Title = $"{(string)this.TryFindResource("Title.Untitled") ?? "Untitled"} - TurnEdit";
            tabView.SelectionChanged += TabView_SelectionChanged;
            CheckTurnEditUpdate();
        }

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabView.SelectedItem is TabItem ti)
            {
                if (ti == addTab)
                {
                    // "+" タブを選択したら新しいタブを作る
                    CreateNewTab();
                    return;
                }

                // コンテンツが AvalonEdit のエディタであれば mainTextBox を差し替える
                if (ti.Content is TextEditor editor)
                {
                    // mainTextBox は最初のタブで XAML から生成されるため、動的タブを選んだときは参照を更新する
                    mainTextBox = editor;
                    // ファイル名・状態を更新
                    this.currentFileName = ti.Tag as string;
                    this.IsFileOpened = !string.IsNullOrEmpty(this.currentFileName);
                    this.Title = this.currentFileName != null ? $"{this.currentFileName} - TurnEdit" : $"{(string)this.TryFindResource("Title.Untitled") ?? "Untitled"} - TurnEdit";
                    // フォールディングを再セットアップ（簡易実装）
                    try
                    {
                        //this.foldingManager = FoldingManager.Install(editor.TextArea);
                        //this.foldingStrategy.UpdateFoldings(this.foldingManager, editor.Document);
                    }
                    catch
                    {
                        // 無視（既にインストール済みなど）
                    }
                }
            }
        }

        private void CloseTab(TabItem tab)
        {
            if (tab == null) return;
            int idx = tabView.Items.IndexOf(tab);
            tabView.Items.Remove(tab);
            // 最低1つはタブを維持する
            if (tabView.Items.Count <= 1)
            {
                CreateNewTab();
            }
            else
            {
                // 選択を調整
                if (idx - 1 >= 0 && idx - 1 < tabView.Items.Count)
                    tabView.SelectedIndex = Math.Max(0, idx - 1);
                else
                    tabView.SelectedIndex = 0;
            }
        }

        private void CreateSettingsTab()
        {
            var settingsWindow = new SettingsWindow(this);
            var content = (FrameworkElement)settingsWindow.Content;
            content.DataContext = settingsWindow.DataContext ?? settingsWindow;
            settingsWindow.Content = null;
            settingsWindow.Close();
            var newTab = new TabItem
            {
                Header = (string)this.TryFindResource("Menu.Settings") != null ? (string)this.TryFindResource("Menu.Settings") : "Settings",
                Content = content
            };
            var tabCm = new ContextMenu();
            var closeItem = new MenuItem { Header = "Close" };
            closeItem.Click += (s, e) => CloseTab(newTab);
            tabCm.Items.Add(closeItem);
            newTab.ContextMenu = tabCm;
            newTab.Tag = "SettingsTab";
            tabView.Items.Insert(Math.Max(0, tabView.Items.Count - 1), newTab);
            newTab.Focus();
        }

        private void CreateNewTab(string? filePath = null, string? content = null)
        {
            // エディタ生成
            var editor = new TextEditor
            {
                Background = (Brush)TryFindResource("EditorBackgroundBrush"),
                Foreground = (Brush)TryFindResource("EditorForegroundBrush"),
                FontFamily = new FontFamily(this.font),
                ShowLineNumbers = true,
                Text = content ?? string.Empty,
            };

            // コンテキストメニュー（簡易版）をコピー
            var cm = new ContextMenu();
            var cut = new MenuItem { Command = ApplicationCommands.Cut, Header = (string)TryFindResource("Menu.Cut") ?? "Cut" };
            var copy = new MenuItem { Command = ApplicationCommands.Copy, Header = (string)TryFindResource("Menu.Copy") ?? "Copy" };
            var paste = new MenuItem { Command = ApplicationCommands.Paste, Header = (string)TryFindResource("Menu.Paste") ?? "Paste" };
            cm.Items.Add(cut);
            cm.Items.Add(copy);
            cm.Items.Add(paste);
            cm.Items.Add(new Separator());
            var undo = new MenuItem { Command = ApplicationCommands.Undo, Header = (string)TryFindResource("Menu.Undo") ?? "Undo" };
            var redo = new MenuItem { Command = ApplicationCommands.Redo, Header = (string)TryFindResource("Menu.Redo") ?? "Redo" };
            cm.Items.Add(undo);
            cm.Items.Add(redo);
            // AI actions は既存のコントロールを利用して可視制御するのでここではシンプルに追加
            var aiMenu = new MenuItem { Header = "AI actions", Visibility = Visibility.Collapsed };
            aiMenu.Items.Add(new MenuItem { Header = "Summerize" });
            aiMenu.Items.Add(new MenuItem { Header = "Proofread" });
            aiMenu.Items.Add(new MenuItem { Header = "Write more" });
            cm.Items.Add(aiMenu);
            editor.ContextMenu = cm;

            // イベント接続
            editor.KeyUp += mainTextBox_KeyUp;
            // AvalonEdit の TextChanged は EventHandler<EventArgs> なので既存メソッドを利用
            editor.TextChanged += mainTextBox_TextChanged;
            // 変更時に文字数更新などを行う小さなハンドラを追加
            editor.TextChanged += (s, e) =>
            {
                this.ChangesUnsaved = true;
                int textLength = editor.Text.Length;
                totalTextCount.Text = (string)this.FindResource("Status.TotalTextCount") + ": " + textLength.ToString();
            };
            editor.KeyDown += OnKeyDown;
            // TabItem 準備
            var tab = new TabItem();
            tab.Header = filePath != null ? System.IO.Path.GetFileName(filePath) : (string)this.TryFindResource("Title.Untitled") ?? "Untitled";
            tab.Content = editor;
            tab.Tag = filePath; // ファイルパスは Tag に保存

            // タブに閉じるメニュー
            var tabCm = new ContextMenu();
            var closeItem = new MenuItem { Header = "Close" };
            closeItem.Click += (s, e) => CloseTab(tab);
            tabCm.Items.Add(closeItem);
            tab.ContextMenu = tabCm;

            // 追加位置は最後の "+" の手前
            int insertIndex = Math.Max(0, tabView.Items.Count - 1);
            tabView.Items.Insert(insertIndex, tab);
            tabView.SelectedItem = tab;
        }

        private void Menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.OriginalSource is MenuItem) return;
			if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
		}

        // Menu のプレビュー段階ハンドラ：MenuItem 等の子要素でなければウィンドウを移動
        private void Menu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            // クリック元が MenuItem やその子孫であればドラッグしない
            var source = e.OriginalSource as DependencyObject;
            if (FindAncestor<MenuItem>(source) != null) return;
            if (FindAncestor<Separator>(source) != null) return;

            try
            {
                // 最大化状態から通常に戻してドラッグ開始（既存挙動に合わせる）
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.DragMove();
                }
                else
                {
                    this.DragMove();
                }
                e.Handled = true;
            }
            catch
            {
                // DragMove が例外を投げることがあるので安全に無視
            }
        }

        // ビジュアルツリーをさかのぼって指定型の祖先を探すヘルパ
        private static T? FindAncestor<T>(DependencyObject? d) where T : DependencyObject
        {
            while (d != null)
            {
                if (d is T t) return t;
                d = VisualTreeHelper.GetParent(d);
            }
            return null;
        }
        public SolidColorBrush GetBrushFromHex(string hex) {
			var converter = new BrushConverter();
			SolidColorBrush brush = (SolidColorBrush)converter!.ConvertFromString(hex)!;
			return brush;
		}
		public void ApplySystemTheme() {
			if (IsSystemLightTheme()) {
				ApplicationThemeManager.Apply(
					ApplicationTheme.Light
				);
			} else {
				ApplicationThemeManager.Apply(
					ApplicationTheme.Dark
				);
			}
		}
		public MainWindow(string filePath) : this() {
			OpenInCommandLineArgument(filePath);
		}
		private bool IsSystemLightTheme() {
			const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "AppsUseLightTheme";
			using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath)) {
				var keyvalue = key?.GetValue(valueName);
				return keyvalue is int i && i > 0;
			}
		}
		
		public void occurExc_Click(object? sender, RoutedEventArgs e) {
			throw new Exception("Exception throwed by TurnEdit debug.");
		}
		
        /*
        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                mainTextBox.Width = this.Width;
                mainTextBox.Height = this.Height - mainMenu.Height;
            }
        }
        */
		private void mainTextBox_TextChanged(object sender, EventArgs e) {
			this.ChangesUnsaved = true;
		}
		private async void CheckTurnEditUpdate() {
			try {
            string versionstr = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();
			Version version = Version.Parse(versionstr);
			var client = new System.Net.Http.HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("TurnEdit-Updater");
			var str = await client.GetStringAsync("https://api.github.com/repos/RealRosaYT/TurnEdit/releases/latest");
			GitHubRelease? deserializedJson = JsonSerializer.Deserialize<GitHubRelease>(str);
			Version GitHubVersion = Version.Parse(deserializedJson!.TagName!);
			if (GitHubVersion > version) {
				if (File.Exists("TurnEditUpdater.exe")) {
					MessageBoxResult result = MessageBox.Show((string)this.FindResource("Msg.UpdateAvailable"), (string)this.FindResource("Generic.Information"), MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes) {
						Process.Start("TurnEditUpdater.exe");
					}
				}
			}
			} catch (Exception) {
				return;
			}
		}
		
		/// <summary>
		/// Handles unhandled exception.
		/// </summary>
		public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = (Exception)e.ExceptionObject;
			if (this.DeveloperMode) {
                string details = string.Format((string)this.FindResource("Msg.CriticalErrorWithDetails"), ex.ToString());
			    MessageBox.Show(details, (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
			} else {
				MessageBox.Show((string)this.FindResource("Msg.CriticalError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
			}
			Environment.Exit(1);
		}
        /// <summary>
        /// Checks path is safe.
        /// </summary>
        /// <param name="filePath">File path to check</param>
        /// <returns>bool</returns>
        /// <exception cref="System.Security.SecurityException">This exception throws when path is not safe</exception>
        public bool IsPathSafe(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new SecurityException("Path is empty or whitespace.");

            // 明示的に入力文字列内の相対上位参照を禁止（ポリシー）
            if (filePath.Contains(".."))
                throw new SecurityException("The character \"..\", which represents the next higher directory, is not allowed.");

            string fullPath;
            try
            {
                fullPath = System.IO.Path.GetFullPath(filePath.Trim());
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                throw new SecurityException("Invalid path.", ex);
            }

            if (!filePath.StartsWith(fullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityException("Path traversal is detected!");
            }

            // 予約デバイス名チェック（ファイル名の拡張子を除いた部分で比較）
            string[] reservedDevicesNames = { "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            string fileName = System.IO.Path.GetFileName(fullPath);
            if (!string.IsNullOrEmpty(fileName))
            {
                string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                bool isReserved = reservedDevicesNames.Any(dev =>
                    string.Equals(nameWithoutExtension, dev, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileName, dev, StringComparison.OrdinalIgnoreCase));
                if (isReserved)
                    throw new SecurityException("The file name contains a reserved device name.");
            }

            // 禁止ディレクトリ（正規化して末尾区切り文字を付けて比較）
            string[] forbiddenDirectories = {
                @"C:\Windows",
                @"C:\Program Files",
                @"C:\Program Files (x86)",
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                System.IO.Path.GetTempPath().TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
            };

            string normalizedFullPath = System.IO.Path.GetFullPath(fullPath)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                + System.IO.Path.DirectorySeparatorChar;

            foreach (string dir in forbiddenDirectories)
            {
                if (string.IsNullOrWhiteSpace(dir))
                    continue;
                try
                {
                    string normalizedDir = System.IO.Path.GetFullPath(dir)
                        .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                        + System.IO.Path.DirectorySeparatorChar;
                    if (normalizedFullPath.StartsWith(normalizedDir, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new SecurityException(this.msgboxStringsMain[0].Replace("path", filePath));
                    }
                }
                catch
                {
                    // 無効な forbidden ディレクトリは無視
                    continue;
                }
            }

            return true;
        }

        public void ExitTurnEdit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void newFileNav_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }
        private void NewFile()
        {
            CreateNewTab();
        }

        private void searchNav_Click(object sender, RoutedEventArgs e)
        {
           SearchWindow searchWindow = new SearchWindow(this);
		   searchWindow.Show();
        }

        private void insertDateAndTimeNav_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            string nowStr = now.ToString();
            mainTextBox.Text += nowStr;
        }

        private async void openNav_Click(object sender, RoutedEventArgs e)
        {
            await OpenFile();
        }
        public async void OpenInCommandLineArgument(string filePath)
        {
            try
            {
                string validatedPath = System.IO.Path.GetFullPath(filePath);
                if (!File.Exists(validatedPath))
                {
                    MessageBox.Show("File not found.");
                    return;
                }
                FileInfo fileInfo = new FileInfo(validatedPath);
                long fileSizeInBytes = fileInfo.Length;
                const long maxFileSizeInBytes = 50 * 1024 * 1024;
                if (fileSizeInBytes > maxFileSizeInBytes)
                {
                    MessageBox.Show(this.msgboxStringsMain[18], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
                }
                IsPathSafe(validatedPath);
                string content = await File.ReadAllTextAsync(validatedPath);
                //this.mainTextBox.Text = content;
                //this.currentFileName = validatedPath;
                //this.Title = $"{this.currentFileName} - TurnEdit";
                //this.IsFileOpened = true;
                CreateNewTab(validatedPath, content);
                this.ChangesUnsaved = false;
				IHighlightingDefinition language = HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(filePath));
				mainTextBox.SyntaxHighlighting = language;
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show(filePath + " is not allowed path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error opening file because I/O error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file because unexpected error: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void searchOnGoogle_Click(object sender, RoutedEventArgs e)
        {
            string encodedText = Uri.EscapeDataString(mainTextBox.SelectedText);
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://www.google.com/search?q={encodedText}",
                UseShellExecute = true
            });
        }
        private void searchOnBing_Click(object sender, EventArgs e)
        {
            string encodedText = Uri.EscapeDataString(mainTextBox.SelectedText);
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://www.bing.com/search?q={encodedText}",
                UseShellExecute = true
            });
        }
        private async Task OpenFile()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (this.CreateFileFileNotExists == true)
                {
                    ofd.CheckFileExists = true;
                }
                else
                {
                    ofd.CheckFileExists = false;
                }
                if (this.DefaultDirectory is not null)
                {
                    ofd.DefaultDirectory = this.DefaultDirectory;
                }
                ofd.CheckPathExists = true;
                ofd.Filter = "Text File(*.txt)|*.txt|All File(*.*)|*.*";
                if (ofd.ShowDialog() == true)
                {
                    IsPathSafe(ofd.FileName);
                    string fileText = await File.ReadAllTextAsync(ofd.FileName);
                    CreateNewTab(ofd.FileName, fileText);
                    //mainTextBox.Text = fileText;
                    this.currentFileName = ofd.FileName;
                    this.IsFileOpened = true;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
					IHighlightingDefinition language = HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(ofd.FileName));
					mainTextBox.SyntaxHighlighting = language;
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show((string)this.FindResource("Msg.FileOpenIOError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show((string)this.FindResource("Msg.FileOpenUnexpectedError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void sizeChangedEvent(object sender, SizeChangedEventArgs e)
        {
            mainTextBox.Width = this.ActualWidth;
            mainTextBox.Height = this.ActualHeight - mainMenu.Height;
        }

        private void aboutTurnEditNav_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow(this);
            aboutWindow.ShowDialog();
        }
        private void updaterNav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TurnEditUpdater.exe")))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "TurnEditUpdater.exe",
                        UseShellExecute = false
                    });
                    Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show((string)this.FindResource("Msg.UpdaterNotFound"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{(string)this.FindResource("Msg.UpdaterOpenError")} {ex.Message}", (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void saveNav_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private async void SaveFile()
        {
            try
            {
                if (currentFileName is not null)
                {
                    IsPathSafe(currentFileName);
                    await File.WriteAllTextAsync(currentFileName, mainTextBox.Text);
                    this.ChangesUnsaved = false;
                    if (tabView.SelectedItem is TabItem ti)
                    {
                        ti.Tag = currentFileName;
                        ti.Header = System.IO.Path.GetFileName(currentFileName);
                    }
                }
                else
                {
                    MessageBox.Show((string)this.FindResource("Msg.PleaseOpenFile"), (string)this.FindResource("Generic.Information"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show((string)this.FindResource("Msg.SaveIOError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show((string)this.FindResource("Msg.SaveUnexpectedError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveAs()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text file(*.txt)|*.txt|All file(*.*)|*.*";
                if (sfd.ShowDialog() == true)
                {
                    IsPathSafe(sfd.FileName);
                    await File.WriteAllTextAsync(sfd.FileName, mainTextBox.Text);
                    this.IsFileOpened = true;
                    this.currentFileName = sfd.FileName;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;

                    if (tabView.SelectedContent is TabItem ti)
                    {
                        ti.Tag = sfd.FileName;
                        ti.Header = System.IO.Path.GetFileName(sfd.FileName);
                    }
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show((string)this.FindResource("Msg.SaveIOError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show((string)this.FindResource("Msg.SaveUnexpectedError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void saveAsNav_Click(object sender, RoutedEventArgs e)
        {
            await SaveAs();
        }

        /// <summary>
        /// This function is handle when clicked "Settings" navigation.
        /// </summary>
        private void settingsNav_Click(object sender, RoutedEventArgs e)
        {
            //var settingsWindow = new SettingsWindow(this);
            //settingsWindow.ShowDialog();
            CreateSettingsTab();
        }

        /// <summary>
        /// This function is handle when clicked "Replace" navigation.
        /// </summary>
        private void replaceNav_Click(object sender, RoutedEventArgs e)
        {
			ReplaceWindow replaceWindow = new ReplaceWindow(this);
			replaceWindow.Show();
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                NewFile();
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                await OpenFile();
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.currentFileName is not null)
                {
                    SaveFile();
                }
                else if (this.currentFileName is null)
                {
                    await SaveAs();
                }
            }
            else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SearchPanel.Install(mainTextBox.TextArea);
            }
            else if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ReplaceWindow replaceWindow = new ReplaceWindow(this);
				replaceWindow.Show();
            }
        }

        private void mainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Caret caret = mainTextBox.TextArea.Caret;
			int line = caret.Line;
			int column = caret.Column;
            lineStatus.Text = (string)this.FindResource("Status.Line") + ": " + line.ToString();
            columnStatus.Text = (string)this.FindResource("Status.Column") + ": " + column.ToString();
        }

        private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ChangesUnsaved = true;
            int textLength = mainTextBox.Text.Length;
            totalTextCount.Text = (string)this.FindResource("Status.TotalTextCount") + ": " + textLength.ToString();
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (this.ChangesUnsaved == true)
                {
                    MessageBoxResult msgbox = MessageBox.Show((string)this.FindResource("Msg.UnsavedChangesQuestion"), (string)this.FindResource("Generic.Information"), MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (msgbox == MessageBoxResult.Yes)
                    {
                        if (this.currentFileName is not null)
                        {
                            SaveFile();
							if (Application.Current.Windows.OfType<MainWindow>().Count() <= 1) {
							Application.Current.Shutdown();
							}
                        }
                        else if (this.currentFileName is null)
                        {
                            await SaveAs();
							if (Application.Current.Windows.OfType<MainWindow>().Count() <= 1) {
							Application.Current.Shutdown();
							}
                        }
                    }
                    else if (msgbox == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
						if (Application.Current.Windows.OfType<MainWindow>().Count() <= 1) {
						Application.Current.Shutdown();
						}
                    }
                }
				if (Application.Current.Windows.OfType<MainWindow>().Count() <= 1) {
				Application.Current.Shutdown();
				}
            }
            catch (IOException)
            {
                MessageBox.Show((string)this.FindResource("Msg.WindowCloseIOError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show((string)this.FindResource("Msg.WindowCloseSecurityError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show((string)this.FindResource("Msg.WindowCloseUnexpectedError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void helpNav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var helpWindow = new HelpWindow();
				helpWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show((string)this.FindResource("Msg.FailedToOpenHelp") + " " + ex.Message, (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void undoNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.Undo();
        }

        private void redoNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.Redo();
        }

        private void cutNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.Cut();
        }

        private void copyNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.Copy();
        }

        private void pasteNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.Paste();
        }

        private void selectAllNav_Click(object sender, RoutedEventArgs e)
        {
            mainTextBox.SelectAll();
        }

        private void deleteCurrentLine_Click(object sender, RoutedEventArgs e)
        {
			
        }

        private void pasteWithQuotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    string modifiedText = "\"" + clipboardText + "\"";
                    Clipboard.SetText(modifiedText);
                    mainTextBox.Paste();
                    Clipboard.SetText(clipboardText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show((string)this.FindResource("Msg.ClipboardError"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"TurnEdit: error: failure accessing clipboard: {ex.Message}");
            }
        }

        private void newWindowNav_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        public void pluginsNav_Click(object sender, RoutedEventArgs e)
        {
            PluginsLstWindow _pluginsWindow = new PluginsLstWindow(this);
            _pluginsWindow.ShowDialog();
        }
		public void IsLineNumberShowed_Checked(object sender, RoutedEventArgs e) {
			
		}
		public void IsLineNumberShowed_Unchecked(object sender, RoutedEventArgs e) {
			
		}
		private void MainWindow_MouseLeftButtonDown(object? sender, MouseButtonEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed) {
				Point startPoint = e.GetPosition(this);
				if (this.WindowState == WindowState.Maximized) {
					this.WindowState = WindowState.Normal;
					this.DragMove();
				} else {
					this.DragMove();
				}
			}
		}
		private void Window_Drop(object sender, DragEventArgs e) {
			string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (files == null) {
				return;
			}
			if (System.IO.File.Exists(files[0]) == false) {
				return;
			}
			this.Title = files[0] + " - TurnEdit";
			OpenInCommandLineArgument(files[0]);
		}
		private void Window_PreviewDragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop, true)) {
				e.Effects = DragDropEffects.Copy;
			} else {
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}
		private void InitializeLanguagesList() {
			var manager = HighlightingManager.Instance;
			List<string> languageNames = manager.HighlightingDefinitions
				.Select(def => def.Name)
				.ToList();
			foreach (var lang in languageNames) {
				MenuItem item = new MenuItem();
				item.Header = lang;
				item.Click += (sender, e) => langClick(sender, e, lang);
				languagesNav.Items.Add(item);
			}
		}
        /// <summary>
        /// This method handles when clicked language menu.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        /// <param name="language">Language</param>
		private void langClick(object sender, RoutedEventArgs e, string language) {
			IHighlightingDefinition langDefinition = HighlightingManager.Instance.GetDefinition(language);
			mainTextBox.SyntaxHighlighting = langDefinition;
		}
		private void moveLineNav_Click(object sender, RoutedEventArgs e) {
			MoveLineWindow moveLineWindow = new MoveLineWindow(this);
			moveLineWindow.Show();
		}
		private void scrollToEnd_Click(object sender, RoutedEventArgs e) {
			mainTextBox.ScrollToEnd();
		}

        private void yesterdayInsert_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Today;
            DateTime yesterday = now.AddDays(-1);
            mainTextBox.Text += yesterday.ToShortDateString();
        }

        private void dayBeforeYesterdayInsert_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Today;
            DateTime dayBeforeYesterday = now.AddDays(-2);
            mainTextBox.Text += dayBeforeYesterday.ToShortDateString();
        }

        private void tomorrowInsert_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Today;
            DateTime tomorrow = now.AddDays(1);
            mainTextBox.Text += tomorrow.ToShortDateString();
        }
		
		private void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.F2) {
				if (this.IsEasterEggEnabled) {
					string selectedText = mainTextBox.SelectedText.Trim();
					if (selectedText == "5e7b139a7e6b5a498b56849c882bb749") {
						e.Handled = true;
						InvokeEasterEgg();
					}
				}
			}
		}
		
		private async void InvokeEasterEgg() {
            try
            {
                MessageBox.Show("REALROSA IS THE BEST ROBLOX OBBY PLAYER!!!!", "Easter egg invoked", MessageBoxButton.OK, MessageBoxImage.Information);
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "TurnEdit.EasterEgg.txt";
                string easterEggText;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            easterEggText = await sr.ReadToEndAsync();
                        }
                        CreateNewTab();
                        var selectedTab = tabView.SelectedItem as TabItem;
                        if (selectedTab != null)
                        {
                            selectedTab.Header = "REALROSA IS THE BEST";
                        }
                        mainTextBox.Text = easterEggText ?? "";
                    } else
                    {
                        throw new Exception("Easter egg resource is null.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to display easter egg.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}


        private async void summerizeTextAI_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mainTextBox.SelectedText)) return;
            try
            {
                this.Cursor = Cursors.Wait;
                var aiModelID = AIHelper.GetAIModelString((AIModels)this.selectedAiModel);
                string generatedContent = await AIHelper.SummarizeTextAsync(mainTextBox.SelectedText, aiModelID, this.AiApiKey ?? "");
                mainTextBox.SelectedText = generatedContent;
            } catch (InvalidOperationException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } catch (ArgumentNullException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } catch (Exception)
            {
                return;
            } finally
            {
                this.Cursor = null;
            }
        }

        private async void proofreadTextAI_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mainTextBox.SelectedText)) return;
            try
            {
                this.Cursor = Cursors.Wait;
                var aiModelID = AIHelper.GetAIModelString((AIModels)this.selectedAiModel);
                string generatedContent = await AIHelper.ProofreadTextAsync(mainTextBox.SelectedText, aiModelID, this.AiApiKey ?? "");
                mainTextBox.SelectedText = generatedContent;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception)
            {
                return;
            } finally
            {
                this.Cursor = null;
            }
        }

        private async void writeMoreTextAI_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mainTextBox.SelectedText)) return;
            try
            {
                this.Cursor = Cursors.Wait;
                var aiModelID = AIHelper.GetAIModelString((AIModels)this.selectedAiModel);
                string generatedContent = await AIHelper.WriteMoreAsync(mainTextBox.SelectedText, aiModelID, this.AiApiKey ?? "");
                mainTextBox.SelectedText = generatedContent;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show((string)this.FindResource("AI.ErrorGeneratedNull"), (string)this.FindResource("Generic.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception)
            {
                return;
            } finally
            {
                this.Cursor = null;
            }
        }

        private async void fileOpenToolBar_Click(object sender, RoutedEventArgs e)
        {
            await OpenFile();
        }

        private async void fileSaveToolBar_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private async void fileSaveAsToolBar_Click(object sender, RoutedEventArgs e)
        {
            await SaveAs();
        }

        private void fileNewToolBar_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }

        private void editSearchToolBar_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow(this);
            searchWindow.Show();
        }
    }
}