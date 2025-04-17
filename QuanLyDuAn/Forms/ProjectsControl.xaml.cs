using LiveCharts;
using LiveCharts.Wpf;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyDuAn.Controls
{
    public partial class ProjectsControl : UserControl
    {
        private List<DuAn> _allProjects;
        private List<TrangThai> _statuses;
        private List<Creator> _creators;
        private List<CongViec> _allTasks;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;

        public event EventHandler AddProjectRequested;
        public event EventHandler RefreshRequested;
        public event EventHandler<ProjectEventArgs> EditProjectRequested;
        public event EventHandler<ProjectEventArgs> DeleteProjectRequested;
        public event EventHandler<ProjectEventArgs> ViewDetailsRequested;

        public ProjectsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetProjects(List<DuAn> projects, List<TrangThai> statuses, List<Creator> creators, List<CongViec> allTasks)
        {
            _allProjects = projects ?? new List<DuAn>();
            _statuses = statuses ?? new List<TrangThai>();
            _creators = creators ?? new List<Creator>();
            _allTasks = allTasks ?? new List<CongViec>();

            if (_allProjects.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu dự án để hiển thị.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Kiểm tra dữ liệu ngày và trạng thái
                foreach (var project in _allProjects)
                {
                    System.Diagnostics.Debug.WriteLine($"Project ID: {project.DaId}, Name: {project.DaTen}, StartDate: {project.DaBatDau?.ToString("dd/MM/yyyy") ?? "null"}, EndDate: {project.DaKetThuc?.ToString("dd/MM/yyyy") ?? "null"}, Status: {project.TtMa}");
                }
            }

            UpdateDataGrid();
            UpdatePagingInfo();
            if (_allProjects.Any())
            {
                UpdatePieChart(_allProjects.First());
            }
            else
            {
                UpdatePieChart(null);
            }
        }

        private void UpdateDataGrid()
        {
            if (_allProjects == null)
            {
                _allProjects = new List<DuAn>();
            }

            var pagedProjects = _allProjects
                .Skip((_currentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            ProjectsDataGrid.ItemsSource = pagedProjects;

            if (pagedProjects.Any())
            {
                ProjectsDataGrid.SelectedItem = pagedProjects.First();
                UpdatePieChart(pagedProjects.First());
            }
            else
            {
                UpdatePieChart(null);
            }
        }

        private void UpdatePieChart(DuAn selectedProject)
        {
            if (selectedProject == null || _allTasks == null || !_allTasks.Any())
            {
                CompletedSeries.Values = new ChartValues<int> { 0 };
                RemainingSeries.Values = new ChartValues<int> { 0 };
                NotStartedSeries.Values = new ChartValues<int> { 0 };
                return;
            }

            var projectTasks = _allTasks.Where(t => t != null && t.DaId == selectedProject.DaId).ToList();
            System.Diagnostics.Debug.WriteLine($"Project ID: {selectedProject.DaId}, Task Count: {projectTasks.Count}");

            if (!projectTasks.Any())
            {
                CompletedSeries.Values = new ChartValues<int> { 0 };
                RemainingSeries.Values = new ChartValues<int> { 0 };
                NotStartedSeries.Values = new ChartValues<int> { 0 };
                return;
            }

            int completedTasks = projectTasks.Count(t => t.TtMa == "ht");
            int remainingTasks = projectTasks.Count(t => t.TtMa == "dth");
            int notStartedTasks = projectTasks.Count(t => t.TtMa == "chth");
            int totalTasks = completedTasks + remainingTasks + notStartedTasks;

            // Gán giá trị cho biểu đồ
            CompletedSeries.Values = new ChartValues<int> { completedTasks };
            RemainingSeries.Values = new ChartValues<int> { remainingTasks };
            NotStartedSeries.Values = new ChartValues<int> { notStartedTasks };

            // Tùy chỉnh nhãn hiển thị phần trăm
            CompletedSeries.LabelPoint = chartPoint => totalTasks > 0 ? $"{Math.Round(chartPoint.Y * 100.0 / totalTasks, 1)}%" : "0%";
            RemainingSeries.LabelPoint = chartPoint => totalTasks > 0 ? $"{Math.Round(chartPoint.Y * 100.0 / totalTasks, 1)}%" : "0%";
            NotStartedSeries.LabelPoint = chartPoint => totalTasks > 0 ? $"{Math.Round(chartPoint.Y * 100.0 / totalTasks, 1)}%" : "0%";
        }

        private void UpdatePagingInfo()
        {
            int totalItems = _allProjects?.Count ?? 0;
            int totalPages = (int)Math.Ceiling((double)totalItems / _itemsPerPage);
            PagingInfoTextBlock.Text = $"Trang {_currentPage} / {totalPages} ({totalItems} dự án)";
        }

        private void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectsDataGrid.SelectedItem is DuAn selectedProject)
            {
                UpdatePieChart(selectedProject);
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdateDataGrid();
                UpdatePagingInfo();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)(_allProjects?.Count ?? 0) / _itemsPerPage);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                UpdateDataGrid();
                UpdatePagingInfo();
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
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

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int daId)
            {
                try
                {
                    DeleteProjectRequested?.Invoke(this, new ProjectEventArgs(daId));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa dự án: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
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
    }

    public class ProjectEventArgs : EventArgs
    {
        public int DaId { get; }

        public ProjectEventArgs(int daId)
        {
            DaId = daId;
        }
    }
}