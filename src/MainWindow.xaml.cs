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
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Security;

namespace TurnEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsFileOpened;
        public string? currentFileName;
        public bool ChangesUnsaved;
        public string? AppTheme;
        // public  List<string>? recentFiles;
        public MainWindow()
        {
            InitializeComponent();
            this.IsFileOpened = false;
            this.currentFileName = null;
			this.msgboxStringsMain = new string[19];
            mainTextBox.Width = this.Width;
            mainTextBox.Height = this.Height - mainMenu.Height;
            this.CreateFileFileNotExists = null;
            this.SizeChanged += new SizeChangedEventHandler(sizeChangedEvent);
            this.ChangesUnsaved = false;
			this.TurnEditLanguage = "en-US";
			InitlaizeMsgboxStrings("en-US");
            LoadTurnEditSettings();
            // this.StateChanged += MainWindow_StateChanged;
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

		public bool IsPathSafe(string filePath) {
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
			foreach (string forbiddenDirectory in forbiddenDirectories) {
				string normalizedForbiddenDirectory = System.IO.Path.GetFullPath(forbiddenDirectory);
				if (fullPath.StartsWith(normalizedForbiddenDirectory, StringComparison.OrdinalIgnoreCase)) {
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

        private void openNav_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }
		public void OpenInCommandLineArgument(string filePath) {
			try {
				string validatedPath = System.IO.Path.GetFullPath(filePath);
				if (!File.Exists(validatedPath)) {
				MessageBox.Show("File not found.");
				return;
				}
				FileInfo fileInfo = new FileInfo(validatedPath);
				long fileSizeInBytes = fileInfo.Length;
				const long maxFileSizeInBytes = 50 * 1024 * 1024;
				if (fileSizeInBytes > maxFileSizeInBytes) {
					MessageBox.Show(this.msgboxStringsMain[18], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				}
				IsPathSafe(validatedPath);
				string content = File.ReadAllText(validatedPath);
				this.mainTextBox.Text = content;
				this.currentFileName = validatedPath;
				this.Title = $"{this.currentFileName} - TurnEdit";
				this.IsFileOpened = true;
				this.ChangesUnsaved = false;
			} catch (System.Security.SecurityException ex) {
				MessageBox.Show(filePath + " is not allowed path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			} catch (IOException ex) {
				MessageBox.Show($"Error opening file because I/O error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			} catch (Exception ex) {
				MessageBox.Show($"Error opening file because unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void UpdateLineNumbers() {
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
		private void searchOnGoogle_Click(object sender, RoutedEventArgs e) {
			string encodedText = Uri.EscapeDataString(mainTextBox.SelectedText);
			Process.Start(new ProcessStartInfo{
				FileName = $"https://www.google.com/search?q={encodedText}",
				UseShellExecute = true
			});
		}
		private void searchOnBing_Click(object sender, EventArgs e) {
			string encodedText = Uri.EscapeDataString(mainTextBox.SelectedText);
			Process.Start(new ProcessStartInfo{
				FileName = $"https://www.bing.com/search?q={encodedText}",
				UseShellExecute = true
			});
		}
        private void OpenFile()
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
                    string fileText = File.ReadAllText(ofd.FileName);
                    mainTextBox.Text = fileText;
                    this.currentFileName = ofd.FileName;
                    this.IsFileOpened = true;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
                }
            }
			catch (System.Security.SecurityException ex) {
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
		private void updaterNav_Click(object sender, RoutedEventArgs e) {
			try {
				if (File.Exists("TurnEditUpdater.exe")) {
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{
				FileName = "TurnEditUpdater.exe",
				UseShellExecute = false
				});
				Environment.Exit(0);
				} else {
					MessageBox.Show(this.msgboxStringsMain[17], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				}
			} catch (Exception ex) {
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
			catch (System.Security.SecurityException ex) {
				MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (IOException) {
				MessageBox.Show(this.msgboxStringsMain[4], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			}
            catch (Exception)
            {
                MessageBox.Show(this.msgboxStringsMain[5], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAs()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text file(*.txt)|*.txt|All file(*.*)|*.*";
                if (sfd.ShowDialog() == true)
                {
					IsPathSafe(sfd.FileName);
                    File.WriteAllText(sfd.FileName, mainTextBox.Text);
                    this.IsFileOpened = true;
                    this.currentFileName = sfd.FileName;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
                }
            }
			catch (System.Security.SecurityException ex) {
				MessageBox.Show(ex.Message, this.msgboxStringsMain[15], MessageBoxButton.OK, MessageBoxImage.Error);
			}
            catch (IOException)
            {
                MessageBox.Show(this.msgboxStringsMain[4], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
            }
			catch (Exception) {
				MessageBox.Show(this.msgboxStringsMain[5], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        private void saveAsNav_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                NewFile();
            } else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                OpenFile();
            } else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.currentFileName is not null)
                {
                    SaveFile();
                } else if (this.currentFileName is null)
                {
                    SaveAs();
                }
            } else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SearchWindow searchWindow = new SearchWindow(this);
                searchWindow.Show();
            } else if (e.Key == Key.H && Keyboard.Modifiers == ModifierKeys.Control)
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
			if (this.TurnEditLanguage == "ja-JP") {
            lineStatus.Text = $@"行: {lineIndex + 1}";
            columnStatus.Text = $@"列: {columnIndex + 1}";
			} else if (this.TurnEditLanguage == "en-US") {
			lineStatus.Text = $@"Line: {lineIndex + 1}";
			columnStatus.Text = $@"Column: {columnIndex + 1}";
			}
        }

        private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ChangesUnsaved = true;
            int textLength = mainTextBox.Text.Replace("\n", null).Length;
			if (this.TurnEditLanguage == "en-US") {
            totalTextCount.Text = "Total text count: " + textLength;
			} else if (this.TurnEditLanguage == "ja-JP") {
				totalTextCount.Text = "文字の総数: " + textLength;
			}
			UpdateLineNumbers();
        }
		private void mainTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			UpdateLineNumbers();
			lineNumberTxtBox.ScrollToVerticalOffset(mainTextBox.VerticalOffset);
		}
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			try {
            if (this.ChangesUnsaved == true)
            {
                MessageBoxResult msgbox = MessageBox.Show(this.msgboxStringsMain[6], this.msgboxStringsMain[14], MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (msgbox == MessageBoxResult.Yes)
                {
                    if (this.currentFileName is not null)
                    {
                        SaveFile();
                    } else if (this.currentFileName is null)
                    {
                        SaveAs();
                    }
                } else if (msgbox == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                } else
                {
                    // Do nothing
                }
            }
			} catch (IOException) {
				MessageBox.Show(this.msgboxStringsMain[7], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			} catch (System.Security.SecurityException) {
				MessageBox.Show(this.msgboxStringsMain[8], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			} catch (Exception) {
				MessageBox.Show(this.msgboxStringsMain[9], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }
		private void helpNav_Click(object sender, RoutedEventArgs e) {
			try {
				if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "turnedit-help.chm"))) {
					System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{
					FileName = "C:\\Windows\\hh.exe",
					Arguments = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "turnedit-help.chm"),
					UseShellExecute = false
					});
				} else {
					MessageBox.Show(this.msgboxStringsMain[10], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				}
			} catch (Exception ex) {
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
			try {
				if (Clipboard.ContainsText(TextDataFormat.Text)) {
				string clipboardText = Clipboard.GetText(TextDataFormat.Text);
				string modifiedText = "\"" + clipboardText + "\"";
				Clipboard.SetText(modifiedText);
				mainTextBox.Paste();
				Clipboard.SetText(clipboardText);
				}
			} catch (Exception ex) {
				MessageBox.Show(this.msgboxStringsMain[12], this.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        private void newWindowNav_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}