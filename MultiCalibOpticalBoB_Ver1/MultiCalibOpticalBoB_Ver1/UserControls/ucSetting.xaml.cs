using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MultiCalibOpticalBoB_Ver1.Function;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {
        public ucSetting() {
            InitializeComponent();
            this.DataContext = GlobalData.initSetting;
            cbbSettingOntType.ItemsSource = Parameters.ListOntType;
            cbbSettingPwPort.ItemsSource = Parameters.ListIQSPort;
            cbbSettingSwPort.ItemsSource = Parameters.ListIQSPort;
            cbbSettingUsb1.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb2.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb3.ItemsSource = Parameters.ListUsbComport;
            cbbSettingUsb4.ItemsSource = Parameters.ListUsbComport;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "SAVE SETTING": {
                        GlobalData.initSetting.Save();
                        BaseFunctions.display_Port_Name();
                        if (GlobalData.listBosaInfo.Count == 0) BaseFunctions.loadBosaReport();
                        GlobalData.initSetting.ENABLETUNINGCROSSING = GlobalData.initSetting.ONTTYPE == "GW040H" ? false : true;
                        GlobalData.initSetting.ENABLESIGNALOFF = GlobalData.initSetting.ONTTYPE == "GW040H" ? true : false;
                        GlobalData.initSetting.ENABLEWRITEFLASH = GlobalData.initSetting.ONTTYPE == "GW040H" ? true : false;
                        MessageBox.Show("Success.", "SAVE SETTING", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (GlobalData.connectionManagement.IQS1700STATUS == false || GlobalData.connectionManagement.IQS9100BSTATUS == false || GlobalData.connectionManagement.DCAX86100DSTATUS == false || GlobalData.connectionManagement.SQLSTATUS == false)
                            BaseFunctions.connect_Instrument();
                        break;
                    }
                case "Browser...": {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Bosa report files |*.xls; *.xlsx; *.xlsm";
                        if (openFileDialog.ShowDialog() == true)
                            GlobalData.initSetting.BOSAREPORT = openFileDialog.FileName;
                        break;
                    }
                case "Hỗ trợ cài đặt": {
                        this.Opacity = 0.3;
                        wUsbDebugger w = new wUsbDebugger();
                        w.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
                case "Import to SQL": {
                        this.Opacity = 0.3;
                        ImportToSQL imp = new ImportToSQL();
                        imp.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }

                default: break;
            }
        }
    }
}
