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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TurnEdit
{
    /// <summary>
    /// Tab.xaml の相互作用ロジック
    /// </summary>
    public partial class Tab : UserControl
    {
        public Tab(string fileName)
        {
            InitializeComponent();
            tabLabel.Content = fileName;
        }
        private void closeTab_Click(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)this.Parent;
            TabControl tabControl = (TabControl)item.Parent;
            tabControl.SelectedIndex--;
            tabControl.Items.Remove(item);
        }
    }
}
