using CoronaV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace CoronaV2.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        [HttpGet]
        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Search()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                if (model.ImageFile!=null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    model.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"),fileName);
                    model.ImageFile.SaveAs(fileName);
                }
                using (var dc = new CoronaEntities())
                {
                    var user = 1;
                    if (System.Web.HttpContext.Current.User.Identity.Name.ToString() != string.Empty)
                    {
                        user = dc.User.Where(x => x.EmailID == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserID;
                    }
                    model.UserID = user;
                    model.CrtdDate = DateTime.Today;
                    model.Active = true;
                    dc.Register.Add(model);
                    dc.SaveChanges();
                    TempData["Message"] = "Success";
                    return RedirectToAction("Search");                    
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error al agregar el registro", ex);
                return View();
            }

        }



        #region ListaMarcadores
        public ActionResult Markers()
        {
            using (var dc = new CoronaEntities())
            {
                return PartialView(dc.Marker.ToList());
            }
        }
        #endregion

        [HttpPost]
        public JsonResult SearchByCity(int CityID)
        {
            using (var dc = new CoronaEntities())
            {
                var City = dc.City
                  .Where(x => x.CityID == CityID)
                  .Select(a => new
                  {
                      Lat = a.Lat,
                      Lon = a.Lon
                  }).ToList();
                return Json(City);
            }
        }
        [HttpPost]
        public JsonResult RegisterByCity(int CityID)
        {
            using (var dc = new CoronaEntities())
            {
                var City = dc.V_RegByCity
                  .Where(x => x.CityID == CityID)
                  .Select(a => new
                  {
                      RegisterID = a.RegisterID,
                      MarkerID = a.MarkerID,
                      Latlng = a.Latlng,
                      Message = a.Message,
                      Url = a.Url,
                      Lat=a.Lat,
                      Lon=a.Lon,
                      Address=a.Address,
                      FirstName=a.FirstName,
                      ImageHtml = a.ImageHtml

                  }).ToList();
                return Json(City);
            }
        }
        [HttpPost]
        public JsonResult RegisterByCityMarker(int CityID, int MarkerID=0)
        {
            using (var dc = new CoronaEntities())
            {
                if (MarkerID!=0)
                {
                    var City = dc.V_RegByCity
                  .Where(x => x.CityID == CityID)
                  .Where(y => y.MarkerID== MarkerID)
                    .Select(a => new
                  {
                      RegisterID = a.RegisterID,
                      MarkerID = a.MarkerID,
                      Latlng = a.Latlng,
                      Message = a.Message,
                      Url = a.Url,
                      Lat = a.Lat,
                      Lon = a.Lon,
                      Address = a.Address,
                      FirstName = a.FirstName,
                      ImageHtml = a.ImageHtml,
                      Contact = a.Contact

                    }).ToList();
                    return Json(City);
                }
                else
                {
                    var City = dc.V_RegByCity
                  .Where(x => x.CityID == CityID)
                  .Select(a => new
                  {
                      RegisterID = a.RegisterID,
                      MarkerID = a.MarkerID,
                      Latlng = a.Latlng,
                      Message = a.Message,
                      Url = a.Url,
                      Lat = a.Lat,
                      Lon = a.Lon,
                      Address = a.Address,
                      FirstName = a.FirstName,
                      ImageHtml = a.ImageHtml,
                      Contact = a.Contact

                  }).ToList();
                    return Json(City);
                }
            }
        }
    }
}