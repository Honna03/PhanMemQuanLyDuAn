using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using QuanLyDuAn.Forms;

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachCongViec : UserControl
    {
        public DanhSachCongViec()
        {
            InitializeComponent();
        }

        private void btn_AddCongViec_Click_1(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new ChiTietCongViec(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}
