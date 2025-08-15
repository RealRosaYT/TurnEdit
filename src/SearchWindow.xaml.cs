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

namespace TurnEdit
{
    /// <summary>
    /// SearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window
    {
        private MainWindow _mainWindow;
        public SearchWindow(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
        }

        private void closeSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (this._mainWindow is null || this._mainWindow.mainTextBox.Text is null || this._mainWindow.mainTextBox is null)
            {
                MessageBox.Show("Main text box is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string text = this._mainWindow.mainTextBox.Text;
            string searchtx = searchText.Text;
            int start = text.IndexOf(searchtx);
            if (start != -1)
            {
                this._mainWindow.mainTextBox.SelectionStart = start;
                this._mainWindow.mainTextBox.SelectionLength = searchText.Text.Length;
                this._mainWindow.mainTextBox.Focus();
            } else
            {
                MessageBox.Show($@"{searchText.Text} not found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
