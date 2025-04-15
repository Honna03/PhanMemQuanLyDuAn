using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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
using QuanLyDuAn.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for ThongTinCongViec.xaml
    /// </summary>
    public partial class ThongTinCongViec : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();
        List<Models.TrangThai> trangThai = new List<TrangThai>();
        List<Models.NhanVien> nhanVien = new List<NhanVien>();
        List<Models.DuAn> duAn = new List<DuAn>();
        public event EventHandler CongViecDeleted;

        private string _CvMa;
        private dynamic? congViec;

        public static int idDaAn;
        public static int idCongViec;
        public static int idNguoiNhan;
        public ThongTinCongViec(string CvMa)
        {
            InitializeComponent();
            _CvMa = CvMa;
        }
        //public ThongTinCongViec()
        //{
        //    InitializeComponent();
        //    LoadCapNhat();
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCongViec();
            LoadTrangThai();
            LoadNhanVien();
            LoadDuAn();
            LoadCapNhat();
            False();

            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;
            int nguoiNhancbo = (cbNguoiNhan.SelectedValue as int?) ?? -1;
            int duAncbo = (int)cboDuAn.SelectedValue;

            var NguoiNhan = (from nv in context.NhanViens
                             join pc in context.PhanCongCongViecs on nv.NvId equals pc.NvId
                             where nv.NvTaiKhoan == tenTaiKhoan
                             select nv.NvId).Distinct().FirstOrDefault();

            var VaiTro = (from nv in context.NhanViens
                          join tg in context.NhanVienThamGiaDuAns on nv.NvId equals tg.NvId
                          where nv.NvTaiKhoan == tenTaiKhoan && tg.DaId == duAncbo
                          select tg.VtMa).Distinct().FirstOrDefault();

            if (nguoiNhancbo != NguoiNhan)
            {
                btnCapNhat.Visibility = Visibility.Collapsed;
            }

            if (VaiTro != "ql")
            {
                btnSua.Visibility = Visibility.Collapsed;
                btnLuu.Visibility = Visibility.Collapsed;
                btnXoa.Visibility = Visibility.Collapsed;
                btnHuy.Visibility = Visibility.Collapsed;
                btnHoanThanh.Visibility = Visibility.Collapsed;
                btnPhanCong.Visibility = Visibility.Collapsed;
            }
        }
        private void False()
        {
            txtMaCV.IsEnabled = false;
            txtTenCV.IsEnabled = false;
            txtMoTa.IsEnabled = false;
            txtNgayBD.IsEnabled = false;
            txtNgayKT.IsEnabled = false;
            cboDuAn.IsEnabled = false;
            txtNguoiTao.IsEnabled = false;
            cbNguoiNhan.IsEnabled = false;
            cbTrangThai.IsEnabled = false;
            txtNgayHoanThanh.IsEnabled = false;
            txtFIle.IsEnabled = false;
            txtPath.IsEnabled = false;

            btnLuu.IsEnabled = false;
            btnChonFile.IsEnabled = false;

            btnMoFile.IsEnabled = true;
            btnXoa.IsEnabled = true;
            btnSua.IsEnabled = true;
            btnHoanThanh.IsEnabled = dgCapNhat.Items.Count > 0;
        }

        private void True()
        {
            txtTenCV.IsEnabled = true;
            txtMoTa.IsEnabled = true;
            txtNgayBD.IsEnabled = true;
            txtNgayKT.IsEnabled = true;
            cbNguoiNhan.IsEnabled = true;

            btnLuu.IsEnabled = true;
            btnChonFile.IsEnabled = true;
            btnMoFile.IsEnabled = false;
            btnXoa.IsEnabled = false;
            btnSua.IsEnabled = false;
            btnHoanThanh.IsEnabled = false;
        }

        public void LoadCapNhat()
        {
            int cvID = int.Parse(txtMaCV.Text.Substring(2));
            var ketQua = context.CapNhatCongViecs
            .Where(c => c.CvId == cvID)
            .Select(c => new { c.CnId, c.CnMoTa, c.CnThoiGian })
            .ToList();
            dgCapNhat.ItemsSource = ketQua;
        }
        private void LoadLoadCapNhat(object sender, EventArgs e)
        {
            LoadCapNhat();
        }

        private void LoadLoadCongViec(object sender, EventArgs e)
        {
            LoadCongViec();


            string tenTaiKhoan;
            tenTaiKhoan = MainWindow.nguoidangnhap;
            int nguoiNhancbo = (cbNguoiNhan.SelectedValue as int?) ?? -1;

            var NguoiNhan = (from nv in context.NhanViens
                             join pc in context.PhanCongCongViecs on nv.NvId equals pc.NvId
                             where nv.NvTaiKhoan == tenTaiKhoan
                             select nv.NvId).Distinct().FirstOrDefault();

            if (nguoiNhancbo != NguoiNhan)
            {
                btnCapNhat.Visibility = Visibility.Collapsed;
            }
        }
        public void LoadCongViec()
        {
            congViec = (from cv in context.CongViecs
                        join da in context.DuAns on cv.DaId equals da.DaId
                        join nt in context.NhanViens on cv.NvIdNguoiTao equals nt.NvId
                        join tt in context.TrangThais on cv.TtMa equals tt.TtMa
                        join pc in context.PhanCongCongViecs on cv.CvId equals pc.CvId into pcGroup
                        from pc in pcGroup.DefaultIfEmpty()
                        join nv in context.NhanViens on pc.NvId equals nv.NvId into nvGroup
                        from nv in nvGroup.DefaultIfEmpty()
                        where cv.CvMa == _CvMa
                        select new
                        {
                            CvMa = cv.CvMa,
                            CvTen = cv.CvTen,
                            DaTen = cv.DaId,
                            CvMoTa = cv.CvMoTa,
                            CvBatDau = cv.CvBatDau,
                            CvKetThuc = cv.CvKetThuc,
                            NguoiTao = nt.NvTen,
                            NguoiNhan = nv != null ? nv.NvTen : null,
                            TtMa = cv.TtMa,
                            ThoiGianHoanThanh = cv.CvThoiGianHoanThanh,
                            cvFile = cv.CvFile,
                            cvPath = cv.CvPath,
                            NvID = pc != null ? pc.NvId : (int?)null
                        }).FirstOrDefault();
            if (congViec != null)
            {
                txtMaCV.Text = congViec.CvMa;
                txtTenCV.Text = congViec.CvTen;
                txtMoTa.Text = congViec.CvMoTa;
                //txtNgayBD.Text = congViec.CvBatDau.HasValue
                //? congViec.CvBatDau.Value.ToString("dd/MM/yyyy")
                //: "";

                //txtNgayKT.Text = congViec.CvKetThuc.HasValue
                //? congViec.CvKetThuc.Value.ToString("dd/MM/yyyy")
                //: "";

                //txtNgayHoanThanh.Text = congViec.ThoiGianHoanThanh.HasValue
                //? congViec.ThoiGianHoanThanh.Value.ToString("dd/MM/yyyy")
                //: "";

                txtNgayBD.Text = congViec.CvBatDau?.ToString("dd/MM/yyyy") ?? "";
                txtNgayKT.Text = congViec.CvKetThuc?.ToString("dd/MM/yyyy") ?? "";
                txtNgayHoanThanh.Text = congViec.ThoiGianHoanThanh?.ToString("dd/MM/yyyy") ?? "";

                txtNguoiTao.Text = congViec.NguoiTao;
                txtFIle.Text = congViec.cvFile;
                txtPath.Text = congViec.cvPath;

                cbTrangThai.SelectedValue = congViec.TtMa;
                
                //if (congViec.NvID.HasValue) { 
                //    cbNguoiNhan.SelectedValue = congViec.NvID.Value;
                //}
                //else
                //{
                //    cbNguoiNhan.SelectedIndex = -1;
                //}
                if (congViec.NvID != null)
                {
                    cbNguoiNhan.SelectedValue = (int)congViec.NvID;
                }
                else
                {
                    cbNguoiNhan.SelectedIndex = -1;
                }
                ///asdasdasdas

                if (congViec.DaTen != null)
                {
                    cboDuAn.SelectedValue = (int)congViec.DaTen;
                }
                else
                {
                    cboDuAn.SelectedIndex = -1;
                }

            }
        }
        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            CongViecDeleted?.Invoke(this, EventArgs.Empty);
        }

        private void LoadTrangThai()   //bo cmt
        {

            // Lấy danh sách quyền từ CSDL
            trangThai = context.TrangThais.ToList();

            // Gán danh sách quyền vào ComboBox
            cbTrangThai.ItemsSource = trangThai;
            cbTrangThai.DisplayMemberPath = "TtTen";  // Hiển thị tên quyền
            cbTrangThai.SelectedValuePath = "TtMa";  // Lưu mã quyền khi chọn
        }

        private void cbTrangThai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTrangThai = cbTrangThai.SelectionBoxItem as TrangThai;
            if (selectedTrangThai != null)
            {
                string maTrangThai = selectedTrangThai.TtMa;

                if (maTrangThai == "ht") // hoặc selectedTrangThai.TtTen == "Hoàn thành"
                {
                    btnCapNhat.IsEnabled = false;
                }
                else
                {
                    btnCapNhat.IsEnabled = true;
                }
            }
        }

        private void LoadNhanVien()
        {
            var congViec = context.CongViecs.FirstOrDefault(cv => cv.CvMa == _CvMa);
            if (congViec != null)
            {
                int duAnId = congViec.DaId;

                var nhanVien = context.NhanViens
                    .Where(nv => nv.NhanVienThamGiaDuAns.Any(da => da.DaId == duAnId))
                    .ToList();

                cbNguoiNhan.ItemsSource = nhanVien;
                cbNguoiNhan.DisplayMemberPath = "NvTen";
                cbNguoiNhan.SelectedValuePath = "NvId"; // ✅ sửa chỗ này
            }
        }

        private void cbNguoiNhan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNhanVien = cbNguoiNhan.SelectedItem as NhanVien;
            if (selectedNhanVien != null)
            {
                int idNhanVien = (int)selectedNhanVien.NvId;
            }
        }

        private void LoadDuAn()
        {
            duAn = context.DuAns.Where(da => da.NhanVienThamGiaDuAns.Any(tg => tg.CongViecs.Any(cv => cv.CvMa == _CvMa))).ToList();
            cboDuAn.ItemsSource = duAn;
            cboDuAn.DisplayMemberPath = "DaTen";
            cboDuAn.SelectedValuePath = "DaId";
        }
        private void cboDuAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDuAn = cboDuAn.SelectionBoxItem as DuAn;
            if (selectedDuAn != null)
            {
                int idDuAn = (int)selectedDuAn.DaId;
            }
        }

        private void btnMoFile_Click(object sender, RoutedEventArgs e)
        {
            LoadCongViec();
            if (congViec != null && !string.IsNullOrEmpty(congViec.cvPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = congViec.cvPath,
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

        private void btnChonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Tất cả các file (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);
                //string folderPath = System.IO.Path.GetDirectoryName(filePath);

                txtFIle.Text = fileName;
                txtPath.Text = filePath;
            }
        }

      

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            True();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenCV.Text))
            {
                MessageBox.Show("Vui lòng nhập tên công việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var cv = context.CongViecs.FirstOrDefault(cv => cv.CvMa == _CvMa);
            int daID = cv.DaId;
            int _CvId = int.Parse(_CvMa.Substring(2));
            Models.CongViec congViec = context.CongViecs.Find(_CvId, daID);
            if (congViec != null)
            {

                DateOnly NgayBatDau;
                if (DateOnly.TryParseExact(txtNgayBD.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out NgayBatDau))
                {
                    // Chuyển đổi thành công, gán giá trị cho nhân viên
                    congViec.CvBatDau = NgayBatDau;
                }
                else
                {
                    // Thông báo lỗi nếu định dạng không hợp lệ
                    MessageBox.Show("Ngày bắt đầu không hợp lệ, vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //aaa
                DateOnly NgayKetThuc;
                if (DateOnly.TryParseExact(txtNgayKT.Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out NgayKetThuc))
                {
                    // Kiểm tra ngày kết thúc phải lớn hơn ngày bắt đầu
                    if (NgayKetThuc > NgayBatDau)
                    {
                        congViec.CvKetThuc = NgayKetThuc;
                    }
                    else
                    {
                        MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Ngày kết thúc không hợp lệ, vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                congViec.CvTen = txtTenCV.Text;
                congViec.CvMoTa = txtMoTa.Text;
                congViec.CvFile = txtFIle.Text;
                congViec.CvPath = txtPath.Text;
                context.CongViecs.Update(congViec);
                context.SaveChanges();
                False();
                MessageBox.Show("Thay đổi thông tin công việc thành công!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnXoa_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa loại bệnh nhân?", "Cảnh báo!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int _NvId = int.Parse(_CvMa.Substring(2));
                var cv = context.CongViecs.FirstOrDefault(cv => cv.CvMa == _CvMa);
                int daID = cv.DaId;
                Models.CongViec congViec = context.CongViecs.Find(_NvId, daID);
                if (congViec != null)
                {
                    context.CongViecs.Remove(congViec);
                }
                context.SaveChanges();
                MessageBox.Show("Xóa công việc thành công!", "Thông báo!", MessageBoxButton.OK, MessageBoxImage.Information);
                CongViecDeleted?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
        }

        private void btnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            idNguoiNhan = (int)cbNguoiNhan.SelectedValue;
            idDaAn = (int)cboDuAn.SelectedValue;
            int idCV = int.Parse(txtMaCV.Text.Substring(2));
            idCongViec = idCV;
            CapNhatCongViec capNhatCongViec = new CapNhatCongViec();
            capNhatCongViec.CapNhatAdded += LoadLoadCapNhat;
            capNhatCongViec.ShowDialog();
        }

        private void dgCapNhat_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedCapNhat = (dynamic)dgCapNhat.SelectedItem;
            if (selectedCapNhat != null)
            {
                ThongTinCapNhat thongTinCapNhat = new ThongTinCapNhat(selectedCapNhat.CnId);
                thongTinCapNhat.ShowDialog();
            }
        }

        private void btnPhanCong_Click(object sender, RoutedEventArgs e)
        {
            if (cbNguoiNhan.SelectedValue != null)
            {
                idNguoiNhan = (int)cbNguoiNhan.SelectedValue;
            }
            else
            {
                idNguoiNhan = -1; // hoặc giá trị mặc định anh muốn
            }
            idDaAn = (int)cboDuAn.SelectedValue;
            int idCV = int.Parse(txtMaCV.Text.Substring(2));
            idCongViec = idCV;
            PhanCongCongViec phanCongCongViec = new PhanCongCongViec();
            phanCongCongViec.PhanCongAdded += LoadLoadCongViec;
            phanCongCongViec.ShowDialog();
        }

        private void btnHoanThanh_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
           "Bạn có chắc chắn muốn đánh dấu công việc này là HOÀN THÀNH?",
           "Xác nhận hoàn thành",
           MessageBoxButton.YesNo,
           MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int idCV = int.Parse(txtMaCV.Text.Substring(2));
                idCongViec = idCV;

                int id = (int)cboDuAn.SelectedValue;
                DateOnly? ngayCapNhatCuoi = context.CapNhatCongViecs
                .Where(cn => cn.DaId == id)
                .Max(cn => (DateTime?)cn.CnThoiGian)
                ?.ToDateOnly();

                Models.CongViec congViec = context.CongViecs.Find(idCongViec,id);
                congViec.TtMa = "ht";
                congViec.CvThoiGianHoanThanh = ngayCapNhatCuoi;
                context.CongViecs.Update(congViec);
                context.SaveChanges();
                MessageBox.Show("Công việc đã được cập nhật là HOÀN THÀNH.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCongViec();
                btnHoanThanh.IsEnabled = false;
            }
        }

    }
}
