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
using System.Reflection;

namespace TurnEdit {
	using System.IO;
	public partial class SaveWindow : Window {
		public SaveWindow() {
			InitializeComponent();
			inputDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			MoveDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}
		private void MoveDirectory(string path) {
			try {
				directoryView.Items.Clear();
				string[] allFilesAndFolders = Directory.GetFileSystemEntries(path);
				foreach (string entry in allFilesAndFolders) {
					directoryView.Items.Add(entry);
				}
			} catch (DirectoryNotFoundException) {
				MessageBox.Show($"The directory \"{path}\" is not found.\r\nplease check a directory is exist properly.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
			} catch (Exception ex) {
				MessageBox.Show("An unexpected error occurred while moving directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Diagnostics.Debug.WriteLine($"TurnEdit: error: an unexpected error occurred while moving directory: {ex.Message}");
			}
		}
		private void cancelSaving_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}
		private void goDirectory_Click(object sender, RoutedEventArgs e) {
			MoveDirectory(inputDirectory.Text);
		}
	}
}