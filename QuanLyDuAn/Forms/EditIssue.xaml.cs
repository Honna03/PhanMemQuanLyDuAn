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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for EditIssue.xaml
    /// </summary>
    public partial class EditIssue : UserControl
    {
        private List<Issue> issues;
        public EditIssue(List<Issue> issueList)
        {
            InitializeComponent();
            issues = issueList; // Nhận danh sách từ form chính
            IssuesList.ItemsSource = issues; // Gán dữ liệu cho ListView
        }
        private void AddIssue_Click(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new Window
            {
                Title = "Thêm rủi ro",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = "Mô tả:" });
            TextBox descriptionBox = new TextBox { Margin = new Thickness(0, 5, 0, 5) };
            panel.Children.Add(descriptionBox);

            panel.Children.Add(new TextBlock { Text = "Mức độ:" });
            ComboBox severityBox = new ComboBox { Margin = new Thickness(0, 5, 0, 5) };
            severityBox.Items.Add("Cao");
            severityBox.Items.Add("Trung bình");
            severityBox.Items.Add("Thấp");
            severityBox.SelectedIndex = 0;
            panel.Children.Add(severityBox);

            Button saveButton = new Button { Content = "Lưu", Margin = new Thickness(0, 10, 0, 0), Width = 80 };
            saveButton.Click += (s, args) =>
            {
                if (!string.IsNullOrWhiteSpace(descriptionBox.Text))
                {
                    issues.Add(new Issue { Description = descriptionBox.Text, Severity = severityBox.SelectedItem.ToString() });
                    IssuesList.ItemsSource = null;
                    IssuesList.ItemsSource = issues;
                    inputWindow.Close();
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập mô tả!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            panel.Children.Add(saveButton);

            inputWindow.Content = panel;
            inputWindow.ShowDialog();
        }

        private void EditIssue_Click(object sender, RoutedEventArgs e)
        {
            if (IssuesList.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một rủi ro để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Issue selectedIssue = (Issue)IssuesList.SelectedItem;
            Window inputWindow = new Window
            {
                Title = "Sửa rủi ro",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = "Mô tả:" });
            TextBox descriptionBox = new TextBox { Text = selectedIssue.Description, Margin = new Thickness(0, 5, 0, 5) };
            panel.Children.Add(descriptionBox);

            panel.Children.Add(new TextBlock { Text = "Mức độ:" });
            ComboBox severityBox = new ComboBox { Margin = new Thickness(0, 5, 0, 5) };
            severityBox.Items.Add("Cao");
            severityBox.Items.Add("Trung bình");
            severityBox.Items.Add("Thấp");
            severityBox.SelectedItem = selectedIssue.Severity;
            panel.Children.Add(severityBox);

            Button saveButton = new Button { Content = "Lưu", Margin = new Thickness(0, 10, 0, 0), Width = 80 };
            saveButton.Click += (s, args) =>
            {
                if (!string.IsNullOrWhiteSpace(descriptionBox.Text))
                {
                    selectedIssue.Description = descriptionBox.Text;
                    selectedIssue.Severity = severityBox.SelectedItem.ToString();
                    IssuesList.ItemsSource = null;
                    IssuesList.ItemsSource = issues;
                    inputWindow.Close();
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập mô tả!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            panel.Children.Add(saveButton);

            inputWindow.Content = panel;
            inputWindow.ShowDialog();
        }

        private void DeleteIssue_Click(object sender, RoutedEventArgs e)
        {
            if (IssuesList.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một rủi ro để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Issue selectedIssue = (Issue)IssuesList.SelectedItem;
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa rủi ro: {selectedIssue.Description}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                issues.Remove(selectedIssue);
                IssuesList.ItemsSource = null;
                IssuesList.ItemsSource = issues;
            }
        }

        private void IssuesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Không cần xử lý thêm
        }
    }
}
