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
            if (user == null)
            {
                ViewBag.text = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }
            if (!string.IsNullOrEmpty(tk) && tk.Contains(" "))
            {
                ViewBag.text = "Tên đăng nhập không chứa khoảng trắng!";
                return View();
            }
            if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(mk))
            {
                ViewBag.text = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }
            
            if (user.TrangThai == "Dừng hoạt động" && (user.MaNhom == 2 || user.MaNhom == 4 || user.MaNhom == 5))
            {
                ViewBag.text = "Tài khoản đã bị đình chỉ!";
                return View();
            }

            if (user != null && user.TrangThai == "Đang hoạt động" && (user.MaNhom == 1 || user.MaNhom == 4 || user.MaNhom == 5))
            {
                Session["user"] = user;
                return RedirectToAction("BangDieuKhien", "QuanTriVien");
            }
            else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhom == 2)
            {
                Session["user"] = user;
                return RedirectToAction("Index", "GiangVien");
            }
            else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhom == 3)
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

            // Kiểm tra xem MaNhom có giá trị không
            if (!user.MaNhom.HasValue)
            {
                ViewBag.errMaNhom = "Vui lòng chọn nhóm người dùng!";
                return View(user); // Trả về view với thông báo lỗi
            }

            var nguoiDung = new NguoiDung
            {
                TenDangNhap = user.TenDangNhap,
                MatKhau = user.MatKhau,
                Email = user.Email,
                NgayTao = DateTime.Now,
                TrangThai = "Đang hoạt động",
                Avatar = user.Avatar,
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
        [HttpGet]
        public ActionResult TimKiemQuanTriVien(string search, int page = 1, int pageSize = 5)
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
                              TenNhom = quantri.ChucVu
                          };

            // Xử lý tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                dsAdmin = dsAdmin.Where(q => q.HoTen.Contains(search));
            }

            var totalRecords = dsAdmin.Count();

            var adminList = dsAdmin.OrderBy(q => q.MaQuanTriVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new QuanTriVienPagedList
            {
                AdminList = adminList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = search // Truyền searchQuery vào model
            };

            return View(model);
        }
        public ActionResult ChiTietQuanTriVien(int id)
        {
            var quantrivien = db.QuanTriViens.FirstOrDefault(t => t.MaQuanTriVien == id);
            var nguoiDung = db.NguoiDungs.FirstOrDefault(t => t.MaNguoiDung == quantrivien.MaNguoiDung);
            ViewBag.NhomND = nguoiDung.MaNhom == 4 ? "Thu ngân" : "Kĩ thuật viên";
            var viewModel = new ThemQuanTriVienModel
            {
                TenDangNhap = nguoiDung.TenDangNhap,
                MatKhau = nguoiDung.MatKhau,
                Email = nguoiDung.Email,
                NgayTao = nguoiDung.NgayTao,
                TrangThai = nguoiDung.TrangThai,
                Avatar = nguoiDung.Avatar,
                MaNhom = nguoiDung.MaNhom,
                HoTen = quantrivien.HoTen,
                ChucVu = quantrivien.ChucVu
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ChiTietQuanTriVien(ThemQuanTriVienModel user)
        {
            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser == null)
            {                
                return View(user); // Trả về lại trang với model để hiển thị lỗi
            }

            if (string.IsNullOrEmpty(user.Avatar))
            {
                user.Avatar = findUser.Avatar;
            }

            // Cập nhật các thông tin của người dùng
            findUser.MatKhau =user.MatKhau;
            findUser.Email = user.Email;

            findUser.Avatar = user.Avatar;
            findUser.MaNhom = user.MaNhom.Value;

            var quanTriVien = db.QuanTriViens.FirstOrDefault(q => q.MaNguoiDung == findUser.MaNguoiDung);
            if(findUser.MaNguoiDung == 1) {
                findUser.TrangThai =  "Đang hoạt động";
                quanTriVien.ChucVu = "Quản trị viên";
            }
            else
            {
                findUser.TrangThai = user.TrangThai;
                quanTriVien.ChucVu = (user.MaNhom == 4) ? "Thu ngân" : "Kĩ thuật viên";
            }
            quanTriVien.HoTen = user.HoTen;

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
                ViewBag.errTenDangNhap1 = "Tên đăng nhập đã tồn tại!";
                return View(user);  // Trả về view với thông báo lỗi
            }

            var nguoiDung = new NguoiDung
            {
                TenDangNhap = user.TenDangNhap,
                MatKhau = user.MatKhau,
                Email = user.Email,
                NgayTao = DateTime.Now,
                TrangThai = "Đang hoạt động",
                Avatar = user.Avatar,
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
        [HttpGet]
        public ActionResult TimKiemGiangVien(string search, int page = 1, int pageSize = 5)
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
            // Xử lý tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                dsInstructor = dsInstructor.Where(q => q.HoTen.Contains(search));
            }

            var totalRecords = dsInstructor.Count();
            var instructorList = dsInstructor.OrderBy(q => q.MaGiangVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new GiangVienPagedList
            {
                InstructorList = instructorList,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                SearchQuery = search
            };

            return View(model);
        }
        public ActionResult ChiTietGiangVien(int id)
        {
            var giangvien = db.GiangViens.FirstOrDefault(t => t.MaGiangVien == id);
            var nguoiDung = db.NguoiDungs.FirstOrDefault(t => t.MaNguoiDung == giangvien.MaNguoiDung);
            var viewModel = new ThemGiangVienModel
            {
                TenDangNhap = nguoiDung.TenDangNhap,
                MatKhau = nguoiDung.MatKhau,
                Email = nguoiDung.Email,
                NgayTao = nguoiDung.NgayTao,
                TrangThai = nguoiDung.TrangThai,
                Avatar = nguoiDung.Avatar,
                MaNhom = nguoiDung.MaNhom,
                HoTen = giangvien.HoTen,
                SoDienThoai = giangvien.SoDienThoai,
                ChuyenNganh = giangvien.ChuyenNganh,
                DiaChi = giangvien.DiaChi
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ChiTietGiangVien(ThemGiangVienModel user)
        {
            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser == null)
            {
                return View(user); // Trả về lại trang với model để hiển thị lỗi
            }

            if (string.IsNullOrEmpty(user.Avatar))
            {
                user.Avatar = findUser.Avatar;
            }

            // Cập nhật các thông tin của người dùng
            findUser.MatKhau = user.MatKhau;
            findUser.Email = user.Email;
            findUser.TrangThai = user.TrangThai;
            findUser.Avatar = user.Avatar;

            var giangvien = db.GiangViens.FirstOrDefault(q => q.MaNguoiDung == findUser.MaNguoiDung);
            if (giangvien != null)
            {
                // Cập nhật thông tin quản trị viên
                giangvien.HoTen = user.HoTen;
                giangvien.SoDienThoai = user.SoDienThoai;
                giangvien.ChuyenNganh = user.ChuyenNganh;
                giangvien.DiaChi = user.DiaChi;
            }

            db.SubmitChanges();
            return RedirectToAction("GiangVien");
        }

        // trang học viên
        public ActionResult HocVien(int page = 1, int pageSize = 5)
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
                                   NgaySinh = hocvien.NgaySinh ?? DateTime.Now, // Sử dụng giá trị mặc định nếu NgaySinh là null
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

        public ActionResult ThemHocVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var model = new ThemHocVienModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemHocVien(ThemHocVienModel user)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser != null)
            {
                ViewBag.errTenDangNhap2 = "Tên đăng nhập đã tồn tại!";
                return View(user);  // Trả về view với thông báo lỗi
            }

            var nguoiDung = new NguoiDung
            {
                TenDangNhap = user.TenDangNhap,
                MatKhau = user.MatKhau,
                Email = user.Email,
                NgayTao = DateTime.Now,
                TrangThai = "Đang hoạt động",
                Avatar = user.Avatar,
                MaNhom = 3
            };
            db.NguoiDungs.InsertOnSubmit(nguoiDung);
            db.SubmitChanges();

            var hocVien = new HocVien
            {
                MaNguoiDung = nguoiDung.MaNguoiDung,
                HoTen = user.HoTen,
                NgaySinh = user.NgaySinh,
                GioiTinh = user.GioiTinh,
                SoDienThoai = user.SoDienThoai,
                DiaChi = user.DiaChi
            };
            db.HocViens.InsertOnSubmit(hocVien);
            db.SubmitChanges();
            return RedirectToAction("HocVien");
        }
        public ActionResult ChiTietHocVien(int id)
        {
            var hocvien = db.HocViens.FirstOrDefault(t => t.MaHocVien == id);
            ViewBag.GioiTinh = hocvien.GioiTinh;
            var nguoiDung = db.NguoiDungs.FirstOrDefault(t => t.MaNguoiDung == hocvien.MaNguoiDung);
            var viewModel = new ThemHocVienModel
            {
                TenDangNhap = nguoiDung.TenDangNhap,
                MatKhau = nguoiDung.MatKhau,
                Email = nguoiDung.Email,
                NgayTao = nguoiDung.NgayTao,
                TrangThai = nguoiDung.TrangThai,
                Avatar = nguoiDung.Avatar,
                MaNhom = nguoiDung.MaNhom,
                HoTen = hocvien.HoTen,
                NgaySinh = hocvien.NgaySinh.HasValue ? hocvien.NgaySinh.Value : (DateTime?)null,
                GioiTinh = hocvien.GioiTinh,
                SoDienThoai = hocvien.SoDienThoai,
                DiaChi = hocvien.DiaChi
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult ChiTietHocVien(ThemHocVienModel user)
        {
            var findUser = db.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == user.TenDangNhap);
            if (findUser == null)
            {
                return View(user); // Trả về lại trang với model để hiển thị lỗi
            }

            if (string.IsNullOrEmpty(user.Avatar))
            {
                user.Avatar = findUser.Avatar;
            }

            // Cập nhật các thông tin của người dùng
            findUser.MatKhau = user.MatKhau;
            findUser.Email = user.Email;
            findUser.TrangThai = user.TrangThai;
            findUser.Avatar = user.Avatar;

            var hocvien = db.HocViens.FirstOrDefault(q => q.MaNguoiDung == findUser.MaNguoiDung);
            if (hocvien != null)
            {
                // Cập nhật thông tin quản trị viên
                hocvien.HoTen = user.HoTen;
                hocvien.SoDienThoai = user.SoDienThoai;
                hocvien.NgaySinh = user.NgaySinh;
                hocvien.DiaChi = user.DiaChi;
                hocvien.GioiTinh = user.GioiTinh;
            }

            db.SubmitChanges();
            return RedirectToAction("HocVien");
        }
        [HttpGet]
        public ActionResult TimKiemHocVien(string search, int page = 1, int pageSize = 5)
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
                                NgaySinh = hocvien.NgaySinh ?? DateTime.Now, // Sử dụng giá trị mặc định nếu NgaySinh là null
                                TrangThai = nd.TrangThai
                            };

            if (!string.IsNullOrEmpty(search))
            {
                dsStudent = dsStudent.Where(t => t.HoTen.Contains(search));
            }

            var totalRecords = dsStudent.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var studentList = dsStudent.OrderBy(q => q.MaHocVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new HocVienPagedList
            {
                StudentList = studentList,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentPage = page,
                SearchQuery = search
            };

            return View(model);
        }

        public ActionResult LoaiKhoaHoc(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var dsLoaiKH = from loai in db.LoaiKhoaHocs
                          select new LoaiKhoaHocModel
                          {
                              MaLoaiKhoaHoc = loai.MaLoaiKhoaHoc,
                              TenLoai = loai.TenLoai,
                              MoTa = loai.MoTa,
                          };
            var totalRecords = dsLoaiKH.Count();

            var dsLoaiKHList = dsLoaiKH.OrderBy(q => q.MaLoaiKhoaHoc).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new LoaiKhoaHocPagedList
            {
                LoaiKhoaHoc = dsLoaiKHList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        public ActionResult TimKiemLoaiKhoaHoc(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var dsLoaiKH = from loai in db.LoaiKhoaHocs
                          select new LoaiKhoaHocModel
                          {
                              MaLoaiKhoaHoc = loai.MaLoaiKhoaHoc,
                              TenLoai = loai.TenLoai,
                              MoTa = loai.MoTa,
                          };

            if (!string.IsNullOrEmpty(search))
            {
                dsLoaiKH = dsLoaiKH.Where(t => t.TenLoai.Contains(search));
            }

            var totalRecords = dsLoaiKH.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var khoahocList = dsLoaiKH.OrderBy(q => q.MaLoaiKhoaHoc).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new LoaiKhoaHocPagedList
            {
                LoaiKhoaHoc = khoahocList,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentPage = page,
                SearchQuery = search
            };

            return View(model);
        }
        public ActionResult ChiTietLoaiKhoaHoc(int id)
        {
            var loaiKH = db.LoaiKhoaHocs.FirstOrDefault(t => t.MaLoaiKhoaHoc == id);
            List<KhoaHoc> listKH = db.KhoaHocs.Where(t => t.MaLoaiKhoaHoc == loaiKH.MaLoaiKhoaHoc).ToList();

            var model = new ChiTietKhoaHoc
            {
                loaiKH = loaiKH,
                listKH_loai = listKH
            };
            return View(model);
        }

        public ActionResult ThemLoaiKhoaHoc()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemLoaiKhoaHoc(LoaiKhoaHoc kh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var addLoaiKH = new LoaiKhoaHoc
            {
                TenLoai = kh.TenLoai,
                MoTa = kh.MoTa
            };
            db.LoaiKhoaHocs.InsertOnSubmit(addLoaiKH);
            db.SubmitChanges();
            return RedirectToAction("LoaiKhoaHoc");
        }
        
        public ActionResult KhoaHoc()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<KhoaHoc> listKhoaHoc = db.KhoaHocs.Where(t => t.TrangThai == true).ToList();
            return View(listKhoaHoc);
        }

        public ActionResult ChiTietKhoaHoc(int makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            KhoaHoc KhoaHoc = db.KhoaHocs.FirstOrDefault(t => t.MaKhoaHoc == makh);
            return View(KhoaHoc);
        }

        public ActionResult XuLyDieuHuong(int makh, string page)
        {
            TempData["DieuHuong"] = page;
            return RedirectToAction("ChiTietKhoaHoc", new { makh = makh });
        }

        public ActionResult ThongTinKhoaHoc(int makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            KhoaHoc KhoaHoc = db.KhoaHocs.FirstOrDefault(t => t.MaKhoaHoc == makh);
            int chuong = db.Chuongs.Count(c => c.MaKhoaHoc == makh);
            ViewBag.SoLuongChuong = chuong;
            int soLuongBaiGiang = db.BaiGiangs.Count(b => db.Chuongs.Any(c => c.MaChuong == b.MaChuong && c.MaKhoaHoc == makh));
            ViewBag.SoLuongBaiGiang = soLuongBaiGiang;

            int soLuongBaiTap = db.BaiTaps.Count(b => db.Chuongs.Any(c => c.MaChuong == b.MaChuong && c.MaKhoaHoc == makh));
            ViewBag.SoLuongBaiTap = soLuongBaiTap;
            return PartialView(KhoaHoc);
        }

        public ActionResult BaiHoc(int makh)
        {
            KhoaHoc kh = db.KhoaHocs.FirstOrDefault(t => t.MaKhoaHoc == makh);
            return PartialView(kh);
        }

        public ActionResult XemBaiGiang(int mabg)
        {
            var baiGiang = db.BaiGiangs.FirstOrDefault(t => t.MaBaiGiang == mabg);
            if (baiGiang == null)
            {
                return new HttpStatusCodeResult(404, "Bài giảng không tồn tại");
            }
            return PartialView("_XemBaiGiangPartial", baiGiang); // Trả về partial view kèm dữ liệu
        }

        public ActionResult KhoaHocPheDuyet()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<KhoaHoc> listKhoaHoc = db.KhoaHocs.Where(t => t.TrangThai == false).ToList();
      
            return View(listKhoaHoc);
        }
    }
}
