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
                    foreach (var quyen in _quyenList)
                    {
                        if (quyen.QLuongCoBan < 0)
                        {
                            MessageBox.Show($"Lương cơ bản của quyền '{quyen.QTen}' không được âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var quyenInDb = context.Quyens.FirstOrDefault(q => q.QMa == quyen.QMa);
                        if (quyenInDb != null)
                        {
                            quyenInDb.QLuongCoBan = quyen.QLuongCoBan;
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Lưu thay đổi lương cơ bản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thay đổi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}