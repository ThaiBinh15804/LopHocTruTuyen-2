using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LopHocTrucTuyen.Models.QuanTriVien
{
    public class QuanTriVienModel
    {
            public int MaNguoiDung { get; set; }
            public string HoTen { get; set; }
            public string TenDangNhap { get; set; }
            public string TrangThai { get; set; }
            public string TenNhom { get; set; }  // Tên nhóm người dùng từ bảng NhomNguoiDung
    }
}