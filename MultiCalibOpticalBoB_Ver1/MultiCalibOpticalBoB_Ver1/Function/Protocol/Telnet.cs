using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Collections;

namespace MultiCalibOpticalBoB_Ver1.Function.Protocol {
    public abstract class Telnet
    {
        public bool IsConnected {
            get { return this.clients.Connected; }
            set { }
        }

        TcpClient clients;
        enum Verbs { WILL = 251, WONT = 252, DO = 253, DONT = 254, IAC = 255 }
        enum Options { SGA = 3 }
        public string host;
        public int port;

        /// <summary>
        /// Constructor with no parameters
        /// </summary>
        public Telnet() { }

        /// <summary>
        /// Constructor with 2 parameters
        /// </summary>
        /// <param name="_host"></param>
        /// <param name="_port"></param>
        /// <param name="_timeout"></param>
        public Telnet(string _host, int _port)
        {
            this.host = _host;
            this.port = _port;
        }

        /// <summary>
        /// Config TCP Clients
        /// </summary>
        private void configTCP()
        {
            // Don't allow another process to bind to this port.
            this.clients.ExclusiveAddressUse = false;
            // sets the amount of time to linger after closing, using the LingerOption public property.
            this.clients.LingerState = new LingerOption(false, 0);
            // Sends data immediately upon calling NetworkStream.Write.
            this.clients.NoDelay = true;
            // Sets the receive buffer size using the ReceiveBufferSize public property.
            this.clients.ReceiveBufferSize = 1024;
            // Sets the receive time out using the ReceiveTimeout public property.
            this.clients.ReceiveTimeout = 5000;
            // Sets the send buffer size using the SendBufferSize public property.
            this.clients.SendBufferSize = 1024;
            // sets the send time out using the SendTimeout public property.
            this.clients.SendTimeout = 5000;
        }

        /// <summary>
        /// Connect to TCP server
        /// </summary>
        /// <returns></returns>
        public bool Connection() {
            this.clients = new TcpClient();
            this.configTCP();
            try {
                this.clients.ConnectAsync(host, port).Wait(1000);
                return this.clients.Connected;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Write data to TCP Server
        /// </summary>
        /// <param name="_cmd"></param>
        public void Write(string _cmd)
        {
            if (!IsConnected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(_cmd.Replace("\0xFF", "\0xFF\0xFF")); //convert string to array[bytes]
            this.clients.GetStream().Write(buf, 0, buf.Length);

        }

        /// <summary>
        /// Write data to TCP Server line by line
        /// </summary>
        /// <param name="_cmd"></param>
        public void WriteLine(string _cmd)
        {
            this.Write(_cmd + "\r\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_listcmd"></param>
        /// <returns></returns>
        public bool sendListCommand(params string[] _listcmd) {
            if (!IsConnected) { return false; }
            try {
                foreach (var _cmd in _listcmd) {
                    this.WriteLine(_cmd);
                }
            }
            catch {
                return false;
            }
            return true;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_listcmd"></param>
        /// <returns></returns>
        public bool sendListCommand(IEnumerable<string> _listcmd) {
            if (!IsConnected) { return false; }
            try {
                foreach (var _cmd in _listcmd) {
                    this.WriteLine(_cmd);
                }
            }
            catch {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Send list of command into DUT
        /// </summary>
        /// <param name="_listcmd"></param>
        /// <returns></returns>
        public bool sendListCommand(IEnumerable<string> _listcmd, out string msg)
        {
            if (!IsConnected) { msg = "Disconnected"; return false; }
            try {
                foreach (var _cmd in _listcmd) {
                    this.WriteLine(_cmd);
                }
                msg = this.Read();
            }
            catch (Exception ex) {
                msg = ex.Message.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Read data from TCP Server
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            if (!IsConnected) return string.Empty;
            while (!this.clients.GetStream().DataAvailable) ; //Đã có dữ liệu trong bộ đệm nhận
            NetworkStream stream = this.clients.GetStream();
            StringBuilder sb = new StringBuilder();
            int input = 0;
            do {
                input = this.clients.GetStream().ReadByte();
                switch (input) {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = stream.ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb) {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = stream.ReadByte();
                                if (inputoption == -1) break;
                                stream.WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    stream.WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    stream.WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                stream.WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
                if (this.clients.Available == 0 && input == 32) break;
            } while (input > 0);
            return sb.ToString();
        }

        public string Read0() {
            if (!this.clients.Connected) return null;
            StringBuilder sb = new StringBuilder();
            do {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(300);
            } while (this.clients.Available > 0);
            return sb.ToString();
        }

        void ParseTelnet(StringBuilder sb) {

            while (this.clients.Available > 0) {
                int input = this.clients.GetStream().ReadByte();
                switch (input) {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = this.clients.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb) {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = this.clients.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                this.clients.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    this.clients.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    this.clients.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                this.clients.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }

        /// <summary>
        /// Close connection with TCP Server
        /// </summary>
        public void Close()
        {
            this.clients.Close();
        }
    }
}
