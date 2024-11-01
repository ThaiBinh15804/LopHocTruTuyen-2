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

    public class QuanTriVienPagedList
    {
        public List<QuanTriVienModel> AdminList { get; set; } // Danh sách quản trị viên
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }   // Tổng số trang
        public int PageSize { get; set; }     // Số lượng bản ghi mỗi trang
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

    public class GiangVienPagedList
    {
        public List<GiangVienModel> InstructorList { get; set; } // Danh sách quản trị viên
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }   // Tổng số trang
        public int PageSize { get; set; }     // Số lượng bản ghi mỗi trang
    }

    public class ThemGiangVienModel
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }
        public int MaNhom { get; set; }
        public string HoTen { get; set; }
        public string ChuyenNganh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class HocVienModel
    {
        public int MaHocVien { get; set; }
        public string HoTen { get; set; }
        public string TenDangNhap { get; set; }
        public string NgaySinh { get; set; }
        public string TrangThai { get; set; }
    }

    public class HocVienPagedList
    {
        public List<HocVienModel> StudentList { get; set; } // Danh sách quản trị viên
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }   // Tổng số trang
        public int PageSize { get; set; }     // Số lượng bản ghi mỗi trang
    }
}