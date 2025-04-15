using QuanLyDuAn.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyDuAn.Forms
{
    public partial class Login : Window
    {
        private readonly ThucTapQuanLyDuAnContext _context;
        private bool _isPasswordVisible = false;

        public Login()
        {
            InitializeComponent();
            _context = new ThucTapQuanLyDuAnContext();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;

            var nhanVien = _context.NhanViens
                .FirstOrDefault(nv => nv.NvTaiKhoan == username && nv.NvMatKhau == password);

            if (nhanVien != null)
            {
                MainWindow mainWindow = new MainWindow(nhanVien.NvId);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                UsernameTextBox.Tag = "Error";
                if (_isPasswordVisible)
                    PasswordTextBox.Tag = "Error";
                else
                    PasswordBox.Tag = "Error";
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                TogglePasswordIcon.Text = "👁️‍🗨️";
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                TogglePasswordIcon.Text = "👁️";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void tb_Welcome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TitlePanel.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
        }

        private void btn_Login_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hiệu ứng nếu cần
        }

        private void btn_Login_MouseLeave(object sender, MouseEventArgs e)
        {
            // Hiệu ứng nếu cần
        }
    }
}
