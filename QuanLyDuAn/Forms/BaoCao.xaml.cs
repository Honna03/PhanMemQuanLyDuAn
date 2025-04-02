using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace QuanLyDuAn.Forms
{
    public partial class BaoCao : UserControl
    {
        private List<Issue> issues;

        public BaoCao()
        {
            InitializeComponent();
            issues = new List<Issue>();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Thông tin dự án
            ProjectName.Text = "Dự án phát triển phần mềm ABC";
            ProjectCode.Text = "PM-001";
            StartDate.Text = "01/01/2025";
            EndDate.Text = "30/06/2025";
            ProjectStatus.SelectedIndex = 0; // Đang thực hiện

            // Tiến độ
            Progress.Value = 65;
            ProgressText.Text = "65%";
            TasksCompleted.Text = "32/50";

            // Tài nguyên
            TeamSize.Text = "10";
            Budget.Text = "5,000,000 / 8,000,000 VNĐ";

            // Rủi ro và vấn đề
            issues.AddRange(new List<Issue>
            {
                new Issue { Description = "Thiếu nhân sự kỹ thuật", Severity = "Cao" },
                new Issue { Description = "Chậm tiến độ module X", Severity = "Trung bình" }
            });
            IssuesList.ItemsSource = issues;

            // Ghi chú
            Notes.Text = "Cần tăng cường nhân sự trong tháng tới để đảm bảo tiến độ.";
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = new FlowDocument();
                doc.PagePadding = new Thickness(50);
                doc.ColumnWidth = printDialog.PrintableAreaWidth;

                Paragraph titleParagraph = new Paragraph(new Run("BÁO CÁO DỰ ÁN"))
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(titleParagraph);

                doc.Blocks.Add(new Paragraph(new Run("Thông tin dự án") { FontSize = 14, FontWeight = FontWeights.Bold }));
                doc.Blocks.Add(new Paragraph(new Run($"Tên dự án: {ProjectName.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Mã dự án: {ProjectCode.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Ngày bắt đầu: {StartDate.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Ngày kết thúc dự kiến: {EndDate.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Trạng thái: {ProjectStatus.SelectedItem.As<ComboBoxItem>().Content}")));

                doc.Blocks.Add(new Paragraph(new Run("Tiến độ công việc") { FontSize = 14, FontWeight = FontWeights.Bold }));
                doc.Blocks.Add(new Paragraph(new Run($"Phần trăm hoàn thành: {ProgressText.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Nhiệm vụ hoàn thành: {TasksCompleted.Text}")));

                doc.Blocks.Add(new Paragraph(new Run("Tài nguyên") { FontSize = 14, FontWeight = FontWeights.Bold }));
                doc.Blocks.Add(new Paragraph(new Run($"Số nhân sự: {TeamSize.Text}")));
                doc.Blocks.Add(new Paragraph(new Run($"Ngân sách đã sử dụng / Tổng: {Budget.Text}")));

                doc.Blocks.Add(new Paragraph(new Run("Rủi ro và vấn đề") { FontSize = 14, FontWeight = FontWeights.Bold }));
                foreach (Issue issue in issues)
                {
                    doc.Blocks.Add(new Paragraph(new Run($"Mô tả: {issue.Description}, Mức độ: {issue.Severity}")));
                }

                doc.Blocks.Add(new Paragraph(new Run("Ghi chú") { FontSize = 14, FontWeight = FontWeights.Bold }));
                doc.Blocks.Add(new Paragraph(new Run(Notes.Text)));

                IDocumentPaginatorSource paginator = doc;
                printDialog.PrintDocument(paginator.DocumentPaginator, "Báo cáo dự án");
            }
        }

        private void ManageIssues_Click(object sender, RoutedEventArgs e)
        {
            Window issueWindow = new Window
            {
                Title = "Quản lý rủi ro và vấn đề",
                Width = 600,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new EditIssue(issues) // Truyền danh sách issues vào
            };
            issueWindow.ShowDialog();

            // Cập nhật lại danh sách sau khi đóng form quản lý
            IssuesList.ItemsSource = null;
            IssuesList.ItemsSource = issues;
        }

        private void IssuesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Không cần xử lý thêm
        }
    }

    public class Issue
    {
        public string Description { get; set; }
        public string Severity { get; set; }
    }

    public static class Extensions
    {
        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }
    }
}