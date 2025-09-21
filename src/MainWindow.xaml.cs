using System;
using System.Text;
using System.IO;
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
using System.Runtime.CompilerServices;
using System.Linq;
using System.Security;
using System.Windows.Controls.Ribbon;

namespace TurnEdit
{
	using System.Text.Json;
	using System.Text.Json.Serialization;
	public class GitHubRelease {
		[JsonPropertyName("tag_name")]
		public string? TagName {get; set;}
	}
    /// <summary>
    /// The TurnEdit main window
    /// </summary>
    public partial class MainWindow : Window
    {
		private static MainWindow _instance;
        public bool IsFileOpened;
        public string? currentFileName;
        public bool ChangesUnsaved;
        public string? AppTheme;
        public string? CommandLineArgumentFileName;
		public bool? AcssFromApp;
		public bool DeveloperMode;
        // public  List<string>? recentFiles;
        public MainWindow()
        {
            //this.CommandLineArgumentFileName = null;
            InitializeComponent();
			Loaded += (sender, args) =>
        {
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                this,                                    // Window class
                Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
                true                                     // Whether to change accents automatically
            );
        };
            this.IsFileOpened = false;
            this.currentFileName = null;
            this.msgboxStringsMain = new string[29];
            mainTextBox.Width = this.Width;
            mainTextBox.Height = this.Height - mainMenu.Height;
            this.CreateFileFileNotExists = null;
            this.SizeChanged += new SizeChangedEventHandler(sizeChangedEvent);
            this.ChangesUnsaved = false;
            this.TurnEditLanguage = "en-US";
            InitlaizeMsgboxStrings("en-US");
            LoadTurnEditSettings();
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
            // this.StateChanged += MainWindow_StateChanged;
            // this is using when officially released
            //this.ThemeMode = ThemeMode.Light;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
			CheckTurnEditUpdate();
        }
		public MainWindow(string filePath) : this() {
			OpenInCommandLineArgument(filePath);
		}
		public static MainWindow Instance {
			get {
				if (_instance == null) {
					_instance = new MainWindow();
				}
				return _instance;
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
		
		private async void CheckTurnEditUpdate() {
			try {
			Version version = Version.Parse(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			var client = new System.Net.Http.HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("TurnEdit-Updater");
			var str = await client.GetStringAsync("https://api.github.com/repos/RealRosaYT/TurnEdit/releases/latest");
			var deserializedJson = JsonSerializer.Deserialize<GitHubRelease>(str);
			Version GitHubVersion = Version.Parse(deserializedJson.TagName);
			if (GitHubVersion > version) {
				if (File.Exists("TurnEditUpdater.exe")) {
					MessageBoxResult result = MessageBox.Show(this.msgboxStringsMain[27], this.msgboxStringsMain[13], MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes) {
						Process.Start("TurnEditUpdater.exe");
					}
				}
			}
			} catch (Exception ex) {
				return;
			}
		}
		
		/// <summary>
		/// Handles unhandled exception.
		/// </summary>
		public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = (Exception)e.ExceptionObject;
			if (this.DeveloperMode) {
			MessageBox.Show(this.msgboxStringsMain[26].Replace("exc", ex.ToString()), this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			} else {
				MessageBox.Show(this.msgboxStringsMain[28], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
            string[] forbiddenDirectories = {
                @"C:\Windows\",
                @"C:\Program Files\",
                @"C:\Program Files (x86)\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                System.IO.Path.GetTempPath()
            };
            string fullPath = System.IO.Path.GetFullPath(filePath);
            foreach (string forbiddenDirectory in forbiddenDirectories)
            {
                string normalizedForbiddenDirectory = System.IO.Path.GetFullPath(forbiddenDirectory);
                if (fullPath.StartsWith(normalizedForbiddenDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    throw new System.Security.SecurityException(this.msgboxStringsMain[0].Replace("path", filePath));
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
            mainTextBox.Clear();
            this.IsFileOpened = false;
            this.currentFileName = null;
            this.ChangesUnsaved = false;
            this.Title = "Untitled - TurnEdit";
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
                this.mainTextBox.Text = content;
                this.currentFileName = validatedPath;
                this.Title = $"{this.currentFileName} - TurnEdit";
                this.IsFileOpened = true;
                this.ChangesUnsaved = false;
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
        private void UpdateLineNumbers()
        {
            var text = mainTextBox.Text;
            var lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);
            int lineCount = lines.Length;
            if (text.Length > 0 && lines.Last().Length == 0)
            {
                lineCount--;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= lineCount; i++)
            {
                sb.AppendLine(i.ToString());
            }
            lineNumberTxtBox.Text = sb.ToString();
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
                    mainTextBox.Text = fileText;
                    this.currentFileName = ofd.FileName;
                    this.IsFileOpened = true;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(this.msgboxStringsMain[1], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this.msgboxStringsMain[2], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (File.Exists("TurnEditUpdater.exe"))
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
                    MessageBox.Show(this.msgboxStringsMain[17], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{this.msgboxStringsMain[16]}{ex.Message}", this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void saveNav_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            try
            {
                if (currentFileName is not null)
                {
                    IsPathSafe(currentFileName);
                    File.WriteAllText(currentFileName, mainTextBox.Text);
                    this.ChangesUnsaved = false;
                }
                else
                {
                    MessageBox.Show(this.msgboxStringsMain[3], this.msgboxStringsMain[14], MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(this.msgboxStringsMain[4], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this.msgboxStringsMain[5], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
                }
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(this.msgboxStringsMain[4], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this.msgboxStringsMain[5], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.ShowDialog();
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
                SearchWindow searchWindow = new SearchWindow(this);
                searchWindow.Show();
            }
            else if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ReplaceWindow replaceWindow = new ReplaceWindow(this);
                replaceWindow.Show();
            }
        }

        private void mainTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            int caretIndex = mainTextBox.CaretIndex;
            int lineIndex = mainTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int firstCharIndexInLine = mainTextBox.GetCharacterIndexFromLineIndex(lineIndex);
            int columnIndex = caretIndex - firstCharIndexInLine;
            if (this.TurnEditLanguage == "ja-JP")
            {
                lineStatus.Text = $@"行: {lineIndex + 1}";
                columnStatus.Text = $@"列: {columnIndex + 1}";
            }
            else if (this.TurnEditLanguage == "en-US")
            {
                lineStatus.Text = $@"Line: {lineIndex + 1}";
                columnStatus.Text = $@"Column: {columnIndex + 1}";
            }
        }

        private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ChangesUnsaved = true;
            int textLength = mainTextBox.Text.Replace("\n", null).Length;
            if (this.TurnEditLanguage == "en-US")
            {
                totalTextCount.Text = "Total text count: " + textLength;
            }
            else if (this.TurnEditLanguage == "ja-JP")
            {
                totalTextCount.Text = "文字の総数: " + textLength;
            }
            UpdateLineNumbers();
        }
        private void mainTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateLineNumbers();
            lineNumberTxtBox.ScrollToVerticalOffset(mainTextBox.VerticalOffset);
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (this.ChangesUnsaved == true)
                {
                    MessageBoxResult msgbox = MessageBox.Show(this.msgboxStringsMain[6], this.msgboxStringsMain[14], MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
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
                MessageBox.Show(this.msgboxStringsMain[7], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show(this.msgboxStringsMain[8], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this.msgboxStringsMain[9], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(this.msgboxStringsMain[11] + ex.Message, this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
            int caretIndex = mainTextBox.CaretIndex;
            int lineIndex = mainTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int firstCharIndexInLine = mainTextBox.GetCharacterIndexFromLineIndex(lineIndex);
            int columnIndex = caretIndex - firstCharIndexInLine;
            string oldLineText = mainTextBox.GetLineText(lineIndex);
            mainTextBox.SelectionStart = firstCharIndexInLine;
            mainTextBox.SelectionLength = oldLineText.Length;
            mainTextBox.SelectedText = "";
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
                MessageBox.Show(this.msgboxStringsMain[12], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
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
			if (lineNumberTxtBox != null) {
			lineNumberTxtBox.Visibility = Visibility.Visible;
			}
		}
		public void IsLineNumberShowed_Unchecked(object sender, RoutedEventArgs e) {
			if (lineNumberTxtBox != null) {
			lineNumberTxtBox.Visibility = Visibility.Collapsed;
			}
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
			string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
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
    }
}