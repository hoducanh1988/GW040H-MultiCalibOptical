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

namespace MultiCalibOpticalBoB_Ver1 {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window {
        public Login() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            txtUser.Focus();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e) {
            GlobalData.loginUser = txtUser.Text;
            GlobalData.loginPass = txtPass.Password.ToString();
            this.Close();
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                GlobalData.loginUser = txtUser.Text;
                GlobalData.loginPass = txtPass.Password.ToString();
                this.Close();
            }
        }

    }
}
