using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class CongViec

{

    public int CvId { get; set; }

    public string? CvMa { get; set; }

    public string CvTen { get; set; } = null!;

    public string? CvMoTa { get; set; }

    public DateTime? CvBatDau { get; set; }

    public DateTime? CvKetThuc { get; set; }

    public string? CvFile { get; set; }

    public string? CvPath { get; set; }

    public DateTime? CvThoiGianHoanThanh { get; set; }

    public int NvIdNguoiTao { get; set; }

    public int DaId { get; set; }

    public string TtMa { get; set; } = null!;

    public virtual DuAn Da { get; set; } = null!;

    public virtual NhanVienThamGiaDuAn NhanVienThamGiaDuAn { get; set; } = null!;

    public virtual ICollection<PhanCongCongViec> PhanCongCongViecs { get; set; } = new List<PhanCongCongViec>();

    public virtual TrangThai TtMaNavigation { get; set; } = null!;
}
