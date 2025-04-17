using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDuAn.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Forms
{
    public partial class AdjustBaseSalary : UserControl
    {
        private List<Quyen> _quyenList;
        // Định nghĩa sự kiện để thông báo khi lưu thành công
        public event EventHandler SalaryAdjusted;
        public AdjustBaseSalary()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    _quyenList = context.Quyens.ToList();
                    dgQuyen.ItemsSource = _quyenList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu quyền: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    var currentDate = DateTime.Now;
                    foreach (var quyen in _quyenList)
                    {
                        if (quyen.QLuongCoBan < 0)
                        {
                            MessageBox.Show($"Lương cơ bản của quyền '{quyen.QTen}' không được âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        // Gọi stored procedure với tham số truyền trực tiếp
                        context.Database.ExecuteSqlRaw(
                            "EXEC CapNhatLuongTheoQuyen {0}, {1}",
                            quyen.QMa,
                            quyen.QLuongCoBan
                        );

                        // Đồng bộ lương cơ bản sang nhân viên có quyền tương ứng
                        var nhanViens = context.NhanViens.Where(nv => nv.QMa == quyen.QMa).ToList();
                        foreach (var nhanVien in nhanViens)
                        {
                            var luongCu = nhanVien.NvLuongCoBan;
                            nhanVien.NvLuongCoBan = quyen.QLuongCoBan;

                            // Ghi lịch sử cập nhật lương
                            context.LichSuCapNhatLuongs.Add(new LichSuCapNhatLuong
                            {
                                QMa = quyen.QMa,
                                NvId = nhanVien.NvId,
                                LuongCu = luongCu,
                                LuongMoi = quyen.QLuongCoBan,
                                ThoiGianCapNhat = currentDate
                            });
                        }
                    }
                    context.SaveChanges();
                    MessageBox.Show("Lưu thay đổi lương cơ bản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Kích hoạt sự kiện sau khi lưu thành công
                    SalaryAdjusted?.Invoke(this, EventArgs.Empty);

                    // Đóng cửa sổ AdjustBaseSalary
                    Window.GetWindow(this).Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thay đổi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}