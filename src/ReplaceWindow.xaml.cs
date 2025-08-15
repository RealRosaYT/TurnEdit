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
    /// ReplaceWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ReplaceWindow : Window
    {
        private MainWindow _mainWindow;
        public ReplaceWindow(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
        }

        private void replaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._mainWindow is null || this._mainWindow.mainTextBox.Text is null || this._mainWindow.mainTextBox is null)
            {
                MessageBox.Show("Main text box is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string text = this._mainWindow.mainTextBox.Text;
            string searchtx = whatToReplace.Text;
            int start = text.IndexOf(searchtx);
            if (start != -1)
            {
                this._mainWindow.mainTextBox.SelectionStart = start;
                this._mainWindow.mainTextBox.SelectionLength = whatToReplace.Text.Length;
                this._mainWindow.mainTextBox.Focus();
                this._mainWindow.mainTextBox.SelectedText = replaceDestination.Text;
            }
            else
            {
                MessageBox.Show($@"{whatToReplace.Text} not found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void cancelReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
