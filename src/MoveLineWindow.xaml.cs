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

namespace TurnEdit {
	public partial class MoveLineWindow : Window {
		private MainWindow _mainWindow;
		private string[] MoveLineWindowMsgboxStrings;
		public MoveLineWindow(MainWindow _mainWindow) {
			InitializeComponent();
			this._mainWindow = _mainWindow;
			this.MoveLineWindowMsgboxStrings = new string[2];
			InitializeMoveLineWindowI18n();
		}
		public void InitializeMoveLineWindowI18n() {
			if (this._mainWindow.TurnEditLanguage == "ja-JP") {
				moveLineToText.Text = "移動先:";
				moveLine.Content = "行を移動";
				cancelMoveLine.Content = "キャンセル";
				this.Title = "行を移動";
				this.MoveLineWindowMsgboxStrings[0] = "メインウィンドウが初期化されていません。";
				this.MoveLineWindowMsgboxStrings[1] = "入力した数値は正しくありません。";
			} else if (this._mainWindow.TurnEditLanguage == "en-US") {
				moveLineToText.Text = "To:";
				moveLine.Content = "Move Line";
				cancelMoveLine.Content = "Cancel";
				this.Title = "Move Line";
				this.MoveLineWindowMsgboxStrings[0] = "Main Window is not initialized.";
				this.MoveLineWindowMsgboxStrings[1] = "The integer you entered is invalid.";
			}
		}
		public void moveLine_Click(object sender, RoutedEventArgs e) {
			try {
				if (this._mainWindow == null) {
					MessageBox.Show(this.MoveLineWindowMsgboxStrings[0], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				this._mainWindow.mainTextBox.ScrollToLine(int.Parse(moveLineTo.Text));
				this.Close();
			} catch (FormatException) {
				MessageBox.Show(this.MoveLineWindowMsgboxStrings[1], this._mainWindow.msgboxStringsMain[13], MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			} catch (Exception ex) {
				return;
			}
		}
		public void cancelMoveLine_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}