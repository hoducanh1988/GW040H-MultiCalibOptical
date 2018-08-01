using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NationalInstruments.VisaNS;
using Ivi.Visa.Interop;

namespace MultiCalibOpticalBoB_Ver1.Function.Instrument {
    public class DCAX86100D : IInstrument {

        string _visaAddress = "";
        private FormattedIO488 myN1010A = null;
        int Delaytime_short = 300;
        int Delaytime_long = 700;
        private Object thislock = new Object();

        public DCAX86100D(string _visaaddr) {
            this._visaAddress = _visaaddr;
        }

        public bool Close() {
            try {
                myN1010A.IO.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        public bool Initialize() {
            try {
                // query instrument ID
                myN1010A.WriteString("*CLS", true);
                myN1010A.WriteString("*IDN?", true);

                //reset instrument
                myN1010A.WriteString(":SYSTem:DEFault", true);
                myN1010A.WriteString("*OPC?", true);
                string complete = myN1010A.ReadString();
                //
                myN1010A.WriteString(":SYSTem:MODE EYE", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString(":TRIGger:SOURce FPANel", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString(":CHAN1A:FILTer 1", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString(":CHAN1A:WAVelength:VALue 1310E-9", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString(":CHAN1A:ATTenuator:STATe 1", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString("*OPC", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString("*STB?", true);
                Thread.Sleep(Delaytime_short);
                myN1010A.WriteString("*ESE 1", true);
                Thread.Sleep(Delaytime_short);

                return true;
            }
            catch {
                return false;
            }
        }

        public bool Open(out string message) {
            message = "";
            try {
                // create the resource manager
                Ivi.Visa.Interop.ResourceManager mgr = new Ivi.Visa.Interop.ResourceManager();
                // create the formatted io object
                myN1010A = new FormattedIO488Class(); //Nếu bị báo lỗi FormattedIO488Class này thì phải vào Property của Reference Ivi.Visa.Interop -> Set "Embed Interop Type" = False;
                // open IO driver session
                myN1010A.IO = (IMessage)mgr.Open(this._visaAddress);
                //set timeout
                myN1010A.IO.Timeout = 20000;
                //set termination character to CHR(10) (i.e. "\n")
                //enable terminate reads on termination character
                myN1010A.IO.TerminationCharacter = 10;
                myN1010A.IO.TerminationCharacterEnabled = true;
                //
                return true;
            }
            catch {
                return false;
            }
        }

        // Ham kiem tra ket noi toi may do DCA
        public bool isConnected() {
            lock (thislock) {
                bool _result = false;
                int _count = 0;
                //-------------------------------------//
                REP:
                _count++;
                try {
                    myN1010A.WriteString("*IDN?", true);
                    Thread.Sleep(500);
                    string data = myN1010A.ReadString();
                    if (data.Contains("86100D")) {
                        _result = true;
                        goto END;
                    } else {
                        if (_count < 10) goto REP;
                        else goto END;
                    }
                } catch {
                    if (_count < 10) goto REP;
                    else goto END;
                }
                //-------------------------------------//
                END:
                return _result;
            }
        }

        // Hàm đọc ER từ máy DCA
        public string getER(int _port) {
            lock (thislock) {
                try {
                    string _erAtt = "";
                    switch (_port) {
                        case 1: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION1; break; }
                        case 2: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION2; break; }
                        case 3: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION3; break; }
                        case 4: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION4; break; }
                    }
                    myN1010A.WriteString(":CHAN1A:ATTenuator:DECibels " + _erAtt, true);
                    Thread.Sleep(100);
                    myN1010A.WriteString(":SYSTem:AUToscale", true);
                    Thread.Sleep(100);
                    myN1010A.WriteString("*OPC?", true);
                    Thread.Sleep(100);
                    myN1010A.WriteString(":MEASure:EYE:ERATio", true);
                    Thread.Sleep(100);
                    string _txt = "";
                    int _timeout = 0;

                    REP:
                    _timeout++;
                    myN1010A.WriteString(":MEASure:EYE:ERATio:STATus?", true);
                    Thread.Sleep(100);
                    _txt = myN1010A.ReadString();
                    if (!_txt.Contains("CORR")) {
                        if (_timeout >= 300) return string.Empty;
                        else goto REP;
                    }

                    //while (!_txt.Contains("CORR")) {
                    //    _timeout++;
                    //    if (_timeout >= 300) break;
                    //    myN1010A.WriteString(":MEASure:EYE:ERATio:STATus?", true);
                    //    Thread.Sleep(10);
                    //    _txt = myN1010A.ReadString();
                    //}
                    //if (_timeout >= 300) return string.Empty;


                    myN1010A.WriteString(":MEASure:EYE:ERATio?", true);
                    Thread.Sleep(200);
                    string er = myN1010A.ReadString();
                    return er;
                }
                catch {
                    return string.Empty;
                }
            }
        }

        // Hàm đọc Power dBm từ máy đo DCA
        public string getdBm() {
            lock (thislock) {
                try {
                    myN1010A.WriteString(":SYSTem:AUToscale", true);
                    Thread.Sleep(Delaytime_long);
                    myN1010A.WriteString("*OPC?", true);
                    Thread.Sleep(Delaytime_long);

                    myN1010A.WriteString(":MEASure:EYE:APOWer", true);
                    Thread.Sleep(Delaytime_short);
                    myN1010A.WriteString(":MEASure:EYE:APOWer:UNITs DBM", true);
                    Thread.Sleep(Delaytime_short);
                    myN1010A.WriteString(":MEASure:EYE:APOWer?", true);
                    Thread.Sleep(Delaytime_short);
                    return myN1010A.ReadString();
                } catch (Exception error) {
                    return error.Message;
                }
            }
        }


        // Hàm đọc Crossing % từ máy DCA
        public string getCrossing(int _port) {
            lock (thislock) {
                string crossing_point = "";
                try {
                    string _erAtt = "";
                    switch (_port) {
                        case 1: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION1; break; }
                        case 2: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION2; break; }
                        case 3: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION3; break; }
                        case 4: { _erAtt = GlobalData.initSetting.ERCABLEATTENUATION4; break; }
                    }
                    myN1010A.WriteString(":CHAN1A:ATTenuator:DECibels " + _erAtt, true);
                    Thread.Sleep(100);
                    myN1010A.WriteString(":SYSTem:AUToscale", true);
                    Thread.Sleep(100);
                    myN1010A.WriteString("*OPC?", true);
                    Thread.Sleep(100);
                    myN1010A.WriteString(":MEASure:EYE:CROSsing", true);
                    Thread.Sleep(100);
                    string _txt = "";
                    int _timeout = 0;
                    while (!_txt.Contains("CORR")) {
                        _timeout++;
                        if (_timeout >= 300) break;
                        myN1010A.WriteString(":MEASure:EYE:CROSsing:STATus?", true);
                        Thread.Sleep(10);
                        _txt = myN1010A.ReadString();
                    }
                    if (_timeout >= 300) return string.Empty;

                    myN1010A.WriteString(":MEASure:EYE:CROSsing?", true);
                    crossing_point = myN1010A.ReadString();
                    return crossing_point;
                }
                catch (Exception error) {
                    return error.Message;
                }
            }
        }


        // Calib máy đo
        public bool Calibrate() {
            lock (thislock) {
                try {
                    string _data = "";
                    myN1010A.WriteString(":CAL:SLOT1:STAT?", true);
                    Thread.Sleep(100);
                    _data = myN1010A.ReadString();
                    if (_data.Contains("UNCALIBRATED") == true) {
                        myN1010A.WriteString(":CAL:SLOT1:STAR", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString("*OPC?", true);
                    }

                    myN1010A.WriteString(":CAL:DARK:CHAN1A:STAT?", true);
                    Thread.Sleep(100);
                    _data = myN1010A.ReadString();
                    if (_data.Contains("UNCALIBRATED") == true) {
                        myN1010A.WriteString(":CAL:DARK:CHAN1A:STAR", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString("*OPC?", true); 
                    }
                   
                    myN1010A.WriteString(":CAL:DARK:CHAN2A:STAT?", true);
                    Thread.Sleep(100);
                    _data = myN1010A.ReadString();
                    if (_data.Contains("UNCALIBRATED") == true) {
                        myN1010A.WriteString(":CAL:DARK:CHAN2A:STAR", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString(":CAL:SDON?", true);
                        myN1010A.WriteString(":CAL:CONT", true);
                        myN1010A.WriteString("*OPC?", true);
                    }

                    return true;
                }
                catch {
                    return false;
                }
            }
        }

    }
}
