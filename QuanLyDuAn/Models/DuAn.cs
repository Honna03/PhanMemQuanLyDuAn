
using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class DuAn
{
    public int DaId { get; set; }

    public string? DaMa { get; set; }

    public string? DaMoTa { get; set; }

    public string DaTen { get; set; } = null!;

    public DateTime? DaBatDau { get; set; }

    public DateTime? DaKetThuc { get; set; }

    public string? DaFile { get; set; }

    public string? DaPath { get; set; }

    public decimal? DaTienDo { get; set; }

    public DateTime? DaThoiGianHoanThanh { get; set; }

    public int NvIdNguoiTao { get; set; }

    public string TtMa { get; set; } = null!;

    public virtual ICollection<NhanVienThamGiaDuAn> NhanVienThamGiaDuAns { get; set; } = new List<NhanVienThamGiaDuAn>();

    public virtual NhanVien NvIdNguoiTaoNavigation { get; set; } = null!;

    public virtual TrangThai TtMaNavigation { get; set; } = null!;
}
