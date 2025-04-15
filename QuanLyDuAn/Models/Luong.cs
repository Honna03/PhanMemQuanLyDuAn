using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class Luong
{
    public DateOnly LuongThangNam { get; set; }

    public decimal? LuongPhuCap { get; set; }

    public decimal? LuongThucNhan { get; set; }

    public int NvId { get; set; }

    public virtual NhanVien Nv { get; set; } = null!;
}
