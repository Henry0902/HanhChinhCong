using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HanhChinhCong.Models;

namespace HanhChinhCong.Controllers
{
    public class PhongBanController : Controller
    {
        private DbConnectContext context = new DbConnectContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetList()
        {
            var result = context.PhongBan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddPhongBan(PhongBan phongBan)
        {
            context.PhongBan.Add(phongBan);
            context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditPhongBan(PhongBan phongBan)
        {
            var existing = context.PhongBan.FirstOrDefault(x => x.Id == phongBan.Id);
            if (existing != null)
            {
                existing.TenPhongBan = phongBan.TenPhongBan;
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult DeletePhongBan(int id)
        {
            // Kiểm tra có LinhVuc nào tham chiếu không
            var hasLinhVuc = context.LinhVuc.Any(lv => lv.IdPhongBan == id);
            if (hasLinhVuc)
            {
                return Json(new { success = false, message = "Không thể xóa! Phòng ban này đang được sử dụng trong lĩnh vực." });
            }

            var phongBan = context.PhongBan.FirstOrDefault(x => x.Id == id);
            if (phongBan != null)
            {
                context.PhongBan.Remove(phongBan);
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public JsonResult GetPagedPhongBan(string searchName, int page = 1, int pageSize = 5)
        {
            var query = context.PhongBan.AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(x => x.TenPhongBan.Contains(searchName));
            }
            int totalRows = query.Count();
            var data = query
                .OrderBy(x => x.TenPhongBan)
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
    }
}