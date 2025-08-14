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
using System.Data.Entity;

namespace HanhChinhCong.Controllers
{
    public class UserController : Controller
    {
        private DbConnectContext context = null;
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult PhanQuyen()
        {
         
            return View();
        }


        [HttpGet]
        public ActionResult GetList()
        {
            context = new DbConnectContext();
            var result = context.User.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllCanBoXuLy()
        {
            var repo = new UserRepository();
            var userInfo = repo.GetAllCanBoXuLy(3); // 3 = Cán bộ xử lý
            return Json(userInfo, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddUser(User user)
        {
            // Lưu user vào database (Entity Framework hoặc ADO.NET)
            // Ví dụ:
            // Kiểm tra trùng username
            using (var db = new DbConnectContext())
            {
                if (db.User.Any(u => u.UserName == user.UserName))
            {
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
            }
            context = new DbConnectContext();
            user.PassWord = BCrypt.Net.BCrypt.HashPassword(user.PassWord);
            context.User.Add(user);
            context.SaveChanges();
                // Gán role cho user (bảng UserRole)
                if (user.Role > 0)
                {
                    db.UserRole.Add(new UserRole { UserId = user.Id, RoleId = user.Role });
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult EditUser(User user)
        {
            context = new DbConnectContext();
            var existingUser = context.User.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.HoTen = user.HoTen;
                existingUser.Role = user.Role;
                //existingUser.UserName = user.UserName;
                //existingUser.PassWord = user.PassWord;
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var db = new DbConnectContext())
            {
                var user = db.User.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    db.User.Remove(user);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });
            }
        }


        [HttpGet]
        public JsonResult GetPagedUsers(string searchName, int? searchRole, int page = 1, int pageSize = 5)
        {
            var repo = new UserRepository();
            int totalRows;
            var users = repo.SearchUsersWithPaging(searchName, searchRole, page, pageSize, out totalRows);

            return Json(new
            {
                data = users,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetAllUsers()
        {
         
            using (var db = new DbConnectContext())
            {
                var users = db.User.Select(u => new { u.Id, u.HoTen, u.UserName }).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAllRoles()
        {
         
            using (var db = new DbConnectContext())
            {
                var roles = db.Role.Select(r => new { r.Id, r.Name }).ToList();
                return Json(roles, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetUserRoles(int userId)
        {
           
            using (var db = new DbConnectContext())
            {
                var roleIds = db.UserRole.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToList();
                return Json(roleIds, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateUserRoles(int userId, List<int> roleIds)
        {
          
            using (var db = new DbConnectContext())
            {
                var existing = db.UserRole.Where(ur => ur.UserId == userId).ToList();
                db.UserRole.RemoveRange(existing);
                foreach (var roleId in roleIds)
                {
                    db.UserRole.Add(new UserRole { UserId = userId, RoleId = roleId });
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
        }

        //đăng ký người dùng mới là công dân
        [HttpPost]
        public JsonResult Register(User user)
        {
            using (var db = new DbConnectContext())
            {
                // Kiểm tra trùng username
                if (db.User.Any(u => u.UserName == user.UserName))
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
                }

                // Mã hóa mật khẩu bằng BCrypt
                user.PassWord = BCrypt.Net.BCrypt.HashPassword(user.PassWord);

                // Gán role mặc định ( 6 : công dân)
                user.Role = 6;

                db.User.Add(user);
                db.SaveChanges();
                // Thêm vào bảng UserRole nếu dùng phân quyền động
                db.UserRole.Add(new UserRole { UserId = user.Id, RoleId = 6 });
                db.SaveChanges();

                return Json(new { success = true });
            }
        }



    }
}