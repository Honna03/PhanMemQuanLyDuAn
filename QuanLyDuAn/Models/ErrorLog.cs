using System;
using System.Collections.Generic;

namespace QuanLyDuAn.Models;

public partial class ErrorLog
{
    public int ErrorId { get; set; }

    public DateTime? ErrorTime { get; set; }

    public string? ErrorMessage { get; set; }
}
