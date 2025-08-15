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
        public MainWindow()
        {
            InitializeComponent();
            this.IsFileOpened = false;
            this.currentFileName = null;
            mainTextBox.Width = this.Width;
            mainTextBox.Height = this.Height - mainMenu.Height;
            this.CreateFileFileNotExists = null;
            this.SizeChanged += new SizeChangedEventHandler(sizeChangedEvent);
            this.ChangesUnsaved = false;
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
            this.Title = "New File - TurnEdit";
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
                    string fileText = File.ReadAllText(ofd.FileName);
                    mainTextBox.Text = fileText;
                    this.currentFileName = ofd.FileName;
                    this.IsFileOpened = true;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("File opening failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void sizeChangedEvent(object sender, SizeChangedEventArgs e)
        {
            mainTextBox.Width = this.ActualWidth;
            mainTextBox.Height = this.ActualHeight - mainMenu.Height;
        }

        private void aboutTurnEditNav_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
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
                    File.WriteAllText(currentFileName, mainTextBox.Text);
                    this.ChangesUnsaved = false;
                }
                else
                {
                    MessageBox.Show("Please open file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    File.WriteAllText(sfd.FileName, mainTextBox.Text);
                    this.IsFileOpened = true;
                    this.currentFileName = sfd.FileName;
                    this.Title = $@"{this.currentFileName} - TurnEdit";
                    this.ChangesUnsaved = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            lineStatus.Text = $@"Line: {lineIndex + 1}";
            columnStatus.Text = $@"Column: {columnIndex + 1}";
        }

        private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ChangesUnsaved = true;
            totalTextCount.Text = "Total text count(includes returning code): " + mainTextBox.Text.Length;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.ChangesUnsaved == true)
            {
                MessageBoxResult msgbox = MessageBox.Show("Changes are unsaved. do you want save a changes?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
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
        }
		private void helpNav_Click(object sender, RoutedEventArgs e) {
			try {
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{
					FileName = "hh.exe",
					Arguments = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "turnedit-help.chm")
				});
			} catch (Exception ex) {
				MessageBox.Show("Failed to open help: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);	
			}
		}
    }
}