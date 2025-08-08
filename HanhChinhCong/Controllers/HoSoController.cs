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
using System.Net;
using System.Web.Helpers;

namespace HanhChinhCong.Controllers
{
    public class HoSoController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuanLyHoSo()
        {
            return View();
        }

        public ActionResult PhanCong()
        {
            return View();
        }

        public ActionResult XuLy()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetPagedHoSo(string searchName, string searchTenCongDan, string searchCMND_CCCD, int page = 1, int pageSize = 5)
        {
            var repo = new HoSoRepository();
            int totalRows;
            var data = repo.SearchHoSoWithPaging(searchName, searchTenCongDan, searchCMND_CCCD, page, pageSize, out totalRows);

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

        // Lấy danh sách file đính kèm
        [HttpGet]
        public JsonResult GetFilesByHoSoId(int hoSoId)
        {
            try
            {
                var repo = new HoSoRepository();
                var files = repo.GetFilesByHoSoId(hoSoId);
                return Json(files, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        // Sửa hồ sơ (có xử lý file)
        [HttpPost]
        public JsonResult EditHoSo()
        {
            try {

                var hoSo = new HoSo
                {
                    Id = ParseNullableInt(Request.Form["Id"]) ?? 0,
                    MaHoSo = Request.Form["MaHoSo"],
                    TieuDe = Request.Form["TieuDe"],
                    MoTa = Request.Form["MoTa"],
                    NgayTiepNhan = ParseDate(Request.Form["NgayTiepNhan"]),
                    HanXuLy = ParseDate(Request.Form["HanXuLy"]),
                    NgayHoanThanh = null,
                    GhiChu = Request.Form["GhiChu"],
                    IdCanBoTiepNhan = ParseNullableInt(Request.Form["IdCanBoTiepNhan"]),
                    IdPhongBan = ParseNullableInt(Request.Form["IdPhongBan"]),
                    IdLinhVuc = ParseNullableInt(Request.Form["IdLinhVuc"]),
                    IdLoaiHoSo = ParseNullableInt(Request.Form["IdLoaiHoSo"]),
                    IdTrangThai = ParseNullableInt(Request.Form["IdTrangThai"]),
                    TenCongDan = Request.Form["TenCongDan"],
                    SoDienThoai = Request.Form["SoDienThoai"],
                    CMND_CCCD = Request.Form["CMND_CCCD"],
                    DiaChi = Request.Form["DiaChi"],
                    Email = Request.Form["Email"]
                };

                hoSo.IdTrangThai = 1; // Trạng thái mặc định
                var repo = new HoSoRepository();
                repo.EditHoSo(hoSo);

                // Xử lý xóa file đính kèm
                var filesToDeleteJson = Request.Form["FilesToDelete"];
                if (!string.IsNullOrEmpty(filesToDeleteJson))
                {
                    var filesToDelete = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(filesToDeleteJson);
                    foreach (var tenFile in filesToDelete)
                    {
                        repo.DeleteFileHoSo(hoSo.Id, tenFile);

                        // Xóa file vật lý trên server nếu cần
                        var filePath = Server.MapPath("~/Uploads/HoSo/" + tenFile);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                // Xử lý thêm file mới
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var path = Server.MapPath("~/Uploads/HoSo/" + fileName);
                        file.SaveAs(path);
                        repo.AddFileHoSo(hoSo.Id, fileName, "/Uploads/HoSo/" + fileName);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult PhanCongHoSo()
        {
            var hoSoId = Convert.ToInt32(Request.Form["hoSoId"]);
            var userId = Convert.ToInt32(Request.Form["userId"]);
            var ghiChu = Request.Form["ghiChu"];
            // Xử lý file
            var files = Request.Files;
            var repo = new HoSoRepository();
            bool success = false;

            // Nếu không có file thì vẫn lưu một bản ghi với FileDinhKem = null
            if (files == null || files.Count == 0)
            {
                success = repo.PhanCongHoSo(hoSoId, userId, ghiChu, null);
            }
            else
            {
                // Lưu mỗi file một bản ghi
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var savePath = Server.MapPath("~/Uploads/PhanCongHoSo/" + fileName);
                        file.SaveAs(savePath);
                        var fileDinhKem = "/Uploads/PhanCongHoSo/" + fileName;
                        success = repo.PhanCongHoSo(hoSoId, userId, ghiChu, fileDinhKem);
                    }
                }
            }

            // Cập nhật trạng thái hồ sơ sang đã phân công (IdTrangThai = 3)
            if (success)
            {
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdCanBoXuLy = userId;
                        hoSo.IdTrangThai = 3;// đã phân công
                        context.SaveChanges();
                    }
                }
            }
            return Json(new { Success = success });
        }

        //Xử lý hồ sơ
        [HttpPost]
        public JsonResult XacNhanXuLyHoSo()
        {
            try
            {
                var hoSoId = Convert.ToInt32(Request.Form["hoSoId"]);
                var ghiChuXuLy = Request.Form["ghiChuXuLy"];
                var files = Request.Files;
                var repo = new HoSoRepository();

                // Cập nhật trạng thái và ghi chú xử lý
                repo.XacNhanXuLyHoSo(hoSoId, ghiChuXuLy);

                // Lưu file xử lý (nếu có)
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var path = Server.MapPath("~/Uploads/HoSoXuLy/" + fileName);
                        file.SaveAs(path);
                        repo.AddFileHoSo(hoSoId, fileName, "/Uploads/HoSoXuLy/" + fileName, "XuLy");

                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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

        private int? ParseNullableInt(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "null")
                return null;
            int result;
            return int.TryParse(value, out result) ? (int?)result : null;
        }




    }


}