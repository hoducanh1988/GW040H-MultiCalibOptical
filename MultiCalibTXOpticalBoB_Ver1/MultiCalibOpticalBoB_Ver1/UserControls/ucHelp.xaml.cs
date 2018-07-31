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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucHelp.xaml
    /// </summary>
    public partial class ucHelp : UserControl {
        public ucHelp() {
            InitializeComponent();
            XpsDocument xpsDocument = new XpsDocument(string.Format("{0}Help\\UserGuide.xps", System.AppDomain.CurrentDomain.BaseDirectory), System.IO.FileAccess.Read);
            FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
            docViewer.Document = fds;
        }
    }
}
