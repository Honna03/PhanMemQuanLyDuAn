using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class ThongBao
{
    public int TbId { get; set; }

    public string TbNoiDung { get; set; } = null!;

    public DateTime? TbThoiGian { get; set; }

    public bool? TbTinhTrang { get; set; }

    public string TbLoai { get; set; } = null!;

    public int NvIdNguoiNhan { get; set; }

    public int NvIdNguoiGui { get; set; }

    public int? CvId { get; set; }

    public int? DaId { get; set; }

    public virtual CongViec? CongViec { get; set; }

    public virtual NhanVien NvIdNguoiGuiNavigation { get; set; } = null!;

    public virtual NhanVien NvIdNguoiNhanNavigation { get; set; } = null!;
}
