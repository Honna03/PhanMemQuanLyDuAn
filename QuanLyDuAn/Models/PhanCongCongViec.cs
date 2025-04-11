using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class PhanCongCongViec
{
    public int CvId { get; set; }

    public int DaId { get; set; }

    public int NvId { get; set; }

    public virtual ICollection<CapNhatCongViec> CapNhatCongViecs { get; set; } = new List<CapNhatCongViec>();

    public virtual CongViec CongViec { get; set; } = null!;

    public virtual NhanVienThamGiaDuAn NhanVienThamGiaDuAn { get; set; } = null!;
}
