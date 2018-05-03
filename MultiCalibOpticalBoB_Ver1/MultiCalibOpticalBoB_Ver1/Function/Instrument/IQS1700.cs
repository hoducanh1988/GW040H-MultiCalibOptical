using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Instrument
{
    public class IQS1700 : Function.Protocol.Telnet, IInstrument {

        #region Reference
        struct powerUnits {
            public const string _dbm = "DBM";
            public const string _watt = "WATT";
        }
        struct waveLengths {
            public const double _850 = 850.00;
            public const double _980 = 980.00;
            public const double _1300 = 1300.00;
            public const double _1310 = 1310.00;
            public const double _1480 = 1480.00;
            public const double _1550 = 1550.00;

        }
        struct Rates {
            public const double _0f25 = 0.25;
            public const double _0f33 = 0.33;
            public const double _0f50 = 0.50;
            public const double _1f00 = 1.00;
            public const double _2f00 = 2.00;
            public const double _4f00 = 4.00;
            public const double _8f00 = 8.00;
            public const double _20f03 = 20.03;
            public const double _127f02 = 127.02;
            public const double _248f00 = 248.00;
            public const double _520f80 = 520.80;
            public const double _1041f60 = 1041.60;
            public const double _1302f00 = 1302.00;
            public const double _1736f00 = 1736.00;
            public const double _2604f00 = 2604.00;
            public const double _5208f00 = 5208.00;
        }
        struct Averagings {
            public const int OFF = 0;
            public const int ON = 1;
        }
        struct Absolutes {
            public const int dbm_watt = 0;
            public const int db_ww = 1;
        }
        struct AutoRanges {
            public const int Enable = 1;
            public const int Disable = 0;
        }
        #endregion

        string linsPort = "";
        private Object thisLock = new Object();

        public IQS1700(string _host, int _port) : base(_host, _port) {
            this.linsPort = string.Format("LINS1{0}", GlobalData.initSetting.PWINSTRPORT);
        }

        /// <summary>
        /// KẾT NỐI TELNET TỚI MÁY ĐO IQS610P - MODULE IQS1700
        /// </summary>
        /// <param name="messsage"></param>
        /// <returns></returns>
        public bool Open(out string messsage) {
            messsage = "";
            if (base.Connection() == false) return false;
            int counter = 0;
            REP:
            counter++;
            base.WriteLine(string.Format("CLOSE {0}", this.linsPort));
            Thread.Sleep(100);
            base.WriteLine(string.Format("CONNECT {0}", this.linsPort));
            Thread.Sleep(100);
            messsage += base.Read();
            if (messsage == null) {
                if (counter <= 3) goto REP;
                else return false;
            }
            if (messsage.Contains(string.Format("connected to Module at {0} now.", linsPort)) == true) return true;
            else {
                if (counter <= 3) goto REP;
                else return false;
            }
        }

        /// <summary>
        /// ĐÓNG KẾT NỐI TELNET TỚI MÁY ĐO IQS610P - MODULE IQS1700
        /// </summary>
        /// <returns></returns>
        bool IInstrument.Close() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SEND ONE COMMAND TO INSTRUMENT AND CONFIRM OK/NG => NG RETRY 3 TIMES
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        bool _sendCommand(string cmd) {
            int count = 0;
            REP:
            count++;
            try {
                base.WriteLine(cmd);
                Thread.Sleep(100);
                string _data = base.Read();
                if (_data.Contains("executed successfully.") == false) {
                    if (count <= 3) goto REP;
                    else return false;
                }
                return true;
            } catch {
                if (count <= 3) goto REP;
                else return false;
            }
        }

        /// <summary>
        /// KHỞI TẠO MÁY ĐO VỀ CHẾ ĐỘ MẶC ĐỊNH / THEO TỪNG CHANNEL
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private bool _initByChannel(int channel) {
            if (base.IsConnected == false) return false;
            //Absolute = dBm/Watt
            bool ret1 = this._sendCommand(string.Format("{0}:SENS{1}:POW:REF:STAT {2}", this.linsPort, channel, Absolutes.dbm_watt));
            //Power unit = dBm
            bool ret2 = this._sendCommand(string.Format("{0}:UNIT{1}:POW {2}", this.linsPort, channel, powerUnits._dbm));
            //Wave length = 1310 nm
            bool ret3 = this._sendCommand(string.Format("{0}:SENS{1}:POW:WAV {2} nm", this.linsPort, channel, waveLengths._1310));
            //Rate sampling = 5208
            bool ret4 = this._sendCommand(string.Format("{0}:SENS{1}:FREQ:CONT {2}", this.linsPort, channel, Rates._5208f00));
            //Averaging = ON, value =3
            bool ret5 = this._sendCommand(string.Format("{0}:SENS{1}:AVER:STAT {2}", this.linsPort, channel, Averagings.ON)) &&
                        this._sendCommand(string.Format("{0}:SENS{1}:AVER:COUN {2}", this.linsPort, channel, 3));
            //Auto range = Enable
            bool ret6 = this._sendCommand(string.Format("{0}:SENS{1}:POW:RANG:AUTO {2}", this.linsPort, channel, AutoRanges.Enable));
            return ret1 && ret2 && ret3 && ret4 && ret5 && ret6;
        }

        /// <summary>
        /// KHỞI TẠO MÁY ĐO VỀ CHẾ ĐỘ MẶC ĐỊNH / TẤT CẢ CÁC CHANNEL
        /// </summary>
        /// <returns></returns>
        public bool Initialize() {
            if (!base.IsConnected) return false;
            bool ret = true;
            for(int i = 1; i <= 4; i++) {
                if (!_initByChannel(i)) ret = false;
            }
            return ret;
        }

        /// <summary>
        /// ĐỌC GIÁ TRỊ POWER TỪ MÁY ĐO (dBm) - CHUYỂN KẾT QUẢ TRẢ VỀ TỪ NRZ3 => Double
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string getPower_dBm(int channel) {
            lock (thisLock) {
                if (base.IsConnected == false) return "-1000";
                try {
                    int count = 0;
                    REP:
                    count++;
                    base.WriteLine(string.Format("{0}:READ{1}:POW:DC?", this.linsPort, channel));
                    Thread.Sleep(100);
                    string readStr = base.Read();
                    if (readStr.Contains("Underrange")) {
                        if (count <= 3) { Thread.Sleep(100); goto REP; }
                        else return "-1000";
                    }
                    string[] buffer = readStr.Split('\r');
                    readStr = buffer[0];
                    double _value = double.Parse(readStr);
                    double _result = _value + double.Parse(GlobalData.initSetting.POWERCABLEATTENUATION);
                    if (_result < -30) {
                        if (count <= 3) { Thread.Sleep(100); goto REP; }
                    }
                    return _result.ToString();
                }
                catch {
                    return "-1000";
                }
            }
        }

        public double getPower_mW(int channel) {
            lock (thisLock) {
                if (base.IsConnected == false) return double.MinValue;
                try {
                    this._sendCommand(string.Format("{0}:UNIT{1}:POW {2}", this.linsPort, channel, powerUnits._watt));
                    base.WriteLine(string.Format("{0}:READ{1}:POW:DC?", this.linsPort, channel));
                    //write some code here
                    return 0;
                }
                catch {
                    return double.MinValue;
                }
            }
        }

    }
}
