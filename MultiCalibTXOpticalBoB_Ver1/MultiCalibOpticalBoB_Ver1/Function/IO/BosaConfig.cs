using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MultiCalibOpticalBoB_Ver1.Function.IO {
    public class BosaConfig {

        public static double SlopeUp { get; set; }
        public static double SlopeDown { get; set; }
        public static double Offset { get; set; }

        static BosaConfig() {
            string _file = string.Format("{0}ApdFiles\\BosaConfig.ini", System.AppDomain.CurrentDomain.BaseDirectory);
            string[] lines = File.ReadAllLines(_file);

            SlopeUp = double.Parse(lines[0].Split('=')[1]);
            SlopeDown = double.Parse(lines[1].Split('=')[1]);
            Offset = double.Parse(lines[2].Split('=')[1]);
        }

    }
}
