using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.BLL;
using WebApi.Model;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        private IlogininfoServece logininfoService = BLL.Container.Resolve<IlogininfoServece>();

        public ActionResult LoginIndex()
        {
            List<logininfo> list = logininfoService.GetModels(p => true).ToList();
            return View(list);
        }

        public ActionResult Add(logininfo logininfo)
        {
            if(logininfoService.Add(logininfo))
            {
                return Redirect("LoginIndex");
            }
            else
            {
                return Content("No");
            }
        }

        public ActionResult Update(logininfo logininfo)
        {
            if(logininfoService.Update(logininfo))
            {
                return Redirect("LoginIndex");
            }
            else
            {
                return Content("NO");
            }
        }

        public ActionResult Delete(logininfo logininfo)
        {
            var user = logininfoService.GetModels(p => p.username == logininfo.username&&p.userpwd == logininfo.userpwd).FirstOrDefault();
            if(user==null)
            {
                return Content("username or userpassword error");
            }
            if(logininfoService.Delete(user))
            {
                return Redirect("LoginIndex");
            }
            else
            {
                return Content("no");
            }
        }
    }
}
