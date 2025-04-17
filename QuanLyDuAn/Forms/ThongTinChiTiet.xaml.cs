using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Forms
{
    public partial class ThongTinChiTiet : Window
    {
        private ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        private ThongTinCongTy thongTinCongTy;
        private bool isEditing = false;
        public event EventHandler ThongTinDeleted;

        public ThongTinChiTiet(int ctyId)
        {
            InitializeComponent();
            LoadThongTin(ctyId);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetReadOnly(true); // Khóa các trường khi tải form
        }

        private void LoadThongTin(int ctyId)
        {
            try
            {
                thongTinCongTy = context.ThongTinCongTies.FirstOrDefault(cty => cty.CtyId == ctyId);
                if (thongTinCongTy == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin công ty!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                txtTenCongTy.Text = thongTinCongTy.CtyTen;
                txtSDT.Text = thongTinCongTy.CtySdt;
                txtEmail.Text = thongTinCongTy.CtyEmail;
                txtLogo.Text = thongTinCongTy.CtyLogo;
                txtDiaChi.Text = thongTinCongTy.CtyDiaChi;
                txtMoTa.Text = thongTinCongTy.CtyMoTa;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing) return; // Chỉ cho phép chọn file khi đang sửa

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*",
                Title = "Chọn file logo"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtLogo.Text = openFileDialog.FileName;
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            isEditing = true;
            SetReadOnly(false); // Mở khóa các trường để chỉnh sửa
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing) return;

            try
            {
                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(txtTenCongTy.Text) ||
                    string.IsNullOrWhiteSpace(txtSDT.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtLogo.Text) ||
                    string.IsNullOrWhiteSpace(txtMoTa.Text) ||
                    string.IsNullOrWhiteSpace(txtDiaChi.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ các trường bắt buộc (*)", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật thông tin công ty
                thongTinCongTy.CtyTen = txtTenCongTy.Text.Trim();
                thongTinCongTy.CtySdt = txtSDT.Text.Trim();
                thongTinCongTy.CtyEmail = txtEmail.Text.Trim();
                thongTinCongTy.CtyLogo = txtLogo.Text.Trim();
                thongTinCongTy.CtyDiaChi = txtDiaChi.Text.Trim();
                thongTinCongTy.CtyMoTa = txtMoTa.Text.Trim();

                context.SaveChanges();
                MessageBox.Show("Cập nhật thông tin công ty thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                isEditing = false;
                SetReadOnly(true); // Khóa lại các trường sau khi lưu
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin công ty này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    context.ThongTinCongTies.Remove(thongTinCongTy);
                    context.SaveChanges();

                    ThongTinDeleted?.Invoke(this, EventArgs.Empty);
                    MessageBox.Show("Xóa thông tin công ty thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                LoadThongTin(thongTinCongTy.CtyId); // Tải lại dữ liệu gốc
                isEditing = false;
                SetReadOnly(true);
            }
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SetReadOnly(bool isReadOnly)
        {
            txtTenCongTy.IsReadOnly = isReadOnly;
            txtSDT.IsReadOnly = isReadOnly;
            txtEmail.IsReadOnly = isReadOnly;
            txtLogo.IsReadOnly = isReadOnly;
            txtDiaChi.IsReadOnly = isReadOnly;
            txtMoTa.IsReadOnly = isReadOnly;

            btnLuu.IsEnabled = !isReadOnly;
            btnHuy.IsEnabled = !isReadOnly;
            btnChonFile.IsEnabled = !isReadOnly;
        }
    }
}