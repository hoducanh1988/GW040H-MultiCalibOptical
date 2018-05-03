using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiCalibOpticalBoB_Ver1.Function {
    public class BaseFunctions {

        static ConnectInstrument ct = null;

        /// <summary>
        /// LIỆT KÊ TÊN TẤT CẢ CÁC CỔNG SERIAL PORT ĐANG KẾT NỐI VÀO MÁY TÍNH
        /// </summary>
        /// <returns></returns>
        public static List<string> get_Array_Of_SerialPort() {
            try {
                // Get a list of serial port names.
                //string[] ports = SerialPort.GetPortNames();
                List<string> list = new List<string>();
                list.Add("-");
                for (int i = 1; i < 100; i++) {
                    list.Add(string.Format("COM{0}", i));
                }
                //foreach (var item in ports) {
                //    list.Add(item);
                //}
                return list;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// KIỂM TRA SỐ BOSA SERIAL NUMBER NHẬP VÀO CÓ ĐÚNG ĐỊNH DẠNG HAY KHÔNG
        /// </summary>
        /// <param name="_bosaSN"></param>
        /// <returns></returns>
        public static bool bosa_SerialNumber_Is_Correct(string _bosaSN) {
            try {
                //Kiểm tra số lượng kí tự trên tem SN Bosa 
                int lent = GlobalData.initSetting.BOSASNLEN;
                return _bosaSN.Length == lent;

                //Lấy thông tin Bosa từ SQL Server

            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// HIỂN THỊ TÊN CỔNG COMPORT LÊN GIAO DIỆN PHẦN MỀM
        /// </summary>
        public static void display_Port_Name() {
            GlobalData.testingDataDut1.COMPORT = GlobalData.initSetting.USBDEBUG1;
            GlobalData.testingDataDut2.COMPORT = GlobalData.initSetting.USBDEBUG2;
            GlobalData.testingDataDut3.COMPORT = GlobalData.initSetting.USBDEBUG3;
            GlobalData.testingDataDut4.COMPORT = GlobalData.initSetting.USBDEBUG4;
        }

        /// <summary>
        /// LẤY VÀ KHỞI TẠO THÔNG TIN TESTINGINFO THEO TÊN NÚT NHẤN
        /// </summary>
        /// <param name="_btnname"></param>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static bool get_Testing_Info_By_Name(string _btnname, ref testinginfo tf) {
            try {
                tf = new testinginfo();
                switch (_btnname) {
                    case "btnTestingStart1": {
                            tf = GlobalData.testingDataDut1;
                            break;
                        }
                    case "btnTestingStart2": {
                            tf = GlobalData.testingDataDut2;
                            break;
                        }
                    case "btnTestingStart3": {
                            tf = GlobalData.testingDataDut3;
                            break;
                        }
                    case "btnTestingStart4": {
                            tf = GlobalData.testingDataDut4;
                            break;
                        }
                }
                //tf.Initialization();
                tf.ONTINDEX = _btnname.Substring(_btnname.Length - 1, 1);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// CHUYỂN ĐỔI DỮ LIỆU CHUẨN NRZ3 => DOUBLE
        /// </summary>
        /// <param name="_data">-8.780000E+001/-8.780000E-001</param>
        /// <returns></returns>
        public static double convert_NRZ3_To_Double(string _data) {
            try {
                _data = _data.Trim().Replace("\r", "").Replace("\n", "");
                string[] buffer = _data.Split('E');
                double fNum = double.Parse(buffer[0]);
                double lNum = double.Parse(buffer[1].Trim());
                double number = fNum * Math.Pow(10, lNum);
                return (double)Math.Round((decimal)number, 2);
            }
            catch {
                return double.MinValue;
            }
        }


        public static void connect_Instrument() {
            Thread t = new Thread(new ThreadStart(() => {
                App.Current.Dispatcher.Invoke(new Action(() => {
                    ct = new ConnectInstrument();
                    ct.Show();
                }));

                Thread t1 = new Thread(new ThreadStart(() => { })); t1.IsBackground = true;
                Thread t2 = new Thread(new ThreadStart(() => { })); t2.IsBackground = true;
                Thread t3 = new Thread(new ThreadStart(() => { })); t3.IsBackground = true;
                Thread t4 = new Thread(new ThreadStart(() => { })); t4.IsBackground = true;

                //Power Instrument
                if (!t1.IsAlive) {
                    t1 = new Thread(new ThreadStart(() => {
                        if (GlobalData.connectionManagement.IQS1700STATUS == false) {
                            string _message = "";
                            bool ret = false;
                            GlobalData.powerDevice = new Instrument.IQS1700(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                            ret = GlobalData.powerDevice.Open(out _message);
                            if (ret == true) {
                                GlobalData.powerDevice.Initialize();
                                GlobalData.connectionManagement.IQS1700STATUS = true;
                            }
                        }
                    }));
                    t1.Start();
                }

                ////Switch Instrument
                if (!t2.IsAlive) {
                    t2 = new Thread(new ThreadStart(() => {
                        if (GlobalData.connectionManagement.IQS9100BSTATUS == false) {
                            string _message = "";
                            bool ret = false;
                            GlobalData.switchDevice = new Instrument.IQS9100B(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                            ret = GlobalData.switchDevice.Open(out _message);
                            if (ret == true) {
                                GlobalData.switchDevice.Initialize();
                                GlobalData.connectionManagement.IQS9100BSTATUS = true;
                            }
                        }
                    }));
                    t2.Start();
                }

                ////ER Instrument
                if (!t3.IsAlive) {
                    t3 = new Thread(new ThreadStart(() => {
                        if (GlobalData.connectionManagement.DCAX86100DSTATUS == false) {
                            string _message = "";
                            bool ret = false;
                            GlobalData.erDevice = new Instrument.DCAX86100D(GlobalData.initSetting.ERINSTRGPIB);
                            ret = GlobalData.erDevice.Open(out _message);
                            if (ret == true) {
                                GlobalData.erDevice.Initialize();
                                GlobalData.connectionManagement.DCAX86100DSTATUS = true;
                            }
                        }
                    }));
                    t3.Start();
                }

                //SQL Server
                if (!t4.IsAlive) {
                    t4 = new Thread(new ThreadStart(() => {
                        //if (GlobalData.connectionManagement.SQLSTATUS == false) {
                        //    bool ret = false;
                        //    GlobalData.sqlServer = new Protocol.Sql();
                        //    ret = GlobalData.sqlServer.Connection();
                        //    GlobalData.connectionManagement.SQLSTATUS = ret;
                        //}
                    }));
                    t4.Start();
                }

                while(t1.IsAlive==true || t2.IsAlive==true || t3.IsAlive == true || t4.IsAlive == true) {
                    Thread.Sleep(100);
                }

                App.Current.Dispatcher.Invoke(new Action(() => {
                    ct.Close();
                }));
                
            }));
            t.IsBackground = true;
            t.Start();
        }

    }
}
