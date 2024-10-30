using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LopHocTrucTuyen.Models;
using System.Data.Entity;

namespace LopHocTrucTuyen.Controllers
{
    public class GiangVienController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        public ActionResult Index()
        {
            GiangVien gv = (GiangVien)Session["user"];
            return View(data.KhoaHocs.Where(t => t.MaGiangVien == gv.MaGiangVien).Take(5).ToList());
        }

        public ActionResult HienThiKhoaHoc()
        {
            GiangVien gv = (GiangVien)Session["user"];
            return View(data.KhoaHocs.Where(t => t.MaGiangVien == gv.MaGiangVien).ToList());
        }

        public ActionResult KhoaHoc(string makh)
        {
            KhoaHoc kh = data.KhoaHocs.FirstOrDefault(t => t.MaKhoaHoc.ToString() == makh); 
            return View(kh);
        }

        public ActionResult XuLyDieuHuong(string makh, string page)
        {
            TempData["DieuHuong"] = page;
            return RedirectToAction("KhoaHoc", new { makh = makh });
        }

        public ActionResult BaiHoc(string makh)
        {
            KhoaHoc kh = data.KhoaHocs.FirstOrDefault(t => t.MaKhoaHoc.ToString() == makh);
            return PartialView(kh);
        }

        public ActionResult BaoCao(string makh)
        {
            return PartialView();
        }

        public ActionResult DanhGia(string makh)
        {
            return PartialView();
        }

        public ActionResult HocVien(string makh)
        {
            return PartialView();
        }

        public ActionResult ThietLap(string makh)
        {
            return PartialView();
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        public ActionResult XuLyDangNhap(FormCollection c)
        {
            string tenDN = c["username"];
            string mk = c["password"];

            NguoiDung user = data.NguoiDungs.FirstOrDefault(t => t.TenDangNhap == tenDN && t.MatKhau == mk);

            if (user != null)
            {
                GiangVien gv = data.GiangViens.FirstOrDefault(t => t.MaNguoiDung == user.MaNguoiDung);
                Session["user"] = gv;
                TempData["ThongBao"] = "Đăng nhập thành công";
            }
            else
            {
                TempData["ThongBao"] = "Đăng nhập thất bại";
            }

            return RedirectToAction("Index");
        }

        public ActionResult TaoKH_1()
        {
            KhoaHoc kh = new KhoaHoc();
            return PartialView(kh);
        }

        [HttpPost]
        public ActionResult XuLyTaoKH_1(KhoaHoc kh)
        {
            if (ModelState.IsValid)
            {
                kh.MaGiangVien = 1;
                kh.MaLoaiKhoaHoc = 1;
                kh.Gia = 1000;
                kh.TrangThai = false;
                data.KhoaHocs.InsertOnSubmit(kh);
                data.SubmitChanges();

                TempData["ThongBao"] = "Thêm khoá học thành công";
            }
            else
            {
                TempData["ThongBao"] = "Thêm khoá học không thành công";

            }
            return RedirectToAction("Index");
        }

        public ActionResult TestUploadVideo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XuLy_TestUploadVideo(HttpPostedFileBase video, string url, string optionUpload)
        {
            if (optionUpload == "file")
            {
                if (video != null)
                {
                    string filename = video.FileName;

                    string duongdan = Path.Combine(Server.MapPath("~/Content/GiangVien/Video"), filename);

                    video.SaveAs(duongdan);

                    TempData["ThongBao"] = "Upload file thành công";
                }
                else
                {
                    TempData["ThongBao"] = "Upload file thất bại";
                }
            }
            else
            {
                if (url != "")
                {
                    string s = "Upload url thành công" + url;
                    TempData["ThongBao"] = url;
                }
                else
                    TempData["ThongBao"] = "Upload url không thành công" ;
            }

            return RedirectToAction("Index", "GiangVien");


        }

        public ActionResult Test()
        {
            KhoaHoc kh = data.KhoaHocs
                .Include(k => k.Chuongs.Select(c => c.BaiGiangs)) // Eager load các chương và bài giảng
                .FirstOrDefault(t => t.MaKhoaHoc == 1);

            kh.Chuongs.ToList();

            return View(kh);
        }

        [HttpPost]
        public ActionResult TaoChuong(string makh, FormCollection c)
        {
            Chuong ch = new Chuong();
            ch.TenChuong = c["TenChuong"];
            ch.MaKhoaHoc = int.Parse(makh);
            ch.MoTa = c["MoTa"];
            ch.ThuTu = int.Parse(c["ThuTu"]) + 1;

            data.Chuongs.InsertOnSubmit(ch);
            data.SubmitChanges();

            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult SuaChuong(string machuong)
        {
            return View(data.Chuongs.FirstOrDefault(t => t.MaChuong.ToString() == machuong));
        }


        public ActionResult XuLySuaChuong(Chuong ch)
        {
            Chuong cu = data.Chuongs.FirstOrDefault(t => t.MaChuong == ch.MaChuong);
            //cu.TenChuong = ch.TenChuong;
            //cu.MoTa = ch.MoTa;
            //cu.ThuTu = ch.ThuTu;
            //UpdateModel(cu);
            UpdateModel(cu, new[] { "TenChuong", "MoTa", "ThuTu" });
            data.SubmitChanges();

            return RedirectToAction("KhoaHoc", new { makh = ch.MaKhoaHoc });
        }

        public ActionResult TaoBaiGiang(string machuong, FormCollection c)
        {
            Chuong ch = data.Chuongs.FirstOrDefault(t => t.MaChuong.ToString() == machuong);

            BaiGiang bg = new BaiGiang();
            bg.TenBaiGiang = c["TenBaiGiang"];
            bg.MaChuong = ch.MaChuong;
            bg.ThuTu = int.Parse(c["ThuTu"]) + 1;
            data.BaiGiangs.InsertOnSubmit(bg);
            data.SubmitChanges();

            return RedirectToAction("KhoaHoc", new { makh = ch.MaKhoaHoc });
        }
    }
}
