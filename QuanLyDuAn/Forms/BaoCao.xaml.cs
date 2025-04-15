using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace QuanLyDuAn.Forms
{
    public partial class BaoCao : UserControl
    {
        private string connectionString = "Data Source=ThanhHuy;Initial Catalog=ThucTap_QuanLyDuAn;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private int projectId;
        private DataTable projectInfoTable;
        private DataTable tasksTable;

        public BaoCao(int da_ID)
        {
            InitializeComponent();
            projectId = da_ID;
            LoadReportData();
        }

        private void LoadReportData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Tạo DataTable để lưu dữ liệu cho báo cáo
                    projectInfoTable = new DataTable("ProjectInfo");
                    projectInfoTable.Columns.Add("ProjectName", typeof(string));
                    projectInfoTable.Columns.Add("ProjectCode", typeof(string));
                    projectInfoTable.Columns.Add("StartDate", typeof(string));
                    projectInfoTable.Columns.Add("EndDate", typeof(string));
                    projectInfoTable.Columns.Add("ProjectStatus", typeof(string));
                    projectInfoTable.Columns.Add("CreatorName", typeof(string));
                    projectInfoTable.Columns.Add("Progress", typeof(string));
                    projectInfoTable.Columns.Add("TotalTasks", typeof(string));
                    projectInfoTable.Columns.Add("TasksCompleted", typeof(string));
                    projectInfoTable.Columns.Add("TeamSize", typeof(string));
                    projectInfoTable.Columns.Add("Notes", typeof(string));

                    tasksTable = new DataTable("Tasks");
                    tasksTable.Columns.Add("TaskCode", typeof(string));
                    tasksTable.Columns.Add("TaskName", typeof(string));
                    tasksTable.Columns.Add("StartDate", typeof(string));
                    tasksTable.Columns.Add("EndDate", typeof(string));
                    tasksTable.Columns.Add("Status", typeof(string));
                    tasksTable.Columns.Add("AssignedTo", typeof(string));

                    // Load thông tin dự án
                    string projectQuery = @"
                        SELECT da_Ten, da_Ma, da_BatDau, da_KetThuc, tt_Ten, nv_Ten
                        FROM DuAn DA
                        JOIN TrangThai TT ON TT.tt_Ma = DA.tt_Ma
                        JOIN NhanVien NV ON NV.nv_ID = DA.nv_ID_NguoiTao
                        WHERE da_ID = @da_ID";
                    using (SqlCommand cmd = new SqlCommand(projectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@da_ID", projectId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            ProjectName.Text = reader["da_Ten"].ToString();
                            ProjectCode.Text = reader["da_Ma"].ToString();
                            StartDate.Text = Convert.ToDateTime(reader["da_BatDau"]).ToString("dd/MM/yyyy");
                            EndDate.Text = Convert.ToDateTime(reader["da_KetThuc"]).ToString("dd/MM/yyyy");
                            ProjectStatus.Text = reader["tt_Ten"].ToString();
                            CreatorName.Text = reader["nv_Ten"].ToString();
                        }
                        reader.Close();
                    }

                    // Load tiến độ dự án
                    string progressQuery = @"
                        SELECT 
                            (SELECT COUNT(*) FROM CongViec WHERE da_ID = @da_ID) AS TotalTasks,
                            (SELECT COUNT(*) FROM CongViec WHERE da_ID = @da_ID AND tt_Ma = 'ht') AS CompletedTasks
                        FROM DuAn WHERE da_ID = @da_ID";
                    using (SqlCommand cmd = new SqlCommand(progressQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@da_ID", projectId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            int totalTasks = Convert.ToInt32(reader["TotalTasks"]);
                            int completedTasks = Convert.ToInt32(reader["CompletedTasks"]);
                            double progress = totalTasks > 0 ? (completedTasks * 100.0 / totalTasks) : 0;
                            Progress.Value = progress;
                            ProgressText.Text = $"{progress:F0}%";
                            TotalTasks.Text = totalTasks.ToString();
                            TasksCompleted.Text = completedTasks.ToString();
                        }
                        reader.Close();
                    }

                    // Load danh sách công việc
                    string taskQuery = @"
                        SELECT cv_Ma, cv_Ten, cv_BatDau, cv_KetThuc, tt_Ten, nv_Ten
                        FROM CongViec CV
                        JOIN TrangThai TT ON TT.tt_Ma = CV.tt_Ma
                        LEFT JOIN PhanCongCongViec PCCV ON PCCV.cv_ID = CV.cv_ID AND PCCV.da_ID = CV.da_ID
                        LEFT JOIN NhanVien NV ON NV.nv_ID = PCCV.nv_ID
                        WHERE CV.da_ID = @da_ID";
                    using (SqlCommand cmd = new SqlCommand(taskQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@da_ID", projectId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        var tasks = new List<dynamic>();
                        while (reader.Read())
                        {
                            var task = new
                            {
                                TaskCode = reader["cv_Ma"].ToString(),
                                TaskName = reader["cv_Ten"].ToString(),
                                StartDate = Convert.ToDateTime(reader["cv_BatDau"]).ToString("dd/MM/yyyy"),
                                EndDate = reader["cv_KetThuc"] != DBNull.Value ? Convert.ToDateTime(reader["cv_KetThuc"]).ToString("dd/MM/yyyy") : "Chưa xác định",
                                Status = reader["tt_Ten"].ToString(),
                                AssignedTo = reader["nv_Ten"]?.ToString() ?? "Chưa phân công"
                            };
                            tasks.Add(task);
                            tasksTable.Rows.Add(task.TaskCode, task.TaskName, task.StartDate, task.EndDate, task.Status, task.AssignedTo);
                        }
                        TaskList.ItemsSource = tasks;
                        reader.Close();
                    }

                    // Load tài nguyên
                    string teamSizeQuery = "SELECT COUNT(*) FROM NhanVienThamGiaDuAn WHERE da_ID = @da_ID";
                    using (SqlCommand cmd = new SqlCommand(teamSizeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@da_ID", projectId);
                        TeamSize.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Thêm dữ liệu vào DataTable ProjectInfo
                    projectInfoTable.Rows.Add(
                        ProjectName.Text,
                        ProjectCode.Text,
                        StartDate.Text,
                        EndDate.Text,
                        ProjectStatus.Text,
                        CreatorName.Text,
                        ProgressText.Text,
                        TotalTasks.Text,
                        TasksCompleted.Text,
                        TeamSize.Text,
                        Notes.Text
                    );
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi kết nối SQL khi tải dữ liệu báo cáo: {ex.Message}\nError Number: {ex.Number}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo file PDF
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"BaoCaoDuAn_{projectId}.pdf");
                using (FileStream fs = new FileStream(tempFilePath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Font chữ hỗ trợ tiếng Việt
                    BaseFont baseFont = BaseFont.CreateFont("c:\\windows\\fonts\\times.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font titleFont = new Font(baseFont, 16, Font.BOLD);
                    Font normalFont = new Font(baseFont, 12);

                    // Tiêu đề
                    document.Add(new Paragraph("Báo Cáo Tiến Độ Dự Án", titleFont) { Alignment = Element.ALIGN_CENTER });
                    document.Add(new Paragraph(" ", normalFont)); // Dòng trống

                    // Thông tin dự án
                    document.Add(new Paragraph("Thông Tin Dự Án", normalFont) { Alignment = Element.ALIGN_LEFT });
                    document.Add(new Paragraph($"Tên dự án: {ProjectName.Text}", normalFont));
                    document.Add(new Paragraph($"Mã dự án: {ProjectCode.Text}", normalFont));
                    document.Add(new Paragraph($"Ngày bắt đầu: {StartDate.Text}", normalFont));
                    document.Add(new Paragraph($"Ngày kết thúc dự kiến: {EndDate.Text}", normalFont));
                    document.Add(new Paragraph($"Trạng thái: {ProjectStatus.Text}", normalFont));
                    document.Add(new Paragraph($"Người tạo: {CreatorName.Text}", normalFont));
                    document.Add(new Paragraph(" ", normalFont)); // Dòng trống

                    // Tiến độ dự án
                    document.Add(new Paragraph("Tiến Độ Dự Án", normalFont));
                    document.Add(new Paragraph($"Phần trăm hoàn thành: {ProgressText.Text}", normalFont));
                    document.Add(new Paragraph($"Tổng số công việc: {TotalTasks.Text}", normalFont));
                    document.Add(new Paragraph($"Công việc hoàn thành: {TasksCompleted.Text}", normalFont));
                    document.Add(new Paragraph(" ", normalFont)); // Dòng trống

                    // Danh sách công việc
                    document.Add(new Paragraph("Danh Sách Công Việc", normalFont));
                    PdfPTable table = new PdfPTable(6) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 1f, 2f, 1.5f, 1.5f, 1f, 1.5f });
                    table.AddCell(new PdfPCell(new Phrase("Mã CV", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Tên công việc", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Ngày bắt đầu", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Ngày kết thúc", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Trạng thái", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Người thực hiện", normalFont)));

                    foreach (DataRow row in tasksTable.Rows)
                    {
                        table.AddCell(new PdfPCell(new Phrase(row["TaskCode"].ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(row["TaskName"].ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(row["StartDate"].ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(row["EndDate"].ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(row["Status"].ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(row["AssignedTo"].ToString(), normalFont)));
                    }
                    document.Add(table);
                    document.Add(new Paragraph(" ", normalFont)); // Dòng trống

                    // Tài nguyên
                    document.Add(new Paragraph("Tài Nguyên", normalFont));
                    document.Add(new Paragraph($"Số nhân sự tham gia: {TeamSize.Text}", normalFont));
                    document.Add(new Paragraph(" ", normalFont)); // Dòng trống

                    // Ghi chú
                    document.Add(new Paragraph("Ghi Chú", normalFont));
                    document.Add(new Paragraph(Notes.Text, normalFont));

                    document.Close();
                }

                // Mở file PDF bằng ứng dụng mặc định
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo hoặc in báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}