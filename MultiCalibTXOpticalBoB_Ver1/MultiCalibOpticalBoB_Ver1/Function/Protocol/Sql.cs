using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MultiCalibOpticalBoB_Ver1.Function.Protocol {
    public class Sql {

        SqlConnection cn;
        SqlCommand cmd;
        SqlDataAdapter adp;
        bool _isConnected = false;
        private Object thislock = new Object();

        public bool Connection() {
            lock (thislock) {
                try {
                    string constr = string.Format("Server={0};Database={1};User ID={2};Password={3};", GlobalData.initSetting.SQLNAME, GlobalData.initSetting.SQLDB, GlobalData.initSetting.SQLUSER, GlobalData.initSetting.SQLPASS);
                    cn = new SqlConnection(constr);
                    cn.Open();
                    System.Threading.Thread.Sleep(10);
                    _isConnected = cn.State == System.Data.ConnectionState.Open;
                    if (_isConnected) return true;
                    else return false;
                }
                catch {
                    return false;
                }
            }
        }

        public bool insertData(bosainfo _bs) {
            lock (thislock) {
                try {
                    if (cn.State != System.Data.ConnectionState.Open) cn.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = string.Format("INSERT INTO Data (BosaSN,Ith,Vbr) VALUES('{0}','{1}','{2}')", _bs.BosaSN, _bs.Ith, _bs.Vbr);
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        public bosainfo getDataByBosaSN(string _bosaSN) {
            lock (thislock) {
                try {
                    bosainfo bs = new bosainfo();
                    if (cn.State != System.Data.ConnectionState.Open) cn.Open();
                    DataTable dt = new DataTable();

                    cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = string.Format("SELECT tbID,BosaSN,Ith,Vbr FROM Data WHERE BosaSN='{0}'", _bosaSN);
                    adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0) {
                        bs.tbID = dt.Rows[0].ItemArray[0].ToString();
                        bs.BosaSN = dt.Rows[0].ItemArray[1].ToString();
                        bs.Ith = dt.Rows[0].ItemArray[2].ToString();
                        bs.Vbr = dt.Rows[0].ItemArray[3].ToString();
                        return bs;
                    }
                    else return null;
                }
                catch {
                    return null;
                }
            }   
        }
        
        public bool Close() {
            lock (thislock) {
                try {
                    cn.Close();
                    return true;
                }
                catch {
                    return false;
                }
            }
        }

        

    }
}
