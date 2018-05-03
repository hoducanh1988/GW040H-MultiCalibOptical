using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Instrument {
    public interface IInstrument {

        bool Open(out string messsage);
        bool Close();
        bool Initialize();

    }
}
