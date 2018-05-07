using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MultiCalibOpticalBoB_Ver1.Function {
    public class BaseFunctions {

        static ConnectInstrument ct = null;

        private static string BinToHex(string bin) {
            string output = "";
            try {
                int rest = bin.Length % 4;
                bin = bin.PadLeft(rest, '0'); //pad the length out to by divideable by 4

                for (int i = 0; i <= bin.Length - 4; i += 4) {
                    output += string.Format("{0:X}", Convert.ToByte(bin.Substring(i, 4), 2));
                }

                return output;
            }
            catch {
                return "ERROR";
            }
        }

        private static Byte[] HexToBin(string pHexString) {
            if (String.IsNullOrEmpty(pHexString))
                return new Byte[0];

            if (pHexString.Length % 2 != 0)
                throw new Exception("Hexstring must have an even length");

            Byte[] bin = new Byte[pHexString.Length / 2];
            int o = 0;
            int i = 0;
            for (; i < pHexString.Length; i += 2, o++) {
                switch (pHexString[i]) {
                    case '0': bin[o] = 0x00; break;
                    case '1': bin[o] = 0x10; break;
                    case '2': bin[o] = 0x20; break;
                    case '3': bin[o] = 0x30; break;
                    case '4': bin[o] = 0x40; break;
                    case '5': bin[o] = 0x50; break;
                    case '6': bin[o] = 0x60; break;
                    case '7': bin[o] = 0x70; break;
                    case '8': bin[o] = 0x80; break;
                    case '9': bin[o] = 0x90; break;
                    case 'A': bin[o] = 0xa0; break;
                    case 'a': bin[o] = 0xa0; break;
                    case 'B': bin[o] = 0xb0; break;
                    case 'b': bin[o] = 0xb0; break;
                    case 'C': bin[o] = 0xc0; break;
                    case 'c': bin[o] = 0xc0; break;
                    case 'D': bin[o] = 0xd0; break;
                    case 'd': bin[o] = 0xd0; break;
                    case 'E': bin[o] = 0xe0; break;
                    case 'e': bin[o] = 0xe0; break;
                    case 'F': bin[o] = 0xf0; break;
                    case 'f': bin[o] = 0xf0; break;
                    default: throw new Exception("Invalid character found during hex decode");
                }

                switch (pHexString[i + 1]) {
                    case '0': bin[o] |= 0x00; break;
                    case '1': bin[o] |= 0x01; break;
                    case '2': bin[o] |= 0x02; break;
                    case '3': bin[o] |= 0x03; break;
                    case '4': bin[o] |= 0x04; break;
                    case '5': bin[o] |= 0x05; break;
                    case '6': bin[o] |= 0x06; break;
                    case '7': bin[o] |= 0x07; break;
                    case '8': bin[o] |= 0x08; break;
                    case '9': bin[o] |= 0x09; break;
                    case 'A': bin[o] |= 0x0a; break;
                    case 'a': bin[o] |= 0x0a; break;
                    case 'B': bin[o] |= 0x0b; break;
                    case 'b': bin[o] |= 0x0b; break;
                    case 'C': bin[o] |= 0x0c; break;
                    case 'c': bin[o] |= 0x0c; break;
                    case 'D': bin[o] |= 0x0d; break;
                    case 'd': bin[o] |= 0x0d; break;
                    case 'E': bin[o] |= 0x0e; break;
                    case 'e': bin[o] |= 0x0e; break;
                    case 'F': bin[o] |= 0x0f; break;
                    case 'f': bin[o] |= 0x0f; break;
                    default: throw new Exception("Invalid character found during hex decode");
                }
            }
            return bin;
        }


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

            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// KIỂM TRA ĐỊA CHỈ MAC NHẬP VÀO CÓ ĐÚNG ĐỊNH DẠNG HAY KHÔNG
        /// </summary>
        /// <param name="_mac"></param>
        /// <returns></returns>
        public static bool mac_Address_Is_Correct(string _mac) {
            try {
                //Kiểm tra chiều dài địa chỉ MAC
                if (_mac.Length != 12) return false;
                //Kiểm tra 6 kí tự đầu tiên của MAC có đúng định dạng hay không?
                string _f6 = _mac.Substring(0, 6);
                if (!GlobalData.initSetting.MAC6DIGIT.Contains(_f6)) return false;
                //Kiểm tra 6 kí tự cuối của MAC có đúng định dạng hay không?
                string patterns = "";
                for (int i = 0; i < 12; i++) {
                    patterns += "[0-9,A-F]";
                }
                if (!Regex.IsMatch(_mac, patterns)) return false;

                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// GEN MÃ GPON SERIAL TỪ ĐỊA CHỈ MAC
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public static string GEN_SERIAL_ONT(string MAC) {
            try {
                string low_MAC = MAC.Substring(6, 6);
                string origalByteString = Convert.ToString(HexToBin(low_MAC)[0], 2).PadLeft(8, '0');
                string VNPT_SERIAL_ONT = null;

                origalByteString = origalByteString + "" + Convert.ToString(HexToBin(low_MAC)[1], 2).PadLeft(8, '0');
                origalByteString = origalByteString + "" + Convert.ToString(HexToBin(low_MAC)[2], 2).PadLeft(8, '0');
                //----HEX to BIN Cach 2-------
                string value = low_MAC;
                var s = String.Join("", low_MAC.Select(x => Convert.ToString(Convert.ToInt32(x + "", 16), 2).PadLeft(4, '0')));
                //----HEX to BIN Cach 2-------
                string shiftByteString = "";
                shiftByteString = origalByteString.Substring(1, origalByteString.Length - 1) + origalByteString[0];

                if (MAC.Contains("A06518")) {
                    VNPT_SERIAL_ONT = "VNPT" + "00" + BinToHex(shiftByteString); //"'00' --> dải MAC đang được đăng ký, sau này nếu đăng ký thêm dải mới thì giá trị này sẽ thành '01'"
                }
                else if (MAC.Contains("A4F4C2")) //Dải mác mới của VNPT. Hòa Add: 16/03/2017
                {
                    VNPT_SERIAL_ONT = "VNPT" + "01" + BinToHex(shiftByteString);
                }
                return VNPT_SERIAL_ONT;
            }
            catch {
                return "ERROR";
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
                        //Power Instrument
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

                        ////Switch Instrument
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
                    t1.Start();
                }

                ////Switch Instrument
                if (!t2.IsAlive) {
                    t2 = new Thread(new ThreadStart(() => {
                       
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
