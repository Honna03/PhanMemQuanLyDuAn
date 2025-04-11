using System;
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
using QuanLyDuAn.Models;

namespace QuanLyDuAn
{
    public partial class MainWindow : Window
    {
        private const string PlaceholderText = "Tìm kiếm...";
        private DispatcherTimer timer;
        private readonly ThucTapQuanLyDuAnContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ThucTapQuanLyDuAnContext();
            MainContent.Content = new TrangChu();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateCurrentTime();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        private void UpdateCurrentTime()
        {
            CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == PlaceholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.White;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = PlaceholderText;
                textBox.Foreground = Brushes.Gray;
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

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserButton.ContextMenu != null)
            {
                UserButton.ContextMenu.PlacementTarget = UserButton;
                UserButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                UserButton.ContextMenu.IsOpen = true;
            }
        }

        private void btn_QLCV_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DanhSachCongViec();
        }

        private void btn_QLDA_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProjectsControl(_context);
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = Window.GetWindow(this) as MainWindow;
            if (main != null)
            {
                MainContent.Content = new TrangChu();
            }
        }

        private void btn_QLKPI_Click(object sender, RoutedEventArgs e)
        {
            subMenuPanel.Visibility = subMenuPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btn_CongThuc_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CongThucKPI();
            subMenuPanel.Visibility = Visibility.Collapsed;
        }

        private void btn_KPI_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new KPI();
            subMenuPanel.Visibility = Visibility.Collapsed;
        }

        private void btn_AddDuAn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Content = new Edit_DuAn(null, _context),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }

        private void btn_Luong_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuanLyDuAn.Forms.Luong(); // Chỉ định rõ namespace
        }

        private void btn_DangXuat_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Window parentWindow = Window.GetWindow(sender as DependencyObject);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
            loginWindow.Show();
        }

        private void btn_Thoat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc thoát không?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}