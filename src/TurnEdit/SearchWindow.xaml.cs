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
    public partial class SearchWindow
    {
        private MainWindow _mainWindow;
		private string[] SearchWindowMsgboxStrings;
        public SearchWindow(MainWindow _mainWindow)
        {
            InitializeComponent();
            this._mainWindow = _mainWindow;
			this.SearchWindowMsgboxStrings = new string[7];
			InitializeSearchWindowi18n();
        }
		private void InitializeSearchWindowi18n() {
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				this.Title = "検索";
				searchTextblk.Text = "検索する文字";
				searchButton.Content = "検索";
				closeSearch.Content = "キャンセル";
				SearchWindowMsgboxStrings[0] = "検索する文字を入力してください。";
				SearchWindowMsgboxStrings[1] = "メインテキストボックスが初期化されていません。";
				SearchWindowMsgboxStrings[2] = "「txt」が見つかりません。";
				SearchWindowMsgboxStrings[3] = "予期しないエラーにより文字の検索が失敗しました。";
				SearchWindowMsgboxStrings[4] = "エラー";
				SearchWindowMsgboxStrings[5] = "警告";
				SearchWindowMsgboxStrings[6] = "情報";
			} else if (this._mainWindow.TurnEditLanguage == "en-US") {
				SearchWindowMsgboxStrings[0] = "Please input search text.";
				SearchWindowMsgboxStrings[1] = "Main text box is not initialized.";
				SearchWindowMsgboxStrings[2] = "txt not found.";
				SearchWindowMsgboxStrings[3] = "Text search failed because unexpected error.";
				SearchWindowMsgboxStrings[4] = "Error";
				SearchWindowMsgboxStrings[5] = "Warning";
				SearchWindowMsgboxStrings[6] = "Information";
			}
		}
        private void closeSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
           try {
			  if (string.IsNullOrEmpty(searchText.Text)) {
				  MessageBox.Show(this.SearchWindowMsgboxStrings[0], this.SearchWindowMsgboxStrings[5], MessageBoxButton.OK, MessageBoxImage.Warning);
			  }
            if (this._mainWindow is null || this._mainWindow.mainTextBox.Text is null || this._mainWindow.mainTextBox is null)
            {
                MessageBox.Show(this.SearchWindowMsgboxStrings[1], this.SearchWindowMsgboxStrings[4], MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(this.SearchWindowMsgboxStrings[2].Replace("txt", searchtx), this.SearchWindowMsgboxStrings[6], MessageBoxButton.OK, MessageBoxImage.Information);
            }
		   }
		   catch (ArgumentNullException) {
			 MessageBox.Show(this.SearchWindowMsgboxStrings[3], this.SearchWindowMsgboxStrings[4], MessageBoxButton.OK, MessageBoxImage.Error);
		   }
		   catch (Exception) {
			   MessageBox.Show(this.SearchWindowMsgboxStrings[3], this.SearchWindowMsgboxStrings[4], MessageBoxButton.OK, MessageBoxImage.Error);
		   }
        }
    }
}
