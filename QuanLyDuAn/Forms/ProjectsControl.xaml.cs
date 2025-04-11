using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyDuAn.Controls
{
    public partial class ProjectsControl : UserControl
    {
        private readonly ThucTapQuanLyDuAnContext _context;
        private List<DuAn> _projects;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalItems;
        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public ProjectsControl(ThucTapQuanLyDuAnContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            SeriesCollection = new SeriesCollection();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                if (_context == null || _context.Database == null || !_context.Database.CanConnect())
                {
                    MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Context không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StatusFilterComboBox.ItemsSource = await _context.TrangThais.ToListAsync();
                CreatorFilterComboBox.ItemsSource = await _context.NhanViens.Select(n => new { n.NvId, Name = n.NvTen }).ToListAsync();

                await LoadProjectsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                if (_context == null || _context.Database == null || !_context.Database.CanConnect())
                {
                    throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu. Context không hợp lệ.");
                }

                var query = _context.DuAns
                    .Include(d => d.TtMaNavigation)
                    .Include(d => d.NvIdNguoiTaoNavigation)
                    .Include(d => d.NhanVienThamGiaDuAns).ThenInclude(n => n.CongViecs).ThenInclude(c => c.TtMaNavigation)
                    .AsQueryable();

                if (StatusFilterComboBox.SelectedValue != null)
                {
                    string selectedStatus = StatusFilterComboBox.SelectedValue.ToString();
                    query = query.Where(d => d.TtMa == selectedStatus);
                }

                if (CreatorFilterComboBox.SelectedValue != null)
                {
                    int selectedCreator = (int)CreatorFilterComboBox.SelectedValue;
                    query = query.Where(d => d.NvIdNguoiTao == selectedCreator);
                }

                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Tìm kiếm...")
                {
                    string searchText = SearchTextBox.Text.ToLower();
                    query = query.Where(d => d.DaTen.ToLower().Contains(searchText));
                }

                _totalItems = await query.CountAsync();

                _projects = await query
                    .OrderBy(d => d.DaId)
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

                ProjectsDataGrid.ItemsSource = _projects;

                UpdatePaginationInfo();

                if (_projects.Any())
                {
                    ProjectsDataGrid.SelectedItem = _projects.First();
                }
                else
                {
                    UpdateChart(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách dự án: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePaginationInfo()
        {
            int totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
            PageInfoTextBlock.Text = $"Trang {_currentPage}/{totalPages} ({_totalItems} dự án)";
        }

        private async void UpdateChart(DuAn project)
        {
            try
            {
                SeriesCollection.Clear();

                if (project == null)
                {
                    ChartTitleTextBlock.Text = "Vui lòng chọn một dự án để xem trạng thái công việc.";
                    return;
                }

                var taskStatusGroups = await _context.CongViecs
                    .Where(cv => cv.DaId == project.DaId)
                    .GroupBy(cv => cv.TtMaNavigation.TtTen)
                    .Select(g => new
                    {
                        Status = g.Key ?? "Không xác định",
                        Count = g.Count()
                    })
                    .ToListAsync();

                if (!taskStatusGroups.Any())
                {
                    ChartTitleTextBlock.Text = $"Trạng thái công việc của dự án: {project.DaTen} (Không có công việc)";
                    return;
                }

                ChartTitleTextBlock.Text = $"Trạng thái công việc của dự án: {project.DaTen}";

                foreach (var group in taskStatusGroups)
                {
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = group.Status,
                        Values = new ChartValues<int> { group.Count },
                        DataLabels = true,
                        LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P0})"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật biểu đồ: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                ChartTitleTextBlock.Text = "Không thể hiển thị biểu đồ do lỗi.";
            }
        }

        private async void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectsDataGrid.SelectedItem is DuAn selectedProject)
            {
                UpdateChart(selectedProject);
            }
            else
            {
                UpdateChart(null);
            }
        }

        private async void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            await LoadProjectsAsync();
        }

        private async void CreatorFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            await LoadProjectsAsync();
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentPage = 1;
            await LoadProjectsAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = -1;
            CreatorFilterComboBox.SelectedIndex = -1;
            SearchTextBox.Text = "Tìm kiếm...";
            _currentPage = 1;
            await LoadProjectsAsync();
        }

        private async void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadProjectsAsync();
            }
        }

        private async void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)_totalItems / _pageSize);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                await LoadProjectsAsync();
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayGrid.Visibility = Visibility.Visible;
            EditContentControl.Content = new Edit_DuAn(null, _context);
            (EditContentControl.Content as Edit_DuAn).ProjectSaved += async () =>
            {
                OverlayGrid.Visibility = Visibility.Collapsed;
                EditContentControl.Content = null;
                _currentPage = 1;
                await LoadProjectsAsync();
            };
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                OverlayGrid.Visibility = Visibility.Visible;
                EditContentControl.Content = new Edit_DuAn(daId, _context);
                (EditContentControl.Content as Edit_DuAn).ProjectSaved += async () =>
                {
                    OverlayGrid.Visibility = Visibility.Collapsed;
                    EditContentControl.Content = null;
                    await LoadProjectsAsync();
                };
            }
        }

        private async void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dự án này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var project = await _context.DuAns
                            .Include(d => d.NhanVienThamGiaDuAns)
                            .ThenInclude(n => n.CongViecs)
                            .FirstOrDefaultAsync(d => d.DaId == daId);

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
                            await LoadProjectsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa dự án: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                OverlayGrid.Visibility = Visibility.Visible;
                var editControl = new Edit_DuAn(daId, _context) { IsReadOnly = true };
                EditContentControl.Content = editControl;
                editControl.ProjectSaved += () =>
                {
                    OverlayGrid.Visibility = Visibility.Collapsed;
                    EditContentControl.Content = null;
                };
            }
        }
    }
}