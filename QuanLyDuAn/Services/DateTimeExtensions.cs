using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyDuAn.Services
{
    public static class DateTimeExtensions
    {
        public static DateOnly ToDateOnly(this DateTime dt)
        {
            return DateOnly.FromDateTime(dt);
        }
    }
}
