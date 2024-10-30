using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LopHocTrucTuyen.Filter; 
using LopHocTrucTuyen.Models;


namespace LopHocTrucTuyen.Controllers
{
    [YeuCauDangNhap]
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

        public ActionResult ChiTietKhoaHoc(int id)
        {
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            var chuongs = db.Chuongs.Where(ch => ch.MaKhoaHoc == id).Select(ch => 
                new{Chuong = ch, BaiGiangs = db.BaiGiangs.Where(bg => bg.MaChuong == ch.MaChuong).OrderBy(bg => bg.ThuTu).ToList()}).OrderBy(ch => ch.Chuong.ThuTu).ToList();

            
            return View(khoaHoc);
        }

        public ActionResult ThongTinNguoiDung()
        {
            return View();
        }

        public ActionResult GioHang()
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
                Session["User"] = user; 
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
            Session.Clear(); 
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(string username, string password, string email)
        {
            try
            {
                // Thêm vào bảng NguoiDung
                NguoiDung nguoiDung = new NguoiDung
                {
                    TenDangNhap = username,
                    MatKhau = password,
                    Email = email,
                    NgayTao = DateTime.Now,
                    TrangThai = "Hoạt động",
                    MaNhom = 3
                };
                db.NguoiDungs.InsertOnSubmit(nguoiDung);
                db.SubmitChanges(); 

                int maNguoiDungNew = nguoiDung.MaNguoiDung;
                HocVien hocVien = new HocVien
                {
                    MaNguoiDung = maNguoiDungNew,
                    HoTen = "Nguoi dung " + maNguoiDungNew,
                    NgaySinh = null,
                    GioiTinh = null,
                    SoDienThoai = null,
                    DiaChi = null
                };
                db.HocViens.InsertOnSubmit(hocVien);
                db.SubmitChanges(); 

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đăng ký thất bại: " + ex.Message;
                return View();
            }
        }
    }
}
