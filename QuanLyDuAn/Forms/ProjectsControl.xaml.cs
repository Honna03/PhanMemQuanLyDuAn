using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using QuanLyDuAn.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using static QuanLyDuAn.Forms.KPI;

namespace QuanLyDuAn.Controls
{
    public partial class ProjectsControl : UserControl
    {
        private List<DuAn> danhSachDuAn = new List<DuAn>();
        private List<CongViec> danhSachCongViec = new List<CongViec>();
        private int currentPageDuAn = 1;
        private int pageSizeDuAn = 30;
        private int totalPagesDuAn;

        public ProjectsControl()
        {
            InitializeComponent();
            LoadDuAn();
            UpdateTotalRecords();
            UpdatePaginationDuAn();
        }

        private void btn_AddDuAn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_DuAn(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        private void LoadDuAn()
        {
            // Dữ liệu mẫu cho dự án
            danhSachDuAn = new List<DuAn>
            {
                new DuAn { STT = 1, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 2, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 3, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 4, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 5, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 6, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 7, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 8, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 9, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 10, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 11, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 12, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 13, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 14, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 15, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 16, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 17, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 18, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 19, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 20, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 21, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },
                new DuAn { STT = 22, MaDuAn = "DA001", TenDuAn = "Dự án A", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 6, 30), TrangThai = "Đang thực hiện", NguoiNhan = "Nguyễn Văn A", NguoiTao = "Admin" },
                new DuAn { STT = 23, MaDuAn = "DA002", TenDuAn = "Dự án B", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 7, 31), TrangThai = "Hoàn thành", NguoiNhan = "Trần Thị B", NguoiTao = "Admin" },
                new DuAn { STT = 24, MaDuAn = "DA003", TenDuAn = "Dự án C", NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 8, 31), TrangThai = "Tạm dừng", NguoiNhan = "Lê Văn C", NguoiTao = "Admin" },

            };

            // Dữ liệu mẫu cho công việc
            danhSachCongViec = new List<CongViec>
            {
                new CongViec { MaDuAn = "DA001", TenCongViec = "Lập kế hoạch", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 1, 15), TienDo = 100 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "Thiết kế", NgayBatDau = new DateTime(2025, 1, 16), NgayKetThuc = new DateTime(2025, 2, 15), TienDo = 70 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "Phát triển", NgayBatDau = new DateTime(2025, 2, 16), NgayKetThuc = new DateTime(2025, 3, 15), TienDo = 50 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "aaa", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 1, 15), TienDo = 100 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "bbb", NgayBatDau = new DateTime(2025, 1, 16), NgayKetThuc = new DateTime(2025, 2, 15), TienDo = 70 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "ccc", NgayBatDau = new DateTime(2025, 2, 16), NgayKetThuc = new DateTime(2025, 3, 15), TienDo = 50 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "d", NgayBatDau = new DateTime(2025, 1, 1), NgayKetThuc = new DateTime(2025, 1, 15), TienDo = 100 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "e", NgayBatDau = new DateTime(2025, 1, 16), NgayKetThuc = new DateTime(2025, 2, 15), TienDo = 70 },
                new CongViec { MaDuAn = "DA001", TenCongViec = "cadfdsfsadfsdfc", NgayBatDau = new DateTime(2025, 2, 16), NgayKetThuc = new DateTime(2025, 3, 15), TienDo = 50 },
                new CongViec { MaDuAn = "DA002", TenCongViec = "Phân tích", NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 2, 20), TienDo = 100 },
                new CongViec { MaDuAn = "DA002", TenCongViec = "Kiểm thử", NgayBatDau = new DateTime(2025, 2, 21), NgayKetThuc = new DateTime(2025, 3, 10), TienDo = 80 }
            };

            totalPagesDuAn = (int)Math.Ceiling((double)danhSachDuAn.Count / pageSizeDuAn);
            HienThiDuAnTheoTrang(currentPageDuAn);
        }

        private void UpdateTotalRecords()
        {
            int totalRecords = danhSachDuAn.Count;
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
        }

        private void HienThiDuAnTheoTrang(int page)
        {
            var duAnTheoTrang = danhSachDuAn
                .Skip((page - 1) * pageSizeDuAn)
                .Take(pageSizeDuAn)
                .ToList();

            dgDuAn.ItemsSource = duAnTheoTrang;
            UpdatePaginationDuAn();
        }

        private void UpdatePaginationDuAn()
        {
            txtPaginationDuAn.Text = $"{currentPageDuAn} trong {totalPagesDuAn}";
            btnPrevPageDuAn.IsEnabled = currentPageDuAn > 1;
            btnNextPageDuAn.IsEnabled = currentPageDuAn < totalPagesDuAn;
        }

        private void btnPrevPageDuAn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageDuAn > 1)
            {
                currentPageDuAn--;
                HienThiDuAnTheoTrang(currentPageDuAn);
            }
        }

        private void btnNextPageDuAn_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageDuAn < totalPagesDuAn)
            {
                currentPageDuAn++;
                HienThiDuAnTheoTrang(currentPageDuAn);
            }
        }

        private void dgDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDuAn.SelectedItem is DuAn selectedDuAn)
            {
                HienThiBieuDoTron(selectedDuAn.MaDuAn);
            }
        }

        private void HienThiBieuDoTron(string maDuAn)
        {
            pieChart.Series.Clear();

            // Lọc công việc theo mã dự án
            var congViecCuaDuAn = danhSachCongViec.FindAll(cv => cv.MaDuAn == maDuAn);
            if (congViecCuaDuAn.Count == 0)
            {
                pieChart.Series.Add(new PieSeries
                {
                    Title = "Chưa có dữ liệu",
                    Values = new ChartValues<double> { 1 },
                    Fill = new SolidColorBrush(Color.FromRgb(211, 211, 211)) // Màu xám
                });
                return;
            }

            // Thêm các công việc vào biểu đồ tròn
            foreach (var congViec in congViecCuaDuAn)
            {
                pieChart.Series.Add(new PieSeries
                {
                    Title = congViec.TenCongViec,
                    Values = new ChartValues<double> { congViec.TienDo },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y}%",
                    Fill = new SolidColorBrush(GetColorForProgress(congViec.TienDo))
                });
            }
        }

        private Color GetColorForProgress(int tienDo)
        {
            // Gán màu tùy theo tiến độ
            if (tienDo == 100) return Colors.Green;
            if (tienDo >= 70) return Colors.Orange;
            return Colors.Red;
        }

        private void dgDuAn_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgDuAn.SelectedItem != null)
            {
                DuAn selectedDuAn = dgDuAn.SelectedItem as DuAn;
                if (selectedDuAn == null) return;

                Window editWindow = new Window
                {
                    Title = $"Chỉnh sửa dự án: {selectedDuAn.TenDuAn}",
                    Width = 1000,
                    Height = 450,
                    Content = new Edit_DuAn(),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };
                editWindow.Show();
            }
        }
    }

    public class DuAn
    {
        public int STT { get; set; }
        public string MaDuAn { get; set; }
        public string TenDuAn { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }
        public string NguoiNhan { get; set; }
        public string NguoiTao { get; set; }
    }

    public class CongViec
    {
        public string MaDuAn { get; set; }
        public string TenCongViec { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int TienDo { get; set; }
    }
}