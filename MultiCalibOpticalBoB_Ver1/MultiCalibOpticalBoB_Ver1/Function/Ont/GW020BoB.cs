using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont {
    public class GW020BoB : GW {

        int delay = 100;

        public GW020BoB(string _portname) : base(_portname) { }

        public override bool Login(out string message) {
            message = "";
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while (!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    message += "Gửi lệnh Enter để truy nhập vào login...\r\n";
                    base.WriteLine("\r\n");
                    Thread.Sleep(250);
                    string data = "";
                    data = base.Read();
                    message += string.Format("Feedback:=> {0}\r\n", data);
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains("#")) return true;
                    while (!data.Contains("tc login:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin User
                    base.Write(GlobalData.initSetting.ONTLOGINUSER + "\n");
                    message += "Gửi thông tin user: " + GlobalData.initSetting.ONTLOGINUSER + "...\r\n";

                    //Chờ ONT xác nhận User
                    while (!data.Contains("Password:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin Password
                    base.Write(GlobalData.initSetting.ONTLOGINPASS + "\n");
                    message += "Gửi thông tin password: " + GlobalData.initSetting.ONTLOGINPASS + "...\r\n";

                    //Chờ ONT xác nhận Password
                    while (!data.Contains("root login  on `console'")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;
                    else _flag = true;
                }
                return _flag;
            }
            catch (Exception ex) {
                message += ex.ToString() + "\r\n";
                return false;
            }
        }

        void Write_To_Register(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 1 " + value);
        }

        string DecToHex_2Symbol(int dec) {
            string hex = "";
            hex = dec.ToString("X2");
            if (hex.Length > 2) {
                hex = hex.Substring(hex.Length - 2, 2);
            }
            return hex;
        }

        private string Read_from_I2C(string Reg_Address) {
            string str = "";
            string value_register = "";

            base.WriteLine("laser msg --get " + Reg_Address + " 1");
            Thread.Sleep(delay);
            str = base.Read();
            for (int j = 0; j < str.Split('\n').Length; j++) {
                if (str.Split('\n')[j].Contains("I2C")) {
                    value_register = str.Split('\n')[j].Split(':')[1].Trim();
                }
            }
            return value_register;
        }

        private string DectoBin(int DEC) {
            string Bin = Convert.ToString(DEC, 2);
            int leng;
            leng = Bin.Length;
            if (leng > 6) {
                Bin = Bin.Substring(leng - 6, 6);
            }
            if (leng < 6) {
                for (int i = 1; i <= 6 - leng; i++) {
                    Bin = "0" + Bin;
                }

            }
            return Bin;
        }

        string hex2bin(string value) {
            return Convert.ToString(Convert.ToInt32(value, 16), 2).PadLeft(value.Length * 4, '0');
        }

        string bin2hex(string value) {
            string hex_value = "";
            hex_value = Convert.ToInt32(value, 2).ToString("X2");
            return hex_value;
        }

        string DecToHex(int dec) {
            string hex = "";
            hex = dec.ToString("X4");
            if (hex.Length > 4) {
                hex = hex.Substring(hex.Length - 4, 4);
            }
            return hex;
        }

        string HexToDec(string Hex) {
            int decValue = int.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
            return decValue.ToString();

        }

        string Read_ADC_value_Tx_Power_dec(string register_address) {
            string message_Respond = "";
            string ADC_value = "NONE";
            try {
                base.WriteLine("laser msg --get " + register_address + " 2");
                Thread.Sleep(100);
                message_Respond = base.Read();
                // Định dạng lại kết quả đọc để lấy giá trị Decimal của thanh ghi
                for (int i = 0; i < message_Respond.Split('\n').Length; i++) {
                    if (message_Respond.Split('\n')[i].Contains("I2C")) {
                        ADC_value = message_Respond.Split('\n')[i].Split(':')[1].Trim();
                    }
                }
                ADC_value = HexToDec(ADC_value);

                return ADC_value;
            }
            catch (Exception) {
                return "NONE";
            }
        }

        // Hàm tính Slope_Tx => Complete
        // Nếu giá trị Slope không hợ lệ hàm sẽ trả về "NONE"
        string Calculate_Slope_Tx(string ADC1_Value, string ADC2_value, string Tx1_Power, string Tx2_Power) {
            float slope_Tx_f;
            string slope_Tx_Hex = "";
            int slope_Tx_Int;
            try {
                // Code tính Slope_Tx: slope = (y1-y2)/(x1-x2)
                slope_Tx_f = ((float.Parse(Tx1_Power) - float.Parse(Tx2_Power))) / ((float.Parse(ADC1_Value) - float.Parse(ADC2_value))); //slope = (y1-y2)/(x1-x2)

                //Chuyển sang số nguyên
                slope_Tx_f = (slope_Tx_f / 0.1f) * float.Parse(Math.Pow(2, 8).ToString()); //TxPowerSlope = ( (TxPowerSlope/0.1) * 2^8)
                if (slope_Tx_f < 0) {
                    return "NONE";
                }
                else if (slope_Tx_f > 0) {
                    slope_Tx_Int = (int)Math.Round((double)slope_Tx_f);
                    slope_Tx_Hex = DecToHex(slope_Tx_Int);
                    return slope_Tx_Hex;
                }
                else {
                    return "0000";
                }
            }
            catch (Exception) {
                return "NONE";
            }


            //

        }


        // Hàm tính Offset_Tx 
        // Nếu giá trị Slope không hợp lệ hàm sẽ trả về "NONE"
        string Calculate_Offset_Tx(string ADC1_Value, string ADC2_value, string Tx1_Power, string Tx2_Power) {
            float offset_Tx = 0;
            float slope_Tx = 0;
            string offset_Tx_Hex = "";
            int offset_Tx_Int;
            // Code tính Slope_Tx
            try {
                slope_Tx = ((float.Parse(Tx1_Power) - float.Parse(Tx2_Power))) / ((float.Parse(ADC1_Value) - float.Parse(ADC2_value)));

                offset_Tx = float.Parse(Tx1_Power) - (slope_Tx * float.Parse(ADC1_Value)); // offset = y-(slope*x)

                //Chuyển sang số nguyên
                offset_Tx = (offset_Tx / 0.1f) * float.Parse(Math.Pow(2, 5).ToString()); //TxPowerOffset =( (TxPowerOffset/0.1) * 2^5) 

                offset_Tx_Int = (int)Math.Round((double)offset_Tx);
                // Chuyển sang Hexa
                offset_Tx_Hex = DecToHex(offset_Tx_Int);
                //
                return offset_Tx_Hex;

            }
            catch (Exception) {

                return "NONE";
            }


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <param name="_var"></param>
        /// <returns></returns>
        public override bool calibER(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            double[] No_Trim_ER_Set = { -32, -31, -30, -29, -28, -27, -26, -25, -24, -23, -22, -21, -20, -19, -18, -17, -16, -15, -14, -13, -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0,
                                      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

            double[] No_Target_ER_Set = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 18, 20, 22, 25, 28 };
            double[] Target_ER_Set = { 6.89, 7.78, 8.45, 9.03, 9.45, 10, 10.41, 10.79, 11.13, 11.46, 12.04, 12.55, 13.01, 13.42, 13.97, 14.47 };
            //   i = ----------------------0-----1------2----3-----4-----5----6------7------8------9------10-----11-----12-----13-----14-----15

            double[] Trim_ER_Group = { -10, -9.375, -8.75, -8.125, -7.5, -6.875, -6.25, -5.625, -5, -4.375, -3.75, -3.125, -2.5, -1.875, -1.25, -0.625 };


            int i = 12;
            double Target_ER_SET = Target_ER_Set[i]; 
            //double Target_ER_actual = 12; // ER mục tiêu.
            //double Min_TrimER = -20;
            //double Max_TrimER = 19.375;
            double Trim_ER = -10;
            double ER_measure;
            string Read_Value = "";

            string B0_TRIM_ER_bit76 = "";
            string B0_TRIM_ER_bit50 = "";

            string A5_AUTO_ER_CTRL_Reg_bin = "";
            string A5_AUTO_ER_CTRL_Reg_Hex = "";
            string A5_AUTO_ER_CTRL_bit74 = "";
            string A5_AUTO_ER_CTRL_bit30 = "";
            string Trim_ER_NEW = "";
            bool Tuning_ER_Result = false;

            try {
                // Đọc giá trị thanh ghi B0_TRIM_ER (AUTO_ER_3) 
                Read_Value = Read_from_I2C("b0");
                Read_Value = hex2bin(Read_Value);
                B0_TRIM_ER_bit50 = Read_Value.Substring(2, 6);
                B0_TRIM_ER_bit76 = Read_Value.Substring(0, 2);
                Read_Value = "";
                // Đọc giá trị thanh ghi A5(AUTO_ER_CTRL)
                Read_Value = Read_from_I2C("a5");
                Read_Value = hex2bin(Read_Value);
                A5_AUTO_ER_CTRL_bit74 = Read_Value.Substring(0, 4); //đọc 4 bit thiết lập target ER
                A5_AUTO_ER_CTRL_bit30 = Read_Value.Substring(4, 4);

                A5_AUTO_ER_CTRL_bit74 = DectoBin(20); // Set Target ER = 20 tương đương 13.01
                A5_AUTO_ER_CTRL_Reg_bin = A5_AUTO_ER_CTRL_bit74 + A5_AUTO_ER_CTRL_bit30;
                A5_AUTO_ER_CTRL_Reg_Hex = Convert.ToInt32((A5_AUTO_ER_CTRL_Reg_bin), 2).ToString("X2");

                Write_To_Register("a5", A5_AUTO_ER_CTRL_Reg_Hex);
                Thread.Sleep(100);


                for (int j = 0; j < 10; j++) {
                    _testinfo.SYSTEMLOG += string.Format("Đang chờ set TRIM = {0}%\r\n", Trim_ER);
                    Trim_ER_NEW = bin2hex(B0_TRIM_ER_bit76 + hex2bin(DecToHex(Convert.ToInt32(Trim_ER / 0.625)).Substring(2, 2)).Substring(2, 6));
                    Write_To_Register("b0", Trim_ER_NEW);

                    ER_measure = Convert.ToDouble(GlobalData.erDevice.getER(Port));
                    _testinfo.SYSTEMLOG += "ER_current = " + ER_measure + "dBm\r\n";

                    if (ER_measure >= 14) {
                        _testinfo.SYSTEMLOG += "Đang giảm TrimER.\r\n";
                        Trim_ER = Trim_ER - 7.5;
                        _testinfo.SYSTEMLOG += "TrimER_current = " + Trim_ER + "%\r\n";
                    }
                    else if (ER_measure > 13 && ER_measure < 14) {
                        _testinfo.SYSTEMLOG += "Đang giảm TrimER.\r\n";
                        Trim_ER = Trim_ER - 2.5;
                        _testinfo.SYSTEMLOG += "TrimER_current = " + Trim_ER + "%\r\n";
                    }
                    else if (ER_measure < 11) {
                        _testinfo.SYSTEMLOG += "Đang tăng TrimER.\r\n";
                        Trim_ER = Trim_ER + 7.5;
                        _testinfo.SYSTEMLOG += "TrimER_current = " + Trim_ER + "%\r\n";
                    }
                    else if (ER_measure < 12 && ER_measure >= 11) {
                        _testinfo.SYSTEMLOG += "Đang tăng TrimER.\r\n";
                        Trim_ER = Trim_ER + 2.5;
                        _testinfo.SYSTEMLOG += "TrimER_current = " + Trim_ER + "%\r\n";
                    }
                    else {
                        _testinfo.SYSTEMLOG += string.Format("Trim ER để 12 < ER < 13 = {0}\r\n", Trim_ER);
                        _testinfo.SYSTEMLOG += "[OK] TX Tuning ER thành công.\r\n";
                        Tuning_ER_Result = true;
                        break;
                    }
                }
            }
            catch (Exception ex) {
                _testinfo.SYSTEMLOG += "[ERROR] Lỗi tiến trình calib ER.\r\n";
                _testinfo.SYSTEMLOG += ex.ToString() + "\r\n";
                Tuning_ER_Result = false;
            }

            return Tuning_ER_Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <param name="_var"></param>
        /// <returns></returns>
        public override bool calibPower(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            float power_temp;
            double Ibias;
            bool Tuning_Tx_Power_Result = false;
            bool Check_Ibias_Result = false;
            int A8_DEC = 160;

            for (int i = 0; i < 10; i++) {
                _testinfo.SYSTEMLOG += string.Format("A8_DEC = {0}\r\n", A8_DEC.ToString());
                Write_To_Register("a8", DecToHex_2Symbol(A8_DEC)); //Set giá trị thanh ghi A8 = 160 tương đương với mức Pwr 1
                Thread.Sleep(100);
                power_temp = float.Parse(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Power Level = {0}\r\n", power_temp);

                if (power_temp > 4) {
                    A8_DEC = A8_DEC - 20;
                }

                else if (power_temp > 3) {
                    A8_DEC = A8_DEC - 3;
                }

                else if (power_temp < 1) {
                    A8_DEC = A8_DEC + 15;
                }

                else if (power_temp < 2.5) {
                    A8_DEC = A8_DEC + 3;
                }
                else if (power_temp <= 3 && power_temp >= 2.5) {
                    _testinfo.SYSTEMLOG += "[OK] Tuning Tx Power => PASS\r\n";
                    Tuning_Tx_Power_Result = true;
                    break;
                }
                else {
                    _testinfo.SYSTEMLOG += "Công suất khởi tạo nhỏ hơn công suất mục tiêu\r\n";
                    Tuning_Tx_Power_Result = false;
                    break;
                }
            }

            base.WriteLine("laser txbias --read");
            Thread.Sleep(200);
            Ibias = (Convert.ToDouble(base.Read().Split('=')[1])) / 1000;
            _testinfo.SYSTEMLOG += string.Format("Ibias = {0} mA\r\n", Ibias);
            if (Ibias > 13) Check_Ibias_Result = true;
            else Check_Ibias_Result = false;

            Tuning_Tx_Power_Result = Tuning_Tx_Power_Result && Check_Ibias_Result;
            return Tuning_Tx_Power_Result;
        }


        public override string getMACAddress(testinginfo _testinfo) {
            //Get MAC Address
            string _mac = "";
            try {
                _testinfo.SYSTEMLOG += string.Format("Get mac address...\r\n");
                base.Write("ifconfig\n");
                Thread.Sleep(300);
                string _tmpStr = base.Read();
                _tmpStr = _tmpStr.Replace("\r", "").Replace("\n", "").Trim();
                string[] buffer = _tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                _tmpStr = buffer[1].Trim();
                _mac = _tmpStr.Substring(0, 17).Replace(":", "");
                _testinfo.SYSTEMLOG += string.Format("...PASS. {0}\r\n", _mac);
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-GM-0001)";
                _testinfo.SYSTEMLOG += string.Format("...FAIL. {0}. {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
            }

            //Write Pass
            base.WriteLine("laser msg --set 7b 1 ff"); //Chuỗi lệnh Write Password
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7c 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7d 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7e 1 ff"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7f 1 2"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c4 1 0"); //
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c7 1 0"); //Lệnh Enable Edit EEPROM
            Thread.Sleep(100);

            base.WriteLine("laser msg --set 6e 1 44"); //2 lệnh Soft Reset
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 6e 1 04"); //
            Thread.Sleep(100);

            base.WriteLine("sh");
            Thread.Sleep(100);
            base.WriteLine("bs /misc prbs gpon 23 0"); //Lệnh phát tín hiệu PRBS
            Thread.Sleep(100);
            base.WriteLine("exit");

            base.WriteLine("laser msg --set a2 1 3a"); //Lệnh set Board ở chế độ CW Mode
            Thread.Sleep(100);

            _testinfo.SYSTEMLOG += "[OK] Đã Write Pass xong.\r\n";

            return _mac;
        }

        public override bool loginToONT(testinginfo _testinfo) {
            _testinfo.SYSTEMLOG += string.Format("Verifying type of ONT...\r\n...{0}\r\n", GlobalData.initSetting.ONTTYPE);
            bool _result = false;
            string _message = "";

            _testinfo.SYSTEMLOG += "Open comport of ONT...\r\n";
            if (!base.Open(out _message)) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-LI-0001)";
                _testinfo.SYSTEMLOG += string.Format("...{0}, {1}\r\n", _testinfo.ERRORCODE, _message);
                return false;
            }
            _testinfo.SYSTEMLOG += "...PASS\r\n";

            _testinfo.SYSTEMLOG += "Login to ONT...\r\n";
            _result = this.Login(out _message);

            if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-LI-0002)";
            _testinfo.SYSTEMLOG += string.Format("...{0}, {1}\r\n", _testinfo.ERRORCODE, _message);
            _testinfo.SYSTEMLOG += _result == true ? "PASS\r\n" : "FAIL\r\n";

            return _result;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <param name="_var"></param>
        /// <returns></returns>
        public override bool txDDMI(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            bool _result = false;
            string value = "";
            string value_register_A8 = "";

            base.WriteLine("laser msg --get A8 1");
            Thread.Sleep(100);
            value = base.Read();
            for (int j = 0; j < value.Split('\n').Length; j++) {
                if (value.Split('\n')[j].Contains("I2C")) {
                    value_register_A8 = value.Split('\n')[j].Split(':')[1].Trim();
                }
            }

            _result = Calibrate_TX_GN25L98( Port, _testinfo, Int32.Parse(HexToDec(value_register_A8)) - 30, Int32.Parse(HexToDec(value_register_A8)));
            if (_result) {
                base.WriteLine("laser msg --set a2 1 1a"); //Lệnh set Board ở chế độ Burst Mode
                Thread.Sleep(100);
            }
            
            return _result;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <param name="_var"></param>
        /// <returns></returns>
        public override bool calibCrossing(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            string A3_TX_CTRL_hex;
            string A3_TX_CTRL_bin;
            string A3_TX_CTRL_bit30 = "";
            string A3_TX_CTRL_bit74_bin;
            double Crossing_measure;
            int A3_TX_CTRL_bit74_dec;
            bool Tuning_Crossing_Result = false;

            A3_TX_CTRL_hex = Read_from_I2C("A3");
            A3_TX_CTRL_bin = hex2bin(A3_TX_CTRL_hex);
            A3_TX_CTRL_bit74_bin = A3_TX_CTRL_bin.Substring(0, 4);
            A3_TX_CTRL_bit30 = A3_TX_CTRL_bin.Substring(4, 4);
            A3_TX_CTRL_bit74_dec = Convert.ToInt32(A3_TX_CTRL_bit74_bin, 2);
            _testinfo.SYSTEMLOG += string.Format("A3h old = {0}\r\n", A3_TX_CTRL_hex);

            for (int i = 0; i < 5; i++) {
                Crossing_measure = Convert.ToDouble(GlobalData.erDevice.getCrossing(Port));
                //MessageBox.Show("Crossing = " + Crossing_measure.ToString());
                _testinfo.SYSTEMLOG += string.Format("Crossing Point % = {0}\r\n", Crossing_measure);
                if (Crossing_measure >= 49.5 && Crossing_measure <= 51) {
                    _testinfo.SYSTEMLOG += "[OK] Crossing Point đã đạt 50% +- 0.5\r\n";
                    Tuning_Crossing_Result = true;
                    break;
                }
                else if (Crossing_measure < 49.5) {
                    _testinfo.SYSTEMLOG += "Thực hiện giảm giá trị thanh ghi A3.\r\n";
                    A3_TX_CTRL_bit74_dec = A3_TX_CTRL_bit74_dec - 1;
                    A3_TX_CTRL_bit74_bin = DectoBin(A3_TX_CTRL_bit74_dec);
                    A3_TX_CTRL_bin = A3_TX_CTRL_bit74_bin + A3_TX_CTRL_bit30;
                    A3_TX_CTRL_hex = bin2hex(A3_TX_CTRL_bin);
                    _testinfo.SYSTEMLOG += string.Format("A3h new = {0}\r\n", A3_TX_CTRL_hex);
                    Write_To_Register("A3", A3_TX_CTRL_hex);
                    Thread.Sleep(delay);
                    continue;
                }
                else {
                    _testinfo.SYSTEMLOG += "Thực hiện tăng giá trị thanh ghi A3.\r\n";
                    A3_TX_CTRL_bit74_dec = A3_TX_CTRL_bit74_dec + 1;
                    A3_TX_CTRL_bit74_bin = DectoBin(A3_TX_CTRL_bit74_dec);
                    A3_TX_CTRL_bin = A3_TX_CTRL_bit74_bin + A3_TX_CTRL_bit30;
                    A3_TX_CTRL_hex = bin2hex(A3_TX_CTRL_bin);
                    _testinfo.SYSTEMLOG += string.Format("A3h new = {0}\r\n", A3_TX_CTRL_hex);
                    Write_To_Register("A3", A3_TX_CTRL_hex);
                    Thread.Sleep(delay);
                    continue;
                }
            }
            return Tuning_Crossing_Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Port"></param>
        /// <param name="_testinfo"></param>
        /// <param name="end_point"></param>
        /// <param name="start_point"></param>
        /// <returns></returns>
        bool Calibrate_TX_GN25L98(int Port, testinginfo _testinfo, int end_point, int start_point) {
            string TX1_Power = "NONE";
            string TX2_Power = "NONE";

            string ADC_TX_Point1 = "";
            string ADC_TX_Point2 = "";

            string Slope = "";
            string Offset = "";
            bool TX_Calibration_Power_Result = false;
            string Power_DDMI = "";

            int delay = 100;
            try {
                for (int i = 0; i < 3; i++) {
                    // Tính Slope - Offset
                    _testinfo.SYSTEMLOG += "Set Point 1: Reg A8 = " + end_point + " -> HEX\r\n";
                    Write_To_Register("a8", DecToHex_2Symbol(end_point));
                    Thread.Sleep(delay);

                    // Đọc thanh ghi EE, EF -> chuyển sang Decimal
                    ADC_TX_Point1 = Read_ADC_value_Tx_Power_dec("ee");
                    TX1_Power = GlobalData.powerDevice.getPower_mW(Port).ToString();


                    // Phát điểm công suất 2:point 2 - A8 = BE 
                    _testinfo.SYSTEMLOG += "Set Point 1: Reg A8 = " + start_point + " -> HEX\r\n";
                    Write_To_Register("a8", DecToHex_2Symbol(start_point));
                    Thread.Sleep(delay);

                    //Đọc thanh ghi EE, EF -> chuyển sang Decimal
                    ADC_TX_Point2 = Read_ADC_value_Tx_Power_dec("ee"); ;
                    TX2_Power = GlobalData.powerDevice.getPower_mW(Port).ToString();

                    _testinfo.SYSTEMLOG += " \r\n";
                    _testinfo.SYSTEMLOG += string.Format("ADC_TX_Point1 = {0}\r\n", ADC_TX_Point1);
                    _testinfo.SYSTEMLOG += string.Format("ADC_TX_Point2 = {0}\r\n", ADC_TX_Point2);
                    _testinfo.SYSTEMLOG += string.Format("TX1_Power(uW) = {0}\r\n", TX1_Power);
                    _testinfo.SYSTEMLOG += string.Format("TX2_Power(uW) = {0}\r\n", TX2_Power);

                    Slope = Calculate_Slope_Tx(ADC_TX_Point1, ADC_TX_Point2, TX1_Power, TX2_Power);
                    Offset = Calculate_Offset_Tx(ADC_TX_Point1, ADC_TX_Point2, TX1_Power, TX2_Power);
                    _testinfo.SYSTEMLOG += string.Format("Slope = {0}\r\n", Slope);
                    _testinfo.SYSTEMLOG += string.Format("Offset = {0}\r\n", Offset);

                    if (Slope != "NONE" && Offset != "NONE") {
                        // Write Slope, Offset tới GN25L98
                        _testinfo.SYSTEMLOG += "Thực hiện Write Slope & Offset to EEPROM\r\n";
                        Write_To_Register("de", Slope.Substring(0, 2));
                        Thread.Sleep(delay);
                        Write_To_Register("df", Slope.Substring(2, 2));
                        Thread.Sleep(delay);
                        Write_To_Register("e0", Offset.Substring(0, 2));
                        Thread.Sleep(delay);
                        Write_To_Register("e1", Offset.Substring(2, 2));
                        Thread.Sleep(delay);
                        base.Read();
                        base.WriteLine("laser power --txread");
                        Thread.Sleep(200);
                        Power_DDMI = base.Read().Split('=')[2].Substring(0, 5);
                        _testinfo.SYSTEMLOG += string.Format("Power DDMI = {0}\r\n", Power_DDMI);
                        string TX2_Power_dBm = "";
                        TX2_Power_dBm = GlobalData.powerDevice.getPower_dBm(Port);
                        Thread.Sleep(delay);
                        _testinfo.SYSTEMLOG += string.Format("Power thực tế = {0}\r\n", TX2_Power_dBm);
                        if (Convert.ToDouble(Power_DDMI) > (Convert.ToDouble(TX2_Power_dBm) - 0.5) && Convert.ToDouble(Power_DDMI) < (Convert.ToDouble(TX2_Power_dBm) + 0.5)) {
                            _testinfo.SYSTEMLOG += "[OK] Hoàn thành TX Calibration Power\r\n";
                            TX_Calibration_Power_Result = true;
                        }
                        else {
                            _testinfo.SYSTEMLOG += "[FAIL] Lỗi TX Calibration Power\r\n";
                            TX_Calibration_Power_Result = false;
                        }
                        break;
                    }
                    else {
                        continue;
                    }
                }
            }
            catch (Exception ee) {
                _testinfo.SYSTEMLOG += string.Format("[FAIL] Lỗi trong quá trình TX Calibration Power: {0}\r\n" + ee);
                TX_Calibration_Power_Result = false;
            }
            return TX_Calibration_Power_Result;
        }


        public override bool verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool writeMAC(testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool outTXPower() {
            throw new NotImplementedException();
        }

        public override bool signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

    }
}
