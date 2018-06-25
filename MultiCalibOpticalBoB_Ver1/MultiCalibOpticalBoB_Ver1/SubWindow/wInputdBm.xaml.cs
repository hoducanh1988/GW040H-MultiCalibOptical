using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiCalibOpticalBoB_Ver1.Function;

namespace MultiCalibOpticalBoB_Ver1
{
    /// <summary>
    /// Interaction logic for wInputdBm.xaml
    /// </summary>
    public partial class wInputdBm : Window
    {
        public wInputdBm(string _JigNumber)
        {
            InitializeComponent();
            this.lblTitle.Content = string.Format("JIG SỐ {0}", _JigNumber);
            this.txtPower.Focus();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            double _power = 0;
            if (double.TryParse(txtPower.Text, out _power) == false) {
                MessageBox.Show("Giá trị công suất sai định dạng.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            GlobalData.isPower = true;
            GlobalData.attOntPower = _power;
            this.Close();
        }

    }
}
