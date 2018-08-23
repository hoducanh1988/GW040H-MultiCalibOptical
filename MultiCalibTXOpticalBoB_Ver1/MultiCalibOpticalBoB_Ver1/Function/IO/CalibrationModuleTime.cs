using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MultiCalibOpticalBoB_Ver1.Function.IO
{
    class CalibrationModuleTime
    {
        private static string filePath = string.Format("{0}CalibModuleTime.ini", System.AppDomain.CurrentDomain.BaseDirectory);

        public static bool Write() {
            try {
                File.WriteAllText(filePath, DateTime.Now.ToString());
                return true;
            } catch {
                return false;
            }
        }

        public static bool Read(out string _result) {
            _result = "03/08/2018 00:00:00";
            try {
                _result = File.ReadAllLines(filePath)[0];
                return true;
            } catch {
                return false;
            }
        }
    }
}
