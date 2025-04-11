using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using QuanLyDuAn.Controls;
using QuanLyDuAn.Forms;
using static QuanLyDuAn.Forms.KPI;

namespace QuanLyDuAn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string PlaceholderText = "Tìm kiếm...";
        private DispatcherTimer timer;
        private bool isInitialized = false; // Biến flag để kiểm soát
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new TrangChu();
            // Khởi tạo timer để cập nhật giờ
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Cập nhật mỗi giây
            timer.Tick += Timer_Tick;
            timer.Start();

            // Cập nhật giờ ngay khi khởi động
            UpdateCurrentTime();
            isInitialized = true; // Đánh dấu là đã khởi tạo xong
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss"); // Định dạng giờ:phút:giây
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == PlaceholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.White; // Đổi màu chữ khi người dùng nhập
            }
        }
        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserButton.ContextMenu != null)
            {
                UserButton.ContextMenu.PlacementTarget = UserButton; // Đặt vị trí ContextMenu tại Button
                UserButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom; // Hiển thị submenu bên dưới Button
                UserButton.ContextMenu.IsOpen = true; // Mở ContextMenu
            }
        }


        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = PlaceholderText;
                textBox.Foreground = Brushes.Gray; // Đặt lại màu chữ placeholder
            }
        }

        private void btn_NhanVien_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachNhanVien();
        }

        private void btn_BaoCao_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BaoCao();
        }

        private void btn_QLCV_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachCongViec();
        }

        private void btn_QLDA_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProjectsControl();
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            // Thay đổi nội dung của khu vực chứa UserControl
            MainWindow main = Window.GetWindow(this) as MainWindow;
            if (main != null)
            {
                MainContent.Content = new TrangChu();
            }
        }

        private void btn_KPI_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new KPI();
        }

        private void btn_AddDuAn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_DuAn(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        private void btn_Luong_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Luong();
        }

        private void btn_DangXuat_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Window parentWindow = Window.GetWindow(sender as DependencyObject);
            if (parentWindow != null)
            {
                parentWindow.Close(); // Đóng cửa sổ hiện tại
            }
            loginWindow.Show(); // Mở cửa sổ Login
        }

        private void btn_Thoat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc thoát không?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Kiểm tra nếu MainContent chưa được khởi tạo thì bỏ qua
            if (MainContent == null)
            {
                return;
            }

            string searchText = (sender as TextBox)?.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText) || searchText == PlaceholderText.ToLower())
            {
                searchText = string.Empty;
            }

            // Kiểm tra UserControl hiện tại trong MainContent
            if (MainContent.Content is KPI kpiControl)
            {
                kpiControl.Search(searchText); // Gọi phương thức tìm kiếm của KPI
            }
        }
    }
}