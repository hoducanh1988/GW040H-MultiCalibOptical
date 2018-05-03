using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace MultiCalibOpticalBoB_Ver1.Function.IO
{
    public class BosaReport
    {
        public static DataTable readData() {
            try {
                string _connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + GlobalData.initSetting.BOSAREPORT + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'";
                using (OleDbConnection conn = new OleDbConnection(_connectionString)) {
                    DataTable dt = new DataTable();
                    conn.Open();
                    OleDbDataAdapter objDA = new System.Data.OleDb.OleDbDataAdapter("select * from [Sheet1$A8:AA]", conn);
                    DataSet excelDataSet = new DataSet();
                    objDA.Fill(excelDataSet);
                    dt = excelDataSet.Tables[0];

                    excelDataSet.Dispose();
                    objDA.Dispose();
                    conn.Dispose();
                    return dt;
                }
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
        }
    }
}
