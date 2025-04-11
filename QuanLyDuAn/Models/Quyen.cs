using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class Quyen
{
    public string QMa { get; set; } = null!;

    public string QTen { get; set; } = null!;

    public string? QMoTa { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
