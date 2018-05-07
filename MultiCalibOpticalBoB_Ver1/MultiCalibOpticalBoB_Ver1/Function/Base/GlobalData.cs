using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiCalibOpticalBoB_Ver1.Function.Instrument;

namespace MultiCalibOpticalBoB_Ver1.Function
{
    public class GlobalData
    {
        public static defaultsetting initSetting = new defaultsetting();
        public static manualtest manualTest = new manualtest();
        public static connectionstatus connectionManagement = new connectionstatus();

        public static testinginfo testingDataDut1 = new testinginfo();
        public static testinginfo testingDataDut2 = new testinginfo();
        public static testinginfo testingDataDut3 = new testinginfo();
        public static testinginfo testingDataDut4 = new testinginfo();

        public static List<string> listSequenceTestER = new List<string>();

        public static IQS1700 powerDevice = null;
        public static IQS9100B switchDevice = null;
        public static DCAX86100D erDevice = null;
        public static Protocol.Sql sqlServer = null;

        public static List<bosainfo> listBosaInfo = new List<bosainfo>();

        public static ObservableCollection<logfileinfo> datagridlogtest = new ObservableCollection<logfileinfo>();
        public static ObservableCollection<logfileinfo> datagridlogdetail = new ObservableCollection<logfileinfo>();
    }
}
