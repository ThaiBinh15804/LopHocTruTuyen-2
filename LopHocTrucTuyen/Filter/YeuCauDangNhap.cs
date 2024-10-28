using System.Web;
using System.Web.Mvc;

namespace LopHocTrucTuyen.Filter
{
    public class YeuCauDangNhap : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (HttpContext.Current.Session["User"] == null)
            {
                // Lấy tên controller và action hiện tại
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;

                // Nếu không phải là trang DangNhap, thì chuyển hướng đến trang đăng nhập
                if (!(controllerName == "HocVien" && actionName == "DangNhap"))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "HocVien" },
                            { "action", "DangNhap" }
                        });
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
