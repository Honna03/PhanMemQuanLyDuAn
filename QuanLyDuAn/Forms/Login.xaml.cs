using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Controls;
using QuanLyDuAn.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Imaging;

namespace QuanLyDuAn.Forms
{
    public partial class Login : Window
    {
        private readonly ThucTapQuanLyDuAnContext _context;
        private bool _isPasswordVisible = false;
        private readonly string _settingsFilePath;

        public Login()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"AllowsTransparency: {this.AllowsTransparency}, WindowStyle: {this.WindowStyle}");
            _context = new ThucTapQuanLyDuAnContext();
            LoginForm.RenderTransform = new TranslateTransform();
            btn_Login.RenderTransform = new ScaleTransform();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "QuanLyDuAn");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            _settingsFilePath = Path.Combine(appFolder, "UserSettings.json");

           // LoadSavedCredentials();
            SetupPasswordVisibility();
        }

        /*private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonConvert.DeserializeObject<UserSettings>(json);
                    if (settings != null)
                    {
                        settings.DecryptUsername();
                        if (settings.IsValid())
                        {
                            UsernameTextBox.Text = settings.Username;
                            //RememberMeCheckBox.IsChecked = settings.RememberMe;
                        }
                        else
                        {
                            File.Delete(_settingsFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc tệp cài đặt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/

       /* private void SaveCredentials()
        {
            try
            {
                var settings = new UserSettings
                {
                    Username = RememberMeCheckBox.IsChecked == true ? UsernameTextBox.Text : string.Empty,
                    RememberMe = RememberMeCheckBox.IsChecked == true
                };

                if (settings.RememberMe)
                {
                    settings.SetExpiryDate(30);
                    settings.EncryptUsername();
                    string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                    File.WriteAllText(_settingsFilePath, json);
                }
                else
                {
                    if (File.Exists(_settingsFilePath))
                    {
                        File.Delete(_settingsFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu tệp cài đặt: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/

        private void SetupPasswordVisibility()
        {
            ShowPasswordButton.Click += (s, e) =>
            {
                _isPasswordVisible = !_isPasswordVisible;
                if (_isPasswordVisible)
                {
                    ShowPasswordButton.Content = new Image { Source = new BitmapImage(new Uri("/Resources/eye_off_icon.png", UriKind.Relative)), Width = 16, Height = 16 };
                }
                else
                {
                    ShowPasswordButton.Content = new Image { Source = new BitmapImage(new Uri("/Resources/eye_icon.png", UriKind.Relative)), Width = 16, Height = 16 };
                }
            };
        }

        private void tb_Welcome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            fadeOutAnimation.Completed += (s, _) => TitlePanel.Visibility = Visibility.Hidden;

            LoginForm.Visibility = Visibility.Visible;
            LoginForm.Opacity = 0;

            DoubleAnimation slideUpAnimation = new DoubleAnimation
            {
                From = 450,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(fadeOutAnimation, TitlePanel);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            Storyboard.SetTarget(slideUpAnimation, LoginForm);
            Storyboard.SetTargetProperty(slideUpAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            Storyboard.SetTarget(fadeInAnimation, LoginForm);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(fadeOutAnimation);
            storyboard.Children.Add(slideUpAnimation);
            storyboard.Children.Add(fadeInAnimation);
            storyboard.Begin();
        }

        private void btn_Login_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation scaleXAnimation = new DoubleAnimation
            {
                From = 1,
                To = 1.1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation scaleYAnimation = new DoubleAnimation
            {
                From = 1,
                To = 1.1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(scaleXAnimation, btn_Login);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(scaleYAnimation, btn_Login);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Begin();
        }

        private void btn_Login_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation scaleXAnimation = new DoubleAnimation
            {
                From = 1.1,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation scaleYAnimation = new DoubleAnimation
            {
                From = 1.1,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(scaleXAnimation, btn_Login);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(scaleYAnimation, btn_Login);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Begin();
        }

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox == null || PasswordBox == null)
            {
                MessageBox.Show("Không tìm thấy control UsernameTextBox hoặc PasswordBox.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ForgetPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public static class CurrentUser
    {
        public static int NvId { get; set; }
        public static string NvTen { get; set; }
    }
}