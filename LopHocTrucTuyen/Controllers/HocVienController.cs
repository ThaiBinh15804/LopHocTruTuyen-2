using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LopHocTrucTuyen.Filter; // Thêm namespace của bộ lọc
using LopHocTrucTuyen.Models;


namespace LopHocTrucTuyen.Controllers
{
     // Áp dụng bộ lọc cho toàn bộ controller
    public class HocVienController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult TrangChu()
        {
            List<KhoaHoc> dskh = db.KhoaHocs.ToList();
            return View(dskh);
        }

        public ActionResult HocTap()
        {
            return View();
        }

        public ActionResult ThongTin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);
            if (user != null)
            {
                Session["User"] = user; // Lưu thông tin người dùng vào Session
                return RedirectToAction("TrangChu", "HocVien");
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }

        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa thông tin Session khi đăng xuất
            return RedirectToAction("DangNhap");
        }
    }
}
