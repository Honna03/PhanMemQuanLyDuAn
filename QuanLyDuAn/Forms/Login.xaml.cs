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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLyDuAn.Forms
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void tb_ABHK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Đặt opacity ban đầu của LoginForm thành 0 và hiện nó
            LoginForm.Opacity = 0;
            LoginForm.Visibility = Visibility.Visible;

            // Tạo hiệu ứng fade-in cho LoginForm
            DoubleAnimation formAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(3.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            LoginForm.BeginAnimation(OpacityProperty, formAnimation);

            // Tạo hiệu ứng di chuyển tiêu đề lên trên
            DoubleAnimation titleMoveUpAnimation = new DoubleAnimation
            {
                From = 100,
                To = 50,
                Duration = new Duration(TimeSpan.FromSeconds(1.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Áp dụng hiệu ứng cho Margin của tiêu đề
            TitlePanel.BeginAnimation(MarginProperty, new ThicknessAnimation
            {
                From = new Thickness(0, 250, 0, 0),
                To = new Thickness(0, 100, 0, 0),
                Duration = new Duration(TimeSpan.FromSeconds(1.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            });

            // Tạo hiệu ứng di chuyển form đăng nhập lên trên
            DoubleAnimation formMoveUpAnimation = new DoubleAnimation
            {
                From = 200,
                To = 50,
                Duration = new Duration(TimeSpan.FromSeconds(2.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Áp dụng hiệu ứng cho Margin của form đăng nhập
            LoginForm.BeginAnimation(MarginProperty, new ThicknessAnimation
            {
                From = new Thickness(0, 150, 0, 0),
                To = new Thickness(0, 20, 0, 0),
                Duration = new Duration(TimeSpan.FromSeconds(2.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            });
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
