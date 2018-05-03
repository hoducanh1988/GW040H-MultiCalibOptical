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
using MultiCalibOpticalBoB_Ver1.Function.Ont;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucTesting.xaml
    /// </summary>
    public partial class ucTesting : UserControl {

        int Delay_modem = 300;

        public ucTesting() {
            InitializeComponent();
            //Display comport
            BaseFunctions.display_Port_Name();
            //Binding data
            this.border_DUT01.DataContext = GlobalData.testingDataDut1;
            this.border_DUT02.DataContext = GlobalData.testingDataDut2;
            this.border_DUT03.DataContext = GlobalData.testingDataDut3;
            this.border_DUT04.DataContext = GlobalData.testingDataDut4;

            this._loadSetting(); //Load setting
            BaseFunctions.connect_Instrument(); //Connect Instrument
        }

        #region Sub Function 
        private Object thisLock = new Object();

        bool _loadSetting() {
            return true;
        }

        bool _addToListSequenceTestER(string index) {
            lock (thisLock) {
                GlobalData.listSequenceTestER.Add(index);
                return true;
            }
        }
            
        bool _waitForTurn(string index) {
            bool _flag = false;
            int timeout = 300; //timeout =30s
            int count = 0;
            while (_flag) {
                string _tmp = "";
                lock (thisLock) { _tmp = GlobalData.listSequenceTestER[0]; }
                if (_tmp == index) _flag = true;
                Thread.Sleep(100);
                count++;
                if (count > timeout) break;
            }
            if (count > timeout) return false; //Request time out
            return true;
        }
       
        bool _removeFromListSequenceTestER(string index) {
            lock (thisLock) {
                GlobalData.listSequenceTestER.Remove(index);
                return true;
            }
        }

        bool _resetDisplay(string index) {
            switch (index) {
                case "1": {
                        GlobalData.testingDataDut1.Initialization();
                        break;
                    }
                case "2": {
                        GlobalData.testingDataDut2.Initialization();
                        break;
                    }
                case "3": {
                        GlobalData.testingDataDut3.Initialization();
                        break;
                    }
                case "4": {
                        GlobalData.testingDataDut4.Initialization();
                        break;
                    }
            }
            return true;
        }

        bosainfo _getDataByBosaSN(string _bosaSN) {
            if (GlobalData.listBosaInfo.Count == 0) return null;
            bosainfo tmp = null;
            foreach (var item in GlobalData.listBosaInfo) {
                if (item.BosaSN == _bosaSN) {
                    tmp = item;
                    break;
                }
            }
            return tmp;
        }

        // CHỜ ONT KHỞI ĐỘNG XONG
        bool _loginToONT(ref GW ont, string serialport, testinginfo _testinfo) {
            _testinfo.SYSTEMLOG += "Login to ONT...\r\n";
            bool _result = false;
            string _message = "";
            switch (GlobalData.initSetting.ONTTYPE) {
                case "GW040H": {
                        ont = new GW040H(serialport);
                        break;
                    }
                case "GW020BoB": {
                        ont = new GW020BoB(serialport);
                        break;
                    }
                default: return false;
            }
            if (!ont.Open()) return false;
            _result = ont.Login(out _message);
            _testinfo.SYSTEMLOG += _result == true? "PASS\r\n" : "FAIL\r\n";
            return _result;
        }

        string _getMACAddress(GW ont) {
            try {
                ont.Write("ifconfig\n");
                Thread.Sleep(300);
                string _tmpStr = ont.Read();
                _tmpStr = _tmpStr.Replace("\r", "").Replace("\n", "").Trim();
                string[] buffer = _tmpStr.Split(new string[] { "HWaddr" }, StringSplitOptions.None);
                _tmpStr = buffer[1].Trim();
                return _tmpStr.Substring(0,17).Replace(":","");
            } catch {
                return string.Empty;
            }
        }

        bool _calibPower(GW ont, int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "Bắt Đầu Thực Hiện Calib TX...\r\n";
                _testinfo.SYSTEMLOG += string.Format("BOSA Serial Number: {0}\r\n", _testinfo.BOSASERIAL);
                _testinfo.SYSTEMLOG += string.Format("Ith = {0}\r\n", _bosainfo.Ith);
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 1: TUNING POWER\r\n";

                ont.WriteLine("echo set_flash_register_default >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                ont.WriteLine("echo flash_dump >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                ont.WriteLine("echo GPON_Tx_cal_init >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                _var.Ith = double.Parse(_bosainfo.Ith);
                _var.Iav_1 = _var.Ith + 10;
                _var.Iav_1_dac = Math.Round(_var.Iav_1 * 4096 / 90);
                _var.Iav_1_dac_hex = int.Parse(_var.Iav_1_dac.ToString()).ToString("X");

                ont.WriteLine("echo IAV 0x" + _var.Iav_1_dac_hex + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                _var.Pwr_1 = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Pwr_1 = {0}\r\n", _var.Pwr_1);

                if (_var.Pwr_1 <= -8) {
                    _testinfo.SYSTEMLOG += "Tuning Power: FAIL.\r\n";
                    _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.FAIL.ToString();
                    return false;
                }

                _var.Iav_2 = _var.Ith + 15;
                _var.Iav_2_dac = Math.Round(_var.Iav_2 * 4096 / 90);
                _var.Iav_2_dac_hex = int.Parse(_var.Iav_2_dac.ToString()).ToString("X");

                ont.WriteLine("echo IAV 0x" + _var.Iav_2_dac_hex + " >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                _var.Pwr_2 = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += string.Format("Pwr_2 = {0}\r\n",_var.Pwr_2);

                _var.Slope = (_var.Pwr_2 - _var.Pwr_1) / (_var.Iav_2 - _var.Iav_1);
                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _var.Iav = ((2.5 - _var.Pwr_1) / _var.Slope) + _var.Iav_1;

                for (int i = 0; i < 9; i++) {
                    _var.Iav_DAC = (Math.Round(_var.Iav * 4096 / 90)).ToString();
                    _var.Iav_DAC_Hex = int.Parse(_var.Iav_DAC).ToString("X");
 
                    ont.WriteLine("echo IAV 0x" + _var.Iav_DAC_Hex + " >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);
                    _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                    _testinfo.SYSTEMLOG += string.Format("Pwr_temp = {0}\r\n", _var.Pwr_temp.ToString());

                    if (_var.Pwr_temp >= 2 && _var.Pwr_temp <= 3) {
                        _result = true;
                        break;
                    }
                    else {
                        _var.Iav = ((2.5 - _var.Pwr_temp) / _var.Slope) + _var.Iav;
                    }
                }

                _testinfo.SYSTEMLOG += _result == true ? "Tuning Power: PASS.\r\n" : "Tuning Power: FAIL.\r\n";
                _testinfo.TUNINGPOWERRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            } catch {
                _testinfo.TUNINGPOWERRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        bool _calibER(GW ont,int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TUNINGERRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 2: TUNING ER\r\n";

                _var.Imod = ((_var.Pwr_temp + 3) / _var.Slope) + _var.Iav - _var.Ith - 1;
                
                for (int k = 0; k < 9; k++) {
                    _var.Imod_DAC = (Math.Round(_var.Imod * 4096 / 90)).ToString();
                    _var.Imod_DAC_Hex = int.Parse(_var.Imod_DAC).ToString("X");
                    ont.WriteLine("echo IMOD 0x" + _var.Imod_DAC_Hex + " >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem);

                    _var.ER_temp = Convert.ToDouble(GlobalData.erDevice.getER(Port));
                    _testinfo.SYSTEMLOG += string.Format("ER_temp = {0}\r\n", _var.ER_temp);

                    if (!_var.ER_temp.ToString().Contains("E+")) {
                        if (_var.ER_temp < 12 || _var.ER_temp > 13) {
                            double ER_err = _var.ER_temp - 12.5;
                            if (ER_err <= -5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 5;
                            }
                            else if (ER_err > -5 && ER_err <= -4) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 4;
                            }
                            else if (ER_err > -4 && ER_err <= -3) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 3;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -3 && ER_err <= -2.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 2;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -2.5 && ER_err <= -2) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 1.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -2 && ER_err <= -1.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -1.5 && ER_err <= -1) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 0.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err > -1 && ER_err <= -0.5) {
                                _testinfo.SYSTEMLOG += "Cần tăng Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod + 0.5;
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
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 4.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 4 && ER_err < 5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 4;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 3 && ER_err < 4) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 3;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 2.5 && ER_err < 3) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 2;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 2 && ER_err < 2.5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 1.5 && ER_err < 2) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 1 && ER_err < 1.5) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 1;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                            else if (ER_err >= 0.5 && ER_err < 1) {
                                _testinfo.SYSTEMLOG += "Cần giảm Imod.\r\n";
                                _testinfo.SYSTEMLOG += "-----------------\r\n";
                                _var.Imod = _var.Imod - 0.5;
                                //Hienthi.SetText(rtb, "Imod mới = " + Imod);
                            }
                        }
                        else if (_var.ER_temp >= 12 && _var.ER_temp <= 13) {
                            ont.WriteLine("echo set_flash_register 0x00060023 0x64 >/proc/pon_phy/debug"); //Bù ER ở nhiệt độ 45*C
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
                _testinfo.SYSTEMLOG += _result == true ? "Tuning ER: PASS\r\n" : "Tuning ER: FAIL.\r\n";
                _testinfo.TUNINGERRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            } catch {
                _testinfo.TUNINGERRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        bool _txDDMI(GW ont, int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.TXDDMIRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 3: TX DDMI\r\n";

                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += "Pwr_temp = " + _var.Pwr_temp + "\r\n";
                ont.WriteLine("echo set_flash_register_default >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                ont.WriteLine("echo set_flash_register_Tx_data >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                string RR;
                string RR_DAC;
                RR = (Math.Round(Math.Pow(10, (_var.Pwr_temp / 10)) * 100)).ToString();

                RR_DAC = int.Parse(RR).ToString("X");
                ont.WriteLine("echo set_flash_register_DDMI_TxPower 0x00" + RR_DAC + " 0x40 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                string str = ont.Read();
                ont.WriteLine("echo DDMI_check_8472 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);

                str = ont.Read();
                for (int n = 0; n < str.Split('\n').Length; n++) {
                    if (str.Split('\n')[n].Contains("Tx power")) {
                        _var.TX_Power_DDMI = str.Split('\n')[n].Split('=')[1].TrimEnd();
                        _var.TX_Power_DDMI = (10 * Math.Log10(Convert.ToDouble(_var.TX_Power_DDMI) / 10000)).ToString("0.##");
                        _testinfo.SYSTEMLOG +=  "TX_Power_DDMI = " + _var.TX_Power_DDMI.ToString() + " dBm\r\n";

                        if (Convert.ToDouble(_var.TX_Power_DDMI) > (_var.Pwr_temp - 0.5) && Convert.ToDouble(_var.TX_Power_DDMI) < (_var.Pwr_temp + 0.5)) {
                            _result = true;
                        }
                        else {
                            _result = false;
                        }
                    }
                }
                _testinfo.SYSTEMLOG += _result == true ? "Check Power DDMI: PASS.\r\n" : "Check Power DDMI: FAIL.\r\n";
                _testinfo.TXDDMIRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            } catch {
                _testinfo.TXDDMIRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        bool _signalOff(GW ont, int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.SIGNALOFFRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 4: TEST OFF SIGNAL\r\n";
                string str = ont.Read();
                ont.WriteLine("echo dis_pattern >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                str = ont.Read();
                _var.Pwr_temp = Convert.ToDouble(GlobalData.powerDevice.getPower_dBm(Port));
                _testinfo.SYSTEMLOG += "Power_Off = " + _var.Pwr_temp + "\r\n";
                if (_var.Pwr_temp < -25) {
                    _result = true;
                }
                else {
                    _result = false;
                }
                _testinfo.SYSTEMLOG += _result == true ? "TxPower Off: PASS\r\n" : "TxPower Off: FAIL\r\n";
                _testinfo.SIGNALOFFRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch {
                _testinfo.SIGNALOFFRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        bool _writeFlash(GW ont, bosainfo _bosainfo, testinginfo _testinfo) {
            try {
                bool _result = false;
                _testinfo.WRITEFLASHRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 5: WRITE INTO FLASH\r\n";

                ont.WriteLine("echo set_flash_register_Tx_data >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                ont.WriteLine("echo set_flash_register 0x07050701 0x94 >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                ont.WriteLine("echo save_flash_matrix >/proc/pon_phy/debug");
                Thread.Sleep(Delay_modem);
                //P.WriteLine("mtd writeflash /tmp/7570_bob.conf 160 198144 reservearea");//single band
                ont.WriteLine("mtd writeflash /tmp/7570_bob.conf 160 656896 reservearea");//dual band
                Thread.Sleep(Delay_modem + 1000);
                for (int m = 0; m < 3; m++) {
                    string str = ont.Read();
                    ont.WriteLine("echo flash_dump >/proc/pon_phy/debug");
                    Thread.Sleep(Delay_modem * 5);
                    str = ont.Read();
                    _testinfo.SYSTEMLOG += str + "\r\n";
                    if (!str.Contains("0x07050701")) {
                        _result = false;
                    }
                    else {
                        _result = true;
                        break;
                    }
                }
                _testinfo.SYSTEMLOG += _result == true ? "Write flash thành công.\r\n" : "Write flash thất bại.\r\n";
                _testinfo.SYSTEMLOG += "Hoàn thành quá trình Calibration.\r\n";
                _testinfo.WRITEFLASHRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch {
                _testinfo.WRITEFLASHRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }

        bool _verifySignal(GW ont, int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            try {
                bool _result = false;
                _testinfo.VERIFYSIGNALRESULT = Parameters.testStatus.Wait.ToString();
                _testinfo.SYSTEMLOG += "--------------------------------------------------------------\r\n";
                _testinfo.SYSTEMLOG += "STEP 6: VERIFY SIGNAL\r\n";

                ont.WriteLine("echo GPON_pattern >/proc/pon_phy/debug");
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

                _testinfo.SYSTEMLOG += _result == true ? "Verify Signal: PASS.\r\n" : "Verify Signal: FAIL.\r\n";
                _testinfo.VERIFYSIGNALRESULT = _result == true ? Parameters.testStatus.PASS.ToString() : Parameters.testStatus.FAIL.ToString();
                return _result;
            }
            catch {
                _testinfo.VERIFYSIGNALRESULT = Parameters.testStatus.FAIL.ToString();
                return false;
            }
        }


        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************
        bool RunAll(testinginfo _testtemp, bosainfo _bosainfo, variables _vari) {
            //login to ONT
            GW ontDevice = null;
            if (this._loginToONT(ref ontDevice, _testtemp.COMPORT, _testtemp) == false) return false;

            //Get MAC Address
            _testtemp.MACADDRESS = this._getMACAddress(ontDevice);
            if (_testtemp.MACADDRESS == string.Empty) return false;

            //Calib Power
            if (GlobalData.initSetting.ENABLETUNINGPOWER) {
                if (this._calibPower(ontDevice, int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) return false;
            }

            //Calib ER
            if (GlobalData.initSetting.ENABLETUNINGER) {
                System.Diagnostics.Stopwatch et = new System.Diagnostics.Stopwatch();
                et.Start();

                //Đăng kí thứ tự Calib ER
                if (this._addToListSequenceTestER(_testtemp.ONTINDEX) == false) return false;
                //Chờ đến lượt timeout 30s
                if (this._waitForTurn(_testtemp.ONTINDEX) == false) {
                    this._removeFromListSequenceTestER(_testtemp.ONTINDEX);
                    return false;
                }
                //Switch Port check ER
                if (GlobalData.switchDevice.switchToPort(int.Parse(_testtemp.ONTINDEX)) == false) return false;
                //Calib ER
                if (this._calibER(ontDevice, int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) return false;
                //Xóa thứ tự đăng kí Calib ER (để Thread # có thể sử dụng)
                this._removeFromListSequenceTestER(_testtemp.ONTINDEX);

                et.Stop();
                _testtemp.SYSTEMLOG += string.Format("ER time = {0} sec\r\n", et.ElapsedMilliseconds / 1000);
            }

            //TX DDMI
            if (GlobalData.initSetting.ENABLETXDDMI) {
                if (this._txDDMI(ontDevice, int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) return false;
            }

            //Signal Off
            if (GlobalData.initSetting.ENABLESIGNALOFF) {
                if (this._signalOff(ontDevice, int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) return false;
            }

            //Write flash
            if (GlobalData.initSetting.ENABLEWRITEFLASH) {
                if (this._writeFlash(ontDevice, _bosainfo, _testtemp) == false) return false;
            }

            //Verify Signal
            if (GlobalData.initSetting.ENABLEVERIFYSIGNAL) {
                if (this._verifySignal(ontDevice, int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) return false;
            }

            return true;
        }

        #endregion


        private void Button_Click(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            string buttonName = b.Name;
            string _index = buttonName.Substring(buttonName.Length - 1, 1);
            this._resetDisplay(_index);
            this.Opacity = 0.3;
            wBosaSerialNumber wb = new wBosaSerialNumber(_index);
            wb.ShowDialog();
            this.Opacity = 1;

            //***BEGIN -----------------------------------------//
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                //Start count time
                System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                st.Start();

                string _name = buttonName;
                testinginfo testtmp = null;
                BaseFunctions.get_Testing_Info_By_Name(_name, ref testtmp);
                variables vari = new variables();

                string _BosaSN = testtmp.BOSASERIAL;
                if (_BosaSN == "--") return;
                //Get Bosa Information from Bosa Serial
                bosainfo bosaInfo = new bosainfo();
                //bosaInfo = GlobalData.sqlServer.getDataByBosaSN(_BosaSN);
                bosaInfo = this._getDataByBosaSN(_BosaSN);
                if (bosaInfo == null) return;
                //Calib
                testtmp.TOTALRESULT = Parameters.testStatus.Wait.ToString();
                testtmp.BUTTONCONTENT = "STOP"; testtmp.BUTTONENABLE = false;
                testtmp.TOTALRESULT = RunAll(testtmp, bosaInfo, vari) == false ? Parameters.testStatus.FAIL.ToString() : Parameters.testStatus.PASS.ToString();
                testtmp.BUTTONCONTENT = "START"; testtmp.BUTTONENABLE = true;
                //Stop count time
                st.Stop();
                testtmp.SYSTEMLOG += string.Format("Total time = {0} seconds\r\n", st.ElapsedMilliseconds / 1000);
            }));
            t.IsBackground = true;
            t.Start();
            //***END -------------------------------------------//
        }

    }
}
