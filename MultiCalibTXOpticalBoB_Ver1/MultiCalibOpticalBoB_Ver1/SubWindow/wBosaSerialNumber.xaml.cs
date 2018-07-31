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
using MultiCalibOpticalBoB_Ver1.Function;

namespace MultiCalibOpticalBoB_Ver1 {
    /// <summary>
    /// Interaction logic for wBosaSerialNumber.xaml
    /// </summary>
    public partial class wBosaSerialNumber : Window {

        string dutNumber = "";
        public wBosaSerialNumber(string _dutnumber) {
            InitializeComponent();
            this.dutNumber = string.Format("0{0}", _dutnumber);
            lblTitle.Content = string.Format("NHẬP THÔNG TIN CỦA DUT #0{0}", _dutnumber);
            if (!GlobalData.initSetting.ENABLEWRITEMAC) this.stMacAddress.Visibility = Visibility.Collapsed;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.txtBosaNumber.Focus();
        }

        private void txtBosaNumber_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                string _text = txtBosaNumber.Text.Trim();
                //check thong tin bosa
                if (!BaseFunctions.bosa_SerialNumber_Is_Correct(_text)) {
                    tbMessage.Text = string.Format("Bosa Serial Number: \"{0}\" không hợp lệ.\nVui lòng nhập lại.", _text);
                    txtBosaNumber.Clear();
                    txtBosaNumber.Focus();
                    return;
                }
                //get thong tin bosa
                switch (this.dutNumber) {
                    case "01": { GlobalData.testingDataDut1.BOSASERIAL = _text; break; }
                    case "02": { GlobalData.testingDataDut2.BOSASERIAL = _text; break; }
                    case "03": { GlobalData.testingDataDut3.BOSASERIAL = _text; break; }
                    case "04": { GlobalData.testingDataDut4.BOSASERIAL = _text; break; }
                    default: break;
                }

                //close form
                if (!GlobalData.initSetting.ENABLEWRITEMAC) this.Close();
                else this.txtMAC.Focus();
            }
        }

        private void txtBosaNumber_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtBosaNumber.Text.Trim().Length > 0) tbMessage.Text = "";
        }

        private void txtMAC_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                string _text = txtMAC.Text.Trim().Replace(":","");
                //check thong tin bosa
                if (!BaseFunctions.mac_Address_Is_Correct(_text)) {
                    tbMessage.Text = string.Format("Mac Address: \"{0}\" không hợp lệ.\nVui lòng nhập lại.", _text);
                    txtMAC.Clear();
                    txtMAC.Focus();
                    return;
                }
                //get thong tin bosa
                switch (this.dutNumber) {
                    case "01": {
                            GlobalData.testingDataDut1.MACADDRESS = _text;
                            GlobalData.testingDataDut1.GPON = BaseFunctions.GEN_SERIAL_ONT(_text);
                            break;
                        }
                    case "02": {
                            GlobalData.testingDataDut2.MACADDRESS = _text;
                            GlobalData.testingDataDut2.GPON = BaseFunctions.GEN_SERIAL_ONT(_text);
                            break;
                        }
                    case "03": {
                            GlobalData.testingDataDut3.MACADDRESS = _text;
                            GlobalData.testingDataDut3.GPON = BaseFunctions.GEN_SERIAL_ONT(_text);
                            break;
                        }
                    case "04": {
                            GlobalData.testingDataDut4.MACADDRESS = _text;
                            GlobalData.testingDataDut4.GPON = BaseFunctions.GEN_SERIAL_ONT(_text);
                            break;
                        }
                    default: break;
                }

                //close form
                this.Close();
            }
        }

        private void txtMAC_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtMAC.Text.Trim().Length > 0) tbMessage.Text = "";
        }
    }
}
