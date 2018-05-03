using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiCalibOpticalBoB_Ver1.Function;
using System.IO.Ports;
using System.Threading;

namespace MultiCalibOpticalBoB_Ver1 {
    /// <summary>
    /// Interaction logic for wUsbDebugger.xaml
    /// </summary>
    public partial class wUsbDebugger : Window {

        List<SerialPort> listSerialPort = new List<SerialPort>();

        public wUsbDebugger() {
            InitializeComponent();
            this.DataContext = GlobalData.manualTest;
            Parameters.ListUsbComport = BaseFunctions.get_Array_Of_SerialPort();
            GlobalData.manualTest.ONTLOG = "";
            foreach (var serialName in Parameters.ListUsbComport) {
                SerialPort serial = new SerialPort();
                serial.PortName = serialName;
                serial.BaudRate = 9600;
                serial.DataBits = 8;
                serial.Parity = Parity.None;
                serial.StopBits = StopBits.One;
                serial.Open();
                serial.DataReceived += new SerialDataReceivedEventHandler(serial_OnReceiveDatazz);
                listSerialPort.Add(serial);
                GlobalData.manualTest.ONTLOG += string.Format("{0} to serialport = {1}\n", serial.IsOpen == true ? "Connected" : "Disconnect", serialName);
            }
            GlobalData.manualTest.ONTLOG += "***\n";
        }

        private void serial_OnReceiveDatazz(object sender, SerialDataReceivedEventArgs e) {
            SerialPort s = (SerialPort)sender;
            string _data = "";
            _data = s.ReadExisting();
            if (_data != string.Empty) {
                GlobalData.manualTest.ONTLOG += string.Format("{0}: {1}\n", s.PortName, _data);
            }
            Thread.Sleep(500);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            foreach (var serial in listSerialPort) serial.Close();
            this.Close();
        }
    }
}
