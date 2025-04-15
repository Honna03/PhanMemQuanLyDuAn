using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyDuAn.Models;
using QuanLyDuAn;
using LiveCharts;
using LiveCharts.Wpf;

namespace QuanLyDuAn.Controls
{
    public partial class ProjectsControl : UserControl
    {
        private List<DuAn> _projects = new List<DuAn>();
        private List<CongViec> _allTasks = new List<CongViec>();
        private int _currentPage = 1;
        private const int PageSize = 10;

        public ProjectsControl()
        {
            InitializeComponent();
        }

        public void SetProjects(List<DuAn> projects, List<TrangThai> statuses, List<Creator> creators, List<CongViec> allTasks)
        {
            _projects = projects ?? new List<DuAn>();
            _allTasks = allTasks ?? new List<CongViec>();
            StatusFilterComboBox.ItemsSource = statuses ?? new List<TrangThai>();
            CreatorFilterComboBox.ItemsSource = creators ?? new List<Creator>();
            LoadPage();
            UpdatePieChart();
        }

        private void LoadPage()
        {
            var filteredProjects = ApplyFilters();
            var pagedProjects = filteredProjects
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            if (ProjectsDataGrid != null)
            {
                ProjectsDataGrid.ItemsSource = pagedProjects;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ProjectsDataGrid is null in LoadPage.");
            }

            UpdatePaginationInfo(filteredProjects.Count);
        }

        private List<DuAn> ApplyFilters()
        {
            if (_projects == null)
            {
                _projects = new List<DuAn>();
            }

            var filtered = _projects.ToList();

            if (StatusFilterComboBox.SelectedValue is TrangThai selectedStatus)
            {
                filtered = filtered.Where(p => p.TtMa == selectedStatus.TtMa).ToList();
            }

            if (CreatorFilterComboBox.SelectedValue is Creator selectedCreator)
            {
                filtered = filtered.Where(p => p.NvIdNguoiTao == selectedCreator.NvId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != "Tìm kiếm...")
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(p => p.DaTen.ToLower().Contains(searchText)).ToList();
            }

            return filtered;
        }

        private void UpdatePieChart()
        {
            if (StatusPieChart == null)
                return;

            // Lấy dự án được chọn
            if (ProjectsDataGrid.SelectedItem is DuAn selectedProject)
            {
                // Lấy danh sách công việc của dự án được chọn
                var projectTasks = _allTasks
                    .Where(t => t.DaId == selectedProject.DaId)
                    .ToList();

                if (projectTasks.Any())
                {
                    // Nhóm công việc theo trạng thái (TtMa)
                    var taskByStatus = projectTasks
                        .GroupBy(t => t.TtMaNavigation.TtTen)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToList();

                    var series = new SeriesCollection();

                    foreach (var status in taskByStatus)
                    {
                        series.Add(new PieSeries
                        {
                            Title = status.Status,
                            Values = new ChartValues<int> { status.Count },
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P0})"
                        });
                    }

                    StatusPieChart.Series = series;
                }
                else
                {
                    // Không có công việc
                    StatusPieChart.Series = new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = "Không có công việc",
                            Values = new ChartValues<int> { 1 },
                            DataLabels = false
                        }
                    };
                }
            }
            else
            {
                // Không có dự án nào được chọn
                StatusPieChart.Series = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Chọn một dự án",
                        Values = new ChartValues<int> { 1 },
                        DataLabels = false
                    }
                };
            }
        }

        private void UpdatePaginationInfo(int totalItems)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (PageInfoTextBlock != null)
            {
                PageInfoTextBlock.Text = $"Trang {_currentPage}/{totalPages} ({totalItems} dự án)";
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            LoadPage();
        }

        private void CreatorFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            LoadPage();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentPage = 1;
            LoadPage();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = -1;
            CreatorFilterComboBox.SelectedIndex = -1;
            if (SearchTextBox != null)
            {
                SearchTextBox.Text = "Tìm kiếm...";
            }
            _currentPage = 1;
            LoadPage();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPage();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            var filteredCount = ApplyFilters().Count;
            if (_currentPage < Math.Ceiling((double)filteredCount / PageSize))
            {
                _currentPage++;
                LoadPage();
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (OverlayGrid != null)
            {
                OverlayGrid.Visibility = Visibility.Visible;
            }
            AddProjectRequested?.Invoke(this, EventArgs.Empty);
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                if (OverlayGrid != null)
                {
                    OverlayGrid.Visibility = Visibility.Visible;
                }
                EditProjectRequested?.Invoke(this, new ProjectEventArgs(daId));
            }
        }

        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                DeleteProjectRequested?.Invoke(this, new ProjectEventArgs(daId));
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                if (OverlayGrid != null)
                {
                    OverlayGrid.Visibility = Visibility.Visible;
                }
                ViewDetailsRequested?.Invoke(this, new ProjectEventArgs(daId));
            }
        }

        private void CloseOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (OverlayGrid != null)
            {
                OverlayGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePieChart();
        }

        public event EventHandler AddProjectRequested;
        public event EventHandler<ProjectEventArgs> EditProjectRequested;
        public event EventHandler<ProjectEventArgs> DeleteProjectRequested;
        public event EventHandler<ProjectEventArgs> ViewDetailsRequested;
    }

    public class ProjectEventArgs : EventArgs
    {
        public int DaId { get; }
        public ProjectEventArgs(int daId) => DaId = daId;
    }
}