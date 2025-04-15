using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Forms;
using QuanLyDuAn.Models;     

namespace QuanLyDuAn.Controls
{
    public partial class DanhSachCongViec : UserControl
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
         List<Models.CongViec> congViec = new List<Models.CongViec>();

            
        public DanhSachCongViec()
        {
            InitializeComponent();
            LoadCongViec();
        }
        public void LoadCongViec()     //bo cmt
        {
          /*  //congViec = context.CongViecs.Include(cv => cv.NhanVienThamGiaDuAn).ThenInclude(tg => tg.Da)
            //    .Include(cv => cv.TtMaNavigation).ToList();

            //dgCongViec.ItemsSource = congViec;
            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;


            var ketQua = from cv in context.CongViecs
                         join da in context.DuAns on cv.DaId equals da.DaId
                         join nt in context.NhanViens on cv.NvIdNguoiTao equals nt.NvId
                         join tt in context.TrangThais on cv.TtMa equals tt.TtMa
                         join pc in context.PhanCongCongViecs on cv.CvId equals pc.CvId into pcGroup
                         from pc in pcGroup.DefaultIfEmpty()
                         join nv in context.NhanViens on pc.NvId equals nv.NvId into nvGroup
                         from nv in nvGroup.DefaultIfEmpty()
                         where (nv != null && nv.NvTaiKhoan == tenTaiKhoan) || nt.NvTaiKhoan == tenTaiKhoan
                         select new
                         {
                             CvMa = cv.CvMa,
                             CvTen = cv.CvTen,
                             DaTen = da.DaTen,
                             CvMoTa = cv.CvMoTa,
                             CvBatDau = cv.CvBatDau,
                             CvKetThuc = cv.CvKetThuc,
                             NguoiTao = nt.NvTen,
                             NguoiNhan = nv != null ? nv.NvTen : null,
                             TrangThai = tt.TtTen,
                             ThoiGianHoanThanh = cv.CvThoiGianHoanThanh
                         };
            var ketQuaList = ketQua.ToList();
            dgCongViec.ItemsSource = ketQuaList;
          */
        }
        private void LoadLoadCongViec(object sender, EventArgs e)
        {
            LoadCongViec();
        }
        private void btn_AddCongViec_Click_1(object sender, RoutedEventArgs e)
        {

            AddCongViec addCongViec = new AddCongViec();
            addCongViec.CongViecAdded += LoadLoadCongViec;
            addCongViec.ShowDialog();
        }

        private void DanhSachCongViec1_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCongViec();
        }

        private void dgCongViec_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectCongViec = (dynamic)dgCongViec.SelectedItem;

            if (selectCongViec != null)
            {
                // Mở Form B và truyền mã nhân viên
                ThongTinCongViec thongTinCongViec = new ThongTinCongViec(selectCongViec.CvMa);
                thongTinCongViec.CongViecDeleted += LoadLoadCongViec;
                thongTinCongViec.Show();
            }
        }
    }
}
