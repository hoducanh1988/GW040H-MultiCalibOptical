using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiCalibOpticalBoB_Ver1.Function;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucLog.xaml
    /// </summary>
    public partial class ucLog : UserControl {
        public ucLog() {
            InitializeComponent();
            this.border_SQL_Param.DataContext = GlobalData.initSetting;
            this.logtest_datagrid.ItemsSource = GlobalData.datagridlogtest;
            this.logdetail_datagrid.ItemsSource = GlobalData.datagridlogdetail;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string _name = b.Name;
            switch (_name) {
                case "sqlGetData": {
                        this.bosareport_datagrid.ItemsSource = GlobalData.listBosaInfo;
                        break;
                    }
                case "logtestListAll": {
                        int _count;
                        Function.IO.LogTest.ListAllFile(out _count);
                        MessageBox.Show(string.Format("Success.\n...\nThere are {0} log files found.", _count),"Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "logtestOpen": {
                        try {
                            var row = (logfileinfo)this.logtest_datagrid.SelectedItem;
                            Function.IO.LogTest.Open(row.FileName);
                        } catch {
                            MessageBox.Show("Vui lòng chọn file cần mở trước.","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case "logtestOpenFolder": {
                        Function.IO.LogTest.OpenFolder();
                        break;
                    }
                case "logdetailListAll": {
                        int _count;
                        Function.IO.LogDetail.ListAllFile(out _count);
                        MessageBox.Show(string.Format("Success.\n...\nThere are {0} log files found.", _count), "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "logdetailOpen": {
                        try {
                            var row = (logfileinfo)this.logdetail_datagrid.SelectedItem;
                            Function.IO.LogDetail.Open(row.FileName);
                        }
                        catch {
                            MessageBox.Show("Vui lòng chọn file cần mở trước.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case "logdetailOpenFolder": {
                        Function.IO.LogDetail.OpenFolder();
                        break;
                    }
                default: break;
            }
        }
    }
}
