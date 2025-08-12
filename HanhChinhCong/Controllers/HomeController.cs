using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using HanhChinhCong.Models;

namespace HanhChinhCong.Controllers
{
    public class HomeController : Controller
    {
        private DbConnectContext context = null;
        public ActionResult Index()
        {
            using (var db = new DbConnectContext())
            {
                ViewBag.AllRoles = db.Role.Select(r => new { r.Id, r.Name }).ToList();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult GetList()
        {
            context = new DbConnectContext();
            var result = context.User.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
    }
}