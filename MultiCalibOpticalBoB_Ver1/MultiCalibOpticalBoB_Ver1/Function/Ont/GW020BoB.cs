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

        public override bool Login() {
            try {
                //write some code here.
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
