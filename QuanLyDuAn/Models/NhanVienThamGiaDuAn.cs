using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class NhanVienThamGiaDuAn
{
    public int NvId { get; set; }

    public int DaId { get; set; }

    public string VtMa { get; set; } = null!;

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual DuAn Da { get; set; } = null!;

    public virtual NhanVien Nv { get; set; } = null!;

    public virtual ICollection<PhanCongCongViec> PhanCongCongViecs { get; set; } = new List<PhanCongCongViec>();

    public virtual VaiTro VtMaNavigation { get; set; } = null!;
}
