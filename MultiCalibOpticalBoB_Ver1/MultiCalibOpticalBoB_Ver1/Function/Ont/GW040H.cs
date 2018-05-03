using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiCalibOpticalBoB_Ver1.Function.Ont
{
    public class GW040H : GW
    {
        public GW040H(string _portname) : base(_portname) { }

        public override bool Login() {
            try {
                bool _flag = false;
                int index = 0;
                int max = 20;
                while(!_flag) {
                    //Gửi lệnh Enter để ONT về trạng thái đăng nhập
                    base.Write("\r\n");
                    Thread.Sleep(100);
                    string data = "";
                    data = base.Read();
                    if (data.Replace("\r", "").Replace("\n", "").Trim().Contains("#")) return true;
                    while(!data.Contains("tc login:")) {
                        data += base.Read();
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin User
                    base.Write(GlobalData.initSetting.ONTLOGINUSER + "\n");

                    //Chờ ONT xác nhận User
                    while (!data.Contains("Password:")) {
                        data += base.Read();
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;

                    //Gửi thông tin Password
                    base.Write(GlobalData.initSetting.ONTLOGINPASS + "\n");

                    //Chờ ONT xác nhận Password
                    while (!data.Contains("root login  on `console'")) {
                        data += base.Read();
                        Thread.Sleep(500);
                        if (index >= max) break;
                        else index++;
                    }
                    if (index >= max) break;
                    else _flag = true;
                }
                return _flag;
            }
            catch {
                return false;
            }
        }

    }
}
