using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiCalibOpticalBoB_Ver1.Function
{
    public class BaseFunctions
    {
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
                for(int i = 1; i < 100; i++) {
                    list.Add(string.Format("COM{0}", i));
                }
                //foreach (var item in ports) {
                //    list.Add(item);
                //}
                return list;
            } catch {
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

            } catch {
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
                 tf.ONTINDEX =  _btnname.Substring(_btnname.Length - 1, 1);
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
            } catch {
                return double.MinValue;
            }
        }


        public static bool connect_Instrument() {
            string message = "";
            bool result = true;
            //Power Instrument
            Thread t1 = new Thread(new ThreadStart(() => {
                if (GlobalData.powerDevice == null) {
                    GlobalData.powerDevice = new Instrument.IQS1700(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                    GlobalData.connectionManagement.IQS1700STATUS = GlobalData.powerDevice.Open(out message);
                    if (GlobalData.connectionManagement.IQS1700STATUS == false) result = false;
                    else GlobalData.powerDevice.Initialize();
                }
            }));
            t1.IsBackground = true;
            t1.Start();

            //Switch Instrument
            Thread t2 = new Thread(new ThreadStart(() => {
                if (GlobalData.switchDevice == null) {
                    GlobalData.switchDevice = new Instrument.IQS9100B(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                    GlobalData.connectionManagement.IQS9100BSTATUS = GlobalData.switchDevice.Open(out message);
                    if (GlobalData.connectionManagement.IQS9100BSTATUS == false) result = false;
                    else GlobalData.switchDevice.Initialize();
                }
            }));
            t2.IsBackground = true;
            t2.Start();

            //ER Instrument
            Thread t3 = new Thread(new ThreadStart(() => {
                if (GlobalData.erDevice == null) {
                    GlobalData.erDevice = new Instrument.DCAX86100D(GlobalData.initSetting.ERINSTRGPIB);
                    GlobalData.connectionManagement.DCAX86100DSTATUS = GlobalData.erDevice.Open(out message);
                    if (GlobalData.connectionManagement.DCAX86100DSTATUS == false) result = false;
                    else {
                        GlobalData.erDevice.Initialize();
                        //GlobalData.erDevice.Calibrate();
                    }
                }
            }));
            t3.IsBackground = true;
            t3.Start();

            //SQL Server
            Thread t4 = new Thread(new ThreadStart(() => {
                if (GlobalData.sqlServer == null) {
                    GlobalData.sqlServer = new Protocol.Sql();
                    GlobalData.connectionManagement.SQLSTATUS = GlobalData.sqlServer.Connection();
                    if (GlobalData.connectionManagement.SQLSTATUS == false) result = false;
                }
            }));
            t4.IsBackground = true;
            t4.Start();
           
            return result;
        }

        /// <summary>
        /// KẾT NỐI LẠI THIẾT BỊ SAU KHI THAY ĐỔI CÀI ĐẶT
        /// </summary>
        /// <returns></returns>
        public static bool reconnect_Instrument() {
            string message = "";
            bool result = true;
            //Power Instrument
            Thread t1 = new Thread(new ThreadStart(() => {
                GlobalData.powerDevice.Close();
                GlobalData.powerDevice = new Instrument.IQS1700(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                GlobalData.connectionManagement.IQS1700STATUS = GlobalData.powerDevice.Open(out message);
                if (GlobalData.connectionManagement.IQS1700STATUS == false) result = false;
                else GlobalData.powerDevice.Initialize();
            }));
            t1.IsBackground = true;
            t1.Start();

            //Switch Instrument
            Thread t2 = new Thread(new ThreadStart(() => {
                GlobalData.switchDevice.Close();
                GlobalData.switchDevice = new Instrument.IQS9100B(GlobalData.initSetting.EXFOIP, GlobalData.initSetting.EXFOPORT);
                GlobalData.connectionManagement.IQS9100BSTATUS = GlobalData.switchDevice.Open(out message);
                if (GlobalData.connectionManagement.IQS9100BSTATUS == false) result = false;
                else GlobalData.switchDevice.Initialize();
            }));
            t2.IsBackground = true;
            t2.Start();

            //ER Instrument
            Thread t3 = new Thread(new ThreadStart(() => {
                GlobalData.switchDevice.Close();
                GlobalData.erDevice = new Instrument.DCAX86100D(GlobalData.initSetting.ERINSTRGPIB);
                GlobalData.connectionManagement.DCAX86100DSTATUS = GlobalData.erDevice.Open(out message);
                if (GlobalData.connectionManagement.DCAX86100DSTATUS == false) result = false;
                else {
                    GlobalData.erDevice.Initialize();
                    //GlobalData.erDevice.Calibrate();
                }
            }));
            t3.IsBackground = true;
            t3.Start();

            //SQL Server
            Thread t4 = new Thread(new ThreadStart(() => {
                GlobalData.sqlServer.Close();
                GlobalData.sqlServer = new Protocol.Sql();
                GlobalData.connectionManagement.SQLSTATUS = GlobalData.sqlServer.Connection();
                if (GlobalData.connectionManagement.SQLSTATUS == false) result = false;
            }));
            t4.IsBackground = true;
            t4.Start();
            
            return result;
        }

    }
}
