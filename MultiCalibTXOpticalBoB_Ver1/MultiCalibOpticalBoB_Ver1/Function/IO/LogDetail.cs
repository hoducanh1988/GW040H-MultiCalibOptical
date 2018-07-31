using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace MultiCalibOpticalBoB_Ver1.Function.IO {

    public class LogDetail {
        private static Object lockthis = new Object();
        private static string _dir_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail";
        private static string _dir_040H_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail\\040H";
        private static string _dir_020BoB_logpath = System.AppDomain.CurrentDomain.BaseDirectory + "LogDetail\\020BoB";

        static LogDetail() {
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

                    StreamWriter st = new StreamWriter(string.Format("{0}\\{1}.txt", _dir, _file), true);
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
                string _dir = GlobalData.initSetting.ONTTYPE == "GW040H" ? _dir_040H_logpath : _dir_020BoB_logpath;
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}\\{1}", _dir, _filename);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}\\{1}", _dir, _filename));
                proc.Start();
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool OpenFolder() {
            try {
                string _dir = GlobalData.initSetting.ONTTYPE == "GW040H" ? _dir_040H_logpath : _dir_020BoB_logpath;
                Process proc = new Process();
                proc.StartInfo.FileName = string.Format("{0}", _dir);
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(string.Format("{0}", _dir));
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
                GlobalData.datagridlogdetail.Clear();
                DirectoryInfo d = new DirectoryInfo(_dir);
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
