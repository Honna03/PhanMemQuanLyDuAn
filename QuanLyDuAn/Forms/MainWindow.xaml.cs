using QuanLyDuAn.Controls;
using QuanLyDuAn.Forms;

using QuanLyDuAn.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace QuanLyDuAn
{
    public partial class MainWindow : Window
    {
        ThucTapQuanLyDuAnContext context = new ThucTapQuanLyDuAnContext();

        private const string PlaceholderText = "Tìm kiếm...";
        private DispatcherTimer timer;

        public static string nguoidangnhap;
        private bool isInitialized = false; // Biến flag để kiểm soát

        private readonly ThucTapQuanLyDuAnContext _context;
        private ProjectsControl _projectsControl;

        public MainWindow(string tenTaiKhoan)
        {
            InitializeComponent();
            _context = new ThucTapQuanLyDuAnContext();
            MainContent.Content = new TrangChu();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateCurrentTime();
            isInitialized = true; // Đánh dấu là đã khởi tạo xong

            //cái này để láy tài khoản nhân viên hiện lên Mainwindow khi đăng nhập
            UserName.Text = tenTaiKhoan;
            nguoidangnhap = UserName.Text;
            //cái này để tự thêm kpi nhân viên nếu nhân viên chưa có công việc được giao
            ThemKpiThangHienTai();
        }

        private void ThemKpiThangHienTai()
        {
            string sql = @"
            DECLARE @currentMonth DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

            INSERT INTO KPI (nv_ID, kpi_ThangNam, kpi_PhanTram)
            SELECT nv_ID, @currentMonth, 0
            FROM NhanVien
            WHERE NOT EXISTS (
                SELECT 1 FROM KPI WHERE nv_ID = NhanVien.nv_ID AND kpi_ThangNam = @currentMonth
            );";
            context.Database.ExecuteSqlRaw(sql);
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
                // Lấy tất cả công việc, bao gồm thông tin dự án và trạng thái
                var allTasks = _context.CongViecs
                    .Include(c => c.Da)
                    .Include(c => c.TtMaNavigation)
                    .ToList();

                _projectsControl = new ProjectsControl();
                _projectsControl.SetProjects(projects, statuses, creators, allTasks);

                _projectsControl.AddProjectRequested += ProjectsControl_AddProjectRequested;
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

        private async System.Threading.Tasks.Task ReloadProjectsAsync()
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

        private void ProjectsControl_AddProjectRequested(object sender, EventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(null, _context),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Closed += async (s, args) =>
            {
                _projectsControl.OverlayGrid.Visibility = Visibility.Collapsed;
                await ReloadProjectsAsync();
            };
            window.ShowDialog();
        }

        private void ProjectsControl_EditProjectRequested(object sender, ProjectEventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(e.DaId, _context),
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
                Content = new Edit_DuAn(e.DaId, _context) { IsReadOnly = true },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Closed += (s, args) => _projectsControl.OverlayGrid.Visibility = Visibility.Collapsed;
            window.ShowDialog();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == PlaceholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.White;
            }
        }
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserButton.ContextMenu != null)
            {
                UserButton.ContextMenu.PlacementTarget = UserButton; // Đặt vị trí ContextMenu tại Button
                UserButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; // Hiển thị submenu bên dưới Button
                UserButton.ContextMenu.IsOpen = true; // Mở ContextMenu
            }
        }


        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = PlaceholderText;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void btn_NhanVien_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachNhanVien();
        }

        private void btn_BaoCao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainContent.Content = new BaoCao(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở form báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btn_AddDuAn_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Content = new Edit_DuAn(null, _context),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Kiểm tra nếu MainContent chưa được khởi tạo thì bỏ qua
            if (MainContent == null)
            {
                return;
            }

            string searchText = (sender as TextBox)?.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText) || searchText == PlaceholderText.ToLower())
            {
                searchText = string.Empty;
            }

            // Kiểm tra UserControl hiện tại trong MainContent
            if (MainContent.Content is KPI kpiControl)
            {
                kpiControl.Search(searchText); // Gọi phương thức tìm kiếm của KPI
            }
        }

        private void btn_QLKPI_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}