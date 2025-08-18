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

        //Nộp hồ sơ
        public ActionResult Index()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 2)) // 1: Cán bộ tiếp nhận
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult QuanLyHoSo()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 1,2)) // 1: admin
                return new HttpStatusCodeResult(403);

            var isTiepNhan = false;
            using (var db = new DbConnectContext())
            {
                var roles = db.UserRole.Where(ur => ur.UserId == (int)userId).Select(ur => ur.RoleId).ToList();
                isTiepNhan = roles.Contains(2) && !roles.Contains(1); // chỉ là cán bộ tiếp nhận, không phải admin
            }
            ViewBag.IsTiepNhan = isTiepNhan;

            return View();
        }

        public ActionResult PhanCong()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId,4)) // 4: lãnh đạo
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult XuLy()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 3)) // 2: Cán bộ xử lý
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult DuyetKetQua()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 4)) // 4: Lãnh đạo
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult TraKetQua()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 5)) // 5: Cán bộ trả kết quả
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult HoSoSuaDoiBoSung()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            if (!HasRole((int)userId, 2)) // 2: Cán bộ tiếp nhận
                return new HttpStatusCodeResult(403);
            return View();
        }

        public ActionResult TheoDoiHoSo()
        {
            var userId = Session["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Login");

            // Cho phép các role: 2, 3, 4, 5
            if (!HasRole((int)userId, 3, 4, 5))
                return new HttpStatusCodeResult(403);
            return View();
        }


        //Kiểm tra quyền
        private bool HasRole(int userId, params int[] allowedRoleIds)
        {
            using (var db = new DbConnectContext())
            {
                var userRoles = db.UserRole.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToList();
                // Nếu là admin thì luôn có quyền
                if (userRoles.Contains(1)) return true;
                return allowedRoleIds.Any(rid => userRoles.Contains(rid));
            }
        }


        [HttpGet]
        public JsonResult GetPagedHoSo(string searchMaHoSo, string searchName, string searchTenCongDan, string searchCMND_CCCD, bool? searchSapHetHan, int? searchIdTrangThai = null, int page = 1, int pageSize = 5)
        {
            var repo = new HoSoRepository();
            int totalRows;
            var data = repo.SearchHoSoWithPaging(searchMaHoSo, searchName, searchTenCongDan, searchCMND_CCCD, searchSapHetHan, searchIdTrangThai, page, pageSize, out totalRows);
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
            // Lấy userId từ session
            var userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
            var userRoles = new List<int>();

            // Lấy danh sách role của user
            if (userId != null)
            {
                using (var db = new HanhChinhCong.Models.DbConnectContext())
                {
                    userRoles = db.UserRole.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToList();
                }
            }

            // Nếu là cán bộ tiếp nhận thì gán IdCanBoTiepNhan, nếu là công dân thì để null
            int? idCanBoTiepNhan = (userRoles.Contains(1) || userRoles.Contains(2)) ? userId : null;

            var hoSo = new HoSo
            {
                MaHoSo = Request.Form["MaHoSo"],
                TieuDe = Request.Form["TieuDe"],
                MoTa = Request.Form["MoTa"],
                NgayTiepNhan = ParseDate(Request.Form["NgayTiepNhan"]),
                HanXuLy = ParseDate(Request.Form["HanXuLy"]),
                NgayHoanThanh = null, // Luôn để null khi nộp hồ sơ
                GhiChu = Request.Form["GhiChu"],
                IdCanBoTiepNhan = idCanBoTiepNhan,
                IdPhongBan = !string.IsNullOrEmpty(Request.Form["IdPhongBan"]) ? (int?)Convert.ToInt32(Request.Form["IdPhongBan"]) : null,
                IdLinhVuc = !string.IsNullOrEmpty(Request.Form["IdLinhVuc"]) ? (int?)Convert.ToInt32(Request.Form["IdLinhVuc"]) : null,
                IdLoaiHoSo = !string.IsNullOrEmpty(Request.Form["IdLoaiHoSo"]) ? (int?)Convert.ToInt32(Request.Form["IdLoaiHoSo"]) : null,
                TenCongDan = Request.Form["TenCongDan"],
                SoDienThoai = Request.Form["SoDienThoai"],
                CMND_CCCD = Request.Form["CMND_CCCD"],
                DiaChi = Request.Form["DiaChi"],
                Email = Request.Form["Email"]
            };

            hoSo.IdTrangThai = 1; // đã tiếp nhận, chờ phân công

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

        //yêu cầu sửa đổi bổ sung
        [HttpPost]
        public JsonResult YeuCauSuaDoiBoSung(int hoSoId, string ghiChu)
        {
            try
            {
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdTrangThai = 6; // Trạng thái "Yêu cầu sửa đổi, bổ sung"
                                              // Nối ghi chú mới vào ghi chú cũ
                        if (!string.IsNullOrEmpty(hoSo.GhiChu))
                            hoSo.GhiChu += "\n--- Yêu cầu sửa đổi, bổ sung: " + ghiChu;
                        else
                            hoSo.GhiChu = "Yêu cầu sửa đổi, bổ sung: " + ghiChu;
                        context.SaveChanges();
                    }
                }
                // Lưu vào bảng quá trình xử lý (nếu cần)
                var repo = new HoSoRepository();
                int? idNguoiThucHien = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                repo.AddQuaTrinhXuLyHoSo(hoSoId, "Yêu cầu sửa đổi bổ sung", ghiChu, null, idNguoiThucHien);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult XuLyLaiHoSo(int hoSoId, string ghiChu)
        {
            try
            {
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdTrangThai = 2; // Chờ xử lý
                                              // Nối ghi chú mới vào ghi chú cũ
                        if (!string.IsNullOrEmpty(hoSo.GhiChu))
                            hoSo.GhiChu += "\n--- Xử lý lại: " + ghiChu;
                        else
                            hoSo.GhiChu = "Xử lý lại: " + ghiChu;
                        context.SaveChanges();
                    }
                }
                // Lưu vào bảng quá trình xử lý (nếu cần)
                var repo = new HoSoRepository();
                int? idNguoiThucHien = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                repo.AddQuaTrinhXuLyHoSo(hoSoId, "Xử lý lại", ghiChu, null, idNguoiThucHien);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // phân công hồ sơ
        [HttpPost]
        public JsonResult PhanCongHoSo()
        {
            var hoSoId = Convert.ToInt32(Request.Form["hoSoId"]);
            var userId = Convert.ToInt32(Request.Form["userId"]);
            var ghiChu = Request.Form["ghiChu"];
            var files = Request.Files;
            var repo = new HoSoRepository();

            // Lấy Id người xử lý từ session
            var idlanhdao = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;

            bool success = false;
            bool hasFile = false;

            if (files != null && files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        hasFile = true;
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var savePath = Server.MapPath("~/Uploads/PhanCongHoSo/" + fileName);
                        file.SaveAs(savePath);
                        var fileDinhKem = "/Uploads/PhanCongHoSo/" + fileName;
                        success = repo.PhanCongHoSo(hoSoId, userId, ghiChu, fileDinhKem);
                        repo.AddQuaTrinhXuLyHoSo(hoSoId, "Phân công", ghiChu, fileDinhKem, idlanhdao);
                    }
                }
            }

            // Nếu không có file, vẫn ghi nhận quá trình xử lý
            if (!hasFile)
            {
                success = repo.PhanCongHoSo(hoSoId, userId, ghiChu, null);
                repo.AddQuaTrinhXuLyHoSo(hoSoId, "Phân công", ghiChu, null, idlanhdao);
            }

            // Cập nhật trạng thái hồ sơ sang đã phân công (IdTrangThai = 2)
            if (success)
            {
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdCanBoXuLy = userId;
                        hoSo.IdLanhDao = idlanhdao;
                        hoSo.IdTrangThai = 2;//  chờ xử lý
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
                //var userId = Convert.ToInt32(Request.Form["userId"]);
                var ghiChuXuLy = Request.Form["ghiChuXuLy"];
                var files = Request.Files;
                var repo = new HoSoRepository();

                // Lấy Id người xử lý từ session
                var idNguoiXuLy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;

                bool hasFile = false;

                // Lưu file xử lý (nếu có)
                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            hasFile = true;
                            var fileName = System.IO.Path.GetFileName(file.FileName);
                            var path = Server.MapPath("~/Uploads/HoSoXuLy/" + fileName);
                            file.SaveAs(path);
                            var fileXuLy = "/Uploads/HoSoXuLy/" + fileName;
                            repo.AddFileHoSo(hoSoId, fileName, fileXuLy, "XuLy");
                            repo.AddQuaTrinhXuLyHoSo(hoSoId, "Xử lý", ghiChuXuLy, fileXuLy, idNguoiXuLy);
                        }
                    }
                }

                // Nếu không có file, vẫn ghi nhận quá trình xử lý
                if (!hasFile)
                {
                    repo.AddQuaTrinhXuLyHoSo(hoSoId, "Xử lý", ghiChuXuLy, null, idNguoiXuLy);
                }

                // Cập nhật trạng thái hồ sơ sang "Chờ phê duyệt" (IdTrangThai = 3)
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdCanBoXuLy = idNguoiXuLy;
                        hoSo.IdTrangThai = 3; // Chờ phê duyệt
                        context.SaveChanges();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //duyệt kết quả hồ sơ
        [HttpPost]
        public JsonResult DuyetKetQuaHoSo()
        {
            try
            {
                var hoSoId = Convert.ToInt32(Request.Form["hoSoId"]);
                //var userId = Convert.ToInt32(Request.Form["userId"]);
                var ghiChuDuyet = Request.Form["ghiChuDuyet"];
                var files = Request.Files;
                var repo = new HoSoRepository();
                var idNguoiDuyet = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                bool hasFile = false;

                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            hasFile = true;
                            var fileName = System.IO.Path.GetFileName(file.FileName);
                            var path = Server.MapPath("~/Uploads/DuyetHoSo/" + fileName);
                            file.SaveAs(path);
                            var fileDuyet = "/Uploads/DuyetHoSo/" + fileName;
                            repo.AddFileHoSo(hoSoId, fileName, fileDuyet, "XuLy");
                            repo.AddQuaTrinhXuLyHoSo(hoSoId, "Duyệt hồ sơ", ghiChuDuyet, fileDuyet, idNguoiDuyet);
                        }
                    }
                }

                // Nếu không có file, vẫn ghi nhận quá trình xử lý
                if (!hasFile)
                {
                    repo.AddQuaTrinhXuLyHoSo(hoSoId, "Duyệt hồ sơ", ghiChuDuyet, null, idNguoiDuyet);
                }

                //repo.DuyetKetQuaHoSo(hoSoId, ghiChuDuyet);

                // Cập nhật trạng thái hồ sơ sang "Chờ trả kết quả" (IdTrangThai = 4)
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdLanhDao = idNguoiDuyet;
                        hoSo.IdTrangThai = 4; // Chờ trả kết quả
                        context.SaveChanges();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // trả kết quả hồ sơ
        [HttpPost]
        public JsonResult TraKetQuaHoSo()
        {
            try
            {
                var hoSoId = Convert.ToInt32(Request.Form["hoSoId"]);
                
                var ghiChuTraKetQua = Request.Form["ghiChuTraKetQua"];
                var files = Request.Files;
                var repo = new HoSoRepository();
                var idNguoiTraKetQua = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                bool hasFile = false;

                if (files != null && files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            hasFile = true;
                            var fileName = System.IO.Path.GetFileName(file.FileName);
                            var path = Server.MapPath("~/Uploads/TraKetQuaHoSo/" + fileName);
                            file.SaveAs(path);
                            var fileTraKetQua = "/Uploads/TraKetQuaHoSo/" + fileName;
                            repo.AddFileHoSo(hoSoId, fileName, fileTraKetQua, "TraKetQua");
                            repo.AddQuaTrinhXuLyHoSo(hoSoId, "Trả kết quả", ghiChuTraKetQua, fileTraKetQua, idNguoiTraKetQua);
                        }
                    }
                }

                // Nếu không có file, vẫn ghi nhận quá trình xử lý
                if (!hasFile)
                {
                    repo.AddQuaTrinhXuLyHoSo(hoSoId, "Trả kết quả", ghiChuTraKetQua, null, idNguoiTraKetQua);
                }

                //repo.TraKetQuaHoSo(hoSoId, ghiChuTraKetQua);

                // Cập nhật trạng thái hồ sơ sang "trả kết quả" (IdTrangThai = 5)
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdCanBoTraKetQua = idNguoiTraKetQua;
                        hoSo.IdTrangThai = 5; // trả kết quả
                        context.SaveChanges();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //lấy hồ sơ phân công theo cán bộ xử lý
        [HttpGet]
        public JsonResult GetHoSoPhanCongXuLy(string searchMaHoSo = "", string searchName = "", string searchTenCongDan = "", string searchCMND_CCCD = "", bool? searchSapHetHan = null, int? searchIdTrangThai = null, int page = 1, int pageSize = 5)
        {
            var userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
            if (userId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            bool isAdmin = false;
            using (var db = new DbConnectContext())
            {
                isAdmin = db.UserRole.Any(ur => ur.UserId == userId && ur.RoleId == 1);
            }

            int totalRows;
            var repo = new HoSoRepository();
            var data = repo.GetHoSoPhanCongXuLy( isAdmin ? (int?)null : userId, searchMaHoSo, searchName, searchTenCongDan,  searchCMND_CCCD, searchSapHetHan,  searchIdTrangThai,   page,  pageSize,   out totalRows  );

            return Json(new
            {
                data = data,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }




        // Lấy quá trình xử lý của hồ sơ
        [HttpGet]
        public JsonResult GetQuaTrinhXuLyByHoSoId(int hoSoId)
        {
            var repo = new HoSoRepository();
            var list = repo.GetQuaTrinhXuLyByHoSoId(hoSoId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // Lấy danh sách trạng thái hồ sơ
        [HttpGet]
        public JsonResult GetTrangThaiHoSo()
        {
            var repo = new HoSoRepository();
            var list = repo.GetTrangThaiHoSo();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // Sinh mã hồ sơ theo loại hồ sơ
        [HttpGet]
        public JsonResult GenerateMaHoSo(int idLoaiHoSo)
        {
            // Ví dụ: sinh mã theo loại hồ sơ, có thể thêm logic theo ngày/tháng/năm hoặc số thứ tự
            using (var db = new DbConnectContext())
            {
                var loaiHoSo = db.LoaiHoSo.FirstOrDefault(lh => lh.Id == idLoaiHoSo);
                if (loaiHoSo == null)
                    return Json(new { maHoSo = "" }, JsonRequestBehavior.AllowGet);

                // Đếm số hồ sơ đã có của loại này
                int count = db.HoSo.Count(h => h.IdLoaiHoSo == idLoaiHoSo) + 1;

                // Lấy ngày hiện tại theo định dạng yyyyMMdd
                string datePart = DateTime.Now.ToString("yyyyMMdd");

                // Ví dụ mã: [MaLoaiHoSo]-[yyyyMMdd]-[SốThứTự]
                string maHoSo = $"{loaiHoSo.MaLoaiHoSo ?? "LH"}-{datePart}-{count:D4}";
                return Json(new { maHoSo = maHoSo }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetHoSoDaXuLyByUser(string searchMaHoSo = "", string searchName = "", string searchTenCongDan = "", string searchCMND_CCCD = "", bool? searchSapHetHan = null, int? searchIdTrangThai = null, int page = 1,   int pageSize = 5)
        {
            var userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
            if (userId == null)
                return Json(new { success = false, message = "Chưa đăng nhập" }, JsonRequestBehavior.AllowGet);

            int totalRows;
            var repo = new HoSoRepository();
            var data = repo.GetHoSoDaXuLyByUser(userId.Value, searchMaHoSo,    searchName,    searchTenCongDan,  searchCMND_CCCD, searchSapHetHan, searchIdTrangThai,    page,   pageSize,  out totalRows
            );

            return Json(new
            {
                data = data,
                totalRows = totalRows,
                page = page,
                pageSize = pageSize
            }, JsonRequestBehavior.AllowGet);
        }

        //Rút hồ sơ
        [HttpPost]
        public JsonResult RutHoSo(int hoSoId, string ghiChu)
        {
            try
            {
                using (var context = new DbConnectContext())
                {
                    var hoSo = context.HoSo.FirstOrDefault(h => h.Id == hoSoId);
                    if (hoSo != null)
                    {
                        hoSo.IdTrangThai = 9; // 9: Đã rút hồ sơ
                        if (!string.IsNullOrEmpty(hoSo.GhiChu))
                            hoSo.GhiChu += "\n--- Rút hồ sơ: " + ghiChu;
                        else
                            hoSo.GhiChu = "Rút hồ sơ: " + ghiChu;
                        context.SaveChanges();
                    }
                }
                // Lưu vào bảng quá trình xử lý (nếu cần)
                var repo = new HoSoRepository();
                int? idNguoiThucHien = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                repo.AddQuaTrinhXuLyHoSo(hoSoId, "Rút hồ sơ", ghiChu, null, idNguoiThucHien);

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