using QuanLyDuAn.Controls;
using QuanLyDuAn.Models; 
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

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for AddNhanVien.xaml
    /// </summary>
    public partial class AddNhanVien : Window
    {
        public event EventHandler NhanVienAdded;
        //private DanhSachNhanVien _danhSachNhanVienForm;
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<Models.Quyen> quyen = new List<Models.Quyen>();
        public AddNhanVien()
        {
            //DanhSachNhanVien danhSachNhanVienForm hai cái này ở trong ngoặc mà sài cách khác gòi
            InitializeComponent();
            LoadQuyen();
            //_danhSachNhanVienForm = danhSachNhanVienForm;
        }
        private void LoadQuyen()        //bo cmt
        {
            //Lấy danh sách quyền từ CSDL
            //quyen = context.Quyens.ToList();

            // Gán danh sách quyền vào ComboBox
            var quyenn = (from q in context.Quyens
                          select new
                          {
                              QMa = q.QMa,
                              QTen = q.QTen,
                          }).ToList();
            // Gán danh sách quyền vào ComboBox
            cbQuyen.ItemsSource = quyenn;
            cbQuyen.DisplayMemberPath = "QTen";  // Hiển thị tên quyền
            cbQuyen.SelectedValuePath = "QMa";  // Lưu mã quyền khi chọn
        }

        private void cbQuyen_SelectionChanged(object sender, SelectionChangedEventArgs e)       //bo cmt
        {
            var selectedQuyen = cbQuyen.SelectedItem as Quyen;
            if (selectedQuyen != null)
            {
                string maQuyen = selectedQuyen.QMa;
                //MessageBox.Show($"Mã quyền đã chọn: {maQuyen}");
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)         //bo cmt
        {
            Models.NhanVien nhanVien = new Models.NhanVien();

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

            nhanVien.NvTen = txtHoTen.Text.ToString();

            string gioiTinh = (cbGioiTinh.SelectedItem as ComboBoxItem)?.Content.ToString();
            nhanVien.NvGioiTinh = gioiTinh;

            nhanVien.NvSdt = txtSDT.Text.ToString();
            nhanVien.NvEmail = txtEmail.Text.ToString();
            nhanVien.NvDiaChi = txtDiaChi.Text.ToString();
            Decimal luongCoBan = Decimal.Parse(txtLuongCoBan.Text);
            nhanVien.NvLuongCoBan = luongCoBan;
            nhanVien.NvTaiKhoan = txtTaiKhoan.Text.ToString();
            nhanVien.NvMatKhau = txtMatKhau.Password.ToString();
            nhanVien.QMa = cbQuyen.SelectedValue.ToString();
            context.NhanViens.Add(nhanVien);
            context.SaveChanges();
            MessageBox.Show("Đã thêm nhân viên thành công!.", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);

            //_danhSachNhanVienForm.LoadNhanVien();
            NhanVienAdded?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
