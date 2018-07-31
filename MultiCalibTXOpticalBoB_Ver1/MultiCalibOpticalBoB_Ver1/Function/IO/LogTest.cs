using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace MultiCalibOpticalBoB_Ver1.Function.IO {
    public class LogTest {

        private static Object lockthis = new Object();

        private static string _dir_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest";
        private static string _dir_040H_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest\\040H";
        private static string _dir_020BoB_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest\\020BoB";

        static LogTest() {
            if (Directory.Exists(_dir_logpath) == false) Directory.CreateDirectory(_dir_logpath);
            Thread.Sleep(100);
            if (Directory.Exists(_dir_040H_logpath) == false) Directory.CreateDirectory(_dir_040H_logpath);
            if (Directory.Exists(_dir_020BoB_logpath) == false) Directory.CreateDirectory(_dir_020BoB_logpath);

        }

        public static bool Save(testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _dir = GlobalData.initSetting.ONTTYPE == "GW040H" ? _dir_040H_logpath : _dir_020BoB_logpath;
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    string _title = GlobalData.initSetting.ONTTYPE == "GW040H" ? "DATETIME,MAC-ADDRESS,BOSA-SERIAL,TUNINGPOWER-RESULT,TUNINGER-RESULT,TXDDMI-RESULT,SIGNALOFF-RESULT,WRITEFLASH-RESULT,VERIFYSIGNAL-RESULT,WRITEMAC-RESULT,ERROR-CODE,TOTAL-RESULT,TOTAL-TIME" : "DATETIME,MAC-ADDRESS,BOSA-SERIAL,WRITEAPD-RESULT,TUNINGPOWER-RESULT,TUNINGER-RESULT,TUNING-CROSSING,TXDDMI-RESULT,ERROR-CODE,TOTAL-RESULT,TOTAL-TIME";

                    string _content = "";
                    _content += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
                    _content += _testinfo.MACADDRESS + ",";
                    _content += _testinfo.BOSASERIAL + ",";
                    if (GlobalData.initSetting.ONTTYPE != "GW040H") _content += _testinfo.WRITEAPDRESULT + ",";
                    _content += _testinfo.TUNINGPOWERRESULT + ",";
                    _content += _testinfo.TUNINGERRESULT + ",";
                    if (GlobalData.initSetting.ONTTYPE != "GW040H") _content += _testinfo.TUNINGCROSSINGRESULT + ",";
                    _content += _testinfo.TXDDMIRESULT + ",";
                    if (GlobalData.initSetting.ONTTYPE == "GW040H") {
                        _content += _testinfo.SIGNALOFFRESULT + ",";
                        _content += _testinfo.WRITEFLASHRESULT + ",";
                        _content += _testinfo.VERIFYSIGNALRESULT + ",";
                        _content += _testinfo.WRITEMACRESULT + ",";
                    }
                    _content += _testinfo.ERRORCODE.Replace("Mã Lỗi", "") + ",";
                    _content += _testinfo.TOTALRESULT + ",";
                    _content += _testinfo.TOTALTIME;

                    if (File.Exists(string.Format("{0}\\{1}.csv", _dir, _file)) == false) {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _dir, _file), true);
                        st.WriteLine(_title);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    else {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _dir, _file), true);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        public static bool Open(string _filename) {
            try {
                string _dir = GlobalData.initSetting.ONTTYPE == "GW040H" ? _dir_040H_logpath : _dir_020BoB_logpath;
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}\\{1}", _dir, _filename);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}\\{1}", _dir, _filename));
                proc.Start();
                return true;
            } catch {
                return false;
            }
        }

        public static bool OpenFolder() {
            try {
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}", _dir_logpath);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}", _dir_logpath));
                proc.Start();
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool ListAllFile(out int _filecount) {
            string _dir = GlobalData.initSetting.ONTTYPE == "GW040H" ? _dir_040H_logpath : _dir_020BoB_logpath;
            _filecount = 0;
            try {
                GlobalData.datagridlogtest.Clear();
                DirectoryInfo d = new DirectoryInfo(_dir);
                FileInfo[] Files = d.GetFiles("*.csv");
                int index = 0;
                foreach (FileInfo file in Files) {
                    index++;
                    logfileinfo log = new logfileinfo();
                    log.ID = index;
                    log.FileName = file.Name;
                    if (file.Length >= (1 << 10)) log.MemorySize = string.Format("{0}Kb", file.Length >> 10);
                    else log.MemorySize = "1Kb";
                    log.DateCreated = file.CreationTime.ToString();
                    log.DateModified = file.LastAccessTime.ToString();
                    GlobalData.datagridlogtest.Add(log);
                }
                _filecount = index;
                return true;
            } catch {
                return false;
            }
        }

    }
}
