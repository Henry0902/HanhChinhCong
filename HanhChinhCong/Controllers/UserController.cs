using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using HanhChinhCong.Models;
using System.Configuration;
using System.Data;

namespace HanhChinhCong.Controllers
{
    public class UserController : Controller
    {
        private DbConnectContext context = null;
        public ActionResult Index()
        {

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

        [HttpPost]
        public JsonResult AddUser(User user)
        {
            // Lưu user vào database (Entity Framework hoặc ADO.NET)
            // Ví dụ:
            context = new DbConnectContext();
            context.User.Add(user);
            context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditUser(User user)
        {
            context = new DbConnectContext();
            var existingUser = context.User.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.HoTen = user.HoTen;
                existingUser.VaiTro = user.VaiTro;
                existingUser.UserName = user.UserName;
                existingUser.PassWord = user.PassWord;
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public JsonResult GetPagedUsers(string searchName, int page = 1, int pageSize = 5)
        {
            var repo = new UserRepository();
            int totalRows;
            var users = repo.SearchUsersWithPaging(searchName, page, pageSize, out totalRows);

            return Json(new
            {
                data = users,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }





    }
}