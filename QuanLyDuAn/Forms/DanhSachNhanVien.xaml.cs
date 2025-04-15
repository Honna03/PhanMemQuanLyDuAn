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

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachNhanVien : UserControl
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<Models.NhanVien> nhanVien = new List<Models.NhanVien>(); 
        //private List<NhanVien> danhSachNhanVien = new List<NhanVien>();
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
            nhanVien = context.NhanViens.Include(nv => nv.QMaNavigation).ToList();
            dgNhanVien.ItemsSource = nhanVien;
            totalPagesNhanVien = (int)Math.Ceiling((double)nhanVien.Count / pageSizeNhanVien);
            HienThiNhanVienTheoTrang(currentPageNhanVien);
            UpdateTotalRecords();
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
    }
}