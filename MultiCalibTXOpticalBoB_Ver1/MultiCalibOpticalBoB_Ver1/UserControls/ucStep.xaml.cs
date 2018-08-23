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
using MultiCalibOpticalBoB_Ver1.Function;
using MultiCalibOpticalBoB_Ver1.Function.Instrument;
using System.IO.Ports;
using MultiCalibOpticalBoB_Ver1.Function.Ont;
using MultiCalibOpticalBoB_Ver1.Function.IO;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucStep.xaml
    /// </summary>
    public partial class ucStep : UserControl {

        public ucStep() {
            InitializeComponent();
            this.border_IQS610P_Param.DataContext = GlobalData.initSetting;
            this.border_DCAX86100D_Param.DataContext = GlobalData.initSetting;
            this.border_ONT_Param.DataContext = GlobalData.initSetting;
            this.border_IQS610P_Log.DataContext = GlobalData.manualTest;
            this.border_DCAX86100D_Log.DataContext = GlobalData.manualTest;
            this.border_ONT_Log.DataContext = GlobalData.manualTest;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            GlobalData.manualTest.IQS610PLOG = "";
            GlobalData.manualTest.DCAX86100DLOG = "";
            GlobalData.manualTest.ONTLOG = "";

            switch (b.Name) {

                case "IQSconn": {
                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";

                            //CONNECT TO IQS1700
                            GlobalData.manualTest.IQS610PLOG += "Connecting to IQS1700...\n";
                            GlobalData.powerDevice = new IQS1700(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                            message = "";
                             ret = GlobalData.powerDevice.Open(out message);
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                            //CONNECT TO IQS9100B
                            GlobalData.manualTest.IQS610PLOG += "Connecting to IQS9100B...\n";
                            GlobalData.switchDevice = new IQS9100B(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                            message = "";
                            ret = GlobalData.switchDevice.Open(out message);
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "IQSinit": {
                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            //KHỞI TẠO MODULE IQS1700
                            GlobalData.manualTest.IQS610PLOG += "Initializing module IQS1700...\n";
                            message = "";
                            try {
                                ret = GlobalData.powerDevice.Initialize();
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");

                            //KHỞI TẠO MODULE IQS9100B
                            GlobalData.manualTest.IQS610PLOG += "Initializing module IQS9100B...\n";
                            message = "";
                            try {
                                ret = GlobalData.switchDevice.Initialize();
                            } catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "IQSgetdBm": {
                        Thread t = new Thread(new ThreadStart(() => {
                            GlobalData.manualTest.IQS610PLOG += string.Format("Read power dBm of module IQS1700 Rack {0}...\n", GlobalData.initSetting.PWINSTRPORT);
                            string message = "";
                            try {
                                double value1=0, value2=0, value3=0, value4=0;
                                Thread t1 = new Thread(new ThreadStart(() => {
                                    value1 = double.Parse(GlobalData.powerDevice.getPower_dBm_NoAtt(1));
                                }));
                                t1.IsBackground = true;
                                t1.Start();
                                Thread t2 = new Thread(new ThreadStart(() => {
                                    value2 = double.Parse(GlobalData.powerDevice.getPower_dBm_NoAtt(2));
                                }));
                                t2.IsBackground = true;
                                t2.Start();
                                Thread t3 = new Thread(new ThreadStart(() => {
                                    value3 = double.Parse(GlobalData.powerDevice.getPower_dBm_NoAtt(3));
                                }));
                                t3.IsBackground = true;
                                t3.Start();
                                Thread t4 = new Thread(new ThreadStart(() => {
                                    value4 = double.Parse(GlobalData.powerDevice.getPower_dBm_NoAtt(4));
                                }));
                                t4.IsBackground = true;
                                t4.Start();
                                while (t1.IsAlive == true || t2.IsAlive == true || t3.IsAlive == true || t4.IsAlive == true) { Thread.Sleep(100); }
                                message += "Channel1: " + value1.ToString() + " dBm\n";
                                message += "Channel2: " + value2.ToString() + " dBm\n";
                                message += "Channel3: " + value3.ToString() + " dBm\n";
                                message += "Channel4: " + value4.ToString() + " dBm\n";
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", "PASS");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "IQSgetmW": {
                        //Thread t = new Thread(new ThreadStart(() => {
                        //    GlobalData.manualTest.IQS610PLOG += string.Format("Read power Watt of module IQS1700 at Port...\n");
                        //    string message = "";
                        //    try {
                        //        double value1 = 0, value2 = 0, value3 = 0, value4 = 0;
                        //        Thread t1 = new Thread(new ThreadStart(() => {
                        //            value1 = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(1));
                        //        }));
                        //        t1.IsBackground = true;
                        //        t1.Start();
                        //        Thread t2 = new Thread(new ThreadStart(() => {
                        //            value2 = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(2));
                        //        }));
                        //        t2.IsBackground = true;
                        //        t2.Start();
                        //        Thread t3 = new Thread(new ThreadStart(() => {
                        //            value3 = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(3));
                        //        }));
                        //        t3.IsBackground = true;
                        //        t3.Start();
                        //        Thread t4 = new Thread(new ThreadStart(() => {
                        //            value4 = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(4));
                        //        }));
                        //        t4.IsBackground = true;
                        //        t4.Start();
                        //        while (t1.IsAlive == true || t2.IsAlive == true || t3.IsAlive == true || t4.IsAlive == true) { Thread.Sleep(100); }
                        //        message += "Channel1: " + value1.ToString() + " uW\n";
                        //        message += "Channel2: " + value2.ToString() + " uW\n";
                        //        message += "Channel3: " + value3.ToString() + " uW\n";
                        //        message += "Channel4: " + value4.ToString() + " uW\n";
                        //    }
                        //    catch (Exception ex) {
                        //        message = ex.ToString();
                        //    }
                        //    GlobalData.manualTest.IQS610PLOG += message + "\n";
                        //    GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", "PASS");
                        //}));
                        //t.IsBackground = true;
                        //t.Start();
                        break;
                    }

                case "IQSSwitch": {
                        SelectPort sp = new SelectPort();
                        sp.ShowDialog();
                        int _port = 0;
                        if (sp.PortSelected.Trim() == "") return;
                        else _port = int.Parse(sp.PortSelected);
                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            GlobalData.manualTest.IQS610PLOG += string.Format("Switch module IQS9100B to Port{0}...\n", _port);
                            message = "";
                            try {
                                ret = GlobalData.switchDevice.switchToPort(_port);
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.IQS610PLOG += message + "\n";
                            GlobalData.manualTest.IQS610PLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "DCAcalib": {
                        //Ngắt kết nối Switch quang
                        GlobalData.manualTest.DCAX86100DLOG += string.Format("Switch off cổng kết nối quang...\n");
                        bool kq = GlobalData.switchDevice.switchOff();
                        GlobalData.manualTest.DCAX86100DLOG += string.Format("=> Result: {0}\n", kq == true ? "PASS" : "FAIL");
                        if (kq == false) return;

                        //Calib máy đo ER
                        CalibratingWindow cw = new CalibratingWindow();
                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            App.Current.Dispatcher.Invoke(new Action(() => { cw.Show(); }));
                            
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("Calibrating DCAX86100D...\n");
                            message = "";
                            try {
                                ret = GlobalData.erDevice.ManualCalibrate();
                                CalibrationModuleTime.Write();
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }

                            App.Current.Dispatcher.Invoke(new Action(() => { cw.Close(); }));
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                            if (ret == true) {
                                App.Current.Dispatcher.Invoke(new Action(() => {
                                    MessageBox.Show("Tool sẽ tự động đóng.\nVui lòng mở lại tool để tiếp tục calib sản phẩm.", "Đóng tool", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Environment.Exit(0);
                                }));
                            }
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "DCAgetdBm": {
                        SelectPort sp = new SelectPort();
                        sp.ShowDialog();
                        int _port = 0;
                        if (sp.PortSelected.Trim() == "") return;
                        else _port = int.Parse(sp.PortSelected);
                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            //Switch Port IQS9100B
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("Switch module IQS9100B to Port{0}...\n", _port);
                            message = "";
                            try {
                                ret = GlobalData.switchDevice.switchToPort(_port);
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.DCAX86100DLOG += message + "\n";
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                            if (!ret) return;

                            //Get Power 
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("Getting Power value (dBm) from DCAX86100D...\n");
                            message = "";
                            try {
                                message = double.Parse(GlobalData.erDevice.getdBm().Replace("\r","").Replace("\n","")).ToString();
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.DCAX86100DLOG += message + " dBm\n";
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "DCAgetER": {
                        SelectPort sp = new SelectPort();
                        sp.ShowDialog();
                        int _port = 0;
                        if (sp.PortSelected.Trim() == "") return;
                        else _port = int.Parse(sp.PortSelected);

                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("Getting ER value from DCAX86100D Port{0}...\n", _port);
                            message = "";
                            try {
                                message = GlobalData.erDevice.getER(_port);
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                            }
                            GlobalData.manualTest.DCAX86100DLOG += message + "\n";
                            GlobalData.manualTest.DCAX86100DLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "ONTlogin": {
                        //SelectPort sp = new SelectPort();
                        //sp.ShowDialog();
                        //int _port = 1;
                        //if (sp.PortSelected.Trim() == "") return;
                        //else _port = int.Parse(sp.PortSelected);
                        //string _comPort = "";
                        //switch (_port) {
                        //    case 1: { _comPort = GlobalData.initSetting.USBDEBUG1; break; }
                        //    case 2: { _comPort = GlobalData.initSetting.USBDEBUG2; break; }
                        //    case 3: { _comPort = GlobalData.initSetting.USBDEBUG3; break; }
                        //    case 4: { _comPort = GlobalData.initSetting.USBDEBUG4; break; }
                        //}
                        
                        //Thread t = new Thread(new ThreadStart(() => {
                        //    bool ret = false;
                        //    string message = "";
                        //    GlobalData.manualTest.ONTLOG += string.Format("Login to ONT {0}...\n", _comPort);
                        //    GW ont = null;
                        //    try {
                        //        switch (GlobalData.initSetting.ONTTYPE) {
                        //            case "GW040H": { ont = new GW040H(_comPort);break; }
                        //            case "GW020BoB": { ont = new GW020BoB(_comPort); break; }
                        //            default: break;
                        //        }
                        //        if (!ont.Open(out message)) ret = false;
                        //        else ret = ont.Login(out message);
                        //    }
                        //    catch (Exception ex) {
                        //        message = ex.ToString();
                        //    }
                        //    GlobalData.manualTest.ONTLOG += message + "\n";
                        //    GlobalData.manualTest.ONTLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                        //    try { ont.Close(); } catch { }
                        //}));
                        //t.IsBackground = true;
                        //t.Start();
                        break;
                    }

                case "ONTtx": {
                        SelectPort sp = new SelectPort();
                        sp.ShowDialog();
                        int _port = 0;
                        if (sp.PortSelected.Trim() == "") return;
                        else _port = int.Parse(sp.PortSelected);
                        string _comPort = "";
                        switch (_port) {
                            case 1: { _comPort = GlobalData.initSetting.USBDEBUG1; break; }
                            case 2: { _comPort = GlobalData.initSetting.USBDEBUG2; break; }
                            case 3: { _comPort = GlobalData.initSetting.USBDEBUG3; break; }
                            case 4: { _comPort = GlobalData.initSetting.USBDEBUG4; break; }
                        }

                        Thread t = new Thread(new ThreadStart(() => {
                            bool ret = false;
                            string message = "";
                            GW ont = null;
                            try {
                                GlobalData.manualTest.ONTLOG += string.Format("Login to ONT {0}...\n", _comPort);
                                switch (GlobalData.initSetting.ONTTYPE) {
                                    case "GW040H": { ont = new GW040H(_comPort); break; }
                                    case "GW020BoB": { ont = new GW020BoB(_comPort); break; }
                                    default: break;
                                }
                                if (!ont.Open(out message)) ret = false;
                                else ret = ont.Login(out message);
                                GlobalData.manualTest.ONTLOG += message + "\n";

                                GlobalData.manualTest.ONTLOG += string.Format("Send command to request ONT output optical Power {0}...\n", _comPort);
                                ont.outTXPower();
                                Thread.Sleep(1000);
                                message = ont.Read();
                                GlobalData.manualTest.ONTLOG += message + "\n";
                            }
                            catch (Exception ex) {
                                message = ex.ToString();
                                GlobalData.manualTest.ONTLOG += message + "\n";
                            }
                            GlobalData.manualTest.ONTLOG += string.Format("=> Result: {0}\n", ret == true ? "PASS" : "FAIL");
                            try { ont.Close(); } catch { }
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                default: break;
            }
        }

    }
}
