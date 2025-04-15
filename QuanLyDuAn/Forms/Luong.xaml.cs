using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyDuAn.Models; // Namespace của DbContext
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace QuanLyDuAn.Forms
{
    public partial class Luong : UserControl
    {
        private List<LuongViewModel> _luongList;
        private List<LuongViewModel> _originalLuongList;
        private int _currentPage = 1;
        private int _pageSize = 10;

        public class LuongViewModel
        {
            public int STT { get; set; }
            public int NvId { get; set; }
            public string ThangNam { get; set; }
            public string MaNhanVien { get; set; }
            public string HoTenNhanVien { get; set; }
            public string Email { get; set; }
            public string SDT { get; set; }
            public string DiaChi { get; set; }
            public decimal LuongCoBan { get; set; }
            public decimal KPI { get; set; }
            public decimal PhuCap { get; set; }
            public decimal TongLuong { get; set; }
        }

        public Luong()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _luongList = new List<LuongViewModel>();
                _originalLuongList = new List<LuongViewModel>();

                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    var query = from nv in context.NhanViens
                                join l in context.Luongs on nv.NvId equals l.NvId into luongGroup
                                from l in luongGroup.DefaultIfEmpty()
                                join k in context.Kpis
                                on new { nv.NvId, ThangNam = l != null ? l.LuongThangNam : (DateOnly?)null }
                                equals new { k.NvId, ThangNam = (DateOnly?)k.KpiThangNam }
                                into kpiGroup
                                from k in kpiGroup.DefaultIfEmpty()
                                select new LuongViewModel
                                {
                                    STT = 0,
                                    NvId = nv.NvId,
                                    ThangNam = l != null ? l.LuongThangNam.ToString("MM/yyyy") : "",
                                    MaNhanVien = nv.NvMa,
                                    HoTenNhanVien = nv.NvTen,
                                    Email = nv.NvEmail,
                                    SDT = nv.NvSdt,
                                    DiaChi = nv.NvDiaChi,
                                    LuongCoBan = nv.NvLuongCoBan, // Sửa lỗi: decimal? -> decimal
                                    KPI = k != null ? (k.KpiPhanTram ?? 0) : 0, // Sửa lỗi: decimal? -> decimal
                                    PhuCap = l != null ? (l.LuongPhuCap ?? 0) : 0, // Sửa lỗi: decimal? -> decimal
                                    TongLuong = l != null ? (l.LuongThucNhan ?? 0) : 0 // Sửa lỗi: decimal? -> decimal
                                };

                    _luongList = query.ToList();
                    for (int i = 0; i < _luongList.Count; i++)
                    {
                        _luongList[i].STT = i + 1;
                    }
                    _originalLuongList = new List<LuongViewModel>(_luongList);

                    UpdatePagination();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            int totalRecords = _luongList.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / _pageSize);

            var pagedData = _luongList.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            dgLuong.ItemsSource = pagedData;
            txtPaginationLuong.Text = $"{_currentPage} trong {totalPages}";
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
        }

        private void btnPrevPageLuong_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void btnNextPageLuong_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)_luongList.Count / _pageSize);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private void btn_SearchByMonthYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpSearchMonthYear.SelectedDate.HasValue)
                {
                    DateTime selectedDate = dpSearchMonthYear.SelectedDate.Value;
                    string monthYear = selectedDate.ToString("MM/yyyy");

                    _luongList = _originalLuongList.FindAll(x => x.ThangNam == monthYear);
                    _currentPage = 1;

                    if (_luongList.Count == 0)
                    {
                        MessageBox.Show($"Không tìm thấy dữ liệu lương cho tháng {monthYear}!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        _luongList = new List<LuongViewModel>(_originalLuongList);
                        dpSearchMonthYear.SelectedDate = null;
                    }

                    UpdatePagination();
                }
                else
                {
                    _luongList = new List<LuongViewModel>(_originalLuongList);
                    _currentPage = 1;
                    UpdatePagination();
                    MessageBox.Show("Vui lòng chọn tháng/năm để tìm kiếm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgLuong_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLuong.SelectedItem is LuongViewModel selectedLuong)
            {
                OpenEditLuongForm(selectedLuong);
            }
        }

        private void btn_EditLuong_Click(object sender, RoutedEventArgs e)
        {
            if (dgLuong.SelectedItem is LuongViewModel selectedLuong)
            {
                OpenEditLuongForm(selectedLuong);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenEditLuongForm(LuongViewModel luong)
        {
            Window window = new Window
            {
                Title = "Chỉnh sửa lương",
                Width = 800,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            // Chuyển đổi chuỗi "MM/yyyy" thành DateTime (gán ngày mặc định là 1)
            DateTime thangNam;
            if (string.IsNullOrEmpty(luong.ThangNam))
            {
                thangNam = DateTime.Now;
            }
            else
            {
                string[] parts = luong.ThangNam.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]);
                thangNam = new DateTime(year, month, 1);
            }

            var editLuongForm = new Edit_Luong(
                luong.NvId,
                thangNam,
                luong.MaNhanVien,
                luong.HoTenNhanVien,
                luong.Email,
                luong.SDT,
                luong.DiaChi,
                luong.LuongCoBan,
                luong.KPI,
                luong.PhuCap
            );

            window.Content = editLuongForm;

            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void btn_ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_luongList == null || _luongList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("DanhSachLuong");

                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Mã nhân viên";
                    worksheet.Cell(1, 3).Value = "Họ tên nhân viên";
                    worksheet.Cell(1, 4).Value = "Email";
                    worksheet.Cell(1, 5).Value = "SĐT";
                    worksheet.Cell(1, 6).Value = "Địa chỉ";
                    worksheet.Cell(1, 7).Value = "Tháng/Năm";
                    worksheet.Cell(1, 8).Value = "Lương cơ bản";
                    worksheet.Cell(1, 9).Value = "KPI";
                    worksheet.Cell(1, 10).Value = "Phụ cấp";
                    worksheet.Cell(1, 11).Value = "Tổng lương";

                    var headerRange = worksheet.Range("A1:K1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (int i = 0; i < _luongList.Count; i++)
                    {
                        var luong = _luongList[i];
                        worksheet.Cell(i + 2, 1).Value = luong.STT;
                        worksheet.Cell(i + 2, 2).Value = luong.MaNhanVien;
                        worksheet.Cell(i + 2, 3).Value = luong.HoTenNhanVien;
                        worksheet.Cell(i + 2, 4).Value = luong.Email;
                        worksheet.Cell(i + 2, 5).Value = luong.SDT;
                        worksheet.Cell(i + 2, 6).Value = luong.DiaChi;
                        worksheet.Cell(i + 2, 7).Value = luong.ThangNam;
                        worksheet.Cell(i + 2, 8).Value = luong.LuongCoBan;
                        worksheet.Cell(i + 2, 8).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Cell(i + 2, 9).Value = luong.KPI;
                        worksheet.Cell(i + 2, 9).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Cell(i + 2, 10).Value = luong.PhuCap;
                        worksheet.Cell(i + 2, 10).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Cell(i + 2, 11).Value = luong.TongLuong;
                        worksheet.Cell(i + 2, 11).Style.NumberFormat.Format = "#,##0.00";
                    }

                    worksheet.Columns().AdjustToContents();

                    string fileName = $"DanhSachLuong_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                    workbook.SaveAs(filePath);

                    MessageBox.Show($"Xuất file Excel thành công!\nĐường dẫn: {filePath}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}