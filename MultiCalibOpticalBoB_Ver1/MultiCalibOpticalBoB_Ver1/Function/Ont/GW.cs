using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont
{
    public abstract class GW : Protocol.Serial
    {
        public GW(string _portname) : base(_portname) { }

        public abstract bool Login(out string message);

        public abstract bool outTXPower();

        //public abstract string getMACAddress(testinginfo _testinfo);

        //public abstract bool _calibPower(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);

        //public abstract bool _calibER(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);

        //public abstract bool _txDDMI(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);

        //public abstract bool _signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);

        //public abstract bool _writeFlash(bosainfo _bosainfo, testinginfo _testinfo);

        //public abstract bool _verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);

        //public abstract bool _writeMAC(testinginfo _testinfo);
    }
}
