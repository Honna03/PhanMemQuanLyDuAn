using QuanLyDuAn.Services;
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

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string taiKhoan = txtTaiKhoan.Text;
            string matKhau = txtMatKhau.Password;

            var authService = new AuthService();
            var nhanVien = authService.DangNhap(taiKhoan, matKhau);

            if (nhanVien != null)
            {
                if (nhanVien.QMa == "admin")
                {
                    MainWindow mainWindow = new MainWindow(nhanVien.NvTaiKhoan);
                    mainWindow.Show();
                }
                else
                {
                    MainWindow mainWindow = new MainWindow(nhanVien.NvTaiKhoan);
                    mainWindow.Show();
                }

                this.Close(); // đóng form đăng nhập
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
