using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyDuAn.Forms
{
    public partial class TrangChu : UserControl
    {
        private readonly string _connectionString = "Data Source=HONNA\\BAO;Initial Catalog=ThucTap_QuanLyDuAn;Integrated Security=True;User Id=sa;Password=1;";
        private readonly int _userId;
        public TrangChu(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Load This Week's Tasks
                    var weekTasks = GetWeekTasks(connection);
                    WeekTotalTasks.Text = weekTasks.total.ToString();
                    WeekCompletedTasks.Text = $"{weekTasks.completed} Đã hoàn thành";
                    WeekNearingDeadlineTasks.Text = $"{weekTasks.nearingDeadline} Sắp đến hạn";
                    WeekOverdueTasks.Text = $"{weekTasks.overdue} Quá hạn";

                    // Load Today's Task Statistics
                    var todayStats = GetTodayTaskStats(connection);
                    TodayStatsTotalTasks.Text = todayStats.total.ToString();
                    TodayStatsEarly.Text = $"{todayStats.early} Hoàn thành trước hạn";
                    TodayStatsOnTime.Text = $"{todayStats.onTime} Hoàn thành đúng hạn";
                    TodayStatsLate.Text = $"{todayStats.late} Hoàn thành trễ hạn";
                    TodayStatsOverdue.Text = $"{todayStats.overdue} Quá hạn";

                    // Load Project Statistics
                    var projectStats = GetProjectStats(connection);
                    ProjectTotal.Text = projectStats.total.ToString();
                    ProjectEarly.Text = $"{projectStats.early} Hoàn thành trước hạn";
                    ProjectOnTime.Text = $"{projectStats.onTime} Hoàn thành đúng hạn";
                    ProjectLate.Text = $"{projectStats.late} Hoàn thành trễ hạn";
                    ProjectOverdue.Text = $"{projectStats.overdue} Quá hạn";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (int total, int completed, int nearingDeadline, int overdue) GetWeekTasks(SqlConnection connection)
        {
            string query = @"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN cv.tt_Ma = 'ht' THEN 1 ELSE 0 END) AS Completed,
                    SUM(CASE WHEN cv.tt_Ma IN ('cht', 'dth') AND cv.cv_KetThuc <= DATEADD(DAY, 7, CAST(GETDATE() AS DATE)) THEN 1 ELSE 0 END) AS NearingDeadline,
                    SUM(CASE WHEN cv.tt_Ma IN ('cht', 'dth') AND cv.cv_KetThuc < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS Overdue
                FROM CongViec cv
                INNER JOIN PhanCongCongViec pc ON cv.cv_ID = pc.cv_ID AND cv.da_ID = pc.da_ID
                WHERE pc.nv_ID = @UserId
                AND cv.cv_BatDau <= DATEADD(DAY, 7, CAST(GETDATE() AS DATE))";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", _userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            GetSafeInt32(reader, "Total"),
                            GetSafeInt32(reader, "Completed"),
                            GetSafeInt32(reader, "NearingDeadline"),
                            GetSafeInt32(reader, "Overdue")
                        );
                    }
                }
            }
            return (0, 0, 0, 0);
        }

        private (int total, int early, int onTime, int late, int overdue) GetTodayTaskStats(SqlConnection connection)
        {
            string query = @"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh < cv.cv_KetThuc THEN 1 ELSE 0 END) AS Early,
                    SUM(CASE WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh = cv.cv_KetThuc THEN 1 ELSE 0 END) AS OnTime,
                    SUM(CASE WHEN cv.tt_Ma = 'ht' AND cv.cv_ThoiGianHoanThanh > cv.cv_KetThuc THEN 1 ELSE 0 END) AS Late,
                    SUM(CASE WHEN cv.tt_Ma IN ('cht', 'dth') AND cv.cv_KetThuc < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS Overdue
                FROM CongViec cv
                INNER JOIN PhanCongCongViec pc ON cv.cv_ID = pc.cv_ID AND cv.da_ID = pc.da_ID
                WHERE pc.nv_ID = @UserId
                AND cv.cv_BatDau <= CAST(GETDATE() AS DATE)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", _userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            GetSafeInt32(reader, "Total"),
                            GetSafeInt32(reader, "Early"),
                            GetSafeInt32(reader, "OnTime"),
                            GetSafeInt32(reader, "Late"),
                            GetSafeInt32(reader, "Overdue")
                        );
                    }
                }
            }
            return (0, 0, 0, 0, 0);
        }

        private (int total, int early, int onTime, int late, int overdue) GetProjectStats(SqlConnection connection)
        {
            string query = @"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN da.tt_Ma = 'ht' AND da.da_ThoiGianHoanThanh < da.da_KetThuc THEN 1 ELSE 0 END) AS Early,
                    SUM(CASE WHEN da.tt_Ma = 'ht' AND da.da_ThoiGianHoanThanh = da.da_KetThuc THEN 1 ELSE 0 END) AS OnTime,
                    SUM(CASE WHEN da.tt_Ma = 'ht' AND da.da_ThoiGianHoanThanh > da.da_KetThuc THEN 1 ELSE 0 END) AS Late,
                    SUM(CASE WHEN da.tt_Ma IN ('cht', 'dth') AND da.da_KetThuc < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS Overdue
                FROM DuAn da
                INNER JOIN NhanVienThamGiaDuAn nvda ON da.da_ID = nvda.da_ID
                WHERE nvda.nv_ID = @UserId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", _userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            GetSafeInt32(reader, "Total"),
                            GetSafeInt32(reader, "Early"),
                            GetSafeInt32(reader, "OnTime"),
                            GetSafeInt32(reader, "Late"),
                            GetSafeInt32(reader, "Overdue")
                        );
                    }
                }
            }
            return (0, 0, 0, 0, 0);
        }
        private int GetSafeInt32(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }
    }
}