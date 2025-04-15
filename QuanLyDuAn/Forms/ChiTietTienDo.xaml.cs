using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDuAn.Models;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace QuanLyDuAn.Forms
{
    public partial class ChiTietTienDo : UserControl
    {
        private readonly ThucTapQuanLyDuAnContext _context = new ThucTapQuanLyDuAnContext();
        private readonly int nvId;
        private readonly DateOnly kpiThoiGian;

        public ChiTietTienDo(int nhanVienId, DateTime thoiGianKPI)
        {
            InitializeComponent();
            nvId = nhanVienId;
            kpiThoiGian = DateOnly.FromDateTime(thoiGianKPI);
            this.Loaded += ChiTietTienDo_Loaded;
        }

        private void ChiTietTienDo_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCTTD();
        }

        private void LoadCTTD()
        {
            var chiTietList = new ObservableCollection<ChiTietTienDoItem>();
            try
            {
                // Tính khoảng thời gian: đầu tháng đến cuối tháng của kpiThoiGian
                DateOnly startDate = kpiThoiGian;
                DateOnly endDate = kpiThoiGian.AddMonths(1).AddDays(-1);

                // Truy vấn dữ liệu từ các bảng liên quan
                var query = from pccv in _context.PhanCongCongViecs
                            join cv in _context.CongViecs on new { pccv.CvId, pccv.DaId } equals new { cv.CvId, cv.DaId }
                            join nv in _context.NhanViens on pccv.NvId equals nv.NvId
                            join tt in _context.TrangThais on cv.TtMa equals tt.TtMa
                            where pccv.NvId == nvId
                                  && cv.CvBatDau >= startDate
                                  && (cv.CvKetThuc == null || cv.CvKetThuc <= endDate)
                            orderby cv.CvBatDau
                            select new ChiTietTienDoItem
                            {
                                STT = 0,
                                MaNhanVien = nv.NvMa,
                                HoTenNhanVien = nv.NvTen,
                                TenCongViec = cv.CvTen,
                                ThoiGianBatDau = cv.CvBatDau,
                                ThoiGianKetThuc = cv.CvKetThuc,
                                TrangThai = tt.TtTen
                            };

                // Chuyển kết quả thành danh sách và thêm STT
                var result = query.ToList();
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].STT = i + 1;
                    chiTietList.Add(result[i]);
                }

                // Gán dữ liệu cho DataGrid
                dgChiTietTienDo.ItemsSource = chiTietList;

                if (chiTietList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu công việc trong khoảng thời gian này.",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\nStackTrace: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_XuatExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ ItemsSource của DataGrid
                var dataToExport = dgChiTietTienDo.ItemsSource as IEnumerable<ChiTietTienDoItem>;

                // Kiểm tra nếu không có dữ liệu
                if (dataToExport == null || !dataToExport.Any())
                {
                    MessageBox.Show("Không có dữ liệu để xuất ra Excel!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Chuyển dữ liệu thành List để xử lý
                var dataList = dataToExport.ToList();

                // Tạo SaveFileDialog để người dùng chọn nơi lưu file
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"ChiTietTienDo_{kpiThoiGian:yyyyMM}_{DateTime.Now:HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        // Tạo worksheet
                        var worksheet = workbook.Worksheets.Add("Chi tiết tiến độ");

                        // Thiết lập header
                        worksheet.Cell(1, 1).Value = "STT";
                        worksheet.Cell(1, 2).Value = "Mã nhân viên";
                        worksheet.Cell(1, 3).Value = "Họ tên nhân viên";
                        worksheet.Cell(1, 4).Value = "Tên công việc";
                        worksheet.Cell(1, 5).Value = "Thời gian bắt đầu";
                        worksheet.Cell(1, 6).Value = "Thời gian kết thúc";
                        worksheet.Cell(1, 7).Value = "Trạng thái";

                        // Format header
                        var headerRange = worksheet.Range(1, 1, 1, 7);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Điền dữ liệu
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            var item = dataList[i];
                            int row = i + 2;

                            worksheet.Cell(row, 1).Value = item.STT;
                            worksheet.Cell(row, 2).Value = item.MaNhanVien;
                            worksheet.Cell(row, 3).Value = item.HoTenNhanVien;
                            worksheet.Cell(row, 4).Value = item.TenCongViec;
                            worksheet.Cell(row, 5).Value = item.ThoiGianBatDau.HasValue
                                ? item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(row, 6).Value = item.ThoiGianKetThuc.HasValue
                                ? item.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(row, 7).Value = item.TrangThai;
                        }

                        // Auto-fit cột
                        worksheet.Columns().AdjustToContents();

                        // Lưu file
                        workbook.SaveAs(saveDialog.FileName);

                        MessageBox.Show($"Đã xuất dữ liệu thành công tới: {saveDialog.FileName}",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}\nStackTrace: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Định nghĩa lớp DTO ChiTietTienDoItem
    public class ChiTietTienDoItem
    {
        public long STT { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTenNhanVien { get; set; }
        public string TenCongViec { get; set; }
        public DateOnly? ThoiGianBatDau { get; set; }
        public DateOnly? ThoiGianKetThuc { get; set; }
        public string TrangThai { get; set; }
    }
}