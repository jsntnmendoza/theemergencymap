using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CoronaV2.Models;

namespace CoronaV2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            using (var dc = new CoronaEntities())
            {
                var user = 1;
                if (System.Web.HttpContext.Current.User.Identity.Name.ToString() != string.Empty)
                {
                    user = dc.User.Where(x => x.EmailID == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserID;
                }
                return View(dc.Register.Where(x => x.UserID ==user).ToList());
            }
        }

        public ActionResult Edit(int RegisterID)
        {
            using (var dc = new CoronaEntities())
            {
                //Register reg = dc.Register.Where(x => x.RegisterID == RegisterID).FirstOrDefault();
                Register reg = dc.Register.Find(RegisterID);
                return View(reg);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Register a)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View();
                //}
                using (var dc=new CoronaEntities())
                {
                    Register reg = dc.Register.Find(a.RegisterID);
                    reg.Message = a.Message;
                    reg.Address = a.Address;
                    dc.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult Delete(int RegisterID)
        {
            using (var dc=new CoronaEntities())
            {
                Register reg = dc.Register.Find(RegisterID);
                dc.Register.Remove(reg);
                dc.SaveChanges();
                return RedirectToAction("Index");
            }
        }


        
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Donations()
        {
            using (var dc = new CoronaEntities())
            {
                var FirstName = "Usuario";
                if (System.Web.HttpContext.Current.User.Identity.Name.ToString() != string.Empty)
                {
                    FirstName = dc.User.Where(x => x.EmailID == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;
                }
                ViewBag.FirstName = FirstName;
                return View();
            }
        }

        public ActionResult Language(string language)
        {
            if (!String.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);            
                HttpCookie cookie = new HttpCookie("Languages");
                cookie.Value = language;
                Response.Cookies.Add(cookie);
            }
            return View();
        }

    }
}