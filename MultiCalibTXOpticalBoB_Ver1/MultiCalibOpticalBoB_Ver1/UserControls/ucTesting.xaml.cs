using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;
using MultiCalibOpticalBoB_Ver1.Function;
using MultiCalibOpticalBoB_Ver1.Function.Instrument;
using MultiCalibOpticalBoB_Ver1.Function.IO;
using MultiCalibOpticalBoB_Ver1.Function.Ont;
using MultiCalibOpticalBoB_Ver1.Function.Protocol;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucTesting.xaml
    /// </summary>
    public partial class ucTesting : UserControl {

        DispatcherTimer timer = null;

        public ucTesting() {
            InitializeComponent();
            //Display comport
            BaseFunctions.display_Port_Name();
            //Binding data
            this.border_DUT01.DataContext = GlobalData.testingDataDut1;
            this.border_DUT02.DataContext = GlobalData.testingDataDut2;
            this.border_DUT03.DataContext = GlobalData.testingDataDut3;
            this.border_DUT04.DataContext = GlobalData.testingDataDut4;

            //this._loadSetting(); //Load setting
            BaseFunctions.connect_Instrument(); //Connect Instrument

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += ((sender, e) => {
                if (GlobalData.testingDataDut1.BUTTONENABLE == false) {
                    this._dut1scrollViewer.ScrollToEnd();
                }
                if (GlobalData.testingDataDut2.BUTTONENABLE == false) {
                    this._dut2scrollViewer.ScrollToEnd();
                }
                if (GlobalData.testingDataDut3.BUTTONENABLE == false) {
                    this._dut3scrollViewer.ScrollToEnd();
                }
                if (GlobalData.testingDataDut4.BUTTONENABLE == false) {
                    this._dut4scrollViewer.ScrollToEnd();
                }
            });
            timer.Start();
        }

        #region Sub Function 
        private Object thisLock = new Object();


        bool _addToListSequenceTestER(testinginfo _testinfo) {
            lock (thisLock) {
                _testinfo.SYSTEMLOG += string.Format("Add index {0} to list sequence test ER...\r\n", _testinfo.ONTINDEX);
                GlobalData.listSequenceTestER.Add(_testinfo.ONTINDEX);
                string data = "";
                foreach (var item in GlobalData.listSequenceTestER) {
                    data += item + ",";
                }
                _testinfo.SYSTEMLOG += string.Format("...{0}\r\n", data);
                return true;
            }
        }

        bool _waitForTurn(testinginfo _testinfo) {
            bool _flag = false;
            int timeout = 900; //timeout =90s
            int count = 0;
            _testinfo.SYSTEMLOG += "Wait for turning ER...\r\n";
            while (!_flag) {
                string _tmp = "";
                lock (thisLock) { _tmp = GlobalData.listSequenceTestER[0]; }
                if (_tmp == _testinfo.ONTINDEX) _flag = true;
                Thread.Sleep(100);
                count++;
                if (count > timeout) break;
            }
            _testinfo.SYSTEMLOG += string.Format("...Waited time:{0} ms\r\n", count * 100);
            if (count > timeout) return false; //Request time out
            return true;
        }

        bool _removeFromListSequenceTestER(testinginfo _testinfo) {
            lock (thisLock) {
                _testinfo.SYSTEMLOG += string.Format("Remove index {0} from list sequence test ER...\r\n", _testinfo.ONTINDEX);
                GlobalData.listSequenceTestER.Remove(_testinfo.ONTINDEX);
                string data = "";
                foreach (var item in GlobalData.listSequenceTestER) {
                    data += item + ",";
                }
                _testinfo.SYSTEMLOG += string.Format("...{0}\r\n", data);
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

        bool _resetDisplay_Auto(string index) {
            switch (index) {
                case "1": {
                        GlobalData.testingDataDut1.Initialization_Auto();
                        break;
                    }
                case "2": {
                        GlobalData.testingDataDut2.Initialization_Auto();
                        break;
                    }
                case "3": {
                        GlobalData.testingDataDut3.Initialization_Auto();
                        break;
                    }
                case "4": {
                        GlobalData.testingDataDut4.Initialization_Auto();
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

        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************
        bool RunAll(testinginfo _testtemp, bosainfo _bosainfo, variables _vari) {
            System.Diagnostics.Stopwatch pt = new System.Diagnostics.Stopwatch();
            pt.Start();
            string _apdTime="", _pwTime = "", _erTime = "", _ddmiTime = "";


            if (!(GlobalData.initSetting.ONTTYPE == "GW020BoB" && GlobalData.initSetting.ONTAPD == "Only Write APD LUT")) {
                //Kiem tra ket noi toi may do Power
                _testtemp.SYSTEMLOG += string.Format("Kiểm tra kết nối tới máy đo EXFO IQS610P {0}...\r\n", GlobalData.initSetting.EXFOIP);
                if (Network.PingNetwork(GlobalData.initSetting.EXFOIP) == false) {
                    _testtemp.SYSTEMLOG += "...Kết quả = FAIL\r\n";
                    GlobalData.connectionManagement.IQS1700STATUS = false;
                    GlobalData.connectionManagement.IQS9100BSTATUS = false;
                    return false;
                }
                _testtemp.SYSTEMLOG += "...Kết quả = PASS\r\n";
            }
            

            //login to ONT
            bool _result = false;
            GW ontDevice = null;
            switch (GlobalData.initSetting.ONTTYPE) {
                case "GW040H": {
                        ontDevice = new GW040H(_testtemp.COMPORT);
                        break;
                    }
                case "GW020BoB": {
                        ontDevice = new GW020BoB(_testtemp.COMPORT);
                        break;
                    }
                default: return false;
            }
            if (ontDevice.loginToONT(_testtemp) == false) goto END;


            //Get MAC Address
            if (!GlobalData.initSetting.ENABLEWRITEMAC) {
                _testtemp.MACADDRESS = ontDevice.getMACAddress(_testtemp);
                if (_testtemp.MACADDRESS == string.Empty) { _testtemp.ERRORCODE = "(Mã Lỗi: COT-GM-0001)"; goto END; }
            }

            //Write APD LUT
            if (GlobalData.initSetting.ENABLEWRITEAPD == true) {
                if (ontDevice.writeAPD(_bosainfo, _testtemp) == false) goto END;
            }
            pt.Stop();
            _apdTime = string.Format("PW time = {0} ms\r\n", pt.ElapsedMilliseconds);
            pt.Reset(); pt.Restart();

            //Calib Power
            if (GlobalData.initSetting.ENABLETUNINGPOWER) {
                if (ontDevice.calibPower(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
            }

            pt.Stop();
            _pwTime = string.Format("PW time = {0} ms\r\n", pt.ElapsedMilliseconds);

            //Calib ER
            if (GlobalData.initSetting.ENABLETUNINGER || GlobalData.initSetting.ENABLETUNINGCROSSING) {
                pt.Reset();
                pt.Restart();

                //Đăng kí thứ tự Calib ER
                if (this._addToListSequenceTestER(_testtemp) == false) goto END;

                //Chờ đến lượt timeout 90s
                if (this._waitForTurn(_testtemp) == false) goto END;

                //Kiem tra ket noi toi may do DCA
                _testtemp.SYSTEMLOG += string.Format("Kiểm tra kết nối tới máy đo DCA X86100D {0}...\r\n", GlobalData.initSetting.ERINSTRGPIB);
                if (GlobalData.erDevice.isConnected() == false) {
                    _testtemp.SYSTEMLOG += "...Kết quả = FAIL\r\n";
                    GlobalData.connectionManagement.DCAX86100DSTATUS = false;
                    return false;
                }
                _testtemp.SYSTEMLOG += "...Kết quả = PASS\r\n";

                //Calib Dark level
                _testtemp.SYSTEMLOG += string.Format("Switching port...{0} OFF\r\n", _testtemp.ONTINDEX);
                GlobalData.switchDevice.switchOff();
                Thread.Sleep(500);
                GlobalData.erDevice.Calibrate();
                Thread.Sleep(500);

                //Switch Port check ER
                _testtemp.SYSTEMLOG += string.Format("Switching port...{0} ON\r\n", _testtemp.ONTINDEX);
                if (GlobalData.switchDevice.switchToPort(int.Parse(_testtemp.ONTINDEX)) == false) goto END;

                //Calib ER
                if (GlobalData.initSetting.ENABLETUNINGER) {
                    if (ontDevice.calibER(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
                }
                
                //Calib Crossing
                if (GlobalData.initSetting.ENABLETUNINGCROSSING && GlobalData.initSetting.ONTTYPE == "GW020BoB") {
                    if (ontDevice.calibCrossing(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
                }

                //Xóa thứ tự đăng kí Calib ER (để Thread # có thể sử dụng)
                this._removeFromListSequenceTestER(_testtemp);

                pt.Stop();
                _erTime = string.Format("ER time = {0} ms\r\n", pt.ElapsedMilliseconds);
            }

            //TX DDMI
            pt.Reset(); pt.Restart();
            if (GlobalData.initSetting.ENABLETXDDMI) {
                if (ontDevice.txDDMI(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
            }

            //Signal Off
            if (GlobalData.initSetting.ENABLESIGNALOFF && GlobalData.initSetting.ONTTYPE == "GW040H") {
                if (ontDevice.signalOff(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
            }

            //Write flash
            if (GlobalData.initSetting.ENABLEWRITEFLASH && GlobalData.initSetting.ONTTYPE == "GW040H") {
                if (ontDevice.writeFlash(_bosainfo, _testtemp) == false) goto END;
            }

            //Verify Signal
            if (GlobalData.initSetting.ENABLEVERIFYSIGNAL && GlobalData.initSetting.ONTTYPE == "GW040H") {
                if (ontDevice.verifySignal(int.Parse(_testtemp.ONTINDEX), _bosainfo, _testtemp, _vari) == false) goto END;
            }

            //Write MAC
            if (GlobalData.initSetting.ENABLEWRITEMAC) {
                if (ontDevice.writeMAC(_testtemp) == false) goto END;
            }

            pt.Stop();
            _ddmiTime = string.Format("DDMI,SIGOFF,WRITE FLASH time = {0} ms\r\n", pt.ElapsedMilliseconds);

            _result = true;

            END:
            _testtemp.SYSTEMLOG += _pwTime;
            _testtemp.SYSTEMLOG += _erTime;
            _testtemp.SYSTEMLOG += _ddmiTime;
            this._removeFromListSequenceTestER(_testtemp);
            try { ontDevice.Close(); } catch { }
            return _result;
        }

        #endregion


        private void Button_Click(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            string buttonName = b.Name;
            string _index = buttonName.Substring(buttonName.Length - 1, 1);
            this._resetDisplay(_index); //manual

            if (GlobalData.initSetting.ONTTYPE == "GW040H" || (GlobalData.initSetting.ONTTYPE == "GW020BoB" && GlobalData.initSetting.ENABLEWRITEAPD == true)) {
                this.Opacity = 0.3;
                wBosaSerialNumber wb = new wBosaSerialNumber(_index);
                wb.ShowDialog();
                this.Opacity = 1;
            }

            //Kiểm tra trạng thái calib của máy đo ER
            try {
                double _temp = 0, _hours;
                string _time;
                bool ret;
                ret = Function.IO.CalibrationModuleTime.Read(out _time);
                //ret = GlobalData.erDevice.getTemperature(out _temp);
                ret = BaseFunctions.last_Time_Calibrate_Module_DCAX86100D_To_Hours(_time, out _hours);
                if (_hours > 5 || _temp > 5) {
                    CalibModuleWarning cm = new CalibModuleWarning(_time.ToString(), _temp.ToString());
                    cm.ShowDialog();
                }
            }
             catch { }
           
            //***BEGIN -----------------------------------------//
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                //Start count time
                System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                st.Start();

                string _name = buttonName;
                testinginfo testtmp = null;
                BaseFunctions.get_Testing_Info_By_Name(_name, ref testtmp);
                variables vari = new variables();

                string _BosaSN = "";
                if (GlobalData.initSetting.ONTTYPE == "GW040H" || (GlobalData.initSetting.ONTTYPE == "GW020BoB" && GlobalData.initSetting.ENABLEWRITEAPD == true)) {
                    testtmp.SYSTEMLOG += string.Format("Input Bosa Serial...\r\n...{0}\r\n", testtmp.BOSASERIAL);
                    _BosaSN = testtmp.BOSASERIAL;
                    if (_BosaSN == "--") return;
                }
                
                if (GlobalData.initSetting.ENABLEWRITEMAC) { if (testtmp.MACADDRESS == "--") return; }

                //Get Bosa Information from Bosa Serial
                bosainfo bosaInfo = new bosainfo();

                if (GlobalData.initSetting.ONTTYPE == "GW040H" || (GlobalData.initSetting.ONTTYPE == "GW020BoB" && GlobalData.initSetting.ENABLEWRITEAPD == true)) {
                    testtmp.SYSTEMLOG += string.Format("Get Bosa information...\r\n");
                    bosaInfo = this._getDataByBosaSN(_BosaSN);
                    if (bosaInfo == null) {
                        testtmp.ERRORCODE = "(Mã Lỗi: COT-BS-0001)";
                        testtmp.SYSTEMLOG += string.Format("...FAIL. {0}. Bosa SN is not existed\r\n", testtmp.ERRORCODE);
                        testtmp.TOTALRESULT = Parameters.testStatus.FAIL.ToString();
                        goto END;
                    }
                    testtmp.SYSTEMLOG += string.Format("...Ith= {0}mA\r\n", bosaInfo.Ith);
                    testtmp.SYSTEMLOG += string.Format("...Vbr= {0}V\r\n", bosaInfo.Vbr);
                    testtmp.SYSTEMLOG += string.Format("...PASS\r\n");
                }

                //Calib
                testtmp.TOTALRESULT = Parameters.testStatus.Wait.ToString();
                testtmp.BUTTONCONTENT = "STOP"; testtmp.BUTTONENABLE = false;
                bool _result = RunAll(testtmp, bosaInfo, vari);

                testtmp.TOTALRESULT = _result == false ? Parameters.testStatus.FAIL.ToString() : Parameters.testStatus.PASS.ToString();

                END:
                testtmp.SYSTEMLOG += string.Format("\r\n----------------------------\r\nTotal Judged={0}\r\n", testtmp.TOTALRESULT);
                testtmp.BUTTONCONTENT = "START"; testtmp.BUTTONENABLE = true;

                //Stop count time
                st.Stop();
                testtmp.SYSTEMLOG += string.Format("Total time = {0} seconds\r\n", st.ElapsedMilliseconds / 1000);
                testtmp.TOTALTIME += (st.ElapsedMilliseconds / 1000).ToString();

                //save log
                Function.IO.LogDetail.Save(testtmp);
                Function.IO.LogTest.Save(testtmp);
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            }));
            t.IsBackground = true;
            t.Start();
            //***END -------------------------------------------//
        }

    }
}
