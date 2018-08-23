using MultiCalibOpticalBoB_Ver1.Function;
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
using System.Windows.Threading;

namespace MultiCalibOpticalBoB_Ver1
{
    /// <summary>
    /// Interaction logic for CalibModuleWarning.xaml
    /// </summary>
    public partial class CalibModuleWarning : Window
    {

        int _count = 0;
        DispatcherTimer timer = null;

        public CalibModuleWarning(string _time, string _temperature)
        {
            InitializeComponent();
            double _hours = 0;
            bool ret = BaseFunctions.last_Time_Calibrate_Module_DCAX86100D_To_Hours(_time, out _hours);
            lblTime.Content = string.Format("Thời điểm calib gần nhất: {0}, chênh lệch {1} giờ (tiêu chuẩn 5 giờ)", _time, Math.Round(_hours, 2));
            lblTemp.Content = string.Format("Nhiệt độ chênh lệch hiện tại: {0} độ C (tiêu chuẩn 5 độ C)", _temperature);

            if (_hours > 5) {
                lblTime.Foreground = Brushes.Red;
                lblTime.FontWeight = FontWeights.Bold;
            }
            if (double.Parse(_temperature) > 5) {
                lblTemp.Foreground = Brushes.Red;
                lblTemp.FontWeight = FontWeights.Bold;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            GlobalData.connectionManagement.WINDOWOPACITY = 0.5;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _count = 0;
            timer.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            timer.Stop();
            GlobalData.connectionManagement.WINDOWOPACITY = 1;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            this._count++;
            this.MainBorder.Background = this._count % 2 == 1 ? (SolidColorBrush)new BrushConverter().ConvertFrom("#EF692B") : (SolidColorBrush)new BrushConverter().ConvertFrom("#FFE738");
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            timer.Stop();
            this.Close();
        }
    }
}
