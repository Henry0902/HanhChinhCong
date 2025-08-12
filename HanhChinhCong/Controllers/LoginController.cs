using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using HanhChinhCong.Models;
using BCrypt.Net;

namespace HanhChinhCong.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AjaxLogin(string username, string password)
        {
            using (var db = new DbConnectContext())
            {
                
                var user = db.User.FirstOrDefault(u => u.UserName == username);
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PassWord))
                {
                    // Lưu session nếu muốn dùng session cho các request sau
                    Session["UserId"] = user.Id;
                    Session["Username"] = user.UserName;
                    Session["HoTen"] = user.HoTen;
                    Session["UserRole"] = user.Role; // Nếu vẫn muốn lưu role đơn

                    // Lưu danh sách các role của user (nếu dùng nhiều role)
                    var userRoles = db.UserRole.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
                    Session["UserRoles"] = userRoles;

                    return Json(new { success = true, role = user.Role });
                }
                return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng!" });
            }
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}