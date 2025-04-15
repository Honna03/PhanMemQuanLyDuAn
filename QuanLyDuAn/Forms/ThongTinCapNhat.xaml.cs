using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for ThongTinCapNhat.xaml
    /// </summary>
    public partial class ThongTinCapNhat : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        private int _CnId;
        private dynamic? capNhat;
        public ThongTinCapNhat(int CnId)
        {
            InitializeComponent();
            _CnId = CnId;
        }
        private void LoadCapNhatCongViec()
        {
            capNhat = (from cn in context.CapNhatCongViecs
                          join cv in context.CongViecs on cn.CvId equals cv.CvId
                          join da in context.DuAns on cn.DaId equals da.DaId
                          join nv in context.NhanViens on cn.NvId equals nv.NvId
                          where cn.CnId == _CnId
                          select new
                          {
                              cn.CnId,
                              cv.CvTen,
                              da.DaTen,
                              nv.NvTen,
                              cn.CnMoTa,
                              cn.CnThoiGian,
                              cn.CnFile,
                              cn.CvPath
                          }).FirstOrDefault();
            txtID.Text = capNhat.CnId.ToString();
            txtCongViec.Text = capNhat.CvTen.ToString();
            txtDuAn.Text = capNhat.DaTen.ToString();
            txtNhanVien.Text = capNhat.NvTen.ToString();
            txtMoTa.Text = capNhat.CnMoTa.ToString();
            txtTenTapTin.Text = capNhat.CnFile.ToString();
            txtDuongDan.Text = capNhat.CvPath.ToString();
            txtThoiGian.Text = capNhat.CnThoiGian.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCapNhatCongViec();
            txtID.IsEnabled = false;
            txtCongViec.IsEnabled = false;
            txtDuAn.IsEnabled = false;
            txtNhanVien.IsEnabled= false;
            txtThoiGian.IsEnabled=false;
            txtMoTa.IsEnabled = false;
            txtTenTapTin.IsEnabled = false;
            txtDuongDan.IsEnabled = false;
        }

        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            if (capNhat != null && !string.IsNullOrEmpty(capNhat.CvPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = capNhat.CvPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở file: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Không có đường dẫn file.");
            }
        }
    }
}
