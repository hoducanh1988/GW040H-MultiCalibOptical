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

namespace MultiCalibOpticalBoB_Ver1.UserControls {
    /// <summary>
    /// Interaction logic for ucAbout.xaml
    /// </summary>
    public partial class ucAbout : UserControl {

        List<history> listHist = new List<history>();

        public ucAbout() {
            InitializeComponent();

            listHist.Add(new history() {
                ID = "1",
                VERSION = "1.0.0.0",
                CONTENT = "- Phát hành lần đầu",
                DATE = "18/04/2018",
                CHANGETYPE = "Tạo mới",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "2", VERSION = "1.0.0.1",
                CONTENT = "- Tích hợp calib quang GW020BoB\n- Thay đổi thuật toán calib ER, Crossing",
                DATE = "29/05/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "3", VERSION = "1.0.0.2",
                CONTENT = "- Thay đổi tiêu chuẩn calib quang TX : cũ 2 - 3dBm, mới : 2.5 - 3dBm",
                DATE = "09/06/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "4",
                VERSION = "1.0.0.3",
                CONTENT = "- Thêm nút đo suy hao quang trong phần Setting.\n" +
                          "- Thêm chức năng kiểm tra kết nối tới máy đo trước khi test sản phẩm.\n" +
                          "- Thêm nút thu nhỏ phần mềm.\n" +
                          "- Chỉnh sửa giao diện phù hợp với kích thước màn hình 1366x768.",
                DATE = "22/06/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "5",
                VERSION = "1.0.0.4",
                CONTENT = "- Khi bật phần mềm, tự động kiểm tra trạng thái calib máy DCA X86100D (nếu chưa sẽ tự động calib).",
                DATE = "23/06/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "6",
                VERSION = "1.0.0.5",
                CONTENT = "- Tích hợp code calib quang GW020BoB và test Ok.\n" +
                          "- Phân biệt log test, log detail 2 dòng sản phẩm GW020BoB và GW040H\n  (thêm loại sản phẩm vào tên file log).",
                DATE = "25/06/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            this.GridAbout.ItemsSource = listHist;
        }

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }
    }
}
