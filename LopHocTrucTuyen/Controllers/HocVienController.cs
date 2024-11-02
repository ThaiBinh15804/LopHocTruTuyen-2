using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LopHocTrucTuyen.Filter;
using LopHocTrucTuyen.Models;
using System.IO; 

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
                new { Chuong = ch, BaiGiangs = db.BaiGiangs.Where(bg => bg.MaChuong == ch.MaChuong).OrderBy(bg => bg.ThuTu).ToList() }).OrderBy(ch => ch.Chuong.ThuTu).ToList();
            return View(khoaHoc);
        }

        public ActionResult ThongTinNguoiDung()
        {
            var userId = Session["UserId"];
            if (userId != null)
            {
                var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == (int)userId);
                var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == nguoiDung.MaNguoiDung);
                if (nguoiDung != null)
                {
                    ViewBag.TenDangNhap = nguoiDung.TenDangNhap ?? string.Empty;
                    ViewBag.Email = nguoiDung.Email ?? string.Empty;

                    if (hocVien != null)
                    {
                        ViewBag.HoTen = hocVien.HoTen ?? string.Empty;
                        ViewBag.GioiTinh = hocVien.GioiTinh ?? string.Empty;
                        ViewBag.SoDienThoai = hocVien.SoDienThoai ?? string.Empty;
                        ViewBag.DiaChi = hocVien.DiaChi ?? string.Empty;
                        ViewBag.NgaySinh = hocVien.NgaySinh.HasValue? hocVien.NgaySinh.Value.ToString("dd/MM/yyyy"): "Chưa xác định";
                    }
                    else
                    {
                        ViewBag.HoTen = string.Empty;
                        ViewBag.GioiTinh = string.Empty;
                        ViewBag.SoDienThoai = string.Empty;
                        ViewBag.DiaChi = string.Empty;
                        ViewBag.NgaySinh = "Chưa xác định";
                    }

                    ViewBag.AnhBia = Session["AnhBia"] ?? "~/Content/HocVien/Images/Default_Avatar.png"; 
                    return View();
                }
            }
            return RedirectToAction("DangNhap", "HocVien");
        }

        [HttpPost]
        public ActionResult ChinhSua(string email, string hoTen, DateTime? ngaySinh, string gioiTinh, string soDienThoai, string diaChi)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == (int)userId);
                    var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == nguoiDung.MaNguoiDung);

                    if (nguoiDung != null && hocVien != null)
                    {
                        // Cập nhật thông tin bảng NguoiDung
                        nguoiDung.Email = email ?? nguoiDung.Email;

                        // Cập nhật thông tin bảng HocVien
                        hocVien.HoTen = hoTen ?? hocVien.HoTen;
                        hocVien.NgaySinh = ngaySinh ?? hocVien.NgaySinh;
                        hocVien.GioiTinh = gioiTinh ?? hocVien.GioiTinh;
                        hocVien.SoDienThoai = soDienThoai ?? hocVien.SoDienThoai;
                        hocVien.DiaChi = diaChi ?? hocVien.DiaChi;

                        db.SubmitChanges();

                        return Json(new { success = true, message = "Thông tin đã được cập nhật thành công!" });
                    }
                }

                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thông tin. Người dùng không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ChinhSuaEmail(string email)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == (int)userId);

                    if (nguoiDung != null)
                    {
                        nguoiDung.Email = email;
                        db.SubmitChanges();

                        return Json(new { success = true, message = "Email đã được cập nhật thành công!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Người dùng không tồn tại." });
                    }
                }
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ChinhSuaSoDienThoai(string soDienThoai)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == (int)userId);

                    if (hocVien != null)
                    {
                        hocVien.SoDienThoai = soDienThoai;
                        db.SubmitChanges();

                        return Json(new { success = true, message = "Số điện thoại đã được cập nhật thành công!" });
                    }
                }
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số điện thoại. Người dùng không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }



        public ActionResult GioHang()
        {
            var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();
            ViewBag.SelectedCourseId = TempData["CourseId"]; 
            return View(gioHang);
        }


        [HttpPost]
        public ActionResult ThemVaoGioHang(int id)
        {
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.MaKhoaHoc == id);
            if (khoaHoc != null)
            {
                var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();

                // Check if the course is already in the cart
                if (!gioHang.Any(kh => kh.MaKhoaHoc == id))
                {
                    gioHang.Add(khoaHoc);
                    Session["GioHang"] = gioHang;
                    TempData["ToastMessage"] = "Sản phẩm đã được thêm vào Giỏ hàng!";
                }
                else
                {
                    TempData["ToastMessage"] = "Khóa học này đã có trong Giỏ hàng!";
                }

                return Json(new { success = true, message = TempData["ToastMessage"].ToString() });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
        }


        [HttpPost]
        public ActionResult XoaKhoiGioHang(int id)
        {
            var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();
            var itemToRemove = gioHang.FirstOrDefault(kh => kh.MaKhoaHoc == id);

            if (itemToRemove != null)
            {
                gioHang.Remove(itemToRemove);
                Session["GioHang"] = gioHang;
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult MuaNgay(int id)
        {
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.MaKhoaHoc == id);
            if (khoaHoc != null)
            {
                var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();

                // Kiểm tra xem khóa học đã có trong giỏ hàng chưa
                if (!gioHang.Any(kh => kh.MaKhoaHoc == id))
                {
                    gioHang.Add(khoaHoc);
                    Session["GioHang"] = gioHang;
                    TempData["CourseId"] = id; // Lưu ID khóa học vào TempData
                }
            }
            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase avatar)
        {
            if (avatar != null && avatar.ContentLength > 0)
            {
                var fileName = Path.GetFileName(avatar.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/HocVien/Images"), fileName);

                // Kiểm tra và tạo thư mục nếu chưa có
                if (!Directory.Exists(Server.MapPath("~/Content/HocVien/Images")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/HocVien/Images"));
                }

                // Lưu file vào thư mục
                avatar.SaveAs(path);

                // Cập nhật Session và trả về tên file
                Session["AnhBia"] = fileName;
                return Json(new { success = true, filename = fileName });
            }

            return Json(new { success = false, message = "Tải lên thất bại" });
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
                Session["UserId"] = user.MaNguoiDung;
                Session["User"] = user;
                Session["UserName"] = user.TenDangNhap;
                Session["UserEmail"] = user.Email; 
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
