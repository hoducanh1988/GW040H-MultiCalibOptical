using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Instrument {
    public class IQS9100B : Function.Protocol.Telnet, IInstrument {

        string linsPort = "";
        private Object thisLock = new Object();

        public IQS9100B(string _host, int _port) : base(_host, _port) {
            this.linsPort = string.Format("LINS1{0}", GlobalData.initSetting.SWINSTRPORT);
        }

        /// <summary>
        /// KẾT NỐI TELNET TỚI MÁY ĐO IQS610P - MODULE IQS9100B 
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
        /// ĐÓNG KẾT NỐI TELNET TỚI MÁY ĐO IQS610P - MODULE IQS9100B
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
            }
            catch {
                if (count <= 3) goto REP;
                else return false;
            }
        }

        /// <summary>
        /// KHỞI TẠO MÁY ĐO VỀ CHẾ ĐỘ MẶC ĐỊNH / ĐÓNG TẤT CẢ CÁC PORT
        /// </summary>
        /// <returns></returns>
        public bool Initialize() {
            if (!base.IsConnected) return false;
            return this._sendCommand(string.Format("{0}:ROUT:CLOSE", this.linsPort));

        }

        /// <summary>
        /// ĐIỀU KHIỂN MÁY ĐO CHUYỂN MẠCH KẾT NỐI VÀO CỔNG CHỈ ĐỊNH
        /// </summary>
        /// <param name="_port"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool switchToPort(int _port) {
            lock (thisLock) {
                if (!base.IsConnected) return false;
                bool ret1 = this._sendCommand(string.Format("{0}:ROUT:SCAN {1}", this.linsPort, _port));
                bool ret2 = this._sendCommand(string.Format("{0}:ROUT:OPEN", this.linsPort));
                return ret1 && ret2;
            }
        }

    }
}
