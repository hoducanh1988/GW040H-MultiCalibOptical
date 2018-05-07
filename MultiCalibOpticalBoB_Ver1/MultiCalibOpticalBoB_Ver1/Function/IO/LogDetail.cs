using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MultiCalibOpticalBoB_Ver1.Function.IO {
    public class LogDetail {
        private static Object lockthis = new Object();
        private static string _logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail";

        static LogDetail() {
            if (Directory.Exists(_logpath) == false) Directory.CreateDirectory(_logpath);
        }

        public static bool Save(testinginfo _testinfo) {
            lock (lockthis) {
                try {
                    string _file = DateTime.Now.ToString("yyyyMMdd");

                    StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.txt", _logpath, _file), true);
                    st.WriteLine(_testinfo.SYSTEMLOG);
                    st.Dispose();
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
            }
            catch {
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
                GlobalData.datagridlogdetail.Clear();
                DirectoryInfo d = new DirectoryInfo(_logpath);
                FileInfo[] Files = d.GetFiles("*.txt");
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
                    GlobalData.datagridlogdetail.Add(log);
                }
                _filecount = index;
                return true;
            }
            catch {
                return false;
            }
        }

    }
}
