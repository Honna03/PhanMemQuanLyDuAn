using QuanLyDuAn.Controls;
using QuanLyDuAn.Forms;
using QuanLyDuAn.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using QuanLyDuAn.functions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Windows.Data;

namespace QuanLyDuAn
{
    public partial class MainWindow : Window
    {
        ThucTapQuanLyDuAnContext context1 = new ThucTapQuanLyDuAnContext();
        private DispatcherTimer timer;
        private readonly ThucTapQuanLyDuAnContext _context;
        private ProjectsControl _projectsControl;
        private int _currentUserId;
        private string _currentUserName;
        public static string nguoidangnhap;
        private ThongTinCongTy selectedCompany;
        public MainWindow(string tenTaiKhoan)
        {
            InitializeComponent();
            _context = new ThucTapQuanLyDuAnContext();
            MainContent.Content = new TrangChu();

            /*  _currentUserId = userId;
              var user = _context.NhanViens.FirstOrDefault(n => n.NvId == userId);
              _currentUserName = user != null ? user.NvTen : "Admin";

              if (UserNameTextBlock != null)
              {
                  UserNameTextBlock.Text = _currentUserName;
              }*/

            var user = _context.NhanViens.FirstOrDefault(n => n.NvTaiKhoan == tenTaiKhoan);
            if (user != null)
            {
                _currentUserId = user.NvId;
                _currentUserName = user.NvTen;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserName.Text = tenTaiKhoan;
            nguoidangnhap = UserName.Text;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateCurrentTime();
            LoadNotifications(_currentUserId);

            ThemKpiThangHienTai();
            LoadLogo();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void LoadProjectsControl()
        {
            try
            {
                var projects = _context.DuAns
                    .Include(d => d.TtMaNavigation)
                    .Include(d => d.NvIdNguoiTaoNavigation)
                    .ToList();
                var statuses = _context.TrangThais.ToList();
                var creators = _context.NhanViens
                    .Select(n => new Creator { NvId = n.NvId, Name = n.NvTen })
                    .ToList();
                var allTasks = _context.CongViecs
                    .Include(c => c.Da)
                    .Include(c => c.TtMaNavigation)
                    .ToList();

                _projectsControl = new ProjectsControl();
                _projectsControl.SetProjects(projects, statuses, creators, allTasks);

                _projectsControl.AddProjectRequested += ProjectsControl_AddProjectRequested;
                _projectsControl.RefreshRequested += ProjectsControl_RefreshRequested;
                _projectsControl.EditProjectRequested += ProjectsControl_EditProjectRequested;
                _projectsControl.DeleteProjectRequested += ProjectsControl_DeleteProjectRequested;
                _projectsControl.ViewDetailsRequested += ProjectsControl_ViewDetailsRequested;

                MainContent.Content = _projectsControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReloadProjectsAsync()
        {
            try
            {
                var projects = await _context.DuAns
                    .Include(d => d.TtMaNavigation)
                    .Include(d => d.NvIdNguoiTaoNavigation)
                    .ToListAsync();
                var statuses = await _context.TrangThais.ToListAsync();
                var creators = await _context.NhanViens
                    .Select(n => new Creator { NvId = n.NvId, Name = n.NvTen })
                    .ToListAsync();
                var allTasks = await _context.CongViecs
                    .Include(c => c.Da)
                    .Include(c => c.TtMaNavigation)
                    .ToListAsync();

                _projectsControl.SetProjects(projects, statuses, creators, allTasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải lại dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ProjectsControl_AddProjectRequested(object sender, EventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(null, _context, _currentUserId),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Closed += async (s, args) =>
            {
                _projectsControl.OverlayGrid.Visibility = Visibility.Collapsed;
                await ReloadProjectsAsync();
            };
            window.ShowDialog();
        }

        private void ProjectsControl_RefreshRequested(object sender, EventArgs e)
        {
            ReloadProjectsAsync();
        }

        private void ProjectsControl_EditProjectRequested(object sender, ProjectEventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(e.DaId, _context, _currentUserId),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Closed += async (s, args) =>
            {
                _projectsControl.OverlayGrid.Visibility = Visibility.Collapsed;
                await ReloadProjectsAsync();
            };
            window.ShowDialog();
        }

        private async void ProjectsControl_DeleteProjectRequested(object sender, ProjectEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dự án này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var project = await _context.DuAns
                        .Include(d => d.NhanVienThamGiaDuAns)
                        .ThenInclude(n => n.CongViecs)
                        .FirstOrDefaultAsync(d => d.DaId == e.DaId);

                    if (project != null)
                    {
                        foreach (var member in project.NhanVienThamGiaDuAns)
                        {
                            _context.CongViecs.RemoveRange(member.CongViecs);
                        }
                        _context.NhanVienThamGiaDuAns.RemoveRange(project.NhanVienThamGiaDuAns);
                        _context.DuAns.Remove(project);
                        await _context.SaveChangesAsync();
                        MessageBox.Show("Xóa dự án thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ReloadProjectsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa dự án: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProjectsControl_ViewDetailsRequested(object sender, ProjectEventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(e.DaId, _context, _currentUserId) { IsReadOnly = true },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Closed += (s, args) => _projectsControl.OverlayGrid.Visibility = Visibility.Collapsed;
            window.ShowDialog();
        }

        private void btn_NhanVien_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachNhanVien();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        private void btn_QLCV_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachCongViec();
        }

        private void btn_QLDA_Click(object sender, RoutedEventArgs e)
        {
            LoadProjectsControl();
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TrangChu();
        }
        private void btn_KPI_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new KPI();
        }

        private void btn_Luong_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuanLyDuAn.Forms.Luong();
        }

        private void btn_DangXuat_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Window parentWindow = Window.GetWindow(sender as DependencyObject);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
            loginWindow.Show();
        }

        private void btn_Thoat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc thoát không?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox searchBox && searchBox.Text == "Tìm kiếm...")
            {
                searchBox.Text = "";
                searchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox searchBox && string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.Text = "Tìm kiếm...";
                searchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ThemKpiThangHienTai()
        {
            string sql = @" DECLARE @currentMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
                        INSERT INTO KPI (nv_ID, kpi_ThangNam, kpi_PhanTram)
                        SELECT nv_ID, @currentMonth, 0
                        FROM NhanVien
                        WHERE NOT EXISTS (
                        SELECT 1 FROM KPI WHERE nv_ID = NhanVien.nv_ID AND kpi_ThangNam = @currentMonth
                    );";
            context1.Database.ExecuteSqlRaw(sql);
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Kiểm tra nếu MainContent chưa được khởi tạo thì bỏ qua
            if (MainContent == null)
            {
                return;
            }

            string searchText = (sender as TextBox)?.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText) || SearchBox.Text == "Tìm kiếm")
            {
                searchText = string.Empty;
            }

            // Kiểm tra UserControl hiện tại trong MainContent
            if (MainContent.Content is KPI kpiControl)
            {
                kpiControl.Search(searchText); // Gọi phương thức tìm kiếm của KPI
            }

            if (MainContent.Content is ProjectsControl projectsControl)
            {
                projectsControl.Search(searchText);
            }

            if (MainContent.Content is DanhSachCongViec danhSachCongViec)
            {
                danhSachCongViec.Search(searchText);
            }

            if (MainContent.Content is DanhSachNhanVien danhSachNhanVien)
            {
                danhSachNhanVien.Search(searchText);
            }

            if (MainContent.Content is QuanLyDuAn.Forms.Luong luong)
            {
                luong.Search(searchText);
            }
        }

        private async void LoadNotifications(int nvId)
        {
            try
            {
                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    using (var connection = context.Database.GetDbConnection() as SqlConnection)
                    {
                        await connection.OpenAsync();

                        using (var command = new SqlCommand("sp_LayDanhSachThongBao", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@nv_ID", nvId);
                            command.Parameters.AddWithValue("@TinhTrang", DBNull.Value); // NULL: lấy tất cả
                            command.Parameters.AddWithValue("@SoLuong", 50);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                var notifications = new List<NotificationViewModel>();
                                while (await reader.ReadAsync())
                                {
                                    notifications.Add(new NotificationViewModel
                                    {
                                        TbId = reader.GetInt32(reader.GetOrdinal("tb_ID")),
                                        TbNoiDung = reader.GetString(reader.GetOrdinal("tb_NoiDung")),
                                        TbThoiGian = reader.IsDBNull(reader.GetOrdinal("tb_ThoiGian")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("tb_ThoiGian")),
                                        TbTinhTrang = reader.GetBoolean(reader.GetOrdinal("tb_TinhTrang")),
                                        TbLoai = reader.GetString(reader.GetOrdinal("tb_Loai")),
                                        NvIdNguoiGui = reader.IsDBNull(reader.GetOrdinal("nv_ID_NguoiGui")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("nv_ID_NguoiGui")),
                                        TenNguoiGui = reader.GetString(reader.GetOrdinal("TenNguoiGui")),
                                        CvId = reader.IsDBNull(reader.GetOrdinal("cv_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("cv_ID")),
                                        DaId = reader.IsDBNull(reader.GetOrdinal("da_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("da_ID")),
                                        TenCongViec = reader.IsDBNull(reader.GetOrdinal("TenCongViec")) ? null : reader.GetString(reader.GetOrdinal("TenCongViec"))
                                    });
                                }
                                NotificationList.ItemsSource = notifications;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshNotifications(int nvId)
        {
            LoadNotifications(nvId);
            UpdateNotificationBadge(nvId);
        }

        private async void UpdateNotificationBadge(int nvId)
        {
            try
            {
                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    using (var connection = context.Database.GetDbConnection() as SqlConnection)
                    {
                        await connection.OpenAsync();

                        using (var command = new SqlCommand("sp_DemThongBaoChuaDoc", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@nv_ID", nvId);

                            var result = await command.ExecuteScalarAsync();
                            int count = result != null ? Convert.ToInt32(result) : 0;
                            NotificationBadge.Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
                            NotificationCountText.Text = count.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật badge: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NotificationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = NotificationList.SelectedItem as NotificationViewModel;
            if (selected != null && !selected.TbTinhTrang) // Chỉ đánh dấu nếu thông báo chưa đọc
            {
                try
                {
                    using (var context = new ThucTapQuanLyDuAnContext())
                    {
                        // Gọi stored procedure sp_DanhDauThongBaoDaDoc
                        context.Database.ExecuteSqlRaw(
                            "EXEC sp_DanhDauThongBaoDaDoc @tb_ID, @nv_ID_NguoiNhan",
                            new SqlParameter("@tb_ID", selected.TbId),
                            new SqlParameter("@nv_ID_NguoiNhan", _currentUserId)
                        );
                    }

                    // Cập nhật lại giao diện
                    LoadNotifications(_currentUserId);
                    UpdateNotificationBadge(_currentUserId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đánh dấu thông báo đã đọc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MarkAllAsRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new ThucTapQuanLyDuAnContext())
                {
                    // Gọi stored procedure sp_DanhDauTatCaThongBaoDaDoc
                    context.Database.ExecuteSqlRaw(
                        "EXEC sp_DanhDauTatCaThongBaoDaDoc @nv_ID",
                        new SqlParameter("@nv_ID", _currentUserId)
                    );
                }

                // Cập nhật lại giao diện
                LoadNotifications(_currentUserId);
                UpdateNotificationBadge(_currentUserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đánh dấu tất cả thông báo đã đọc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NotifyMainWindowToRefreshNotifications(int nvId)
        {
            if (_currentUserId == nvId)
            {
                LoadNotifications(nvId);
                UpdateNotificationBadge(nvId);
            }
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNotifications(_currentUserId);
            UpdateNotificationBadge(_currentUserId);

            // Kiểm tra trạng thái của popup và mở/đóng
            if (NotificationPopup.IsOpen)
            {
                NotificationPopup.IsOpen = false;
            }
            else
            {
                NotificationPopup.IsOpen = true;
            }
        }

        private void btn_Infor_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ThongTin();
        }

        //logo
        private void LoadLogo()
        {
            try
            {
                selectedCompany = _context.ThongTinCongTies.FirstOrDefault();
                if (selectedCompany != null)
                {
                    logoButton.DataContext = selectedCompany;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy logo công ty.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải logo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lớp ánh xạ cho sp_DemThongBaoChuaDoc
        public class DbResult
        {
            public int SoThongBaoChuaDoc { get; set; }
        }

        // Lớp ánh xạ cho sp_LayDanhSachThongBao
        public class NotificationViewModel
        {
            public int TbId { get; set; }
            public string TbNoiDung { get; set; }
            public DateTime? TbThoiGian { get; set; }
            public bool TbTinhTrang { get; set; }
            public string TbLoai { get; set; }
            public int? NvIdNguoiGui { get; set; }
            public string TenNguoiGui { get; set; }
            public int? CvId { get; set; }
            public int? DaId { get; set; }
            public string TenCongViec { get; set; }
        }

        // Converter để thay đổi giao diện thông báo
        public class BooleanToFontWeightConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? FontWeights.Normal : FontWeights.Bold; // Đã đọc: bình thường, chưa đọc: đậm
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class BooleanToBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? Brushes.Gray : Brushes.Black; // Đã đọc: xám, chưa đọc: đen
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}