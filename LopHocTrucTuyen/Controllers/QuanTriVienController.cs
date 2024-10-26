using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LopHocTrucTuyen.Models.QuanTriVien;

namespace LopHocTrucTuyen.Controllers
{
    public class QuanTriVienController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult DashBoard()
        {
            return View();
        }

        // Trang quản trị
        public ActionResult Administrators()
        {
            var dsAdmin = from nd in db.NguoiDungs
                          join nhom in db.NhomNguoiDungs on nd.MaNhom equals nhom.MaNhom
                          join quantri in db.QuanTriViens on nd.MaNguoiDung equals quantri.MaNguoiDung
                          where nd.MaNhom == 1
                          select new QuanTriVienModel
                          {
                              MaNguoiDung = nd.MaNguoiDung,
                              HoTen = quantri.HoTen,
                              TenDangNhap = nd.TenDangNhap,
                              TrangThai = nd.TrangThai,
                              TenNhom = nhom.TenNhom // Lấy tên nhóm từ bảng NhomNguoiDung
                          };
            return View(dsAdmin.ToList());
        }
        // Trang giảng viên
        public ActionResult Instructors()
        {
            return View();
        }
        // Trang học viên
        public ActionResult Students()
        {
            return View();
        }
    }
}
