using QuanLyDuAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyDuAn.Services
{
    internal class AuthService
    {
        private readonly ThucTapQuanLyDuAnContext _context;

        public AuthService()
        {
            _context = new ThucTapQuanLyDuAnContext();
        }

        public NhanVien? DangNhap(string taiKhoan, string matKhau)
        {
            return _context.NhanViens
                .FirstOrDefault(nv => nv.NvTaiKhoan == taiKhoan && nv.NvMatKhau == matKhau);
        }
    }
}
