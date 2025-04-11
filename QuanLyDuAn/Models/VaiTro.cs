using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class VaiTro
{
    public string VtMa { get; set; } = null!;

    public string VtTen { get; set; } = null!;

    public string? VtMoTa { get; set; }

    public virtual ICollection<NhanVienThamGiaDuAn> NhanVienThamGiaDuAns { get; set; } = new List<NhanVienThamGiaDuAn>();
}
