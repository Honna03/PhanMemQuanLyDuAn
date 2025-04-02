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
    /// Interaction logic for KPI.xaml
    /// </summary>
    public partial class KPI : UserControl
    {
        public KPI()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void btn_AddCongThuc_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_CongThuc(),
            };
            window.Show();
        }
        private void LoadSampleData()
        {
            // Tạo danh sách dữ liệu mẫu
            List<KPIModel> kpiList = new List<KPIModel>
            {
                new KPIModel
                {
                    STT = 1,
                    MaKPI = "KPI001",
                    TenKPI = "Doanh thu tháng",
                    MoTa = "Đo lường tổng doanh thu bán hàng trong tháng hiện tại",
                    DonVi = "VND",
                    NgayTao = new DateTime(2025, 1, 1),
                    NgayKetThuc = new DateTime(2025, 1, 31),
                    TrangThai = "Hoạt động",
                    NguoiTao = "Nguyễn Văn A",
                    NguoiBaoCao = "Trần Thị B"
                },
                new KPIModel
                {
                    STT = 2,
                    MaKPI = "KPI002",
                    TenKPI = "Số lượng sản phẩm bán ra",
                    MoTa = "Theo dõi số lượng sản phẩm được bán trong quý 1",
                    DonVi = "Sản phẩm",
                    NgayTao = new DateTime(2025, 1, 1),
                    NgayKetThuc = new DateTime(2025, 3, 31),
                    TrangThai = "Hoạt động",
                    NguoiTao = "Lê Thị C",
                    NguoiBaoCao = "Phạm Văn D"
                },
                new KPIModel
                {
                    STT = 3,
                    MaKPI = "KPI003",
                    TenKPI = "Tỷ lệ hài lòng khách hàng",
                    MoTa = "Đánh giá mức độ hài lòng của khách hàng dựa trên khảo sát",
                    DonVi = "%",
                    NgayTao = new DateTime(2025, 2, 15),
                    NgayKetThuc = new DateTime(2025, 3, 15),
                    TrangThai = "Đã kết thúc",
                    NguoiTao = "Hoàng Văn E",
                    NguoiBaoCao = "Nguyễn Thị F"
                },
                new KPIModel
                {
                    STT = 4,
                    MaKPI = "KPI004",
                    TenKPI = "Thời gian xử lý đơn hàng",
                    MoTa = "Đo lường thời gian trung bình để xử lý một đơn hàng",
                    DonVi = "Giờ",
                    NgayTao = new DateTime(2025, 3, 1),
                    NgayKetThuc = new DateTime(2025, 3, 31),
                    TrangThai = "Hoạt động",
                    NguoiTao = "Trần Văn G",
                    NguoiBaoCao = "Lê Thị H"
                }
            };

            // Gán dữ liệu vào DataGrid
            dgKPI.ItemsSource = kpiList;
        }
        // Lớp mô hình cho KPI
        public class KPIModel
        {
            public int STT { get; set; }
            public string MaKPI { get; set; }
            public string TenKPI { get; set; }
            public string MoTa { get; set; }
            public string DonVi { get; set; }
            public DateTime NgayTao { get; set; }
            public DateTime NgayKetThuc { get; set; }
            public string TrangThai { get; set; }
            public string NguoiTao { get; set; }
            public string NguoiBaoCao { get; set; }
        }

        private void dgKPI_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (dgKPI.SelectedItem != null)
            {
                // Lấy dữ liệu của hàng được chọn
                KPIModel selectedKPI = (KPIModel)dgKPI.SelectedItem;

                // Tạo và hiển thị UserControl Edit_KPI trong một cửa sổ mới
                Window editWindow = new Window
                {
                    Title = "Chỉnh sửa KPI: " + selectedKPI.TenKPI,
                    Width = 500,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                Edit_KPI editKPIControl = new Edit_KPI(selectedKPI);
                editWindow.Content = editKPIControl;
                editWindow.ShowDialog(); // Hiển thị dưới dạng modal
            }
        }

        private void btn_AddKCongThuc_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_CongThuc(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        private void btn_AddKPI_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_KPI(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}
