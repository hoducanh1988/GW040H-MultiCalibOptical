using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont
{
    public class GW040H : GW
    {
        public GW040H(string _portname) : base(_portname) { }

        public override bool Login(out string message) {
            message = "";
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while(!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    message += "Gửi lệnh Enter để truy nhập vào login...\r\n";
                    base.WriteLine("\r\n");
                    Thread.Sleep(250);
                    string data = "";
                    data = base.Read();
                    message += string.Format("Feedback:=> {0}\r\n", data);
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains("#")) return true;
                    while(!data.Contains("tc login:")) {
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

        public override bool outTXPower() {
            try {
                base.WriteLine("echo GPON_pattern >/proc/pon_phy/debug");
                return true;
            } catch {
                return false;
            }
        }

        //Calib Quang------------------------------//
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

        public override bool calibER(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TUNINGERRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 2: TUNING ER\r\n";

                _var.Imod = ((_var.Pwr_temp + 3) / _var.Slope) + _var.Iav - _var.Ith - 1;

                for (int k = 0; k < 12; k++) {
                    _var.Imod_DAC = (Math.Round(_var.Imod * 4096 / 90)).ToString();
                    _var.Imod_DAC_Hex = int.Parse(_var.Imod_DAC).ToString("X");
                    base.WriteLine("echo IMOD 0x" + _var.Imod_DAC_Hex + " >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);

                    _var.ER_temp = Convert.ToDouble(GlobalData.erDevice.getER(Port));
                    _testinfo.SYSTEMLOG += string.Format("ER_temp = {0}\r\n", _var.ER_temp);
                    _testinfo.SYSTEMLOG += string.Format("Imod = {0}\r\n", _var.Imod);

                    if (!_var.ER_temp.ToString().Contains("E+")) {
                        if (_var.ER_temp < 12 || _var.ER_temp > 13) {
                            double ER_err = _var.ER_temp - 12.5;
                            if (ER_err <= -5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +12.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 12;
                            }
                            else if (ER_err > -5 && ER_err <= -4) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +10.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 10;
                            }
                            else if (ER_err > -4 && ER_err <= -3) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +7.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 7;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -3 && ER_err <= -2.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +5.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -2.5 && ER_err <= -2) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +3.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 3;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -2 && ER_err <= -1.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +2.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 2;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -1.5 && ER_err <= -1) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +1.5.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 1.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -1 && ER_err <= -0.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod thêm +1.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            //else if (ER_err >= 0.5)
                            //{
                            //    SetText(Tx_rtbAll, "Cần tăng Imod.");
                            //    Imod = Imod + 0.5;
                            //    SetText(Tx_rtbAll, "Imod mới = " + Imod);
                            //}

                            //------------------------------------------
                            if (ER_err >= 5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -4.5.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 4.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 4 && ER_err < 5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -4.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 4;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 3 && ER_err < 4) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -3.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 3;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 2.5 && ER_err < 3) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -2.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 2;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 2 && ER_err < 2.5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -1.5.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 1.5 && ER_err < 2) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -1.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 1 && ER_err < 1.5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -1.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 0.5 && ER_err < 1) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod -0.5.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 0.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                        }
                        else if (_var.ER_temp >= 12 && _var.ER_temp <= 13) {
                            base.WriteLine("echo set_flash_register 0x00060023 0x64 >/proc/pon_phy/debug"); //Bù ER ở nhiệt độ 45*C
                            Thread.Sleep(Delay_modem);
                            _result = true;
                            break;
                        }

                    }
                    else if (_var.ER_temp.ToString().Contains("E+")) {
                        _result = false;
                        break;
                    }
                }
                
                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-ER-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "Tuning ER: PASS\r\n" : string.Format("Tuning ER: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.TUNINGERRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-ER-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.TUNINGERRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override bool calibPower(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "Bắt Đầu Thực Hiện Calib TX...\r\n";
                _testinfo.SYSTEMLOG += string.Format("BOSA Serial Number: {0}\r\n", _testinfo.BOSASERIAL);
                _testinfo.SYSTEMLOG += string.Format("Ith = {0}\r\n", _bosainfo.Ith);
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 1: TUNING POWER\r\n";

                base.WriteLine("echo set_flash_register_default >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo flash_dump >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo GPON_Tx_cal_init >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                _var.Ith = double.Parse(_bosainfo.Ith);
                _var.Iav_1 = _var.Ith + 10;
                _var.Iav_1_dac = Math.Round(_var.Iav_1 * 4096 / 90);
                _var.Iav_1_dac_hex = int.Parse(_var.Iav_1_dac.ToString()).ToString("X");

                _testinfo.SYSTEMLOG += string.Format("echo IAV 0x{0} >/proc/pon_phy/debug\r\n", _var.Iav_1_dac_hex);
                base.WriteLine("echo IAV 0x" + _var.Iav_1_dac_hex + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                _var.Pwr_1 = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Pwr_1 = {0}\r\n", _var.Pwr_1);

                if (_var.Pwr_1 <= -8) {
                    _testinfo.ERRORCODE = "(Mã Lỗi: COT-PW-0001)";
                    _testinfo.SYSTEMLOG += string.Format("Tuning Power: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                    _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.FAIL.ToString();
                    return false;
                }

                _var.Iav_2 = _var.Ith + 15;
                _var.Iav_2_dac = Math.Round(_var.Iav_2 * 4096 / 90);
                _var.Iav_2_dac_hex = int.Parse(_var.Iav_2_dac.ToString()).ToString("X");

                _testinfo.SYSTEMLOG += string.Format("echo IAV 0x{0} >/proc/pon_phy/debug\r\n", _var.Iav_2_dac_hex);
                base.WriteLine("echo IAV 0x" + _var.Iav_2_dac_hex + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
               
                _var.Pwr_2 = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Pwr_2 = {0}\r\n", _var.Pwr_2);

                _var.Slope = (_var.Pwr_2 - _var.Pwr_1) / (_var.Iav_2 - _var.Iav_1);
                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _var.Iav = ((2.5 - _var.Pwr_1) / _var.Slope) + _var.Iav_1;

                for (int i = 0; i < 9; i++) {
                    _var.Iav_DAC = (Math.Round(_var.Iav * 4096 / 90)).ToString();
                    _var.Iav_DAC_Hex = int.Parse(_var.Iav_DAC).ToString("X");

                    base.WriteLine("echo IAV 0x" + _var.Iav_DAC_Hex + " >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                    _testinfo.SYSTEMLOG += string.Format("Pwr_temp = {0}\r\n", _var.Pwr_temp.ToString());

                    if (_var.Pwr_temp >= 2.5 && _var.Pwr_temp <= 3) {
                        _result = true;
                        break;
                    }
                    else {
                        _var.Iav = ((2.8 - _var.Pwr_temp) / _var.Slope) + _var.Iav;
                    }
                }

                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-PW-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "Tuning Power: PASS.\r\n" : string.Format("Tuning Power: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.TUNINGPOWERRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-PW-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override string getMACAddress(testinginfo _testinfo) {
            try {
                _testinfo.SYSTEMLOG += string.Format("Get mac address...\r\n");
                base.Write("ifconfig\n");
                Thread.Sleep(300);
                string _tmpStr = base.Read();
                _tmpStr = _tmpStr.Replace("\r", "").Replace("\n", "").Trim();
                string[] buffer = _tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                _tmpStr = buffer[1].Trim();
                string mac = _tmpStr.Substring(0, 17).Replace(":", "");
                _testinfo.SYSTEMLOG += string.Format("...PASS. {0}\r\n", mac);
                return mac;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-GM-0001)";
                _testinfo.SYSTEMLOG += string.Format("...FAIL. {0}. {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                return string.Empty;
            }
        }

        public override bool signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.SIGNALOFFRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 4: TEST OFF SIGNAL\r\n";
                string str = base.Read();
                base.WriteLine("echo dis_pattern >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                str = base.Read();
                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += "Power_Off = " + _var.Pwr_temp + "\r\n";
                if (_var.Pwr_temp < -25) {
                    _result = true;
                }
                else {
                    _result = false;
                }
                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-SO-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "TxPower Off: PASS\r\n" : string.Format("TxPower Off: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.SIGNALOFFRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-SO-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.SIGNALOFFRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override bool txDDMI(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TXDDMIRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 3: TX DDMI\r\n";

                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += "Pwr_temp = " + _var.Pwr_temp + "\r\n";
                base.WriteLine("echo set_flash_register_default >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo set_flash_register_Tx_data >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                string RR;
                string RR_DAC;
                RR = (Math.Round(Math.Pow(10, (_var.Pwr_temp / 10)) * 100)).ToString();

                RR_DAC = int.Parse(RR).ToString("X");
                base.WriteLine("echo set_flash_register_DDMI_TxPower 0x00" + RR_DAC + " 0x40 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                string str = base.Read();
                base.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                str = base.Read();
                for (int n = 0; n < str.Split('\n').Length; n++) {
                    if (str.Split('\n')[n].Contains("Tx power")) {
                        _var.TX_Power_DDMI = str.Split('\n')[n].Split('=')[1].TrimEnd();
                        _var.TX_Power_DDMI = (10 * Math.Log10(Convert.ToDouble(_var.TX_Power_DDMI) / 10000)).ToString("0.##");
                        _testinfo.SYSTEMLOG += "TX_Power_DDMI = " + _var.TX_Power_DDMI.ToString() + " dBm\r\n";

                        if (Convert.ToDouble(_var.TX_Power_DDMI) > (_var.Pwr_temp - 0.5) && Convert.ToDouble(_var.TX_Power_DDMI) < (_var.Pwr_temp + 0.5)) {
                            _result = true;
                        }
                        else {
                            _result = false;
                        }
                    }
                }
                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-MI-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "Check Power DDMI: PASS.\r\n" : string.Format("Check Power DDMI: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.TXDDMIRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-MI-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.TXDDMIRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override bool verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.VERIFYSIGNALRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 6: VERIFY SIGNAL\r\n";

                base.WriteLine("echo GPON_pattern >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _var.ER_temp = Convert.ToDouble(GlobalData.erDevice.getER(Port));
                _testinfo.SYSTEMLOG += "ER_temp = " + _var.ER_temp + "\r\n";
                _testinfo.SYSTEMLOG += "Power_temp = " + _var.Pwr_temp + "\r\n";

                if (_var.Pwr_temp > 2 && _var.Pwr_temp < 3 && _var.ER_temp > 11.5 && _var.ER_temp < 13.5) {
                    _result = true;
                }
                else {
                    _result = false;
                }
                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-VS-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "Verify Signal: PASS.\r\n" : string.Format("Verify Signal: FAIL. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.VERIFYSIGNALRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-VS-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.VERIFYSIGNALRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            try {
                bool _result = false;
                _testinfo.WRITEFLASHRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 5: WRITE INTO FLASH\r\n";

                base.WriteLine("echo set_flash_register_Tx_data >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo set_flash_register 0x07050701 0x94 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                base.WriteLine("echo save_flash_matrix >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                //P.WriteLine("mtd writeflash /tmp/7570_bob.conf 160 198144 reservearea");//single band
                base.WriteLine("mtd writeflash /tmp/7570_bob.conf 160 656896 reservearea");//dual band
                Thread.Sleep(Delay_modem + 1000);
                for (int m = 0; m < 3; m++) {
                    string str = base.Read();
                    base.WriteLine("echo flash_dump >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 5);
                    str = base.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    if (!str.Contains("0x07050701")) {
                        _result = false;
                    }
                    else {
                        _result = true;
                        break;
                    }
                }
                if (_result == false) _testinfo.ERRORCODE = "(Mã Lỗi: COT-WF-0001)";
                _testinfo.SYSTEMLOG += _result == true ? "Write flash thành công.\r\n" : string.Format("Write flash thất bại. {0}\r\n", _testinfo.ERRORCODE);
                _testinfo.SYSTEMLOG += "Hoàn thành quá trình Calibration.\r\n";
                _testinfo.WRITEFLASHRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-WF-0002)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                _testinfo.WRITEFLASHRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        public override bool writeMAC(testinginfo _testinfo) {
            try {
                //Write GPON
                base.Write(string.Format("prolinecmd gponsn set {0}\n", _testinfo.GPON));
                string st = string.Format("writeflash: total write");
                string _data = ""; int index = 0;

                while (!_data.Contains(st)) {
                    Thread.Sleep(500);
                    if (index >= 6) break;
                    else index++;
                    _data += base.Read();
                }
                if (index >= 6) {
                    _testinfo.ERRORCODE = "(Mã Lỗi: COT-WM-0001)";
                    _testinfo.SYSTEMLOG += string.Format("{0}\r\n", _testinfo.ERRORCODE);
                    return false;
                }
                //Write WPS

                //Write MAC
                _data = ""; index = 0;
                base.Write(string.Format("sys mac {0}\n", _testinfo.MACADDRESS));
                st = string.Format("new mac addr = {0}:{1}:{2}:{3}:{4}:{5}",
                       _testinfo.MACADDRESS.Substring(0, 2).ToLower(),
                       _testinfo.MACADDRESS.Substring(2, 2).ToLower(),
                       _testinfo.MACADDRESS.Substring(4, 2).ToLower(),
                       _testinfo.MACADDRESS.Substring(6, 2).ToLower(),
                       _testinfo.MACADDRESS.Substring(8, 2).ToLower(),
                       _testinfo.MACADDRESS.Substring(10, 2).ToLower()
                       );
                while (!_data.Contains(st)) {
                    Thread.Sleep(500);
                    if (index >= 6) break;
                    else index++;
                    _data += base.Read();
                }
                if (index >= 6) {
                    _testinfo.ERRORCODE = "(Mã Lỗi: COT-WM-0003)";
                    _testinfo.SYSTEMLOG += string.Format("{0}\r\n", _testinfo.ERRORCODE);
                    return false;
                }
                return true;
            }
            catch (Exception ex) {
                _testinfo.ERRORCODE = "(Mã Lỗi: COT-WM-0004)";
                _testinfo.SYSTEMLOG += string.Format("{0}, {1}\r\n", _testinfo.ERRORCODE, ex.ToString());
                return false;
            }
        }

        public override bool calibCrossing(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }
        //Calib Quang------------------------------//
    }
}
