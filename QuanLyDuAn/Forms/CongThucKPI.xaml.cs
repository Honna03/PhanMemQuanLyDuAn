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
    /// Interaction logic for CongThucKPI.xaml
    /// </summary>
    public partial class CongThucKPI : UserControl
    {
        public CongThucKPI()
        {
            InitializeComponent();
        }

        private void btn_AddCongThucKPI_Click(object sender, RoutedEventArgs e)
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
                Content = new KPI(),
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}
