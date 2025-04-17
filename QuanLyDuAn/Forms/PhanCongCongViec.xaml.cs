using DocumentFormat.OpenXml.InkML;
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
    /// Interaction logic for PhanCongCongViec.xaml
    /// </summary>
    public partial class PhanCongCongViec : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();

        public event EventHandler PhanCongAdded;

        private int _idDuAn = ThongTinCongViec.idDaAn;
        private int _idCongViec = ThongTinCongViec.idCongViec;
        private int _idNhanVien = ThongTinCongViec.idNguoiNhan;
        public PhanCongCongViec()
        {
            InitializeComponent();
            LoadDuAn();
            LoadCongViec();
            LoadNhanVien();
        }

        private void LoadNhanVien()
        {
            var nhanViens = (from nv in context.NhanViens
                             join tg in context.NhanVienThamGiaDuAns on nv.NvId equals tg.NvId
                             where tg.DaId == _idDuAn
                             select new
                             {
                                 nv.NvId,
                                 nv.NvTen
                             }).ToList();
            cboNhanVien.ItemsSource = nhanViens;
            cboNhanVien.DisplayMemberPath = "NvTen";
            cboNhanVien.SelectedValuePath = "NvId";
        }

        private void LoadDuAn()     // bo cmt 
        {
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;
            var nhanvien = context.NhanViens.FirstOrDefault(nv => nv.NvTaiKhoan == tenTaiKhoan);
            List<Models.DuAn> duAn = new List<Models.DuAn>();
            duAn = context.DuAns.Where(da => da.NhanVienThamGiaDuAns.Any(nv => nv.NvId == nhanvien.NvId)).ToList();
            cboDuAn.ItemsSource = duAn;
            cboDuAn.DisplayMemberPath = "DaTen";
            cboDuAn.SelectedValuePath = "DaId";
        }

        private void LoadCongViec()
        {
            List<Models.CongViec> congViec = new List<Models.CongViec>();
            congViec = context.CongViecs.ToList();

            cboCongViec.ItemsSource = congViec;
            cboCongViec.DisplayMemberPath = "CvTen";
            cboCongViec.SelectedValuePath = "CvId";
        }

        private void cboDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDuAn = cboDuAn.SelectedItem as DuAn;
            if (selectedDuAn != null)
            {
                int idDuAn = (int)selectedDuAn.DaId;
            }
        }

        private void cboCongViec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCongViec = cboCongViec.SelectedItem as CongViec;
            if (selectedCongViec != null)
            {
                int idCongViec = (int)selectedCongViec.CvId;
            }
        }
        private void cboNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNhanVien = cboNhanVien.SelectedItem as NhanVien;
            if (selectedNhanVien != null)
            {
                int idNhanVien = (int)selectedNhanVien.NvId;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboDuAn.SelectedValue = _idDuAn;
            cboCongViec.SelectedValue = _idCongViec;
            cboNhanVien.SelectedValue = _idNhanVien;

            cboDuAn.IsEnabled = false;
            cboCongViec.IsEnabled = false;
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int idCV = (int)cboCongViec.SelectedValue;
            int idDA = (int)cboDuAn.SelectedValue;
            int idNV = (int)cboNhanVien.SelectedValue;

            var phanCongCu = context.PhanCongCongViecs
            .FirstOrDefault(p => p.CvId == idCV);

            if (phanCongCu != null)
            {
                // Hỏi người dùng
                var result = MessageBox.Show(
                    "Công việc này đã được phân công rồi. Bạn có muốn thay đổi người được phân công không?",
                    "Xác nhận thay đổi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    // Người dùng chọn No -> không làm gì cả
                    return;
                }

                // Người dùng chọn Yes -> xóa phân công cũ
                context.PhanCongCongViecs.Remove(phanCongCu);
            }

            Models.PhanCongCongViec phanCong = new Models.PhanCongCongViec();
            phanCong.CvId = idCV;
            phanCong.DaId = idDA;
            phanCong.NvId = idNV;
            context.PhanCongCongViecs.Add(phanCong);
            context.SaveChanges();
            Models.CongViec congViec = context.CongViecs.Find(idCV, idDA);
            congViec.TtMa = "dth";
            context.CongViecs.Update(congViec);
            context.SaveChanges();

            // Thông báo đến MainWindow để làm mới danh sách thông báo
            NotifyMainWindowToRefreshNotifications(idNV);

            MessageBox.Show("Phân công công việc thành công!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
            PhanCongAdded?.Invoke(this, EventArgs.Empty);
            this.Close();

        }
        private void NotifyMainWindowToRefreshNotifications(int nvId)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.NotifyMainWindowToRefreshNotifications(nvId);
            }
        }
    }
}
