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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MultiCalibOpticalBoB_Ver1.Function;
using MultiCalibOpticalBoB_Ver1.Function.Ont;
using MultiCalibOpticalBoB_Ver1.Function.Protocol;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {
        public ucSetting() {
            InitializeComponent();
            this.DataContext = GlobalData.initSetting;
            cbbSettingOntType.ItemsSource = Parameters.ListOntType;
            cbbAPD.ItemsSource = Parameters.ListWriteAPDOption;
            cbbSettingPwPort.ItemsSource = Parameters.ListIQSPort;
            cbbSettingSwPort.ItemsSource = Parameters.ListIQSPort;
            cbbSettingUsb1.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb2.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb3.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb4.ItemsSource = Parameters.ListUsbComport;
        }

        private bool outputONTTXOpticalPower(string _index, Button b) {
            try {
                b.Background = Brushes.Lime;
                string _comPort = "";
                switch (_index) {
                    case "1": { _comPort = GlobalData.initSetting.USBDEBUG1; break; }
                    case "2": { _comPort = GlobalData.initSetting.USBDEBUG2; break; }
                    case "3": { _comPort = GlobalData.initSetting.USBDEBUG3; break; }
                    case "4": { _comPort = GlobalData.initSetting.USBDEBUG4; break; }
                }

                Thread t = new Thread(new ThreadStart(() => {
                    bool ret = false;
                    string message = "";
                    GW ont = null;
                    try {
                        switch (GlobalData.initSetting.ONTTYPE) {
                            case "GW040H": { ont = new GW040H(_comPort); break; }
                            case "GW020BoB": { ont = new GW020BoB(_comPort); break; }
                            default: break;
                        }
                        if (!ont.Open(out message)) ret = false;
                        else ret = ont.Login(out message);

                        ont.outTXPower();
                        Thread.Sleep(1000);
                        message = ont.Read();
                    }
                    catch (Exception ex) {
                        message = ex.ToString();
                    }
                    MessageBox.Show(string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL"), "OUTPUT TX POWER", MessageBoxButton.OK, ret == true ? MessageBoxImage.Information : MessageBoxImage.Error);
                    try { ont.Close(); } catch { }
                    App.Current.Dispatcher.Invoke(new Action(() => {
                        b.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#777777");
                    }));
                }));
                t.IsBackground = true;
                t.Start();
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool setAttenuator(string _index, Button b) {
            try {
                b.Background = Brushes.Lime;
                GlobalData.isPower = false;
                GlobalData.attOntPower = 0;
                wInputdBm wi = new wInputdBm(_index);
                wi.ShowDialog();

                if (GlobalData.isPower == false) {
                    MessageBox.Show(string.Format("Phần mềm ko thể tính được suy hao quang của JIG {0}.\nVì giá trị công suất ONT nhập vào ko đúng.\n----------------------\nVui lòng kiểm tra lại.", _index), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    b.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#777777");
                    return false;
                }

                Thread t = new Thread(new ThreadStart(() => {
                    double _PWValue = 0, _ERValue = 0;
                    double _attPW = 0, _attER = 0;

                    //Kiem tra ket noi toi may do Power
                    if (Network.PingNetwork(GlobalData.initSetting.EXFOIP) == false) {
                        MessageBox.Show(string.Format("Không thể kết nối tới máy đo Power {0}.", GlobalData.initSetting.EXFOIP), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        goto END;
                    }
                    //Kiem tra ket noi toi may do ER
                    if (GlobalData.erDevice.isConnected() == false) {
                        MessageBox.Show(string.Format("Không thể kết nối tới máy đo ER {0}.", GlobalData.initSetting.ERINSTRGPIB), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        goto END;
                    }

                    //Doc cong suat may do Power
                    _PWValue = Math.Round(double.Parse(GlobalData.powerDevice.getPower_dBm_NoAtt(int.Parse(_index))), 2);
                    _attPW = GlobalData.attOntPower - _PWValue;

                    //Chuyen mach sang may do ER
                    GlobalData.switchDevice.switchToPort(int.Parse(_index));

                    //Doc cong suat may do ER
                    _ERValue = BaseFunctions.convert_NRZ3_To_Double(GlobalData.erDevice.getdBm());
                    _attER = GlobalData.attOntPower - _ERValue;

                    //Thiet lap gia tri suy hao cho may do Power, ER
                    switch (_index) {
                        case "1": {
                                GlobalData.initSetting.POWERCABLEATTENUATION1 = _attPW.ToString();
                                GlobalData.initSetting.ERCABLEATTENUATION1 = _attER.ToString();
                                break;
                            }
                        case "2": {
                                GlobalData.initSetting.POWERCABLEATTENUATION2 = _attPW.ToString();
                                GlobalData.initSetting.ERCABLEATTENUATION2 = _attER.ToString();
                                break;
                            }
                        case "3": {
                                GlobalData.initSetting.POWERCABLEATTENUATION3 = _attPW.ToString();
                                GlobalData.initSetting.ERCABLEATTENUATION3 = _attER.ToString();
                                break;
                            }
                        case "4": {
                                GlobalData.initSetting.POWERCABLEATTENUATION4 = _attPW.ToString();
                                GlobalData.initSetting.ERCABLEATTENUATION4 = _attER.ToString();
                                break;
                            }
                    }

                    //Thong bao ket qua
                    MessageBox.Show(string.Format("Thiết lập suy hao cho JIG {0} thành công." +
                                                  "\n---------------------------------------" +
                                                  "\n - Suy hao Power = {1} dBm" +
                                                  "\n - Suy hao ER = {2} dBm",
                                                  _index,
                                                  _PWValue,
                                                  _ERValue),
                                                  "THIẾT LẬP SUY HAO",
                                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    END:
                    App.Current.Dispatcher.Invoke(new Action(() => {
                        b.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#777777");
                    }));
                }));
                t.IsBackground = true;
                t.Start();
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        private void _onOffTestCase(params bool[] flags) {
            GlobalData.initSetting.ENABLEWRITEAPD = flags[0];
            GlobalData.initSetting.ENABLETUNINGPOWER = flags[1];
            GlobalData.initSetting.ENABLETUNINGER = flags[2];
            GlobalData.initSetting.ENABLETUNINGCROSSING = flags[3];
            GlobalData.initSetting.ENABLETXDDMI = flags[4];
            GlobalData.initSetting.ENABLESIGNALOFF = flags[5];
            GlobalData.initSetting.ENABLEWRITEFLASH = flags[6];
        }

        /// <summary>
        /// 
        /// </summary>
        private void _setTestCaseFollowOntTypeAndApdLUT() {
            switch (GlobalData.initSetting.ONTTYPE) {
                case "GW020BoB": {
                        switch (GlobalData.initSetting.ONTAPD) {
                            case "Only Write APD LUT": {
                                    _onOffTestCase(true, false, false, false, false, false, false);
                                    break;
                                }
                            case "Write APD LUT": {
                                    _onOffTestCase(true, true, true, true, true, false, false);
                                    break;
                                }
                            case "Don't Write APD LUT": {
                                    _onOffTestCase(false, true, true, true, true, false, false);
                                    break;
                                }
                        }
                        break;
                    }
                case "GW040H": {
                        _onOffTestCase(false, true, true, false, true, true, true);
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "SAVE SETTING": {
                        BaseFunctions.display_Port_Name();
                        if (GlobalData.listBosaInfo.Count == 0) BaseFunctions.loadBosaReport();
                        this._setTestCaseFollowOntTypeAndApdLUT();

                        GlobalData.initSetting.Save();
                        MessageBox.Show("Success.", "SAVE SETTING", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (GlobalData.connectionManagement.IQS1700STATUS == false || GlobalData.connectionManagement.IQS9100BSTATUS == false || GlobalData.connectionManagement.DCAX86100DSTATUS == false || GlobalData.connectionManagement.SQLSTATUS == false)
                            BaseFunctions.connect_Instrument();
                        break;
                    }
                case "Browser...": {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Bosa report files |*.xls; *.xlsx; *.xlsm";
                        if (openFileDialog.ShowDialog() == true)
                            GlobalData.initSetting.BOSAREPORT = openFileDialog.FileName;
                        break;
                    }
                case "Hỗ trợ cài đặt": {
                        this.Opacity = 0.3;
                        wUsbDebugger w = new wUsbDebugger();
                        w.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
                case "Import to SQL": {
                        this.Opacity = 0.3;
                        ImportToSQL imp = new ImportToSQL();
                        imp.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }

                default: break;
            }

            switch (b.Name) {
                //////////////////////////////////////////////////////////
                case "btnOutTX1": {
                        this.outputONTTXOpticalPower("1", b);
                        break;
                    }
                case "btnOutTX2": {
                        this.outputONTTXOpticalPower("2", b);
                        break;
                    }
                case "btnOutTX3": {
                        this.outputONTTXOpticalPower("3", b);
                        break;
                    }
                case "btnOutTX4": {
                        this.outputONTTXOpticalPower("4", b);
                        break;
                    }
                //////////////////////////////////////////////////////////
                case "btnCalAtt1": {
                        this.setAttenuator("1", b);
                        break;
                    }
                case "btnCalAtt2": {
                        this.setAttenuator("2", b);
                        break;
                    }
                case "btnCalAtt3": {
                        this.setAttenuator("3", b);
                        break;
                    }
                case "btnCalAtt4": {
                        this.setAttenuator("4", b);
                        break;
                    }
                //////////////////////////////////////////////////////////
            }
        }

        private void cbbSettingOntType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (cbbSettingOntType.SelectedItem.ToString() == "GW040H")
                this.cbbAPD.IsEnabled = false;
            else
                this.cbbAPD.IsEnabled = true;
        }
    }
}
