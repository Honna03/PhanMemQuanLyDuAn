using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace QuanLyDuAn.Controls
{
    public partial class Edit_DuAn : UserControl, IDisposable
    {
        public bool IsReadOnly { get; set; }
        private readonly ThucTapQuanLyDuAnContext _context;
        private DuAn _project;
        private List<TaskWrapper> _tasks = new List<TaskWrapper>();
        private List<NhanVienThamGiaDuAn> _members = new List<NhanVienThamGiaDuAn>();
        private string _selectedFilePath;
        private bool _disposed = false;

        public event Action ProjectSaved;

        public Edit_DuAn(int? daId, ThucTapQuanLyDuAnContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeAsync(daId);
        }

        private async void InitializeAsync(int? daId)
        {
            await LoadDataAsync(daId);
        }

        private async Task LoadDataAsync(int? daId)
        {
            try
            {
                // Load project data
                _project = daId.HasValue
                    ? await _context.DuAns
                        .Include(d => d.NhanVienThamGiaDuAns).ThenInclude(n => n.Nv)
                        .Include(d => d.NhanVienThamGiaDuAns).ThenInclude(n => n.VtMaNavigation)
                        .Include(d => d.NhanVienThamGiaDuAns).ThenInclude(n => n.CongViecs).ThenInclude(c => c.TtMaNavigation)
                        .FirstOrDefaultAsync(d => d.DaId == daId.Value)
                    : new DuAn();

                if (_project == null)
                {
                    MessageBox.Show("Không tìm thấy dự án!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update title
                TitleTextBlock.Text = daId.HasValue ? "Chỉnh sửa dự án" : "Thêm dự án";

                // Set default creator for new projects
                if (_project.DaId == 0)
                {
                    if (CurrentUser.NvId <= 0)
                    {
                        MessageBox.Show("Không thể xác định người dùng hiện tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _project.NvIdNguoiTao = CurrentUser.NvId;
                    _project.DaBatDau = DateTime.Now;
                }

                // Load dropdown data
                StatusComboBox.ItemsSource = await _context.TrangThais.ToListAsync();
                MemberComboBox.ItemsSource = await _context.NhanViens.ToListAsync();
                RoleComboBox.ItemsSource = await _context.VaiTros.ToListAsync();
                CreatorComboBox.ItemsSource = await _context.NhanViens.ToListAsync();
                TaskStatusComboBox.ItemsSource = await _context.TrangThais.ToListAsync();

                // Load members
                if (_project.NhanVienThamGiaDuAns != null)
                {
                    _members = _project.NhanVienThamGiaDuAns.ToList();
                    MembersListView.ItemsSource = _members;
                }

                // Load tasks
                if (_project.NhanVienThamGiaDuAns != null)
                {
                    var allTasks = _project.NhanVienThamGiaDuAns.SelectMany(n => n.CongViecs).ToList();
                    _tasks = allTasks.Select(t => new TaskWrapper(t) { IsCompleted = t.TtMa == "ht" }).ToList();
                    TasksListView.ItemsSource = _tasks;
                }

                // Calculate progress
                CalculateProgress();

                // Bind data to UI
                DataContext = _project;

                // Set read-only mode if needed
                if (IsReadOnly)
                {
                    NameTextBox.IsReadOnly = true;
                    StartDatePicker.IsEnabled = false;
                    EndDatePicker.IsEnabled = false;
                    StatusComboBox.IsEnabled = false;
                    AddTaskButton.IsEnabled = false;
                    AddMemberButton.IsEnabled = false;
                    UploadFileButton.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateProgress()
        {
            if (_tasks.Any())
            {
                int totalTasks = _tasks.Count;
                int completedTasks = _tasks.Count(t => t.IsCompleted);
                _project.DaTienDo = (decimal)completedTasks / totalTasks * 100;
            }
            else
            {
                _project.DaTienDo = 0;
            }
            ProgressTextBlock.Text = $"Tiến độ dự án: {_project.DaTienDo:F2}%";
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskWrapper taskWrapper)
            {
                try
                {
                    taskWrapper.Task.TtMa = "ht";
                    taskWrapper.IsCompleted = true;
                    await _context.SaveChangesAsync();
                    CalculateProgress();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật trạng thái công việc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskWrapper taskWrapper)
            {
                try
                {
                    taskWrapper.Task.TtMa = "dth";
                    taskWrapper.IsCompleted = false;
                    await _context.SaveChangesAsync();
                    CalculateProgress();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật trạng thái công việc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Word files (*.doc, *.docx)|*.doc;*.docx|Excel files (*.xls, *.xlsx)|*.xls;*.xlsx"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                string uploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
                string savePath = Path.Combine(uploadsDir, Path.GetFileName(_selectedFilePath));
                try
                {
                    Directory.CreateDirectory(uploadsDir);
                    File.Copy(_selectedFilePath, savePath, true);
                    _selectedFilePath = savePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải file lên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text) || TaskNameTextBox.Text == "Tên công việc")
            {
                MessageBox.Show("Tên công việc không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TaskStatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái cho công việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TaskEndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày kết thúc cho công việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TaskStartDatePicker.SelectedDate.HasValue && TaskEndDatePicker.SelectedDate.HasValue &&
                TaskEndDatePicker.SelectedDate.Value < TaskStartDatePicker.SelectedDate.Value)
            {
                MessageBox.Show("Ngày kết thúc không được trước ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var task = new CongViec
                {
                    CvTen = TaskNameTextBox.Text,
                    TtMa = TaskStatusComboBox.SelectedValue.ToString(),
                    CvBatDau = TaskStartDatePicker.SelectedDate ?? DateTime.Now,
                    CvKetThuc = TaskEndDatePicker.SelectedDate,
                    NvIdNguoiTao = CurrentUser.NvId,
                    CvPath = _selectedFilePath,
                    CvFile = _selectedFilePath != null ? Path.GetFileName(_selectedFilePath) : null
                };

                _context.CongViecs.Add(task);
                _tasks.Add(new TaskWrapper(task) { IsCompleted = task.TtMa == "ht" });
                TasksListView.ItemsSource = _tasks.ToList(); // Refresh ListView

                // Reset input fields
                TaskNameTextBox.Text = "Tên công việc";
                TaskStatusComboBox.SelectedIndex = -1;
                TaskStartDatePicker.SelectedDate = null;
                TaskEndDatePicker.SelectedDate = null;
                _selectedFilePath = null;

                CalculateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm công việc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskWrapper taskWrapper)
            {
                try
                {
                    _context.CongViecs.Remove(taskWrapper.Task);
                    await _context.SaveChangesAsync();
                    _tasks.Remove(taskWrapper);
                    TasksListView.ItemsSource = _tasks.ToList();
                    CalculateProgress();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa công việc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberComboBox.SelectedValue == null || RoleComboBox.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên và vai trò!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!(MemberComboBox.SelectedValue is int nvId))
            {
                MessageBox.Show("Không thể xác định nhân viên được chọn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var member = new NhanVienThamGiaDuAn
            {
                NvId = nvId,
                VtMa = RoleComboBox.SelectedValue.ToString()
            };

            if (_members.Any(m => m.NvId == member.NvId && (m.DaId == _project.DaId || _project.DaId == 0)))
            {
                MessageBox.Show("Nhân viên này đã được thêm vào dự án!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _context.NhanVienThamGiaDuAns.Add(member);
                _members.Add(member);
                MembersListView.ItemsSource = _members.ToList();

                MemberComboBox.SelectedIndex = -1;
                RoleComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm thành viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is NhanVienThamGiaDuAn member)
            {
                try
                {
                    _context.NhanVienThamGiaDuAns.Remove(member);
                    await _context.SaveChangesAsync();
                    _members.Remove(member);
                    MembersListView.ItemsSource = _members.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa thành viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(_project.DaTen))
                {
                    MessageBox.Show("Tên dự án không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_project.TtMa == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái cho dự án!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_project.DaTienDo.HasValue && (_project.DaTienDo < 0 || _project.DaTienDo > 100))
                {
                    MessageBox.Show("Tiến độ phải nằm trong khoảng từ 0 đến 100!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_project.DaKetThuc == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày kết thúc cho dự án!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_project.DaBatDau.HasValue && _project.DaKetThuc.HasValue &&
                    _project.DaKetThuc.Value < _project.DaBatDau.Value)
                {
                    MessageBox.Show("Ngày kết thúc không được trước ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate status
                var trangThai = await _context.TrangThais.FirstOrDefaultAsync(t => t.TtMa == _project.TtMa);
                if (trangThai == null)
                {
                    MessageBox.Show($"Trạng thái '{_project.TtMa}' không tồn tại trong cơ sở dữ liệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_project.DaId == 0) // New project
                {
                    var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.NvId == _project.NvIdNguoiTao);
                    if (nhanVien == null)
                    {
                        MessageBox.Show($"Nhân viên với ID '{_project.NvIdNguoiTao}' không tồn tại trong cơ sở dữ liệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _project.DaBatDau = DateTime.Now;
                    _context.DuAns.Add(_project);
                    await _context.SaveChangesAsync();

                    // Update DaId for members and tasks
                    foreach (var member in _members)
                    {
                        member.DaId = _project.DaId;
                        _context.NhanVienThamGiaDuAns.Add(member);
                    }
                    foreach (var taskWrapper in _tasks)
                    {
                        taskWrapper.Task.DaId = _project.DaId;
                        _context.CongViecs.Add(taskWrapper.Task);
                    }
                    await _context.SaveChangesAsync();
                }
                else // Update existing project
                {
                    var existingProject = await _context.DuAns.FirstOrDefaultAsync(d => d.DaId == _project.DaId);
                    if (existingProject == null)
                    {
                        MessageBox.Show($"Dự án với ID '{_project.DaId}' không tồn tại trong cơ sở dữ liệu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    existingProject.DaTen = _project.DaTen;
                    existingProject.TtMa = _project.TtMa;
                    existingProject.DaBatDau = _project.DaBatDau;
                    existingProject.DaKetThuc = _project.DaKetThuc;
                    existingProject.DaTienDo = _project.DaTienDo;
                    await _context.SaveChangesAsync();
                }

                MessageBox.Show("Lưu dự án thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ProjectSaved?.Invoke();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show($"Lỗi khi lưu dự án: {errorMessage}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectSaved?.Invoke();
        }

        // Thêm phương thức xử lý tải file
        private void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy Hyperlink và đường dẫn file từ Tag
                Hyperlink hyperlink = sender as Hyperlink;
                string filePath = hyperlink?.Tag as string;

                // Kiểm tra đường dẫn có hợp lệ không
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Không có file để tải!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File không tồn tại hoặc đường dẫn không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Mở SaveFileDialog để người dùng chọn nơi lưu file
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = Path.GetFileName(filePath), // Tên file mặc định
                    Filter = "All Files (*.*)|*.*", // Bộ lọc file
                    Title = "Chọn nơi lưu file"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Sao chép file từ đường dẫn gốc đến vị trí người dùng chọn
                    File.Copy(filePath, saveFileDialog.FileName, true);
                    MessageBox.Show("Tải file thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        ~Edit_DuAn()
        {
            Dispose(false);
        }
    }

    public class TaskWrapper
    {
        public CongViec Task { get; set; }
        public bool IsCompleted { get; set; }

        public TaskWrapper(CongViec task)
        {
            Task = task;
            IsCompleted = task.TtMa == "ht";
        }
    }

    public static class CurrentUser
    {
        public static int NvId { get; set; }
        public static string NvTen { get; set; }
    }
}