using QuanLyDuAn.Controls;
using QuanLyDuAn.Forms;
using QuanLyDuAn.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuanLyDuAn
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private readonly ThucTapQuanLyDuAnContext _context;
        private ProjectsControl _projectsControl;
        private int _currentUserId;
        private string _currentUserName;
        public static string nguoidangnhap;
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
            
            UserName.Text = tenTaiKhoan;
            nguoidangnhap = UserName.Text;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateCurrentTime();
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
            if (UserButton.ContextMenu != null)
            {
                UserButton.ContextMenu.PlacementTarget = UserButton;
                UserButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                UserButton.ContextMenu.IsOpen = true;
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
    }
}