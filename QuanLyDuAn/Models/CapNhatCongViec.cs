using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class CapNhatCongViec
{
    public int CnId { get; set; }

    public string CnMoTa { get; set; } = null!;

    public string? CnFile { get; set; }

    public string? CvPath { get; set; }

    public DateTime? CnThoiGian { get; set; }

    public int CvId { get; set; }

    public int DaId { get; set; }

    public int NvId { get; set; }

    public virtual PhanCongCongViec PhanCongCongViec { get; set; } = null!;
}
