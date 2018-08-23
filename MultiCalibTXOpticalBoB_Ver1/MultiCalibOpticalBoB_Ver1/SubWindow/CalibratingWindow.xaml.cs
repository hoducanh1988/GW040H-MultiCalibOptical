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

namespace MultiCalibOpticalBoB_Ver1 {
    /// <summary>
    /// Interaction logic for CalibratingWindow.xaml
    /// </summary>
    public partial class CalibratingWindow : Window {

        int _count = 0;
        DispatcherTimer timer = null;

        public CalibratingWindow() {
            InitializeComponent();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            timer.Stop();
            GlobalData.connectionManagement.WINDOWOPACITY = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            GlobalData.connectionManagement.WINDOWOPACITY = 0.5;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            _count = 0;
            timer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            this._count++;
            this.lblwait.Content = string.Format("Vui lòng chờ {0}/90 giây", this._count);
        }
    }
}
