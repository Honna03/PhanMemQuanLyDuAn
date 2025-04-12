using Microsoft.Win32;
using QuanLyDuAn.Models;
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
    /// Interaction logic for AddCongViec.xaml
    /// </summary>
    public partial class AddCongViec : Window
    {
        public event EventHandler CongViecAdded;

        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();            //bo cmt
        public AddCongViec()
        {
            InitializeComponent();
            LoadDuAn();
        }

        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tất cả các file (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);
                //string folderPath = System.IO.Path.GetDirectoryName(filePath);

                txtTenTapTin.Text = fileName;
                txtDuongDan.Text = filePath;
            }
        }

        private void txtNgayBD_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgayBD.Text == "dd/MM/yyyy")
            {
                txtNgayBD.Text = "";
                txtNgayBD.Foreground = Brushes.Black;
            }
        }

        private void txtNgayBD_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNgayBD.Text))
            {
                txtNgayBD.Text = "dd/MM/yyyy";
                txtNgayBD.Foreground = Brushes.Gray;
            }
        }

        private void txtNgayKT_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgayKT.Text == "dd/MM/yyyy")
            {
                txtNgayKT.Text = "";
                txtNgayKT.Foreground = Brushes.Black;
            }
        }

        private void txtNgayKT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNgayKT.Text))
            {
                txtNgayKT.Text = "dd/MM/yyyy";
                txtNgayKT.Foreground = Brushes.Gray;
            }
        }

        private void LoadDuAn()     // bo cmt 
        {
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;
            var nhanvien = context.NhanViens.FirstOrDefault(nv => nv.NvTaiKhoan == tenTaiKhoan);
            List<Models.DuAn> duAn = new List<Models.DuAn>();
            duAn = context.DuAns.Where(da => da.NhanVienThamGiaDuAns.Any(nv => nv.NvId == nhanvien.NvId)).ToList();
            cbDuAn.ItemsSource = duAn;
            cbDuAn.DisplayMemberPath = "DaTen";
            cbDuAn.SelectedValuePath = "DaId";
        }

        private void cbDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDuAn = cbDuAn.SelectedItem as DuAn;
            if (selectedDuAn != null)
            {
                int daID = selectedDuAn.DaId;
                //messagebox.show($"mã quyền đã chọn: {maquyen}");
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            Models.CongViec congViec = new Models.CongViec();
            if (cbDuAn.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn dự án!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenCV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên công việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateOnly NgayBatDau;
            if (DateOnly.TryParseExact(txtNgayBD.Text, "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out NgayBatDau))
            {
                // Chuyển đổi thành công, gán giá trị cho nhân viên
                congViec.CvBatDau = NgayBatDau;
            }
            else
            {
                // Thông báo lỗi nếu định dạng không hợp lệ
                MessageBox.Show("Ngày bắt đầu không hợp lệ, vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //aaa
            DateOnly NgayKetThuc;
            if (DateOnly.TryParseExact(txtNgayKT.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out NgayKetThuc))
            {
                // Kiểm tra ngày kết thúc phải lớn hơn ngày bắt đầu
                if (NgayKetThuc > NgayBatDau)
                {
                    congViec.CvKetThuc = NgayKetThuc;
                }
                else
                {
                    MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Ngày kết thúc không hợp lệ, vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;
            var nhanvien = context.NhanViens.FirstOrDefault(nv => nv.NvTaiKhoan == tenTaiKhoan);

            string daID = cbDuAn.SelectedValue.ToString();
            congViec.DaId = int.Parse(daID);
            congViec.CvTen = txtTenCV.Text;
            congViec.CvMoTa = txtMoTa.Text;
            congViec.CvFile = txtTenTapTin.Text;
            congViec.CvPath = txtDuongDan.Text;
            congViec.TtMa = "cht";
            congViec.NvIdNguoiTao = nhanvien.NvId;
            context.CongViecs.Add(congViec);
            context.SaveChanges();
            MessageBox.Show("Đã thêm nhân viên thành công!.", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);

            CongViecAdded?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
