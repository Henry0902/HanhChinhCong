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
    public class LoaiHoSoController : Controller
    {
        private DbConnectContext context = new DbConnectContext();

        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetList()
        {
            var result = context.LoaiHoSo.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddLoaiHoSo(LoaiHoSo loaiHoSo)
        {
            context.LoaiHoSo.Add(loaiHoSo);
            context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditLoaiHoSo(LoaiHoSo loaiHoSo)
        {
            var existing = context.LoaiHoSo.FirstOrDefault(x => x.Id == loaiHoSo.Id);
            if (existing != null)
            {
                existing.TenLoaiHoSo = loaiHoSo.TenLoaiHoSo;
                existing.IdLinhVuc = loaiHoSo.IdLinhVuc;
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult DeleteLoaiHoSo(int id)
        {
            var loaiHoSo = context.LoaiHoSo.FirstOrDefault(x => x.Id == id);
            if (loaiHoSo != null)
            {
                context.LoaiHoSo.Remove(loaiHoSo);
                context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public JsonResult GetPagedLoaiHoSo(string searchName, int page = 1, int pageSize = 5)
        {
            var repo = new LoaiHoSoRepository();
            int totalRows;
            var data = repo.SearchLoaiHoSoWithPaging(searchName, page, pageSize, out totalRows);

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