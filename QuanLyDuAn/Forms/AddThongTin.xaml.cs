using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Forms
{
    public partial class AddThongTin : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        public event EventHandler ThongTinAdded;

        public AddThongTin()
        {
            InitializeComponent();
        }

        // Xử lý nút Chọn file
        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*",
                Title = "Chọn file logo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtLogo.Text = openFileDialog.FileName; // Hiển thị đường dẫn file trong TextBox
            }
        }

        // Xử lý nút Lưu
        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(txtTenCongTy.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên công ty!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSDT.Text))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng nhập email!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLogo.Text))
                {
                    MessageBox.Show("Vui lòng nhập hoặc chọn logo!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMoTa.Text))
                {
                    MessageBox.Show("Vui lòng nhập mô tả!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tạo đối tượng ThongTinCongTy
                ThongTinCongTy thongTinCongTy = new ThongTinCongTy
                {
                    CtyTen = txtTenCongTy.Text.Trim(),
                    CtySdt = txtSDT.Text.Trim(),
                    CtyEmail = txtEmail.Text.Trim(),
                    CtyLogo = txtLogo.Text.Trim(), // Lưu đường dẫn hoặc text của logo
                    CtyMoTa = txtMoTa.Text.Trim(),
                    CtyDiaChi = txtDiaChi.Text.Trim()
                };

                // Thêm vào database
                context.ThongTinCongTies.Add(thongTinCongTy);
                context.SaveChanges();

                // Kích hoạt sự kiện ThongTinAdded
                ThongTinAdded?.Invoke(this, EventArgs.Empty);

                MessageBox.Show("Đã thêm thông tin công ty thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý nút Hủy
        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}