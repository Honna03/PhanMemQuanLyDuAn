using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;
using QuanLyDuAn.functions;

namespace QuanLyDuAn.Forms
{
    public partial class KPI : UserControl
    {
        private ThucTapQuanLyDuAnContext _context = new ThucTapQuanLyDuAnContext();
        private int currentPage = 1;
        private int recordsPerPage = 30;
        private int totalRecords = 0;
        private List<KPIViewModel> kpiData;
        private List<KPIViewModel> allKpiData; // Lưu toàn bộ dữ liệu cho việc xuất Excel và tìm kiếm
        private List<KPIViewModel> filteredKpiData; // Lưu dữ liệu sau khi lọc để phân trang
        private List<KPIViewModel> completeKpiData; // Lưu trữ toàn bộ dữ liệu KPI không phụ thuộc vào bộ lọc ngày tháng

        public KPI()
        {
            InitializeComponent();
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;

            // Load toàn bộ dữ liệu KPI
            LoadAllKPIData();

            // Load dữ liệu tháng mới nhất để hiển thị ban đầu
            LoadLatestMonthData();
        }

        private void LoadAllKPIData()
        {
            try
            {
                // Load toàn bộ dữ liệu KPI không phụ thuộc vào khoảng thời gian
                var query = from kpi in _context.Kpis
                            join nv in _context.NhanViens on kpi.NvId equals nv.NvId
                            orderby kpi.KpiThangNam descending
                            select new KPIViewModel
                            {
                                MaNhanVien = nv.NvMa,
                                HoTenNhanVien = nv.NvTen,
                                GioiTinh = nv.NvGioiTinh,
                                SDT = nv.NvSdt,
                                Email = nv.NvEmail,
                                TaiKhoan = nv.NvTaiKhoan,
                                ThoiGianKPI = kpi.KpiThangNam,
                                PhanTramKPI = kpi.KpiPhanTram
                            };

                // Lưu toàn bộ dữ liệu cho việc tìm kiếm
                completeKpiData = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải toàn bộ dữ liệu KPI: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadLatestMonthData()
        {
            try
            {
                // Lấy ngày đầu tháng hiện tại
                DateTime now = DateTime.Today;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                // Load dữ liệu của tháng hiện tại nhưng không cập nhật DatePicker
                LoadKPI(firstDayOfMonth, lastDayOfMonth);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tháng hiện tại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadKPI(DateTime startDate, DateTime endDate)
        {
            try
            {
                var start = DateOnly.FromDateTime(startDate);
                var end = DateOnly.FromDateTime(endDate);
                // Join bảng KPI và bảng NhanVien để lấy dữ liệu theo khoảng thời gian
                var query = from kpi in _context.Kpis
                            join nv in _context.NhanViens on kpi.NvId equals nv.NvId
                            where kpi.KpiThangNam >= start && kpi.KpiThangNam <= end
                            orderby kpi.KpiThangNam descending
                            select new KPIViewModel
                            {
                                MaNhanVien = nv.NvMa,
                                HoTenNhanVien = nv.NvTen,
                                GioiTinh = nv.NvGioiTinh,
                                SDT = nv.NvSdt,
                                Email = nv.NvEmail,
                                TaiKhoan = nv.NvTaiKhoan,
                                ThoiGianKPI = kpi.KpiThangNam,
                                PhanTramKPI = kpi.KpiPhanTram
                            };

                // Lưu dữ liệu lọc theo ngày tháng
                allKpiData = query.ToList();
                filteredKpiData = new List<KPIViewModel>(allKpiData); // Ban đầu, dữ liệu lọc bằng toàn bộ dữ liệu

                // Đếm tổng số bản ghi
                totalRecords = filteredKpiData.Count;

                // Lấy dữ liệu theo trang
                UpdatePagedData();

                // Cập nhật thông tin phân trang và tổng số bản ghi
                UpdatePaginationInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu KPI: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagedData()
        {
            // Lấy dữ liệu theo trang từ filteredKpiData
            kpiData = filteredKpiData.Skip((currentPage - 1) * recordsPerPage)
                                     .Take(recordsPerPage)
                                     .ToList();

            // Thêm số thứ tự
            for (int i = 0; i < kpiData.Count; i++)
            {
                kpiData[i].STT = (currentPage - 1) * recordsPerPage + i + 1;
            }

            // Gán dữ liệu cho DataGrid
            dgKPI.ItemsSource = kpiData;
        }

        private void UpdatePaginationInfo()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / recordsPerPage);

            txtPaginationKPI.Text = $"{currentPage} trong {totalPages}";

            // Cập nhật trạng thái các nút điều hướng
            btnPrevPageKPI.IsEnabled = currentPage > 1;
            btnNextPageKPI.IsEnabled = currentPage < totalPages;

            // Cập nhật tổng số bản ghi
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
        }

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                currentPage = 1;
                LoadKPI(dpStartDate.SelectedDate.Value, dpEndDate.SelectedDate.Value);
            }
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                currentPage = 1;
                LoadKPI(dpStartDate.SelectedDate.Value, dpEndDate.SelectedDate.Value);
            }
        }

        private void btnPrevPageKPI_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagedData();
                UpdatePaginationInfo();
            }
        }

        private void btnNextPageKPI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int totalPages = (int)Math.Ceiling((double)totalRecords / recordsPerPage);

                if (currentPage < totalPages)
                {
                    currentPage++;
                    UpdatePagedData();
                    UpdatePaginationInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển trang: {ex.Message}");
            }
        }

        private void btn_Loading_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadLatestMonthData();
        }

        private void dgKPI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgKPI.SelectedItem is KPIViewModel selectedItem && selectedItem.ThoiGianKPI.HasValue)
            {
                try
                {
                    // Lấy nv_ID từ MaNhanVien (loại bỏ tiền tố "NV")
                    int nvId = int.Parse(selectedItem.MaNhanVien.Replace("NV", ""));

                    // Chuyển DateOnly thành DateTime để truyền vào ChiTietTienDo
                    DateTime thoiGianKPI = selectedItem.ThoiGianKPI.Value.ToDateTime(TimeOnly.MinValue);

                    // Tạo instance của ChiTietTienDo với tham số
                    var chiTietTienDo = new ChiTietTienDo(nvId, thoiGianKPI);

                    Window editWindow = new Window
                    {
                        Title = $"Chi tiết tiến độ công việc {selectedItem.HoTenNhanVien} tháng {thoiGianKPI.ToString("MM/yyyy")}",
                        Width = 800,
                        Height = 600,
                        Content = chiTietTienDo,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ResizeMode = ResizeMode.NoResize
                    };
                    editWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở chi tiết: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btn_XuatExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Xác định dữ liệu sẽ xuất: filteredKpiData thay vì allKpiData để xuất đúng dữ liệu đang hiển thị
                var dataToExport = filteredKpiData;

                if (dataToExport == null || dataToExport.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất ra Excel!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Tạo SaveFileDialog để người dùng chọn nơi lưu file
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"KPI_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        // Tạo worksheet
                        var worksheet = workbook.Worksheets.Add("KPI");

                        // Thiết lập header
                        worksheet.Cell(1, 1).Value = "STT";
                        worksheet.Cell(1, 2).Value = "Mã nhân viên";
                        worksheet.Cell(1, 3).Value = "Họ tên nhân viên";
                        worksheet.Cell(1, 4).Value = "Giới tính";
                        worksheet.Cell(1, 5).Value = "Số điện thoại";
                        worksheet.Cell(1, 6).Value = "Email";
                        worksheet.Cell(1, 7).Value = "Tài khoản";
                        worksheet.Cell(1, 8).Value = "Thời gian";
                        worksheet.Cell(1, 9).Value = "Phần trăm KPI";

                        // Format header
                        var headerRange = worksheet.Range(1, 1, 1, 9);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Điền dữ liệu
                        for (int i = 0; i < dataToExport.Count; i++)
                        {
                            var item = dataToExport[i];
                            int row = i + 2;

                            worksheet.Cell(row, 1).Value = i + 1; // STT
                            worksheet.Cell(row, 2).Value = item.MaNhanVien;
                            worksheet.Cell(row, 3).Value = item.HoTenNhanVien;
                            worksheet.Cell(row, 4).Value = item.GioiTinh;
                            worksheet.Cell(row, 5).Value = item.SDT;
                            worksheet.Cell(row, 6).Value = item.Email;
                            worksheet.Cell(row, 7).Value = item.TaiKhoan;

                            if (item.ThoiGianKPI.HasValue)
                            {
                                worksheet.Cell(row, 8).Value = item.ThoiGianKPI.Value.ToString("MM/yyyy");
                            }

                            worksheet.Cell(row, 9).Value = item.PhanTramKPI + "%";
                        }

                        // Auto-fit cột
                        worksheet.Columns().AdjustToContents();

                        // Lưu file
                        workbook.SaveAs(saveDialog.FileName);

                        MessageBox.Show($"Đã xuất dữ liệu thành công tới: {saveDialog.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Thêm phương thức Search để hỗ trợ tìm kiếm từ MainWindow
        public void Search(string query)
        {
            try
            {
                query = RemoveDiacritics.RemoveDiacriticsMethod(query?.Trim().ToLower() ?? string.Empty);

                if (string.IsNullOrEmpty(query))
                {
                    // Nếu từ khóa rỗng và có lọc theo ngày
                    if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
                    {
                        filteredKpiData = new List<KPIViewModel>(allKpiData);
                    }
                    else
                    {
                        // Nếu không có lọc theo ngày, hiển thị tháng hiện tại
                        LoadLatestMonthData();
                        return;
                    }
                }
                else
                {
                    // Tìm kiếm trong toàn bộ dữ liệu, không phụ thuộc vào bộ lọc ngày tháng
                    filteredKpiData = completeKpiData.Where(kpi =>
                    {
                        var maNhanVienNormalized = RemoveDiacritics.RemoveDiacriticsMethod(kpi.MaNhanVien?.Trim().ToLower() ?? string.Empty);
                        var hoTenNhanVienNormalized = RemoveDiacritics.RemoveDiacriticsMethod(kpi.HoTenNhanVien?.Trim().ToLower() ?? string.Empty);
                        var sdtNormalized = RemoveDiacritics.RemoveDiacriticsMethod(kpi.SDT?.Trim().ToLower() ?? string.Empty);
                        var taiKhoanNormalized = RemoveDiacritics.RemoveDiacriticsMethod(kpi.TaiKhoan?.Trim().ToLower() ?? string.Empty);

                        return maNhanVienNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               hoTenNhanVienNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               sdtNormalized.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               taiKhoanNormalized.Contains(query, StringComparison.OrdinalIgnoreCase);
                    }).ToList();

                    // Bỏ qua bộ lọc ngày tháng khi đang tìm kiếm
                    dpStartDate.SelectedDate = null;
                    dpEndDate.SelectedDate = null;
                }

                // Cập nhật tổng số bản ghi sau khi lọc
                totalRecords = filteredKpiData.Count;

                // Đặt lại trang về 1 sau khi tìm kiếm
                currentPage = 1;

                // Cập nhật dữ liệu phân trang và giao diện
                UpdatePagedData();
                UpdatePaginationInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    public class KPIViewModel
        {
            public int STT { get; set; }
            public string MaNhanVien { get; set; }
            public string HoTenNhanVien { get; set; }
            public string GioiTinh { get; set; }
            public string SDT { get; set; }
            public string Email { get; set; }
            public string TaiKhoan { get; set; }
            public DateOnly? ThoiGianKPI { get; set; }
            public decimal? PhanTramKPI { get; set; }
        }
    }