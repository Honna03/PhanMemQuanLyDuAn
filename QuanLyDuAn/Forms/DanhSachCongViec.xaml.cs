using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Forms;
using QuanLyDuAn.Models;
using QuanLyDuAn.functions;

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachCongViec : UserControl
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
         List<Models.CongViec> congViec = new List<Models.CongViec>();
        private List<dynamic> filteredCongViec;
        private int currentPageCongViec = 1;
        private int pageSizeCongViec = 10; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private bool isTabDaTao = true;
        public DanhSachCongViec()
        {
            InitializeComponent();
            LoadCongViecDaTao();
            
        }
        private void LoadCongViecDuocGiao()     //bo cmt
        {
            //congViec = context.CongViecs.Include(cv => cv.NhanVienThamGiaDuAn).ThenInclude(tg => tg.Da)
            //    .Include(cv => cv.TtMaNavigation).ToList();

            //dgCongViec.ItemsSource = congViec;
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;


            var ketQua = from cv in context.CongViecs
                         join da in context.DuAns on cv.DaId equals da.DaId
                         join nt in context.NhanViens on cv.NvIdNguoiTao equals nt.NvId
                         join tt in context.TrangThais on cv.TtMa equals tt.TtMa
                         join pc in context.PhanCongCongViecs on cv.CvId equals pc.CvId into pcGroup
                         from pc in pcGroup.DefaultIfEmpty()
                         join nv in context.NhanViens on pc.NvId equals nv.NvId into nvGroup
                         from nv in nvGroup.DefaultIfEmpty()
                         where (nv != null && nv.NvTaiKhoan == tenTaiKhoan)
                         select new
                         {
                             CvMa = cv.CvMa,
                             CvTen = cv.CvTen,
                             DaTen = da.DaTen,
                             CvMoTa = cv.CvMoTa,
                             CvBatDau = cv.CvBatDau,
                             CvKetThuc = cv.CvKetThuc,
                             NguoiTao = nt.NvTen,
                             NguoiNhan = nv != null ? nv.NvTen : null,
                             TrangThai = tt.TtTen,
                             ThoiGianHoanThanh = cv.CvThoiGianHoanThanh
                         };
            var ketQuaList = ketQua.ToList();
            filteredCongViec = ketQuaList.Cast<dynamic>().ToList();
            dgCongViec.ItemsSource = ketQuaList;
            totalRecords = filteredCongViec.Count;
            currentPageCongViec = 1;
            isTabDaTao = true;
            UpdatePagedData();
            UpdatePaginationCongViec();

        }
        public void LoadCongViecDaTao()
        {
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;


            var ketQua = from cv in context.CongViecs
                         join da in context.DuAns on cv.DaId equals da.DaId
                         join nt in context.NhanViens on cv.NvIdNguoiTao equals nt.NvId
                         join tt in context.TrangThais on cv.TtMa equals tt.TtMa
                         join pc in context.PhanCongCongViecs on cv.CvId equals pc.CvId into pcGroup
                         from pc in pcGroup.DefaultIfEmpty()
                         join nv in context.NhanViens on pc.NvId equals nv.NvId into nvGroup
                         from nv in nvGroup.DefaultIfEmpty()
                         where nt.NvTaiKhoan == tenTaiKhoan
                         select new
                         {
                             CvMa = cv.CvMa,
                             CvTen = cv.CvTen,
                             DaTen = da.DaTen,
                             CvMoTa = cv.CvMoTa,
                             CvBatDau = cv.CvBatDau,
                             CvKetThuc = cv.CvKetThuc,
                             NguoiTao = nt.NvTen,
                             NguoiNhan = nv != null ? nv.NvTen : null,
                             TrangThai = tt.TtTen,
                             ThoiGianHoanThanh = cv.CvThoiGianHoanThanh
                         };
            var ketQuaList = ketQua.ToList();
            filteredCongViec = ketQuaList.Cast<dynamic>().ToList();
            dgCongViec.ItemsSource = ketQuaList;
            totalRecords = filteredCongViec.Count;
            UpdatePagedData();
            UpdatePaginationCongViec();
        }

        private void LoadLoadCongViec(object sender, EventArgs e)
        {
            LoadCongViecDaTao();
        }
        private void btn_AddCongViec_Click_1(object sender, RoutedEventArgs e)
        {

            AddCongViec addCongViec = new AddCongViec();
            addCongViec.CongViecAdded += LoadLoadCongViec;
            addCongViec.ShowDialog();
        }

        private void DanhSachCongViec1_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCongViecDaTao();
        }

        private void dgCongViec_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectCongViec = (dynamic)dgCongViec.SelectedItem;

            if (selectCongViec != null)
            {
                // Mở Form B và truyền mã nhân viên
                ThongTinCongViec thongTinCongViec = new ThongTinCongViec(selectCongViec.CvMa);
                thongTinCongViec.CongViecDeleted += LoadLoadCongViec;
                thongTinCongViec.Show();
            }
        }

        private void btnDaTao_Click(object sender, RoutedEventArgs e)
        {
            LoadCongViecDaTao();
        }

        private void btnDuocGiao_Click(object sender, RoutedEventArgs e)
        {
            LoadCongViecDuocGiao();
        }

        public void Search(string query)
        {
            try
            {
                query = RemoveDiacritics.RemoveDiacriticsMethod(query?.Trim().ToLower() ?? string.Empty);

                if (string.IsNullOrEmpty(query))
                {
                    if (isTabDaTao)
                        LoadCongViecDaTao();
                    else
                        LoadCongViecDuocGiao();
                }
                else
                {
                    filteredCongViec = filteredCongViec.Where(cv =>
                    {
                        var cvTenNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cv.CvTen?.Trim().ToLower() ?? string.Empty);
                        var daTenNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cv.DaTen?.Trim().ToLower() ?? string.Empty);
                        var nguoiTaoNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cv.NguoiTao?.Trim().ToLower() ?? string.Empty);
                        var nguoiNhanNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cv.NguoiNhan?.Trim().ToLower() ?? string.Empty);
                        var trangThaiNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cv.TrangThai?.Trim().ToLower() ?? string.Empty);

                        return cvTenNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               daTenNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nguoiTaoNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nguoiNhanNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               trangThaiNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               cv.CvBatDau.ToString("dd/MM/yyyy").ToLower().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               cv.CvKetThuc.ToString("dd/MM/yyyy").ToLower().Contains(query, StringComparison.OrdinalIgnoreCase);
                    }).ToList();

                    totalRecords = filteredCongViec.Count;
                    currentPageCongViec = 1;
                    UpdatePagedData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cập nhật tổng số bản ghi
        private void UpdateTotalRecords() //bo cmt
        {
            congViec = context.CongViecs.ToList();
            int totalRecords = congViec.Count;
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";

        }

        // Hiển thị nhân viên theo trang
        private void HienThiCongViecTheoTrang(int page)     //bo cmt
        {
            var congviectheotrang = congViec
                .Skip((page - 1) * pageSizeCongViec)
                .Take(pageSizeCongViec)
                .ToList();

            dgCongViec.ItemsSource = congviectheotrang;
            UpdatePaginationCongViec();
        }

        // Cập nhật thông tin phân trang
        private void UpdatePaginationCongViec()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeCongViec);
            if (txtPaginationCongViec != null)
            {
                txtPaginationCongViec.Text = $"{currentPageCongViec} trong {totalPages}";
            }
            if (btnPrevPageCongViec != null)
            {
                btnPrevPageCongViec.IsEnabled = currentPageCongViec > 1;
            }
            if (btnNextPageCongViec != null)
            {
                btnNextPageCongViec.IsEnabled = currentPageCongViec < totalPages;
            }
            if (txtTotalRecords != null)
            {
                txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
            }
        }

        private void btnPrevPageCongViec_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageCongViec > 1)
            {
                currentPageCongViec--;
                UpdatePagedData();
            }
        }

        private void btnNextPageCongViec_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeCongViec);
            if (currentPageCongViec < totalPages)
            {
                currentPageCongViec++;
                UpdatePagedData();
            }
        }

        private void UpdatePagedData()
        {
            var pagedCongViec = filteredCongViec
                .Skip((currentPageCongViec - 1) * pageSizeCongViec)
                .Take(pageSizeCongViec)
                .Select((cv, index) => new
                {
                    STT = (currentPageCongViec - 1) * pageSizeCongViec + index + 1,
                    cv.CvMa,
                    cv.CvTen,
                    cv.DaTen,
                    cv.CvMoTa,
                    cv.CvBatDau,
                    cv.CvKetThuc,
                    cv.NguoiTao,
                    cv.NguoiNhan,
                    cv.TrangThai,
                    cv.ThoiGianHoanThanh
                })
                .ToList();

            if (dgCongViec != null)
            {
                dgCongViec.ItemsSource = pagedCongViec;
            }

            UpdatePaginationCongViec();
        }
    }
}
