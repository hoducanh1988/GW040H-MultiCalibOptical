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
using System.Windows.Threading;

namespace MultiCalibOpticalBoB_Ver1
{
    /// <summary>
    /// Interaction logic for ConnectInstrument.xaml
    /// </summary>
    public partial class ConnectInstrument : Window
    {
        int _count = 0;
        DispatcherTimer timer = null;

        public ConnectInstrument()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            GlobalData.connectionManagement.WINDOWOPACITY = 0.5;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            _count = 0;
            timer.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            GlobalData.connectionManagement.WINDOWOPACITY = 1;
            timer.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            this._count++;
            this.lblwait.Content = string.Format("Please wait...{0}", this._count);
            //switch (this._count) {
            //    case 1: { this.lblwait.Content = "Please wait"; break; }
            //    case 2: { this.lblwait.Content = "Please wait."; break; }
            //    case 3: { this.lblwait.Content = "Please wait.."; break; }
            //    case 4: { this.lblwait.Content = "Please wait..."; break; }
            //    case 5: { this.lblwait.Content = "Please wait...."; break; }
            //    case 6: { this.lblwait.Content = "Please wait....."; break; }
            //    default: { this._count = 0; break; }
            //}
        }
    }
}
