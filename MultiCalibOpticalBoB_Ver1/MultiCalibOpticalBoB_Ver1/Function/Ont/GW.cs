using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont
{
    public abstract class GW : Protocol.Serial
    {
        protected int Delay_modem = 300;

        public GW(string _portname) : base(_portname) { }

        public abstract bool Login(out string message);

        public abstract bool outTXPower();

        //Calib ------------------------------//
        public abstract bool loginToONT(testinginfo _testinfo);
        public abstract string getMACAddress(testinginfo _testinfo);
        public abstract bool calibPower(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);
        public abstract bool calibER(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);
        public abstract bool txDDMI(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);
        public abstract bool signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);
        public abstract bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo);
        public abstract bool verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var);
        public abstract bool writeMAC(testinginfo _testinfo);
    }
}
