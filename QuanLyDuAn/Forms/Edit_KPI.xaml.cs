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
    /// Interaction logic for Edit_KPI.xaml
    /// </summary>
    public partial class Edit_KPI : UserControl
    {
        private KPI.KPIModel _kpi;
        public Edit_KPI()
        {
            InitializeComponent();
        }
        public Edit_KPI(KPI.KPIModel kpi)
        {
            InitializeComponent();
            _kpi = kpi;
            this.DataContext = _kpi; // Gán dữ liệu vào DataContext để binding
        }
    }
}
