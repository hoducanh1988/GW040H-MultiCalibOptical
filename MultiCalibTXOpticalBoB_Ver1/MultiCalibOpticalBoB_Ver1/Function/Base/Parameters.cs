using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function {
    public class Parameters {

        public static List<string> ListOntType = new List<string>() { "GW020BoB", "GW040H" };
        public static List<string> ListWriteAPDOption = new List<string>() { "Only Write APD LUT", "Write APD LUT","Don't Write APD LUT" };
        public static List<int> ListIQSPort = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static List<string> ListUsbComport = BaseFunctions.get_Array_Of_SerialPort();
        public enum testStatus { NONE = 0, Wait = 1, PASS = 2, FAIL = 3 };

    }
}
