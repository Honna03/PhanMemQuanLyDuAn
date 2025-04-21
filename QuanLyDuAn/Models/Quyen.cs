using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDuAn.Models;

public partial class Quyen
{
    public string QMa { get; set; } = null!;

    public string QTen { get; set; } = null!;

    public string? QMoTa { get; set; }
    [Column("q_LuongCoBan")]
    public decimal QLuongCoBan { get; set; }

    public virtual ICollection<LichSuCapNhatLuong> LichSuCapNhatLuongs { get; set; } = new List<LichSuCapNhatLuong>();
    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
