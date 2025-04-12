using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class TrangThai
{
    public string TtMa { get; set; } = null!;

    public string TtTen { get; set; } = null!;

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<DuAn> DuAns { get; set; } = new List<DuAn>();
}
