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
    public class LinhVucController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetList()
        {
            using (var context = new DbConnectContext())
            {
                var result = context.LinhVuc.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpGet]
        public JsonResult GetPagedLinhVuc(string searchName, int page = 1, int pageSize = 5)
        {
            var repo = new LinhVucRepository();
            int totalRows;
            var linhVucs = repo.SearchLinhVucWithPaging(searchName, page, pageSize, out totalRows);

            return Json(new
            {
                data = linhVucs,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLinhVucByPhongBan(int idPhongBan)
        {
            using (var context = new DbConnectContext())
            {
                var linhVucs = context.LinhVuc.Where(x => x.IdPhongBan == idPhongBan).ToList();
                return Json(linhVucs, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddLinhVuc(LinhVuc linhVuc)
        {
            using (var context = new DbConnectContext())
            {
                context.LinhVuc.Add(linhVuc);
                context.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditLinhVuc(LinhVuc linhVuc)
        {
            using (var context = new DbConnectContext())
            {
                var existing = context.LinhVuc.Find(linhVuc.Id);
                if (existing != null)
                {
                    existing.TenLinhVuc = linhVuc.TenLinhVuc;
                    existing.IdPhongBan = linhVuc.IdPhongBan;
                    context.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult DeleteLinhVuc(int id)
        {
            using (var context = new DbConnectContext())
            {
                var linhVuc = context.LinhVuc.Find(id);
                if (linhVuc != null)
                {
                    context.LinhVuc.Remove(linhVuc);
                    context.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
    }


}
