using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace MultiCalibOpticalBoB_Ver1.Function.Protocol {
    public class Serial {

        SerialPort _serialport = null;
        string _PortName = "";

        /// <summary>
        /// CONSTRUCTOR KHỞI TẠO CLASS
        /// </summary>
        /// <param name="_portname"></param>
        public Serial(string _portname) {
            this._PortName = _portname;
        }

        /// <summary>
        /// MỞ KẾT NỐI CỔNG COM PORT / RETRY 3 LẦN
        /// </summary>
        /// <returns></returns>
        public bool Open(out string _message) {
            _message = "";
            int count = 0;
            bool result = false;
            REP:
            count++;
            try {
                this._serialport = new SerialPort();
                this._serialport.PortName = _PortName;
                this._serialport.BaudRate = 115200;
                this._serialport.DataBits = 8;
                this._serialport.Parity = Parity.None;
                this._serialport.StopBits = StopBits.One;
                this._serialport.Open();
                //_serialport.DataReceived += new SerialDataReceivedEventHandler(serial_OnReceiveData);
                result = _serialport.IsOpen;
            } catch (Exception ex) {
                _message = ex.ToString();
                result = false;
            }
            if (!result) { if (count < 3) { Thread.Sleep(100); goto REP; } }
            return result;
        }

        /// <summary>
        /// ĐÓNG KẾT NỐI CỔNG COM
        /// </summary>
        /// <returns></returns>
        public bool Close() {
            try {
                this._serialport.Close();
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// GHI DỮ LIỆU VÀO CỔNG SERIALPORT
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        public bool Write(string _cmd) {
            try {
                this._serialport.Write(_cmd);
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// GHI DỮ LIỆU VÀO CỔNG SERIALPORT
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        public bool WriteLine(string _cmd) {
            try {
                this._serialport.WriteLine(_cmd);
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// ĐỌC DỮ LIỆU TỪ CỔNG SERIALPORT
        /// </summary>
        /// <returns></returns>
        public string Read() {
            return this._serialport.ReadExisting();
        }

    }
}
