using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MultiCalibOpticalBoB_Ver1.Function.IO {
    public class LogTest {

        private static Object lockthis = new Object();

        private static string _logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogTest";
        static LogTest() {
            if (Directory.Exists(_logpath) == false) Directory.CreateDirectory(_logpath);
        }

        public static bool Save(testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _file = DateTime.Now.ToString("yyyyMMdd");
                    string _title = "DATETIME,MAC-ADDRESS,BOSA-SERIAL,TUNINGPOWER-RESULT,TUNINGER-RESULT,TXDDMI-RESULT,SIGNALOFF-RESULT,WRITEFLASH-RESULT,VERIFYSIGNAL-RESULT,WRITEMAC-RESULT,ERROR-CODE,TOTAL-RESULT,TOTAL-TIME";

                    string _content = "";
                    _content += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",";
                    _content += _testinfo.MACADDRESS + ",";
                    _content += _testinfo.BOSASERIAL + ",";
                    _content += _testinfo.TUNINGPOWERRESULT + ",";
                    _content += _testinfo.TUNINGERRESULT + ",";
                    _content += _testinfo.TXDDMIRESULT + ",";
                    _content += _testinfo.SIGNALOFFRESULT + ",";
                    _content += _testinfo.WRITEFLASHRESULT + ",";
                    _content += _testinfo.VERIFYSIGNALRESULT + ",";
                    _content += _testinfo.WRITEMACRESULT + ",";
                    _content += _testinfo.ERRORCODE.Replace("Mã Lỗi", "") + ",";
                    _content += _testinfo.TOTALRESULT + ",";
                    _content += _testinfo.TOTALTIME;

                    if (File.Exists(string.Format("{0}\\{1}.csv", _logpath, _file)) == false) {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _logpath, _file), true);
                        st.WriteLine(_title);
                        st.WriteLine(_content);
                        st.Dispose();
                    }
                    else {
                        StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.csv", _logpath, _file), true);
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
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}\\{1}", _logpath, _filename);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}\\{1}", _logpath, _filename));
                proc.Start();
                return true;
            } catch {
                return false;
            }
        }

        public static bool OpenFolder() {
            try {
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}", _logpath);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}", _logpath));
                proc.Start();
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool ListAllFile(out int _filecount) {
            _filecount = 0;
            try {
                GlobalData.datagridlogtest.Clear();
                DirectoryInfo d = new DirectoryInfo(_logpath);
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
