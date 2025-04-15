using DocumentFormat.OpenXml.InkML;
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
    /// Interaction logic for CapNhatCongViec.xaml
    /// </summary>
    public partial class CapNhatCongViec : Window
    {
        public event EventHandler CapNhatAdded;
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();

        private int _idNguoiNhan = ThongTinCongViec.idNguoiNhan;
        private int _idDuAn = ThongTinCongViec.idDaAn;
        private int _idCongViec = ThongTinCongViec.idCongViec;
        public CapNhatCongViec()
        {
            InitializeComponent();
            LoadDuAn();
            LoadCongViec();
            LoadNguoiNhanViec();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbDuAn.SelectedValue = _idDuAn;
            cboCongViec.SelectedValue = _idCongViec;
            cboNguoiNhan.SelectedValue = _idNguoiNhan;

            cbDuAn.IsEnabled = false;
            cboCongViec.IsEnabled = false;
            cboNguoiNhan.IsEnabled = false;
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

        private void LoadCongViec()
        {
            List<Models.CongViec> congViec = new List<Models.CongViec>();
            congViec = context.CongViecs.ToList();

            cboCongViec.ItemsSource = congViec;
            cboCongViec.DisplayMemberPath = "CvTen";
            cboCongViec.SelectedValuePath = "CvId";
        }
        private void LoadNguoiNhanViec()
        {
            List<Models.NhanVien> nguoiNhanViec = new List<Models.NhanVien>();
            nguoiNhanViec = context.NhanViens.ToList();

            cboNguoiNhan.ItemsSource = nguoiNhanViec;
            cboNguoiNhan.DisplayMemberPath = "NvTen";
            cboNguoiNhan.SelectedValuePath = "NvId";
        }

        private void cbDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDuAn = cbDuAn.SelectedItem as DuAn;
            if (selectedDuAn != null)
            {
                int idNhanVien = (int)selectedDuAn.DaId;
            }
        }

        private void cboCongViec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCongViec = cboCongViec.SelectedItem as CongViec;
            if (selectedCongViec != null)
            {
                int idNhanVien = (int)selectedCongViec.CvId;
            }
        }

        private void cboNguoiNhan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNhanVien = cboNguoiNhan.SelectedItem as NhanVien;
            if (selectedNhanVien != null)
            {
                int idNhanVien = (int)selectedNhanVien.NvId;
            }
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

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMoTa.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả cho cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Models.CapNhatCongViec capNhat = new Models.CapNhatCongViec();

            string daID = cbDuAn.SelectedValue.ToString();
            string cvID = cboCongViec.SelectedValue.ToString();
            string nvID = cboNguoiNhan.SelectedValue.ToString();

            capNhat.DaId = int.Parse(daID);
            capNhat.CvId = int.Parse(cvID);
            capNhat.NvId = int.Parse(nvID);
            capNhat.CnMoTa = txtMoTa.Text.ToString();
            capNhat.CnFile = txtTenTapTin.Text.ToString();
            capNhat.CvPath = txtDuongDan.Text.ToString();

            context.CapNhatCongViecs.Add(capNhat);
            context.SaveChanges();
            MessageBox.Show("Đã thêm cập nhật cho công việc!.", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
            CapNhatAdded?.Invoke(this, EventArgs.Empty);
            this.Close();

        }
    }
}
