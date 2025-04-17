using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using QuanLyDuAn.Forms;
using System.Windows.Input;
using QuanLyDuAn.Models; // bo cmt
using QuanLyDuAn.functions;

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachNhanVien : UserControl
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<Models.NhanVien> nhanVien = new List<Models.NhanVien>(); 
        //private List<NhanVien> danhSachNhanVien = new List<NhanVien>();
        private int currentPageNhanVien = 1;
        private int pageSizeNhanVien = 10; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private List<dynamic> filteredNhanVien;

        public DanhSachNhanVien()
        {
            InitializeComponent();
            LoadNhanVien();
        }

        // Xử lý nút Thêm nhân viên
        private void btn_AddNhanVien_Click(object sender, RoutedEventArgs e)
        {
            
            AddNhanVien addNhanVien = new AddNhanVien();
            addNhanVien.NhanVienAdded += LoadLoadNhanVien;
            addNhanVien.ShowDialog();
        }
        private void LoadLoadNhanVien(object sender, EventArgs e)
        {
            // Gọi lại LoadNhanVien khi nhân viên được thêm
            LoadNhanVien();
        }

        public void LoadNhanVien()         //bo cmt
        {
            // nhanVien = context.NhanViens.Include(nv => nv.QMaNavigation).ToList();
            // dgNhanVien.ItemsSource = nhanVien;
            var result = from nv in context.NhanViens
                         join q in context.Quyens on nv.QMa equals q.QMa
                         select new
                         {
                             nv.NvId,
                             nv.NvMa,
                             nv.NvTen,
                             nv.NvGioiTinh,
                             nv.NvNgaySinh,
                             nv.NvSdt,
                             nv.NvEmail,
                             nv.NvTaiKhoan,
                             nv.NvMatKhau,
                             nv.NvLuongCoBan,
                             q.QTen
                         };
            var resultlist  = result.ToList();
            dgNhanVien.ItemsSource = resultlist;
            filteredNhanVien = resultlist.Cast<dynamic>().ToList();
            totalRecords = filteredNhanVien.Count;
            currentPageNhanVien = 1;
            UpdatePagedData();
        }

        // Cập nhật tổng số bản ghi
        private void UpdateTotalRecords() //bo cmt
        {
            nhanVien = context.NhanViens.ToList();
            int totalRecords = nhanVien.Count;
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";

        }

        // Hiển thị nhân viên theo trang
        private void HienThiNhanVienTheoTrang(int page)     //bo cmt
        {
            var nhanVienTheoTrang = nhanVien
                .Skip((page - 1) * pageSizeNhanVien)
                .Take(pageSizeNhanVien)
                .ToList();

            dgNhanVien.ItemsSource = nhanVienTheoTrang;
            UpdatePaginationNhanVien();
        }

        // Cập nhật thông tin phân trang
        private void UpdatePaginationNhanVien()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeNhanVien);
            txtPaginationNhanVien.Text = $"{currentPageNhanVien} trong {totalPages}";
            btnPrevPageNhanVien.IsEnabled = currentPageNhanVien > 1;
            btnNextPageNhanVien.IsEnabled = currentPageNhanVien < totalPages;
        }

        // Xử lý nút Trang trước
        private void btnPrevPageNhanVien_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNhanVien > 1)
            {
                currentPageNhanVien--;
                UpdatePagedData();
            }
        }

        // Xử lý nút Trang sau
        private void btnNextPageNhanVien_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeNhanVien);
            if (currentPageNhanVien < totalPages)
            {
                currentPageNhanVien++;
                UpdatePagedData();
            }
        }

        private void frmNhanVien_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNhanVien();
        }

        private void dgNhanVien_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)        //bo cmt
        {
            var selectNhanVien = dgNhanVien.SelectedItem as NhanVien;

            if (selectNhanVien != null)
            {
                // Mở Form B và truyền mã nhân viên
                ThongTinNhanVien ThongTinNhanVien = new ThongTinNhanVien(selectNhanVien.NvMa);
                ThongTinNhanVien.NhanVienDeleted += LoadLoadNhanVien;
                ThongTinNhanVien.Show();
            }
        }

        // Hàm tìm kiếm nhân viên
        public void Search(string query)
        {
            try
            {
                query = RemoveDiacritics.RemoveDiacriticsMethod(query?.Trim().ToLower() ?? string.Empty);

                if (string.IsNullOrEmpty(query))
                {
                    LoadNhanVien(); // Tải lại toàn bộ danh sách nếu query rỗng
                }
                else
                {
                    filteredNhanVien = filteredNhanVien.Where(nv =>
                    {
                        var nvMaNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.NvMa?.Trim().ToLower() ?? string.Empty);
                        var nvTenNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.NvTen?.Trim().ToLower() ?? string.Empty);
                        var nvEmailNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.NvEmail?.Trim().ToLower() ?? string.Empty);
                        var nvSdtNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.NvSdt?.Trim().ToLower() ?? string.Empty);
                        var nvTaiKhoanNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.NvTaiKhoan?.Trim().ToLower() ?? string.Empty);
                        var qTenNormalized = RemoveDiacritics.RemoveDiacriticsMethod(nv.QTen?.Trim().ToLower() ?? string.Empty);

                        return nvMaNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nvTenNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nvEmailNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nvSdtNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nvTaiKhoanNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               qTenNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               nv.NvNgaySinh.ToString("dd/MM/yyyy").ToLower().Contains(query, StringComparison.OrdinalIgnoreCase);
                    }).ToList();

                    totalRecords = filteredNhanVien.Count;
                    currentPageNhanVien = 1;
                    UpdatePagedData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Cập nhật dữ liệu phân trang
        private void UpdatePagedData()
        {
            var pagedNhanVien = filteredNhanVien
                .Skip((currentPageNhanVien - 1) * pageSizeNhanVien)
                .Take(pageSizeNhanVien)
                .Select((nv, index) => new
                {
                    STT = (currentPageNhanVien - 1) * pageSizeNhanVien + index + 1,
                    nv.NvId,
                    nv.NvMa,
                    nv.NvTen,
                    nv.NvGioiTinh,
                    nv.NvNgaySinh,
                    nv.NvSdt,
                    nv.NvEmail,
                    nv.NvTaiKhoan,
                    nv.NvMatKhau,
                    nv.NvLuongCoBan,
                    nv.QTen
                })
                .ToList();

            dgNhanVien.ItemsSource = pagedNhanVien;
            UpdateTotalRecords();
            UpdatePaginationNhanVien();
        }
    }
}