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
    public class HoSoController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QlHoSo()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetPagedHoSo(string searchName, int page = 1, int pageSize = 5)
        {
            var repo = new HoSoRepository();
            int totalRows;
            var data = repo.SearchHoSoWithPaging(searchName, page, pageSize, out totalRows);

            return Json(new
            {
                data = data,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddHoSo()
        {
            var hoSo = new HoSo
            {
                MaHoSo = Request.Form["MaHoSo"],
                TieuDe = Request.Form["TieuDe"],
                MoTa = Request.Form["MoTa"],
                NgayTiepNhan = ParseDate(Request.Form["NgayTiepNhan"]),
                HanXuLy = ParseDate(Request.Form["HanXuLy"]),
                NgayHoanThanh = null, // Luôn để null khi nộp hồ sơ
                GhiChu = Request.Form["GhiChu"],
                IdCanBoTiepNhan = !string.IsNullOrEmpty(Request.Form["IdCanBoTiepNhan"]) ? (int?)Convert.ToInt32(Request.Form["IdCanBoTiepNhan"]) : null,
                IdPhongBan = !string.IsNullOrEmpty(Request.Form["IdPhongBan"]) ? (int?)Convert.ToInt32(Request.Form["IdPhongBan"]) : null,
                IdLinhVuc = !string.IsNullOrEmpty(Request.Form["IdLinhVuc"]) ? (int?)Convert.ToInt32(Request.Form["IdLinhVuc"]) : null,
                IdLoaiHoSo = !string.IsNullOrEmpty(Request.Form["IdLoaiHoSo"]) ? (int?)Convert.ToInt32(Request.Form["IdLoaiHoSo"]) : null,
                TenCongDan = Request.Form["TenCongDan"],
                SoDienThoai = Request.Form["SoDienThoai"],
                CMND_CCCD = Request.Form["CMND_CCCD"],
                DiaChi = Request.Form["DiaChi"],
                Email = Request.Form["Email"]
            };

            hoSo.IdTrangThai = 1; // Trạng thái mặc định

            var repo = new HoSoRepository();
            int newHoSoId = repo.AddHoSo(hoSo);

            // Xử lý nhiều file
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    var path = Server.MapPath("~/Uploads/HoSo/" + fileName);
                    file.SaveAs(path);
                    repo.AddFileHoSo(newHoSoId, fileName, "/Uploads/HoSo/" + fileName);
                }
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult EditHoSo(HoSo hoSo)
        {
            var repo = new HoSoRepository();
            repo.EditHoSo(hoSo);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteHoSo(int id)
        {
            var repo = new HoSoRepository();
            repo.DeleteHoSo(id);
            return Json(new { success = true });
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTime temp;
            return DateTime.TryParseExact(value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out temp) ? temp : (DateTime?)null;
        }
    }


}