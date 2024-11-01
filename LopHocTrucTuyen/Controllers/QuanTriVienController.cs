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
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection col)
        {
            string tk = col["inputTenTK"];
            string mk = col["inputMatKhau"];

            ViewBag.tk = tk;
            ViewBag.mk = mk;

            NguoiDung user = db.NguoiDungs.FirstOrDefault(k => k.TenDangNhap == tk && k.MatKhau == mk);
            if (!string.IsNullOrEmpty(tk) && tk.Contains(" "))
            {
                ViewBag.text = "Tên đăng nhập không chứa khoảng trắng!";
                return View();
            }
            if (string.IsNullOrEmpty(tk)|| string.IsNullOrEmpty(mk))
            {
                ViewBag.text = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }
            if (user == null)
            {
                ViewBag.text = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }
            if (user != null && user.MaNhom == 1 || user.MaNhom == 4 || user.MaNhom == 5)
            {
                Session["user"] = user;
                return RedirectToAction("BangDieuKhien", "QuanTriVien");
            }
            else if (user != null && user.MaNhom == 2)
            {
                return RedirectToAction("Index", "GiangVien");
            }
            else
            {
                ViewBag.text = "Tài khoản không đủ quyền truy cập!";
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            if (Session["user"] != null)
            {
                Session["user"] = null;
            }
            return RedirectToAction("DangNhap");
        }

        public ActionResult BangDieuKhien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            return View();
        }

        // Trang quản trị
        public ActionResult QuanTriVien(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var dsAdmin = from nd in db.NguoiDungs
                          join quantri in db.QuanTriViens on nd.MaNguoiDung equals quantri.MaNguoiDung
                          where nd.MaNhom == 1 || nd.MaNhom == 4 || nd.MaNhom == 5
                          select new QuanTriVienModel
                          {
                              MaQuanTriVien = quantri.MaQuanTriVien,
                              HoTen = quantri.HoTen,
                              TenDangNhap = nd.TenDangNhap,
                              TrangThai = nd.TrangThai,
                              TenNhom = quantri.ChucVu // Lấy tên nhóm từ bảng QuanTriVien
                          };
            var totalRecords = dsAdmin.Count();

            var adminList = dsAdmin.OrderBy(q => q.MaQuanTriVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new QuanTriVienPagedList
            {
                AdminList = adminList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        public ActionResult ThemQuanTriVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var model = new ThemQuanTriVienModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemQuanTriVien(ThemQuanTriVienModel user)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser != null)
            {
                ViewBag.errTenDangNhap = "Tên đăng nhập đã tồn tại!";
                return View(user);  // Trả về view với thông báo lỗi
            }

            var nguoiDung = new NguoiDung
            { 
                 MaNguoiDung = db.NguoiDungs.Count(),
                 TenDangNhap = user.TenDangNhap,
                 MatKhau = user.MatKhau,
                 Email = user.Email,
                 NgayTao = DateTime.Now,
                 TrangThai = "Đang Hoạt động",
                 MaNhom = user.MaNhom.Value
             };
             db.NguoiDungs.InsertOnSubmit(nguoiDung);
             db.SubmitChanges();

             var quanTriVien = new QuanTriVien
             {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoTen = user.HoTen,
                ChucVu = (user.MaNhom == 4) ? "Thu ngân" : "Kĩ thuật viên"
             };
             db.QuanTriViens.InsertOnSubmit(quanTriVien);
             db.SubmitChanges(); 
             return RedirectToAction("QuanTriVien"); 
        }
 
        // trang giảng viên
        public ActionResult GiangVien(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var dsInstructor = from nd in db.NguoiDungs
                               join giangvien in db.GiangViens on nd.MaNguoiDung equals giangvien.MaNguoiDung
                          where nd.MaNhom == 2
                          select new GiangVienModel
                          {
                              MaGiangVien = giangvien.MaGiangVien,
                              HoTen = giangvien.HoTen,
                              TenDangNhap = nd.TenDangNhap,
                              TenChuyenNganh = giangvien.ChuyenNganh,
                              TrangThai = nd.TrangThai
                          };
            var totalRecords = dsInstructor.Count();
            var instructorList = dsInstructor.OrderBy(q => q.MaGiangVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new GiangVienPagedList
            {
                InstructorList = instructorList,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(model);
        }

        public ActionResult ThemGiangVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var model = new ThemGiangVienModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemGiangVien(ThemGiangVienModel user)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser != null)
            {
                ViewBag.errTenDangNhap = "Tên đăng nhập đã tồn tại!";
                return View(user);  // Trả về view với thông báo lỗi
            }

            var nguoiDung = new NguoiDung
            {
                MaNguoiDung = db.NguoiDungs.Count(),
                TenDangNhap = user.TenDangNhap,
                MatKhau = user.MatKhau,
                Email = user.Email,
                NgayTao = DateTime.Now,
                TrangThai = "Đang Hoạt động",
                MaNhom = 2
            };
            db.NguoiDungs.InsertOnSubmit(nguoiDung);
            db.SubmitChanges();

            var giangVien = new GiangVien
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoTen = user.HoTen,
                ChuyenNganh = user.ChuyenNganh,
                SoDienThoai = user.SoDienThoai,
                DiaChi = user.DiaChi
            };
            db.GiangViens.InsertOnSubmit(giangVien);
            db.SubmitChanges();
            return RedirectToAction("GiangVien");
        }

        public ActionResult HocVien(int page = 1, int pageSize = 2)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var dsStudent = from nd in db.NguoiDungs
                               join hocvien in db.HocViens on nd.MaNguoiDung equals hocvien.MaNguoiDung
                               where nd.MaNhom == 3
                               select new HocVienModel
                               {
                                   MaHocVien = hocvien.MaHocVien,
                                   HoTen = hocvien.HoTen,
                                   TenDangNhap = nd.TenDangNhap,
                                   NgaySinh = hocvien.NgayDangKy.ToString(),
                                   TrangThai = nd.TrangThai
                               };

            var totalRecords = dsStudent.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var studentList = dsStudent.OrderBy(q => q.MaHocVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new HocVienPagedList
            {
                StudentList = studentList,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(model);
        }

        public ActionResult KhoaHoc()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            
            return View();
        }
    }
}
