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

        // TRANG CHU
        public ActionResult TrangChu()
        {
            List<KhoaHoc> dskh = db.KhoaHocs.ToList();
            return View(dskh);
        }

        public ActionResult TimKiemKhoaHoc(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Chuyển hướng về trang chủ nếu không có từ khóa tìm kiếm
                return RedirectToAction("TrangChu");
            }

            // Thực hiện tìm kiếm nếu có từ khóa
            var ketQua = db.KhoaHocs
                           .Where(kh => kh.TenKhoaHoc.Contains(query))
                           .ToList();

            return View("TrangChu", ketQua);
        }



        // HOC TAP
        public ActionResult HocTap()
        {
            var userId = Session["UserId"];
            if (userId == null)
            {
                return RedirectToAction("DangNhap", "HocVien");
            }

            // Lấy thông tin học viên từ bảng HocVien dựa trên MaNguoiDung
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == (int)userId);
            if (hocVien == null)
            {
                TempData["Message"] = "Người dùng không tồn tại trong hệ thống.";
                return RedirectToAction("TrangChu");
            }

            // Lấy danh sách các khóa học mà học viên đã thanh toán thông qua bảng ThanhToan và ChiTietThanhToan
            var danhSachKhoaHocDaThanhToan = db.ChiTietThanhToans
                .Where(cttt => cttt.ThanhToan.MaHocVien == hocVien.MaHocVien)
                .Select(cttt => cttt.DangKy.KhoaHoc) // Lấy thông tin khóa học từ bảng DangKy qua ChiTietThanhToan
                .ToList();

            return View(danhSachKhoaHocDaThanhToan);
        }

        // CHI TIET KHOA HOC
        public ActionResult ChiTietKhoaHoc(int id)
        {
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return HttpNotFound();
            }

            var userId = Session["UserId"];
            bool isPaid = false;

            if (userId != null)
            {
                var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == (int)userId);
                if (hocVien != null)
                {
                    isPaid = db.ThanhToans.Any(tt => tt.MaHocVien == hocVien.MaHocVien &&
                                                     db.ChiTietThanhToans.Any(ct => ct.MaThanhToan == tt.MaThanhToan &&
                                                                                    ct.DangKy.MaKhoaHoc == id));
                }
            }
            ViewBag.IsPaid = isPaid;
            ViewBag.MaKhoaHoc = id;
            return View(khoaHoc);
        }

        [HttpPost]
        public JsonResult LuuDanhGia(int maKhoaHoc, int rate, string nhanXet)
        {
            var userId = Session["UserId"];
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá khóa học." });
            }

            var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == (int)userId);
            var tenDangNhap = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == (int)userId).TenDangNhap;

            if (hocVien == null || tenDangNhap == null)
            {
                return Json(new { success = false, message = "Không tìm thấy học viên." });
            }

            var dangKy = db.DangKies.FirstOrDefault(dk =>
                dk.MaKhoaHoc == maKhoaHoc &&
                dk.TrangThai == "Đã thanh toán" &&
                db.ThanhToans.Any(tt =>
                    tt.MaHocVien == hocVien.MaHocVien &&
                    db.ChiTietThanhToans.Any(ct =>
                        ct.MaThanhToan == tt.MaThanhToan && ct.MaDangKy == dk.MaDangKy)
                )
            );

            if (dangKy == null)
            {
                return Json(new { success = false, message = "Bạn chưa thanh toán khóa học này, không thể đánh giá." });
            }

            try
            {
                var danhGia = db.DanhGias.FirstOrDefault(dg => dg.MaDangKy == dangKy.MaDangKy && dg.MaKhoaHoc == maKhoaHoc);
                if (danhGia == null)
                {
                    danhGia = new DanhGia
                    {
                        MaDangKy = dangKy.MaDangKy,
                        MaKhoaHoc = maKhoaHoc,
                        Rate = rate,
                        NhanXet = nhanXet,
                        NgayDanhGia = DateTime.Now
                    };
                    db.DanhGias.InsertOnSubmit(danhGia);
                }
                else
                {
                    danhGia.Rate = rate;
                    danhGia.NhanXet = nhanXet;
                    danhGia.NgayDanhGia = DateTime.Now;
                }

                db.SubmitChanges();

                // Trả về tên tài khoản (TenDangNhap) thay vì tên đầy đủ
                return Json(new { success = true, message = "Đánh giá của bạn đã được lưu thành công!", userName = tenDangNhap });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu đánh giá: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult LayDanhGiaKhoaHoc(int maKhoaHoc)
        {
            var danhGiaList = db.DanhGias
                .Where(dg => dg.MaKhoaHoc == maKhoaHoc)
                .Select(dg => new
                {
                    UserName = db.ChiTietThanhToans
                                 .Where(ct => ct.MaDangKy == dg.MaDangKy)
                                 .Select(ct => db.ThanhToans
                                                .Where(tt => tt.MaThanhToan == ct.MaThanhToan)
                                                .Select(tt => tt.HocVien.NguoiDung.TenDangNhap)
                                                .FirstOrDefault())
                                 .FirstOrDefault(),
                    Rate = dg.Rate,
                    NhanXet = dg.NhanXet,
                    NgayDanhGia = dg.NgayDanhGia
                })
                .ToList()
                .Select(dg => new
                {
                    dg.UserName,
                    dg.Rate,
                    dg.NhanXet,
                    NgayDanhGia = dg.NgayDanhGia.HasValue ? dg.NgayDanhGia.Value.ToString("dd/MM/yyyy") : null
                })
                .ToList();

            return Json(danhGiaList, JsonRequestBehavior.AllowGet);
        }

        // THONG TIN NGUOI DUNG
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
        public ActionResult UploadAvatar(HttpPostedFileBase avatar)
        {
            if (avatar != null && avatar.ContentLength > 0)
            {
                var fileName = Path.GetFileName(avatar.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/HocVien/Images"), fileName);
                if (!Directory.Exists(Server.MapPath("~/Content/HocVien/Images")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/HocVien/Images"));
                }
                avatar.SaveAs(path);
                Session["AnhBia"] = fileName;
                return Json(new { success = true, filename = fileName });
            }

            return Json(new { success = false, message = "Tải lên thất bại" });
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
                        nguoiDung.Email = email ?? nguoiDung.Email;
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

        [HttpPost]
        public ActionResult DoiMatKhau(string currentPassword, string newPassword)
        {
            var userId = Session["UserId"];
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đổi mật khẩu." });
            }

            var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == (int)userId);
            if (nguoiDung == null || nguoiDung.MatKhau != currentPassword)
            {
                return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
            }

            nguoiDung.MatKhau = newPassword;
            db.SubmitChanges();
            return Json(new { success = true, message = "Mật khẩu đã được cập nhật thành công!" });
        }


        
        // GIO HANG
        public ActionResult GioHang()
        {
            var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();
            ViewBag.SelectedCourseId = TempData["SelectedCourseId"];
            return View(gioHang);
        }

        [HttpPost]
        public ActionResult ThemVaoGioHang(int id)
        {
            var khoaHoc = db.KhoaHocs.FirstOrDefault(kh => kh.MaKhoaHoc == id);
            if (khoaHoc != null)
            {
                var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();
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
                if (!gioHang.Any(kh => kh.MaKhoaHoc == id))
                {
                    gioHang.Add(khoaHoc);
                    Session["GioHang"] = gioHang;
                }
                TempData["SelectedCourseId"] = id; // Lưu ID khóa học vào TempData
            }
            return RedirectToAction("GioHang");
        }

        // CHI TIET THANH TOAN
        public ActionResult ChiTietThanhToan()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("DangNhap", "HocVien");
            }

            // Lấy danh sách khóa học đã chọn từ Session
            var khoaHocDaChon = (List<KhoaHoc>)Session["KhoaHocDaChon"] ?? new List<KhoaHoc>();

            // Tính tổng chi phí
            decimal tongChiPhi = khoaHocDaChon.Sum(kh => kh.Gia);
            ViewBag.TongChiPhi = tongChiPhi;

            return View(khoaHocDaChon);
        }

        [HttpPost]
        public ActionResult ChonKhoaHocThanhToan(List<int> selectedCourses)
        {
            // Lấy danh sách khóa học đã chọn từ database
            var khoaHocDaChon = db.KhoaHocs.Where(kh => selectedCourses.Contains(kh.MaKhoaHoc)).ToList();

            // Lưu vào Session để hiển thị trong trang thanh toán
            Session["KhoaHocDaChon"] = khoaHocDaChon;

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult HoanTatThanhToan()
        {
            var userId = Session["UserId"];
            if (userId == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để thanh toán.";
                return RedirectToAction("DangNhap", "HocVien");
            }
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == (int)userId);
            if (hocVien == null)
            {
                TempData["Message"] = "Người dùng không tồn tại trong hệ thống. Vui lòng kiểm tra tài khoản của bạn.";
                return RedirectToAction("GioHang");
            }
            var khoaHocDaChon = (List<KhoaHoc>)Session["KhoaHocDaChon"];
            if (khoaHocDaChon == null || !khoaHocDaChon.Any())
            {
                TempData["Message"] = "Không có khóa học nào để thanh toán.";
                return RedirectToAction("GioHang");
            }
            try
            {
                decimal tongTien = khoaHocDaChon.Sum(kh => kh.Gia);
                var thanhToan = new ThanhToan
                {
                    MaHocVien = hocVien.MaHocVien,
                    SoTien = tongTien,
                    NgayThanhToan = DateTime.Now,
                    TrangThai = "Đã thanh toán"
                };
                db.ThanhToans.InsertOnSubmit(thanhToan);
                db.SubmitChanges();
                var danhSachChiTietThanhToan = new List<ChiTietThanhToan>();
                var gioHang = (List<KhoaHoc>)Session["GioHang"] ?? new List<KhoaHoc>();
                foreach (var khoaHoc in khoaHocDaChon)
                {
                    var dangKy = new DangKy
                    {
                        MaKhoaHoc = khoaHoc.MaKhoaHoc,
                        NgayDangKy = DateTime.Now,
                        TrangThai = "Đã thanh toán"
                    };
                    db.DangKies.InsertOnSubmit(dangKy);
                    db.SubmitChanges();
                    var chiTietThanhToan = new ChiTietThanhToan
                    {
                        MaThanhToan = thanhToan.MaThanhToan,
                        MaDangKy = dangKy.MaDangKy,
                        SoTien = khoaHoc.Gia,
                        NgayChiTiet = DateTime.Now
                    };
                    danhSachChiTietThanhToan.Add(chiTietThanhToan);
                    gioHang.RemoveAll(kh => kh.MaKhoaHoc == khoaHoc.MaKhoaHoc);
                }
                db.ChiTietThanhToans.InsertAllOnSubmit(danhSachChiTietThanhToan);
                db.SubmitChanges();
                TempData["Message"] = "Thanh toán thành công!";
                Session["GioHang"] = gioHang;
                Session.Remove("KhoaHocDaChon");
                return RedirectToAction("ChiTietThanhToan");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Có lỗi xảy ra khi thực hiện thanh toán: " + ex.Message;
                return RedirectToAction("ChiTietThanhToan");
            }
        }

        // DANG NHAP, DANG KY
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
