using System.Linq;
using System.Web.Mvc;
using HanhChinhCong.Models;
using System.Collections.Generic;
using System;


namespace HanhChinhCong.Controllers
{
    public class RoleController : Controller
    {
        private DbConnectContext context = new DbConnectContext();

        public ActionResult Index()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasPagePermission((int)userId, "Role_Index"))
                return new HttpStatusCodeResult(403);

            return View();
        }

        private bool HasPagePermission(int userId, string pageKey)
        {
            using (var db = new DbConnectContext())
            {
                var userRoles = db.UserRole.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToList();
                return db.RolePagePermission.Any(rp => userRoles.Contains(rp.RoleId) && rp.PageKey == pageKey);
            }
        }

        // Danh sách vai trò
        [HttpGet]
        public JsonResult GetList()
        {
            var userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int userId = Convert.ToInt32(userIdObj);
            if (!HasPagePermission(userId, "HoSo_QuanLyHoSo"))
                return Json(new { success = false, message = "Không có quyền truy cập" }, JsonRequestBehavior.AllowGet);
            var result = context.Role.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Thêm vai trò
        [HttpPost]
        public JsonResult AddRole(Role role)
        {
            var userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int userId = Convert.ToInt32(userIdObj);
            if (!HasPagePermission(userId, "HoSo_QuanLyHoSo"))
                return Json(new { success = false, message = "Không có quyền truy cập" }, JsonRequestBehavior.AllowGet);
            context.Role.Add(role);
            context.SaveChanges();
            return Json(new { success = true });
        }

        // Sửa vai trò
        [HttpPost]
        public JsonResult EditRole(Role role)
        {
            var userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int userId = Convert.ToInt32(userIdObj);
            if (!HasPagePermission(userId, "HoSo_QuanLyHoSo"))
                return Json(new { success = false, message = "Không có quyền truy cập" }, JsonRequestBehavior.AllowGet);
            var existingRole = context.Role.FirstOrDefault(x => x.Id == role.Id);
            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // Xóa vai trò
        [HttpPost]
        public JsonResult DeleteRole(int id)
        {
            var userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int userId = Convert.ToInt32(userIdObj);
            if (!HasPagePermission(userId, "HoSo_QuanLyHoSo"))
                return Json(new { success = false, message = "Không có quyền truy cập" }, JsonRequestBehavior.AllowGet);
            var role = context.Role.FirstOrDefault(x => x.Id == id);
            if (role != null)
            {
                context.Role.Remove(role);
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // Phân trang, tìm kiếm vai trò
        [HttpGet]
        public JsonResult GetPagedRoles(string searchName, int page = 1, int pageSize = 5)
        {
            var userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int userId = Convert.ToInt32(userIdObj);
            if (!HasPagePermission(userId, "HoSo_QuanLyHoSo"))
                return Json(new { success = false, message = "Không có quyền truy cập" }, JsonRequestBehavior.AllowGet);
            var query = context.Role.AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(r => r.Name.Contains(searchName));
            }
            int totalRows = query.Count();
            var data = query
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new
            {
                data = data,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PhanQuyenTrang()
        {
            using (var db = new DbConnectContext())
            {
                var roles = db.Role.ToList();
                // Lấy map roleId -> List<PageKey> từ bảng RolePagePermission (tự tạo)
                var map = new Dictionary<int, List<string>>();
                foreach (var item in db.RolePagePermission)
                {
                    if (!map.ContainsKey(item.RoleId)) map[item.RoleId] = new List<string>();
                    map[item.RoleId].Add(item.PageKey);
                }
                ViewBag.RolePageMap = map;
                return View(roles);
            }
        }

        [HttpPost]
        public JsonResult SaveRolePagePermission(List<RolePagePermission> items)
        {
            using (var db = new DbConnectContext())
            {
                db.RolePagePermission.RemoveRange(db.RolePagePermission); // Xóa hết, demo đơn giản
                foreach (var item in items)
                {
                    db.RolePagePermission.Add(new RolePagePermission
                    {
                        RoleId = item.RoleId,
                        PageKey = item.PageKey
                    });
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
        }



    }
}
