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

    }
}
