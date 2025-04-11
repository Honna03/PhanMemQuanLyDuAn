using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class Kpi
{
    public DateOnly KpiThangNam { get; set; }

    public decimal? KpiPhanTram { get; set; }

    public int NvId { get; set; }

    public virtual NhanVien Nv { get; set; } = null!;
}
