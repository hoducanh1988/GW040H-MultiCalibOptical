using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont
{
    public class GW020BoB : GW
    {
        public GW020BoB(string _portname) : base(_portname) { }

        public override bool Login(out string message) {
            message = "";
            try {
                //write some code here.
                return true;
            }
            catch {
                return false;
            }
        }

        public override bool calibER(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override bool calibPower(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override string getMACAddress(testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool loginToONT(testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool outTXPower() {
            throw new NotImplementedException();
        }

        public override bool signalOff(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            return true;
        }

        public override bool txDDMI(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override bool verifySignal(int Port, bosainfo _bosainfo, testinginfo _testinfo, variables _var) {
            throw new NotImplementedException();
        }

        public override bool writeFlash(bosainfo _bosainfo, testinginfo _testinfo) {
            throw new NotImplementedException();
        }

        public override bool writeMAC(testinginfo _testinfo) {
            throw new NotImplementedException();
        }
    }
}
