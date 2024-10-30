using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LopHocTrucTuyen.Models.QuanTriVien
{
    public class QuanTriVienModel
    {
            public int MaQuanTriVien { get; set; }
            public string HoTen { get; set; }
            public string TenDangNhap { get; set; }
            public string TrangThai { get; set; }
            public string TenNhom { get; set; }  // Tên nhóm người dùng từ bảng NhomNguoiDung
    }

    public class ThemQuanTriVienModel
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }
        public int? MaNhom { get; set; }
        public string HoTen { get; set; }
        public string ChucVu { get; set; } 
    }
    public class GiangVienModel
    {
        public int MaGiangVien { get; set; }
        public string HoTen { get; set; }
        public string TenDangNhap { get; set; }
        public string TenChuyenNganh { get; set; }  
        public string TrangThai { get; set; }
    }
}