using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LopHocTrucTuyen.Models;

namespace LopHocTrucTuyen.Controllers
{
    public class HocVienController : Controller
    {
        //
        // GET: /HocVien/
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
    }
}
