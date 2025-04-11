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
    /// Interaction logic for Edit_DuAn.xaml
    /// </summary>
    public partial class Edit_DuAn : UserControl
    {
        public Edit_DuAn()
        {
            InitializeComponent();
        }

        private void btn_Luu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}
