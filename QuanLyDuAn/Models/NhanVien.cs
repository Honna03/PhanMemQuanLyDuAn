using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class NhanVien
{
    public int NvId { get; set; }

    public string? NvMa { get; set; }

    public string NvTen { get; set; } = null!;

    public string NvGioiTinh { get; set; } = null!;

    public DateOnly NvNgaySinh { get; set; }

    public string NvSdt { get; set; } = null!;

    public string NvDiaChi { get; set; } = null!;

    public string NvEmail { get; set; } = null!;

    public string NvTaiKhoan { get; set; } = null!;

    public string NvMatKhau { get; set; } = null!;

    public decimal NvLuongCoBan { get; set; }

    public string QMa { get; set; } = null!;

    public virtual ICollection<DuAn> DuAns { get; set; } = new List<DuAn>();

    public virtual ICollection<Kpi> Kpis { get; set; } = new List<Kpi>();
    public virtual ICollection<LichSuCapNhatLuong> LichSuCapNhatLuongs { get; set; } = new List<LichSuCapNhatLuong>();

    public virtual ICollection<Luong> Luongs { get; set; } = new List<Luong>();

    public virtual ICollection<NhanVienThamGiaDuAn> NhanVienThamGiaDuAns { get; set; } = new List<NhanVienThamGiaDuAn>();

    public virtual Quyen QMaNavigation { get; set; } = null!;

    public virtual ICollection<ThongBao> ThongBaoNvIdNguoiGuiNavigations { get; set; } = new List<ThongBao>();

    public virtual ICollection<ThongBao> ThongBaoNvIdNguoiNhanNavigations { get; set; } = new List<ThongBao>();
}
