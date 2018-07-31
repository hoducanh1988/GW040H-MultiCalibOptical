using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MultiCalibOpticalBoB_Ver1.Function.IO;
using System.Diagnostics;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont {
    public class GW020BoB : GW {

        int delay = 10;

        public GW020BoB(string _portname) : base(_portname) { }

        #region Support

        //OK
        public override bool Login(out string message) {
            message = "";
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while (!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    message += "Gửi lệnh Enter để truy nhập vào login...\r\n";
                    base.WriteLine("");
                    Thread.Sleep(200);
                    string data = "";
                    data = base.Read();
                    message += string.Format("Feedback:=> {0}\r\n", data);
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains(">")) return true;
                    while (!data.Contains("Login:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(200);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin User
                    base.Write(GlobalData.initSetting.ONTLOGINUSER + "\n");
                    message += "Gửi thông tin user: " + GlobalData.initSetting.ONTLOGINUSER + "...\r\n";

                    //Chờ ONT xác nhận User
                    data = "";
                    while (!data.Contains("Password:")) {
                        data += base.Read();
                        message += string.Format("Feedback:=> {0}\r\n", data);
                        Thread.Sleep(100);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin Password
                    base.Write(GlobalData.initSetting.ONTLOGINPASS + "\n");
                    message += "Gửi thông tin password: " + GlobalData.initSetting.ONTLOGINPASS + "...\r\n";

                    //Chờ ONT xác nhận Password
                    while (!data.Contains(">")) {
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

        //OK
        void Write_To_Register(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 1 " + value);
        }

        //OK
        string DecToHex_2Symbol(int dec) {
            string hex = "";
            hex = dec.ToString("X2");
            if (hex.Length > 2) {
                hex = hex.Substring(hex.Length - 2, 2);
            }
            return hex;
        }

        //OK
        private string Read_from_I2C(string Reg_Address) {
            string str = "";
            string value_register = "";

            base.Read();
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

        //OK
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

        //OK
        string hex2bin(string value) {
            return Convert.ToString(Convert.ToInt32(value, 16), 2).PadLeft(value.Length * 4, '0');
        }

        //OK
        string bin2hex(string value) {
            string hex_value = "";
            hex_value = Convert.ToInt32(value, 2).ToString("X2");
            return hex_value;
        }

        //OK
        string DecToHex(int dec) {
            string hex = "";
            hex = dec.ToString("X4");
            if (hex.Length > 4) {
                hex = hex.Substring(hex.Length - 4, 4);
            }
            return hex;
        }

        //OK
        string HexToDec(string Hex) {
            int decValue = int.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
            return decValue.ToString();

        }

        //OK
        string Read_ADC_value_Tx_Power_dec(string register_address) {
            string message_Respond = "";
            string ADC_value = "NONE";
            try {
                base.Read();
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
        //OK
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
        //OK
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

        //OK
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

        //OK
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

        #endregion

        #region Support Write APD

        private static string dir = System.AppDomain.CurrentDomain.BaseDirectory;

        private static string file_a2lower = string.Format("{0}ApdFiles\\A2_Lower_Hisen_VNPT.txt", dir);
        private static string file_a2table2 = string.Format("{0}ApdFiles\\A2_Table_2_Hisen_VNPT.txt", dir);
        private static string file_Modulation = string.Format("{0}ApdFiles\\Modualtion_LUT.txt", dir);
        private static string file_bias = string.Format("{0}ApdFiles\\Bias_LUT.txt", dir);
        private static string file_apd = string.Format("{0}ApdFiles\\APD_LUT.txt", dir);


        public void Write_To_Register_2byte_COM(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 2 " + value);
        }

        public void Write_To_Register_1byte_COM(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 1 " + value);
        }

        public void Write_To_Register_4byte_COM(string register_address, string value) {
            base.WriteLine("laser msg --set " + register_address + " 4 " + value);
        }


        public void Write_PassWord_COM() {
            base.WriteLine("laser msg --set 7b 1 ff");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7c 1 ff");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7d 1 ff");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7e 1 ff");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set 7f 1 2");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c4 1 0");
            Thread.Sleep(100);
            base.WriteLine("laser msg --set c7 1 0");
            Thread.Sleep(100);
        }

        public void Load_A2_Lower_2byte_COM(testinginfo _testinfo, string path) {
            int Reg_Address = 0x00;
            string line_1 = null;
            string line_2 = null;
            string line_3 = null;
            string line_4 = null;
            string line_1_register = null;
            string line_2_register = null;
            string line_3_register = null;
            string line_4_register = null;
            string line_1_value = null;
            string line_2_value = null;
            string line_3_value = null;
            string line_4_value = null;
            string[] value = new string[128];
            string[] Register = new string[128];
            base.WriteLine("laser msg --set a2 1 1a"); //Set chế độ Burst Mode
            Thread.Sleep(delay);
            Write_To_Register_1byte_COM("C7", "00");
            Thread.Sleep(delay);

            using (StreamReader reader = new StreamReader(path)) {
                for (int i = 1; i < 124; i++) {
                    if ((i > 0 && i < 111)) {
                        line_1 = reader.ReadLine();
                        line_2 = reader.ReadLine();

                        line_1_register = line_1.Split(' ')[0].Split('x')[1].Trim();
                        if (line_1_register.Length == 1)
                            line_1_register = "0" + line_1_register;
                        line_2_register = line_2.Split(' ')[0].Split('x')[1].Trim();
                        if (line_2_register.Length == 1)
                            line_2_register = "0" + line_2_register;

                        line_1_value = line_1.Split('x')[2];
                        if (line_1_value.Length == 1)
                            line_1_value = "0" + line_1_value;
                        line_2_value = line_2.Split('x')[2];
                        if (line_2_value.Length == 1)
                            line_2_value = "0" + line_2_value;

                        if (line_1 != null) {
                            Register[i] = line_1_register;
                            try {
                                //value[i] = line.Substring(9, 4);
                                value[i] = line_1_value + line_2_value;
                            }
                            catch {
                                MessageBox.Show("Lỗi load A2 Lower");
                            }
                            Write_To_Register_2byte_COM(Register[i], value[i]);
                            Thread.Sleep(delay);
                            _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        }
                        else {
                            break;
                        }
                        i++;
                    }


                    else if ((i > 110 && i < 124)) {
                        line_1 = reader.ReadLine();
                        line_1_register = line_1.Split(' ')[0].Split('x')[1].Trim();
                        if (line_1_register.Length == 1)
                            line_1_register = "0" + line_1_register;

                        line_1_value = line_1.Split('x')[2];
                        if (line_1_value.Length == 1)
                            line_1_value = "0" + line_1_value;

                        if (line_1 != null) {
                            Register[i] = line_1_register;
                            try {
                                value[i] = line_1_value;
                            }
                            catch {
                                MessageBox.Show("Lỗi load A2 Lower");
                            }
                            Write_To_Register_1byte_COM(Register[i], value[i]);
                            Thread.Sleep(delay);
                            _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            _testinfo.SYSTEMLOG += "Load A2 Lower sucessful!\r\n";
        }

        public void Load_A2_Table2_2byte_COM(testinginfo _testinfo, string path) {
            //int Reg_Address = 0x80;
            string line_1 = null;
            string line_2 = null;
            string line_1_register = null;
            string line_2_register = null;
            string line_1_value = null;
            string line_2_value = null;
            string[] value = new string[300];
            string[] Register = new string[300];

            Write_To_Register_1byte_COM("C7", "00");
            Thread.Sleep(delay);

            using (StreamReader reader = new StreamReader(path)) {
                for (int i = 128; i < 256; i++) {
                    if ((i >= 128 && i < 158) || (i >= 160 && i < 169) || (i >= 173 && i < 201) || (i == 209) || (i >= 248 && i < 255)) {
                        line_1 = reader.ReadLine();
                        try {
                            line_1_value = line_1.Split('x')[1];
                            if (line_1_value.Length == 1)
                                line_1_value = "0" + line_1_value;

                            if (line_1 != null) {
                                Register[i] = DecToHex_2Symbol(i);
                                try {
                                    value[i] = line_1_value;
                                }
                                catch {
                                    MessageBox.Show("Lỗi load A2 Lower");
                                }
                                Write_To_Register_1byte_COM(Register[i], value[i]);
                                Thread.Sleep(delay);
                                _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                            }
                            else {
                                break;
                            }
                            //i--;
                        }
                        catch { }
                    }

                    else if ((i == 158) || (i >= 169 && i < 173) || (i >= 201 && i < 209) || (i >= 210 && i < 248)) {
                        line_1 = reader.ReadLine();
                        line_2 = reader.ReadLine();

                        line_1_register = DecToHex_2Symbol(i);
                        line_2_register = DecToHex_2Symbol(i);

                        line_1_value = line_1.Split('x')[1];
                        if (line_1_value.Length == 1)
                            line_1_value = "0" + line_1_value;
                        line_2_value = line_2.Split('x')[1];
                        if (line_2_value.Length == 1)
                            line_2_value = "0" + line_2_value;

                        if (line_1 != null) {
                            Register[i] = line_1_register;
                            try {
                                value[i] = line_1_value + line_2_value;
                            }
                            catch {
                                MessageBox.Show("Lỗi load A2 Lower");
                            }
                            Write_To_Register_2byte_COM(Register[i], value[i]);
                            Thread.Sleep(delay);
                            _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        }
                        else {
                            break;
                        }
                        i++;
                    }
                }
                base.Read();
                Write_To_Register_1byte_COM("98", "6A");
                Thread.Sleep(delay);
                base.WriteLine("laser msg --set a2 1 3a");
                Thread.Sleep(delay);
                _testinfo.SYSTEMLOG += "Load A2 Table 2 sucessful!\r\n";
            }
        }

        public void Load_Modulation_LUT_2byte_COM(testinginfo _testinfo, string path) {
            int Reg_Address = 0x00;
            string line_1 = null;
            string line_2 = null;
            string[] value = new string[300];
            string[] Register = new string[300];
            base.WriteLine("laser msg --set 7F 1 04"); //Select Modulation LUT
            Thread.Sleep(delay);

            using (StreamReader reader = new StreamReader(path)) {
                for (int i = 128; i < 256; i += 2) {
                    line_1 = reader.ReadLine();
                    line_2 = reader.ReadLine();

                    if (line_1.Split('x')[1].Trim().Length == 1)
                        line_1 = "00";
                    else
                        line_1 = line_1.Split('x')[1].Trim();

                    if (line_2.Split('x')[1].Trim().Length == 1)
                        line_2 = "00";
                    else
                        line_2 = line_2.Split('x')[1].Trim();

                    if (line_1 != null) {
                        Register[i] = DecToHex_2Symbol(i);
                        try {
                            value[i] = line_1 + line_2;
                            //MessageBox.Show(Register[i] + " = " + value[i]);
                        }
                        catch {
                            _testinfo.SYSTEMLOG += "Lỗi load Modulation LUT\r\n";
                        }
                        Write_To_Register_2byte_COM(Register[i], value[i]);
                        Thread.Sleep(delay);
                        _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        //Hienthi.SetText(rtb, ONT.Read());
                    }
                    else {
                        break;
                    }
                }
            }
            base.WriteLine("laser msg --set 7F 1 02"); //Select Control & Setting Table
            Thread.Sleep(delay);
            base.WriteLine("laser msg --set a4 1 c5"); //Enable Mod LUT & Bias LUT
            Thread.Sleep(delay);
            _testinfo.SYSTEMLOG += "Load Modulation LUT successful!\r\n";
        }

        public void Load_Bias_LUT_2byte_COM(testinginfo _testinfo, string path) {
            int Reg_Address = 0x00;
            string line_1 = null;
            string line_2 = null;
            string[] value = new string[300];
            string[] Register = new string[300];
            base.WriteLine("laser msg --set 7F 1 05"); //Select Bias LUT
            Thread.Sleep(delay);

            using (StreamReader reader = new StreamReader(path)) {
                for (int i = 128; i < 256; i += 2) {
                    line_1 = reader.ReadLine();
                    line_2 = reader.ReadLine();

                    if (line_1.Split('x')[1].Trim().Length == 1)
                        line_1 = "00";
                    else
                        line_1 = line_1.Split('x')[1].Trim();

                    if (line_2.Split('x')[1].Trim().Length == 1)
                        line_2 = "00";
                    else
                        line_2 = line_2.Split('x')[1].Trim();

                    if (line_1 != null) {
                        Register[i] = DecToHex_2Symbol(i);
                        try {
                            value[i] = line_1 + line_2;
                        }
                        catch {
                            _testinfo.SYSTEMLOG += "Lỗi load Bias LUT\r\n";
                        }
                        Write_To_Register_2byte_COM(Register[i], value[i]);
                        Thread.Sleep(delay);
                        _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        //Hienthi.SetText(rtb, ONT.Read());
                    }
                    else {
                        break;
                    }
                }
            }
            base.WriteLine("laser msg --set 7F 1 02"); //Select Control & Setting Table
            Thread.Sleep(delay);

            base.WriteLine("laser msg --set a4 1 c5"); //Enable Mod LUT & Bias LUT
            Thread.Sleep(delay);
            _testinfo.SYSTEMLOG += "Load Bias LUT successful!\r\n";
        }

        public void Load_APD_LUT_2byte_COM(testinginfo _testinfo, string path) {
            int Reg_Address = 0x00;
            string line_1 = null;
            string line_2 = null;
            string[] value = new string[300];
            string[] Register = new string[300];
            base.WriteLine("laser msg --set 7F 1 06"); //Select APD DAC LUT
            Thread.Sleep(delay);

            using (StreamReader reader = new StreamReader(path)) {
                for (int i = 192; i < 256; i += 2) {
                    line_1 = reader.ReadLine();
                    line_2 = reader.ReadLine();

                    if (line_1.Split('X')[1].Trim().Length == 1)
                        line_1 = "00";
                    else
                        line_1 = line_1.Split('X')[1].Trim();

                    if (line_2.Split('X')[1].Trim().Length == 1)
                        line_2 = "00";
                    else
                        line_2 = line_2.Split('X')[1].Trim();

                    if (line_1 != null) {
                        Register[i] = DecToHex_2Symbol(i);
                        try {
                            value[i] = line_1 + line_2;
                        }
                        catch {
                            _testinfo.SYSTEMLOG += "Lỗi load APD LUT\r\n";
                        }
                        Write_To_Register_2byte_COM(Register[i], value[i]);
                        Thread.Sleep(delay);
                        _testinfo.SYSTEMLOG += Register[i] + " = " + value[i] + "\r\n";
                        //Hienthi.SetText(rtb, ONT.Read());
                    }
                    else {
                        break;
                    }
                }
            }
            base.WriteLine("laser msg --set 7F 1 02"); //Select Control & Setting Table
            Thread.Sleep(delay);

            base.WriteLine("laser msg --set 99 1 8a"); //Enable Mod APD LUT
            Thread.Sleep(delay);
            _testinfo.SYSTEMLOG += "Load APD LUT successful!\r\n";
        }

        public void Create_APD_LUT(string BOSA_SN, double DDMI_Temperature, double Vbr_BOSA_report, double Slope_Up, double Slope_Down, double Offset) {
            //double DDMI_Temperature = 33;
            int count = 0;
            double Vbr;
            double V_APD = 43;
            double APD_DAC;
            string APD_DAC_Bin = "";
            string APD_DAC_Bin_Full = "";
            string register = "";

            //DDMI_Temperature = 
            Vbr = Vbr_BOSA_report + (DDMI_Temperature - 25) * Slope_Up;

            for (int j = 1; j < 5; j++) {
                if (DDMI_Temperature % 5 == 0)
                    break;
                else
                    DDMI_Temperature = DDMI_Temperature - 1;
            }
            for (int i = -40; i < 120; i += 5) {
                if (i < 25) {
                    V_APD = (Vbr - 3) + (i - DDMI_Temperature) * Slope_Down + Offset;
                    APD_DAC = Math.Round(V_APD * (511 * 3900) / (1.261 * (200000 + 3900)));
                    APD_DAC_Bin = Convert.ToString(Convert.ToInt32(APD_DAC), 2);
                    int leng = APD_DAC_Bin.Length;
                    for (int m = 1; m < 16 - leng + 1; m++) {
                        APD_DAC_Bin += "0";
                    }
                    APD_DAC_Bin_Full = APD_DAC_Bin;
                    register = Convert.ToInt32(APD_DAC_Bin_Full, 2).ToString("X");
                    SaveLog_csv(BOSA_SN, i.ToString(), V_APD, APD_DAC, APD_DAC_Bin, APD_DAC_Bin_Full, register);
                    SaveLog_txt(BOSA_SN, count, register.Substring(0, 2), register.Substring(2, 2), count);
                    count += 2;
                }
                else {
                    V_APD = (Vbr - 3) + (i - DDMI_Temperature) * Slope_Up + Offset;
                    APD_DAC = Math.Round(V_APD * (511 * 3900) / (1.261 * (200000 + 3900)));
                    APD_DAC_Bin = Convert.ToString(Convert.ToInt32(APD_DAC), 2);
                    int leng = APD_DAC_Bin.Length;
                    for (int m = 1; m < 16 - leng + 1; m++) {
                        APD_DAC_Bin += "0";
                    }
                    APD_DAC_Bin_Full = APD_DAC_Bin;
                    register = Convert.ToInt32(APD_DAC_Bin_Full, 2).ToString("X");
                    SaveLog_csv(BOSA_SN, i.ToString(), V_APD, APD_DAC, APD_DAC_Bin, APD_DAC_Bin_Full, register);
                    SaveLog_txt(BOSA_SN, count, register.Substring(0, 2), register.Substring(2, 2), count);
                    count += 2;
                }
            }
            //return V_APD.ToString();
            //MessageBox.Show("Tạo APD_LUT thành công.");
        }

        public void SaveLog_txt(string BOSA_SN, int Stt, string Value1, string Value2, int count) {
            using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + "_APD_LUT.txt", true)) {
                // Use string interpolation syntax to make code clearer.
                if (count < 9) {
                    writer.Write(Stt + "   ");
                }
                else {
                    writer.Write(Stt + "  ");
                }
            }
            using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + "_APD_LUT.txt", true)) {
                // Use string interpolation syntax to make code clearer.
                writer.WriteLine("0X" + Value1);
            }

            using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + "_APD_LUT.txt", true)) {
                // Use string interpolation syntax to make code clearer.
                if (count < 9) {
                    writer.Write((Stt + 1) + "   ");
                }
                else {
                    writer.Write((Stt + 1) + "  ");
                }
            }
            using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + "_APD_LUT.txt", true)) {
                // Use string interpolation syntax to make code clearer.
                writer.WriteLine("0X" + Value2);
            }
        }

        public void SaveLog_csv(string BOSA_SN, string Temperature, double V_APD, double APD_DAC, string APD_DAC_Bin, string APD_DAC_Bin_Full, string Register) {
            try {
                if (System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT") == false) {
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT");
                }
                if (System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + ".csv") == false) {
                    StreamWriter csvWriter = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + ".csv", true);
                    csvWriter.WriteLine("Temperature,V_APD,APD_DAC,Register");
                    csvWriter.WriteLine(Temperature + "," + V_APD + "," + APD_DAC + "," + APD_DAC_Bin + "," + APD_DAC_Bin_Full + "," + Register);
                    csvWriter.Dispose();
                }
                else {
                    StreamWriter csvWriter = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\APD_LUT\\" + BOSA_SN + ".csv", true);
                    csvWriter.WriteLine(Temperature + "," + V_APD + "," + APD_DAC + "," + APD_DAC_Bin + "," + APD_DAC_Bin_Full + "," + Register);
                    csvWriter.Dispose();
                }

                //return true;
            }
            catch (Exception Ex) {
                MessageBox.Show("Lỗi trong quá trình ghi file Log csv." + Ex.ToString());
                //return false;
            }
        }


        #endregion

        #region Calibration

        /// <summary>
        /// ---------------------------00----------------------------
        /// </summary>
        /// <param name="_bosainfo"></param>
        /// <param name="_testinfo"></param>
        /// <returns></returns>
        public override bool writeAPD(bosainfo _bosainfo, testinginfo _testinfo) {
            _testinfo.WRITEAPDRESULT = Parameters.testStatus.Wait.ToString();
            try {
                Stopwatch st = new Stopwatch();
                st.Start();
                this.Write_PassWord_COM();
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format ("Write_PassWord_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Load_A2_Lower_2byte_COM(_testinfo, file_a2lower);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Load_A2_Lower_2byte_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Load_A2_Table2_2byte_COM(_testinfo, file_a2table2);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Load_A2_Table2_2byte_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Load_Modulation_LUT_2byte_COM(_testinfo, file_Modulation);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Load_Modulation_LUT_2byte_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Load_Bias_LUT_2byte_COM(_testinfo, file_bias);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Load_Bias_LUT_2byte_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Create_APD_LUT(_bosainfo.BosaSN, 25, double.Parse(_bosainfo.Vbr), BosaConfig.SlopeUp, BosaConfig.SlopeDown, BosaConfig.Offset);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Create_APD_LUT time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();

                this.Load_APD_LUT_2byte_COM(_testinfo, file_apd);
                st.Stop();
                _testinfo.SYSTEMLOG += string.Format("Load_APD_LUT_2byte_COM time = {0}ms\r\n", st.ElapsedMilliseconds);
                st.Reset(); st.Restart();
                _testinfo.WRITEAPDRESULT = Parameters.testStatus.PASS.ToString();
                return true;
            } catch {
                _testinfo.WRITEAPDRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }


        /// <summary>
        /// ---------------------------01----------------------------//OK
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

            _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.Wait.ToString();

            for (int i = 0; i < 10; i++) {
                _testinfo.SYSTEMLOG += string.Format("A8_DEC = {0}\r\n", A8_DEC.ToString());
                Write_To_Register("a8", DecToHex_2Symbol(A8_DEC)); //Set giá trị thanh ghi A8 = 160 tương đương với mức Pwr 1
                Thread.Sleep(100);
                power_temp = float.Parse(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Power Level = {0}\r\n", power_temp);
                if (power_temp == -1000) return false;

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

            base.Read();
            base.WriteLine("laser txbias --read");
            Thread.Sleep(200);
            string _str = base.Read();
            //System.Windows.MessageBox.Show(_str);
            Ibias = (Convert.ToDouble(_str.Split('=')[1].Replace("uA","").Replace("\r","").Replace("\n","").Replace(">","").Trim())) / 1000;
            _testinfo.SYSTEMLOG += string.Format("Ibias = {0} mA\r\n", Ibias);

            //if (Ibias > 13) Check_Ibias_Result = true;
            //else Check_Ibias_Result = false;

            if (Ibias > 10) Check_Ibias_Result = true;
            else Check_Ibias_Result = false;

            Tuning_Tx_Power_Result = Tuning_Tx_Power_Result && Check_Ibias_Result;
            _testinfo.TUNINGPOWERRESULT = Tuning_Tx_Power_Result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
            return Tuning_Tx_Power_Result;
        }


        /// <summary>
        /// ---------------------------02----------------------------//OK
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

            _testinfo.TUNINGERRESULT = Parameters.testStatus.Wait.ToString();

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

            _testinfo.TUNINGERRESULT = Tuning_ER_Result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
            return Tuning_ER_Result;
        }


        /// <summary>
        /// ---------------------------03----------------------------//
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

            _testinfo.TUNINGCROSSINGRESULT = Parameters.testStatus.Wait.ToString();

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

            _testinfo.TUNINGCROSSINGRESULT = Tuning_Crossing_Result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
            return Tuning_Crossing_Result;
        }


        /// <summary>
        /// ---------------------------04----------------------------//
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
            _testinfo.TXDDMIRESULT = Parameters.testStatus.Wait.ToString();

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

            _testinfo.TXDDMIRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
            return _result;
        }


        /// <summary>
        /// ---------------------------04----------------------------//
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
                    TX1_Power = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(Port)).ToString();


                    // Phát điểm công suất 2:point 2 - A8 = BE 
                    _testinfo.SYSTEMLOG += "Set Point 1: Reg A8 = " + start_point + " -> HEX\r\n";
                    Write_To_Register("a8", DecToHex_2Symbol(start_point));
                    Thread.Sleep(delay);

                    //Đọc thanh ghi EE, EF -> chuyển sang Decimal
                    ADC_TX_Point2 = Read_ADC_value_Tx_Power_dec("ee"); ;
                    TX2_Power = BaseFunctions.convert_dBm_To_uW(GlobalData.powerDevice.getPower_dBm(Port)).ToString();

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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool outTXPower() {
            try {
                base.WriteLine("sh");
                Thread.Sleep(100);
                base.WriteLine("bs /misc prbs gpon 23 0");
                Thread.Sleep(100);
                base.WriteLine("exit");
                Thread.Sleep(100);
                return true;
            }
            catch {
                return false;
            }
        }

        #endregion

        #region GW040H
        public override bool verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool writeMAC(testinginfo _testinfo) {
            throw new NotImplementedException();
        }


        public override bool signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }
        #endregion

    }
}
