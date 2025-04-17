using System;
using System.Collections.Generic;
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
using QuanLyDuAn.Forms;
using QuanLyDuAn.Models;              //bo cmt
using Microsoft.EntityFrameworkCore;

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for ThongTinNhanVien.xaml
    /// </summary>
    public partial class ThongTinNhanVien : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<Models.Quyen> quyen = new List<Models.Quyen>();

        public event EventHandler NhanVienDeleted;

        private string _NvMa;
        public ThongTinNhanVien(string NvMa)
        {
            InitializeComponent();
            _NvMa = NvMa;
            LoadNhanVien();
            LoadQuyen();
        }

        private void False()
        {
            txtHoTen.IsEnabled = false;
            cbGioiTinh.IsEnabled = false;
            txtNgaySinh.IsEnabled = false;
            txtSDT.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtDiaChi.IsEnabled = false;
            txtLuongCoBan.IsEnabled = false;
            txtTaiKhoan.IsEnabled = false;
            txtMatKhau.IsEnabled = false;
            cbQuyen.IsEnabled = false;
            btnLuu.IsEnabled = false;
            btnHuy.IsEnabled = false;

            btnSua.IsEnabled = true;
            btnXoa.IsEnabled = true;
        }
        private void True()
        {
            txtHoTen.IsEnabled = true;
            cbGioiTinh.IsEnabled = true;
            txtNgaySinh.IsEnabled = true;
            txtSDT.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtDiaChi.IsEnabled = true;
            txtLuongCoBan.IsEnabled = true;
            //txtTaiKhoan.IsEnabled = false;
            //txtMatKhau.IsEnabled = false;
            cbQuyen.IsEnabled = true;
            btnLuu.IsEnabled = true;
            btnHuy.IsEnabled = true;

            btnSua.IsEnabled = false;
            btnXoa.IsEnabled = false;
        }
        private void DuAn()
        {
            int ID = int.Parse(_NvMa.Substring(2));
            //List<Models.DuAn> duAn = new List<Models.DuAn>();
            //duAn = context.DuAns
            //.Where(da => da.NhanVienThanGiaDuAns.Any(tg => tg.NvId == ID)) // Kiểm tra nhân viên tham gia dự án
            //.Include(da => da.TtMaNavigation)  // Bao gồm thông tin trạng thái
            //.Include(da => da.NhanVienThanGiaDuAns) // Bao gồm nhân viên tham gia dự án
            //.ToList();
            //dgDuAn.ItemsSource = duAn;




            //bo cmt

            var query = from da in context.DuAns
                        join tg in context.NhanVienThamGiaDuAns on da.DaId equals tg.DaId
                        join tt in context.TrangThais on da.TtMa equals tt.TtMa // Lấy trạng thái của dự án từ bảng TrangThais
                        where tg.NvId == ID // Lọc dự án mà nhân viên tham gia
                        select new
                        {
                            DaMa = da.DaMa,       // Mã dự án
                            DaTen = da.DaTen,     // Tên dự án
                            TtTen = tt.TtTen      // Tên trạng thái
                        };

            // Chuyển đổi kết quả thành danh sách
            var duAnList = query.ToList();
            dgDuAn.ItemsSource = duAnList;

            //den thay thoi



            //        var duAn = context.NhanVienThanGiaDuAns
            //.Where(nvta => nvta.NvId == ID)
            //.Join(context.DuAns,
            //      nvta => nvta.DaId,
            //      da => da.DaId,
            //      (nvta, da) => new { nvta, da })
            //.Join(context.TrangThais,
            //      combined => combined.nvta.Da.TtMa,
            //      tt => tt.TtMa,
            //      (combined, tt) => new
            //      {
            //          DaID = combined.da.DaId,
            //          DaMa = combined.da.DaMa,
            //          DaTen = combined.da.DaTen,
            //          TtTen = tt.TtTen
            //      })
            //.ToList();
            //      dgDuAn.ItemsSource = duAn;


        }
        public void LoadNhanVien()  //bo cmt
        {
            //var nhanVien = context.NhanViens.Include(nv => nv.QMaNavigation).Where(nv => nv.NvMa == _NvMa).FirstOrDefault();
            var nhanVien = (from nv in context.NhanViens
                           join q in context.Quyens on nv.QMa equals q.QMa
                           select new
                           {
                               NvId = nv.NvId,
                               NvMa = nv.NvMa,
                               NvTen = nv.NvTen,
                               NvGioiTinh = nv.NvGioiTinh,
                               NvNgaySinh = nv.NvNgaySinh,
                               NvSdt = nv.NvSdt,
                               NvDiaChi = nv.NvDiaChi,
                               NvEmail = nv.NvEmail,
                               NvTaiKhoan = nv.NvTaiKhoan,
                               NvMatKhau = nv.NvMatKhau,
                               NvLuongCoBan = nv.NvLuongCoBan,
                               QMa = nv.QMa,
                           }).FirstOrDefault();
            if (nhanVien != null)
            {
                // Hiển thị thông tin nhân viên vào các TextBox
                txtHoTen.Text = nhanVien.NvTen;
                cbGioiTinh.Text = nhanVien.NvGioiTinh;
                txtNgaySinh.Text = nhanVien.NvNgaySinh.ToString("dd/MM/yyyy");
                txtSDT.Text = nhanVien.NvSdt;
                txtDiaChi.Text = nhanVien.NvDiaChi;
                txtEmail.Text = nhanVien.NvEmail;
                txtLuongCoBan.Text = nhanVien.NvLuongCoBan.ToString("N2");
                txtTaiKhoan.Text = nhanVien.NvTaiKhoan;
                txtMatKhau.Password = nhanVien.NvMatKhau.ToString();
                cbQuyen.SelectedValue = nhanVien.QMa;

                // Và các trường khác như tài khoản, mật khẩu, lương cơ bản...
            }
            else
            {
                MessageBox.Show("Không tìm thấy nhân viên với mã " + _NvMa);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNhanVien();
            False();
            DuAn();
        }

        private void LoadQuyen()   //bo cmt
        {
            // Lấy danh sách quyền từ CSDL
            //quyen = context.Quyens.ToList();
            var quyenn = (from q in context.Quyens
                         select new {
                             QMa = q.QMa,
                             QTen = q.QTen,
                             }).ToList();
            // Gán danh sách quyền vào ComboBox
            cbQuyen.ItemsSource = quyenn;
            cbQuyen.DisplayMemberPath = "QTen";  // Hiển thị tên quyền
            cbQuyen.SelectedValuePath = "QMa";  // Lưu mã quyền khi chọn
        }

        private void cbQuyen_SelectionChanged(object sender, SelectionChangedEventArgs e)  //bo cmt
        {
            var selectedQuyen = cbQuyen.SelectionBoxItem as Quyen;
            if (selectedQuyen != null)
            {
                string maQuyen = selectedQuyen.QMa;
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            True();

        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            False();
            LoadNhanVien();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)  //bo cmt
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(cbGioiTinh.Text))
            {
                MessageBox.Show("Vui lòng chọn giới tính!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập Email!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLuongCoBan.Text))
            {
                MessageBox.Show("Vui lòng nhập lương!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(cbQuyen.SelectedValue as String))
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int _NvId = int.Parse(_NvMa.Substring(2));
            Models.NhanVien nhanVien = context.NhanViens.Find(_NvId);
            if (nhanVien != null)
            {

                DateOnly ngaySinh;
                if (DateOnly.TryParseExact(txtNgaySinh.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out ngaySinh))
                {
                    // Chuyển đổi thành công, gán giá trị cho nhân viên
                    nhanVien.NvNgaySinh = ngaySinh;
                }
                else
                {
                    // Thông báo lỗi nếu định dạng không hợp lệ
                    MessageBox.Show("Ngày sinh không hợp lệ, vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                nhanVien.NvTen = txtHoTen.Text;
                string gioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)?.Content.ToString();
                nhanVien.NvGioiTinh = gioiTinh;
                nhanVien.NvSdt = txtSDT.Text;
                nhanVien.NvEmail = txtEmail.Text;
                nhanVien.NvDiaChi = txtDiaChi.Text;
                Decimal luongCoBan = Decimal.Parse(txtLuongCoBan.Text);
                nhanVien.NvLuongCoBan = luongCoBan;
                context.NhanViens.Update(nhanVien);
                context.SaveChanges();
                False();
                MessageBox.Show("Thay đổi thông tin nhân viên thành công!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)         //bo cmt
        {
            if (MessageBox.Show("Xác nhận xóa loại bệnh nhân?", "Cảnh báo!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int _NvId = int.Parse(_NvMa.Substring(2));
                Models.NhanVien nhanVien = context.NhanViens.Find(_NvId);
                if (nhanVien != null)
                {
                    context.NhanViens.Remove(nhanVien);
                }
                context.SaveChanges();
                MessageBox.Show("Thay đổi thông tin nhân viên thành công!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
                NhanVienDeleted?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
        }

        private void dgDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)        //bo cmt
        {
            var selectedDuAn = (dynamic)dgDuAn.SelectedItem;
            if (selectedDuAn != null)
            {
                string Ma = selectedDuAn.DaMa;
                int daId = int.Parse(Ma.Substring(2));
                int nvId = int.Parse(_NvMa.Substring(2));
                List<Models.CongViec> congViec = new List<Models.CongViec>();
                congViec = context.CongViecs.Where(cv => cv.PhanCongCongViecs.Any(pc => pc.NvId == nvId) && cv.DaId == daId)
                    .Include(tt => tt.TtMaNavigation)
                    .ToList();
                dgCongViec.ItemsSource = congViec;
            }

        }

        private void dgCongViec_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectCongViec = (dynamic)dgCongViec.SelectedItem;

            if (selectCongViec != null)
            {
                // Mở Form B và truyền mã nhân viên
                ThongTinCongViec thongTinCongViec = new ThongTinCongViec(selectCongViec.CvMa);
                //thongTinCongViec.CongViecDeleted += LoadLoadCongViec;
                thongTinCongViec.Show();
            }
        }
    }
}
