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

namespace MultiCalibOpticalBoB_Ver1
{
    /// <summary>
    /// Interaction logic for SelectPort.xaml
    /// </summary>
    public partial class SelectPort : Window
    {
        public string PortSelected = "1";
        List<string> listPort = new List<string>() { "1", "2", "3", "4" };
        public SelectPort()
        {
            InitializeComponent();
            this.cbbPort.ItemsSource = this.listPort;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void cbbPort_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PortSelected = this.cbbPort.SelectedItem.ToString();
            //MessageBox.Show("OK");
        }
    }
}
