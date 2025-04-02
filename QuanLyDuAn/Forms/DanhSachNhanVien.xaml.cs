using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDuAn.Forms; // Giả định có namespace này cho form chỉnh sửa

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachNhanVien : UserControl
    {
        private List<NhanVien> danhSachNhanVien = new List<NhanVien>();
        private int currentPageNhanVien = 1;
        private int pageSizeNhanVien = 10; // Số bản ghi mỗi trang
        private int totalPagesNhanVien;

        public DanhSachNhanVien()
        {
            InitializeComponent();
            LoadNhanVien();
            UpdateTotalRecords();
            UpdatePaginationNhanVien();
        }

        // Xử lý nút Thêm nhân viên
        private void btn_AddNhanVien_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new ChiTietNhanVien(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        // Tải dữ liệu mẫu
        private void LoadNhanVien()
        {
            danhSachNhanVien = new List<NhanVien>
            {
                new NhanVien { STT = 1, MaNhanVien = "NV001", HoTen = "Nguyễn Văn A", GioiTinh = "Nam", NgaySinh = new DateTime(1990, 5, 15), SDT = "0901234567", Email = "nva@example.com", TaiKhoan = "nva", MatKhau = "123456", LuongCoBan = 10000000 },
                new NhanVien { STT = 2, MaNhanVien = "NV002", HoTen = "Trần Thị B", GioiTinh = "Nữ", NgaySinh = new DateTime(1995, 8, 20), SDT = "0912345678", Email = "ttb@example.com", TaiKhoan = "ttb", MatKhau = "abcdef", LuongCoBan = 12000000 },
                new NhanVien { STT = 3, MaNhanVien = "NV003", HoTen = "Lê Văn C", GioiTinh = "Nam", NgaySinh = new DateTime(1988, 3, 10), SDT = "0923456789", Email = "lvc@example.com", TaiKhoan = "lvc", MatKhau = "pass123", LuongCoBan = 15000000 },
                new NhanVien { STT = 4, MaNhanVien = "NV004", HoTen = "Phạm Thị D", GioiTinh = "Nữ", NgaySinh = new DateTime(1992, 12, 25), SDT = "0934567890", Email = "ptd@example.com", TaiKhoan = "ptd", MatKhau = "xyz789", LuongCoBan = 9000000 },
                new NhanVien { STT = 5, MaNhanVien = "NV005", HoTen = "Hoàng Văn E", GioiTinh = "Nam", NgaySinh = new DateTime(1993, 7, 18), SDT = "0945678901", Email = "hve@example.com", TaiKhoan = "hve", MatKhau = "qwerty", LuongCoBan = 11000000 },
                new NhanVien { STT = 6, MaNhanVien = "NV006", HoTen = "Đỗ Thị F", GioiTinh = "Nữ", NgaySinh = new DateTime(1991, 9, 5), SDT = "0956789012", Email = "dtf@example.com", TaiKhoan = "dtf", MatKhau = "abc123", LuongCoBan = 13000000 },
                new NhanVien { STT = 7, MaNhanVien = "NV007", HoTen = "Bùi Văn G", GioiTinh = "Nam", NgaySinh = new DateTime(1987, 4, 22), SDT = "0967890123", Email = "bvg@example.com", TaiKhoan = "bvg", MatKhau = "def456", LuongCoBan = 14000000 },
                new NhanVien { STT = 8, MaNhanVien = "NV008", HoTen = "Ngô Thị H", GioiTinh = "Nữ", NgaySinh = new DateTime(1994, 11, 30), SDT = "0978901234", Email = "nth@example.com", TaiKhoan = "nth", MatKhau = "ghi789", LuongCoBan = 9500000 },
                new NhanVien { STT = 9, MaNhanVien = "NV009", HoTen = "Vũ Văn I", GioiTinh = "Nam", NgaySinh = new DateTime(1989, 6, 15), SDT = "0989012345", Email = "vvi@example.com", TaiKhoan = "vvi", MatKhau = "jkl012", LuongCoBan = 12500000 },
                new NhanVien { STT = 10, MaNhanVien = "NV010", HoTen = "Trương Thị K", GioiTinh = "Nữ", NgaySinh = new DateTime(1996, 2, 8), SDT = "0990123456", Email = "ttk@example.com", TaiKhoan = "ttk", MatKhau = "mno345", LuongCoBan = 10500000 },
                new NhanVien { STT = 11, MaNhanVien = "NV011", HoTen = "Phan Văn L", GioiTinh = "Nam", NgaySinh = new DateTime(1990, 10, 12), SDT = "0909876543", Email = "pvl@example.com", TaiKhoan = "pvl", MatKhau = "pqr678", LuongCoBan = 11500000 }
            };

            totalPagesNhanVien = (int)Math.Ceiling((double)danhSachNhanVien.Count / pageSizeNhanVien);
            HienThiNhanVienTheoTrang(currentPageNhanVien);
        }

        // Cập nhật tổng số bản ghi
        private void UpdateTotalRecords()
        {
            int totalRecords = danhSachNhanVien.Count;
            txtTotalRecords.Text = $"Tổng số bản ghi: {totalRecords}";
        }

        // Hiển thị nhân viên theo trang
        private void HienThiNhanVienTheoTrang(int page)
        {
            var nhanVienTheoTrang = danhSachNhanVien
                .Skip((page - 1) * pageSizeNhanVien)
                .Take(pageSizeNhanVien)
                .ToList();

            dgNhanVien.ItemsSource = nhanVienTheoTrang;
            UpdatePaginationNhanVien();
        }

        // Cập nhật thông tin phân trang
        private void UpdatePaginationNhanVien()
        {
            txtPaginationNhanVien.Text = $"{currentPageNhanVien} trong {totalPagesNhanVien}";
            btnPrevPageNhanVien.IsEnabled = currentPageNhanVien > 1;
            btnNextPageNhanVien.IsEnabled = currentPageNhanVien < totalPagesNhanVien;
        }

        // Xử lý nút Trang trước
        private void btnPrevPageNhanVien_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNhanVien > 1)
            {
                currentPageNhanVien--;
                HienThiNhanVienTheoTrang(currentPageNhanVien);
            }
        }

        // Xử lý nút Trang sau
        private void btnNextPageNhanVien_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNhanVien < totalPagesNhanVien)
            {
                currentPageNhanVien++;
                HienThiNhanVienTheoTrang(currentPageNhanVien);
            }
        }
    }

    // Lớp mô hình cho nhân viên
    public class NhanVien
    {
        public int STT { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public decimal LuongCoBan { get; set; }
    }
}