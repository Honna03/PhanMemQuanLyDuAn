using System;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyDuAn.Forms
{
    public partial class Edit_Luong : UserControl
    {
        private LuongViewModel _luongViewModel;
        private int _nvID;
        private DateTime _thoiGian;

        // ViewModel để kết hợp dữ liệu từ 3 models
        public class LuongViewModel
        {
            public int NvId { get; set; }
            public string ThangNam { get; set; } // Thêm thuộc tính ThangNam
            public string MaNhanVien { get; set; }
            public string HoTenNhanVien { get; set; }
            public string Email { get; set; }
            public string SDT { get; set; }
            public string DiaChi { get; set; }
            public decimal LuongCoBan { get; set; }
            public decimal KPI { get; set; }
            public decimal PhuCap { get; set; }
            public decimal TongLuong { get; set; }
        }

        public Edit_Luong(int nvID, DateTime thoiGian, string maNhanVien, string hoTen, string email, string sdt, string diaChi, decimal luongCoBan, decimal kpiDanhGia, decimal phuCap)
        {
            InitializeComponent();
            _nvID = nvID;
            _thoiGian = thoiGian;

            // Khởi tạo ViewModel
            _luongViewModel = new LuongViewModel
            {
                NvId = nvID,
                ThangNam = thoiGian.ToString("MM/yyyy"), // Chuyển DateTime thành MM/yyyy
                MaNhanVien = maNhanVien,
                HoTenNhanVien = hoTen,
                Email = email,
                SDT = sdt,
                DiaChi = diaChi,
                LuongCoBan = luongCoBan,
                KPI = kpiDanhGia,
                PhuCap = phuCap
            };

            // Hiển thị thông tin lên form
            txtThangNam.Text = _luongViewModel.ThangNam; // Hiển thị Tháng/Năm
            txtMaNhanVien.Text = _luongViewModel.MaNhanVien;
            txtHoTenNhanVien.Text = _luongViewModel.HoTenNhanVien;
            txtEmail.Text = _luongViewModel.Email;
            txtSDT.Text = _luongViewModel.SDT;
            txtDiaChi.Text = _luongViewModel.DiaChi;
            txtLuongCoBan.Text = _luongViewModel.LuongCoBan.ToString("N0");
            txtKPI.Text = (_luongViewModel.KPI * 100).ToString("F0") + "%";
            txtPhuCap.Text = _luongViewModel.PhuCap.ToString("N0");

            // Tính và hiển thị tổng lương (lấy từ cơ sở dữ liệu)
            UpdateTongLuong();

            // Đặt các trường thành chỉ đọc hoặc không chỉ đọc
            SetFieldsReadOnly();

            // Gán sự kiện TextChanged để cập nhật tổng lương
            txtLuongCoBan.TextChanged += txtLuongCoBan_TextChanged;
            txtPhuCap.TextChanged += txtPhuCap_TextChanged;

            // Gán sự kiện Click cho các nút
            btn_Luu.Click += btn_Luu_Click;
            btn_Huy.Click += btn_Huy_Click;
        }

        private void SetFieldsReadOnly()
        {
            txtThangNam.IsReadOnly = true; // Tháng/Năm chỉ đọc
            txtMaNhanVien.IsReadOnly = true;
            txtHoTenNhanVien.IsReadOnly = true;
            txtEmail.IsReadOnly = true;
            txtSDT.IsReadOnly = true;
            txtDiaChi.IsReadOnly = true;
            txtKPI.IsReadOnly = true;
            txtTongLuong.IsReadOnly = true;

            txtLuongCoBan.IsReadOnly = false; // Cho phép chỉnh sửa
            txtPhuCap.IsReadOnly = false; // Cho phép chỉnh sửa
        }

        private void UpdateTongLuong()
        {
            try
            {
                // Lấy giá trị tổng lương từ cột luong_ThucNhan trong bảng Luong
                string connectionString = ConfigurationManager.ConnectionStrings["QLDAConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT luong_ThucNhan FROM Luong WHERE nv_ID = @NvID AND luong_ThangNam = @ThangNam";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NvID", _nvID);
                        command.Parameters.AddWithValue("@ThangNam", _thoiGian.Date);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            decimal tongLuong = Convert.ToDecimal(result);
                            txtTongLuong.Text = tongLuong.ToString("N0");
                            _luongViewModel.TongLuong = tongLuong;
                        }
                        else
                        {
                            // Nếu không tìm thấy bản ghi, hiển thị 0
                            txtTongLuong.Text = "0";
                            _luongViewModel.TongLuong = 0;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                txtTongLuong.Text = "0";
                _luongViewModel.TongLuong = 0;
                MessageBox.Show($"Lỗi khi lấy tổng lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_Luu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra và lấy giá trị từ TextBox
                string luongCoBanText = txtLuongCoBan.Text.Replace(",", "");
                string phuCapText = txtPhuCap.Text.Replace(",", "");

                if (!decimal.TryParse(luongCoBanText, out decimal luongCoBan) || luongCoBan < 0)
                {
                    MessageBox.Show("Lương cơ bản phải là một số không âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(phuCapText, out decimal phuCap) || phuCap < 0)
                {
                    MessageBox.Show("Phụ cấp phải là một số không âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tính tổng lương để lưu vào luong_ThucNhan
                decimal kpiDanhGia = string.IsNullOrWhiteSpace(txtKPI.Text) ? 0 : decimal.Parse(txtKPI.Text.TrimEnd('%')) / 100m;
                decimal luongThuongKPI = luongCoBan * kpiDanhGia; // Lương thưởng KPI = Lương cơ bản * KPI
                decimal tongLuong = luongCoBan + luongThuongKPI + phuCap; // Tổng lương = Lương cơ bản + (Lương cơ bản * KPI) + Phụ cấp

                // Cập nhật ViewModel
                _luongViewModel.LuongCoBan = luongCoBan;
                _luongViewModel.PhuCap = phuCap;
                _luongViewModel.TongLuong = tongLuong;

                // Lưu vào cơ sở dữ liệu
                string connectionString = ConfigurationManager.ConnectionStrings["QLDAConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra và lưu vào bảng Luong
                    string checkLuongQuery = "SELECT COUNT(*) FROM Luong WHERE nv_ID = @NvID AND luong_ThangNam = @ThangNam";
                    using (SqlCommand checkCommand = new SqlCommand(checkLuongQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@NvID", _nvID);
                        checkCommand.Parameters.AddWithValue("@ThangNam", _thoiGian.Date); // Sử dụng luong_ThangNam
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            // Cập nhật bản ghi lương (cập nhật luong_PhuCap và luong_ThucNhan)
                            string updateLuongQuery = @"
                                UPDATE Luong 
                                SET luong_PhuCap = @PhuCap,
                                    luong_ThucNhan = @TongLuong
                                WHERE nv_ID = @NvID AND luong_ThangNam = @ThangNam";
                            using (SqlCommand command = new SqlCommand(updateLuongQuery, connection))
                            {
                                command.Parameters.AddWithValue("@PhuCap", phuCap);
                                command.Parameters.AddWithValue("@TongLuong", tongLuong);
                                command.Parameters.AddWithValue("@NvID", _nvID);
                                command.Parameters.AddWithValue("@ThangNam", _thoiGian.Date);
                                command.ExecuteNonQuery();
                            }

                            // Cập nhật lương cơ bản trong bảng NhanVien
                            string updateNhanVienQuery = @"
                                UPDATE NhanVien 
                                SET nv_LuongCoBan = @LuongCoBan
                                WHERE nv_ID = @NvID";
                            using (SqlCommand command = new SqlCommand(updateNhanVienQuery, connection))
                            {
                                command.Parameters.AddWithValue("@LuongCoBan", luongCoBan);
                                command.Parameters.AddWithValue("@NvID", _nvID);
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Thêm mới bản ghi lương (chèn luong_PhuCap và luong_ThucNhan)
                            string insertLuongQuery = @"
                                INSERT INTO Luong (nv_ID, luong_ThangNam, luong_PhuCap, luong_ThucNhan)
                                VALUES (@NvID, @ThangNam, @PhuCap, @TongLuong)";
                            using (SqlCommand command = new SqlCommand(insertLuongQuery, connection))
                            {
                                command.Parameters.AddWithValue("@NvID", _nvID);
                                command.Parameters.AddWithValue("@ThangNam", _thoiGian.Date);
                                command.Parameters.AddWithValue("@PhuCap", phuCap);
                                command.Parameters.AddWithValue("@TongLuong", tongLuong);
                                command.ExecuteNonQuery();
                            }

                            // Cập nhật lương cơ bản trong bảng NhanVien
                            string updateNhanVienQuery = @"
                                UPDATE NhanVien 
                                SET nv_LuongCoBan = @LuongCoBan
                                WHERE nv_ID = @NvID";
                            using (SqlCommand command = new SqlCommand(updateNhanVienQuery, connection))
                            {
                                command.Parameters.AddWithValue("@LuongCoBan", luongCoBan);
                                command.Parameters.AddWithValue("@NvID", _nvID);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    connection.Close();
                }

                // Cập nhật lại hiển thị tổng lương sau khi lưu
                UpdateTongLuong();

                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Window.GetWindow(this).DialogResult = true;
                Window.GetWindow(this).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }

        private void txtLuongCoBan_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void txtPhuCap_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}