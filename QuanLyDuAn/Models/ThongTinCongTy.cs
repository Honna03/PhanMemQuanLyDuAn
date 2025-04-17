using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.Models;

public partial class ThongTinCongTy
{
    [Key]
    public int CtyId { get; set; }

    public string CtyTen { get; set; } = null!;

    public string CtyDiaChi { get; set; } = null!;

    public string CtySdt { get; set; } = null!;

    public string? CtyEmail { get; set; }

    public string? CtyLogo { get; set; }

    public string? CtyMoTa { get; set; }
}
