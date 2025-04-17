using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyDuAn.Models;
using QuanLyDuAn.functions;

namespace QuanLyDuAn.Forms
{
    public partial class ThongTin : UserControl
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<ThongTinCongTy> thongTinCongTies = new List<ThongTinCongTy>();
        private int currentPageThongTin = 1;
        private int pageSizeThongTin = 10; // Số bản ghi mỗi trang
        private int totalRecords = 0;
        private List<dynamic> filteredThongTin;

        public event EventHandler ThongTinDeleted;

        public ThongTin()
        {
            InitializeComponent();
            LoadThongTin();
        }

        // Xử lý nút Thêm thông tin
        private void btn_AddInfor_Click(object sender, RoutedEventArgs e)
        {
            AddThongTin addThongTin = new AddThongTin();
            addThongTin.ThongTinAdded += LoadLoadThongTin; // Sửa tên event cho phù hợp
            addThongTin.ShowDialog();
        }

        private void LoadLoadThongTin(object sender, EventArgs e)
        {
            LoadThongTin(); // Tải lại danh sách khi thêm thông tin
        }

        // Tải danh sách thông tin công ty
        public void LoadThongTin()
        {
            var result = from cty in context.ThongTinCongTies
                         select new
                         {
                             cty.CtyId,
                             cty.CtyTen,
                             cty.CtyDiaChi,
                             cty.CtySdt,
                             cty.CtyEmail,
                             cty.CtyLogo,
                             cty.CtyMoTa
                         };
            var resultList = result.ToList();
            dgInfor.ItemsSource = resultList;
            filteredThongTin = resultList.Cast<dynamic>().ToList();
            totalRecords = filteredThongTin.Count;
            currentPageThongTin = 1;
            UpdatePagedData();
        }

        // Cập nhật tổng số bản ghi
        private void UpdateTotalRecords()
        {
            thongTinCongTies = context.ThongTinCongTies.ToList();
            totalRecords = thongTinCongTies.Count;
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
        }

        // Cập nhật dữ liệu phân trang
        private void UpdatePagedData()
        {
            var pagedThongTin = filteredThongTin
                .Skip((currentPageThongTin - 1) * pageSizeThongTin)
                .Take(pageSizeThongTin)
                .Select((cty, index) => new
                {
                    STT = (currentPageThongTin - 1) * pageSizeThongTin + index + 1,
                    cty.CtyId,
                    cty.CtyTen,
                    cty.CtyDiaChi,
                    cty.CtySdt,
                    cty.CtyEmail,
                    cty.CtyLogo,
                    cty.CtyMoTa
                })
                .ToList();

            dgInfor.ItemsSource = pagedThongTin;
            UpdateTotalRecords();
            UpdatePaginationThongTin();
        }

        // Cập nhật thông tin phân trang
        private void UpdatePaginationThongTin()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeThongTin);
            txtPaginationThongTin.Text = $"{currentPageThongTin} trong {totalPages}";
            btnPrevPageThongTin.IsEnabled = currentPageThongTin > 1;
            btnNextPageThongTin.IsEnabled = currentPageThongTin < totalPages;
        }

        // Xử lý nút Trang trước
        private void btnPrevPageThongTin_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageThongTin > 1)
            {
                currentPageThongTin--;
                UpdatePagedData();
            }
        }

        // Xử lý nút Trang sau
        private void btnNextPageThongTin_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSizeThongTin);
            if (currentPageThongTin < totalPages)
            {
                currentPageThongTin++;
                UpdatePagedData();
            }
        }

        // Xử lý double-click trên DataGrid
        private void dgThongTin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedThongTin = dgInfor.SelectedItem;
            if (selectedThongTin != null)
            {
                dynamic cty = selectedThongTin;
                ThongTinChiTiet thongTinChiTiet = new ThongTinChiTiet(cty.CtyId);
                thongTinChiTiet.ThongTinDeleted += LoadLoadThongTin;
                thongTinChiTiet.Show();
            }
        }

        // Hàm tìm kiếm thông tin công ty
        public void Search(string query)
        {
            try
            {
                query = RemoveDiacritics.RemoveDiacriticsMethod(query?.Trim().ToLower() ?? string.Empty);

                if (string.IsNullOrEmpty(query))
                {
                    LoadThongTin();
                }
                else
                {
                    filteredThongTin = filteredThongTin.Where(cty =>
                    {
                        var ctyTenNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cty.CtyTen?.Trim().ToLower() ?? string.Empty);
                        var ctyDiaChiNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cty.CtyDiaChi?.Trim().ToLower() ?? string.Empty);
                        var ctySdtNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cty.CtySdt?.Trim().ToLower() ?? string.Empty);
                        var ctyEmailNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cty.CtyEmail?.Trim().ToLower() ?? string.Empty);
                        var ctyMoTaNormalized = RemoveDiacritics.RemoveDiacriticsMethod(cty.CtyMoTa?.Trim().ToLower() ?? string.Empty);

                        return ctyTenNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               ctyDiaChiNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               ctySdtNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               ctyEmailNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               ctyMoTaNormalized.Contains(query, StringComparison.OrdinalIgnoreCase);
                    }).ToList();

                    totalRecords = filteredThongTin.Count;
                    currentPageThongTin = 1;
                    UpdatePagedData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void frmThongTin_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThongTin();
        }
    }
}