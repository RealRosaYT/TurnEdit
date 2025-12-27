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
    public partial class ReplaceWindow
    {
        private MainWindow _mainWindow;
		private string[] ReplaceWindowMsgboxStrings;
        public ReplaceWindow(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
			this.ReplaceWindowMsgboxStrings = new string[4];
			InitializeReplaceWindowi18n();
        }
		private void InitializeReplaceWindowi18n() {
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				whatToReplaceTextBlk.Text = "置換前";
				replaceDestinationTextBlk.Text = "置換後";
				replaceButton.Content = "置換";
				replaceAllButton.Content = "すべて置換";
				cancelReplaceButton.Content = "キャンセル";
				this.Title = "置換";
				this.ReplaceWindowMsgboxStrings[0] = "メインテキストボックスが初期化されていません。";
				this.ReplaceWindowMsgboxStrings[1] = "「txt」が見つかりません。";
				this.ReplaceWindowMsgboxStrings[2] = "エラー";
				this.ReplaceWindowMsgboxStrings[3] = "情報";
			} else if (this._mainWindow.TurnEditLanguage == "en-US") {
				this.ReplaceWindowMsgboxStrings[0] = "Main text box is not initialized.";
				this.ReplaceWindowMsgboxStrings[1] = "txt not found.";
				this.ReplaceWindowMsgboxStrings[2] = "Error";
				this.ReplaceWindowMsgboxStrings[3] = "Information";
			}
		}
        private void replaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._mainWindow is null || this._mainWindow.mainTextBox.Text is null || this._mainWindow.mainTextBox is null)
            {
                MessageBox.Show(this.ReplaceWindowMsgboxStrings[0], this.ReplaceWindowMsgboxStrings[2], MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(this.ReplaceWindowMsgboxStrings[1].Replace("txt", whatToReplace.Text), this.ReplaceWindowMsgboxStrings[3], MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void cancelReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void replaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (this._mainWindow is null || this._mainWindow.mainTextBox.Text is null || this._mainWindow.mainTextBox is null)
            {
                MessageBox.Show("Main text box is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string text = this._mainWindow.mainTextBox.Text;
            int start = text.IndexOf(whatToReplace.Text);
            if (start != -1)
            {
                string replacedText = text.Replace(whatToReplace.Text, replaceDestination.Text);
                text = replacedText;
            } else
            {
                MessageBox.Show($@"{whatToReplace.Text} not found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
