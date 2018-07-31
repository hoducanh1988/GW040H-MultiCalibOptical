using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiCalibOpticalBoB_Ver1.Function;
using MultiCalibOpticalBoB_Ver1.Function.IO;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace MultiCalibOpticalBoB_Ver1
{
    /// <summary>
    /// Interaction logic for ImportToSQL.xaml
    /// </summary>
    public partial class ImportToSQL : Window
    {
        class importinfo : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName = null) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            string _status;
            public string STATUS {
                get { return _status; }
                set {
                    _status = value;
                    OnPropertyChanged(nameof(STATUS));
                }
            }
            int _progressmax;
            public int PROGRESSMAX {
                get { return _progressmax; }
                set {
                    _progressmax = value;
                    PROGRESSSTRING = string.Format("{0}/{1}", PROGRESSVALUE, PROGRESSMAX);
                    OnPropertyChanged(nameof(PROGRESSMAX));
                }
            }
            int _progressvalue;
            public int PROGRESSVALUE {
                get { return _progressvalue; }
                set {
                    _progressvalue = value;
                    PROGRESSSTRING = string.Format("{0}/{1}", PROGRESSVALUE, PROGRESSMAX);
                    OnPropertyChanged(nameof(PROGRESSVALUE));
                }
            }
            string _progressstring;
            public string PROGRESSSTRING {
                get { return _progressstring; }
                set {
                    _progressstring = value;
                    OnPropertyChanged(nameof(PROGRESSSTRING));
                }
            }

            public importinfo() {
                this.STATUS = "Ready";
                this.PROGRESSMAX = 0;
                this.PROGRESSVALUE = 0;

            }
        }

        importinfo importInfo = new importinfo();
        public ImportToSQL()
        {
            InitializeComponent();
            this.DataContext = this.importInfo;
            this.lblTitle.Content = string.Format("IMPORT BOSA REPORT TO SQL SERVER -- {0}", GlobalData.initSetting.SQLNAME);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string _txt = b.Content.ToString();
            b.Content = "Please wait...";

            Thread t = new Thread(new ThreadStart(() => {

                //***************************************************SQL SERVER
                ////Load data from excel to dataGrid
                //DataTable dt = new DataTable();
                //importInfo.STATUS = "Loading data from excel to datagrid...";
                //dt = BosaReport.readData();

                ////Display data to dataGrid
                //Dispatcher.Invoke(new Action(() => {
                //    this.datagrid.ItemsSource = dt.DefaultView;
                //    importInfo.PROGRESSMAX = dt.Rows.Count - 1;
                //    b.Content = _txt;
                //}));

                ////Import data from dataGrid to Sql Server (using Sql Bulk)
                //importInfo.STATUS = "Importing data from datagrid to Sql Server...";
                //int counter = 0;
                //for (int i = 0; i < dt.Rows.Count; i++) {
                //    string _bosaSN = "", _Ith = "", _Vbr = "";
                //    _bosaSN = dt.Rows[i].ItemArray[0].ToString().Trim();
                //    if (_bosaSN.Length > 0 && BaseFunctions.bosa_SerialNumber_Is_Correct(_bosaSN) == true) {
                //        _Ith = dt.Rows[i].ItemArray[1].ToString().Trim();
                //        _Vbr = dt.Rows[i].ItemArray[18].ToString().Trim();

                //        bosainfo _bs = new bosainfo() { BosaSN = _bosaSN, Ith = _Ith, Vbr = _Vbr };
                //        if (GlobalData.sqlServer.insertData(_bs)) counter++;
                //        importInfo.PROGRESSVALUE += 1;
                //    }
                //}
                //importInfo.PROGRESSVALUE = importInfo.PROGRESSMAX;
                //importInfo.STATUS = string.Format("Finished.The Bosa SN already imported {0}/{1}.", counter, dt.Rows.Count);

                //***************************************************NO SQL SERVER

                //Load data from excel to dataGrid
                DataTable dt = new DataTable();
                importInfo.STATUS = "Loading data from excel to datagrid...";
                dt = BosaReport.readData();

                //Display data to dataGrid
                Dispatcher.Invoke(new Action(() => {
                    this.datagrid.ItemsSource = dt.DefaultView;
                    importInfo.PROGRESSMAX = dt.Rows.Count - 1;
                    b.Content = _txt;
                }));

                //Import data from dataGrid to Sql Server (using Sql Bulk)
                importInfo.STATUS = "Importing data from datagrid to Sql Server...";
                int counter = 0;
                GlobalData.listBosaInfo = new List<bosainfo>();

                for (int i = 0; i < dt.Rows.Count; i++) {
                    string _bosaSN = "", _Ith = "", _Vbr = "";
                    _bosaSN = dt.Rows[i].ItemArray[0].ToString().Trim();
                    if (_bosaSN.Length > 0 && BaseFunctions.bosa_SerialNumber_Is_Correct(_bosaSN) == true) {
                        _Ith = dt.Rows[i].ItemArray[1].ToString().Trim();
                        _Vbr = dt.Rows[i].ItemArray[5].ToString().Trim();

                        bosainfo _bs = new bosainfo() { BosaSN = _bosaSN, Ith = _Ith, Vbr = _Vbr };
                        GlobalData.listBosaInfo.Add(_bs);
                        counter++;
                        importInfo.PROGRESSVALUE += 1;
                    }
                }
                importInfo.PROGRESSVALUE = importInfo.PROGRESSMAX;
                importInfo.STATUS = string.Format("Finished.The Bosa SN already imported {0}/{1}.", counter, dt.Rows.Count - 1);
            }));
            t.IsBackground = true;
            t.Start();
            
        }
    }
}
