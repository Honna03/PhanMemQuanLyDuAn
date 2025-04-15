using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class LichSuCapNhatLuong
{
    public int LsId { get; set; }

    public string? QMa { get; set; }

    public int? NvId { get; set; }

    public decimal? LuongCu { get; set; }

    public decimal? LuongMoi { get; set; }

    public DateTime? ThoiGianCapNhat { get; set; }

    public virtual NhanVien? Nv { get; set; }

    public virtual Quyen? QMaNavigation { get; set; }
}
