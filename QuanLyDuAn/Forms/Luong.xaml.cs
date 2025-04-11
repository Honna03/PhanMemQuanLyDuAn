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
using System.Windows.Shapes;
using QuanLyDuAn.Controls;

namespace QuanLyDuAn.Forms
{
    public partial class Luong : UserControl
    {

        public Luong()
        {
            InitializeComponent();
        }

        private void btn_AddNhanVien_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new ChiTietNhanVien(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        private void btn_AddLuong_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_Luong(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}