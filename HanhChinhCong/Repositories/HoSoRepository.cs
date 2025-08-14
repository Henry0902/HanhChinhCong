using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HanhChinhCong.Models;

public class HoSoRepository
{
    private readonly string connectionString;

    public HoSoRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DbConnectContext"].ConnectionString;
    }

    public List<HoSoInfo> SearchHoSoWithPaging(string searchName, string searchTenCongDan, string searchCMND_CCCD, int? searchIdTrangThai, int page, int pageSize, out int totalRows)
    {
        var list = new List<HoSoInfo>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchHoSoWithPaging", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@SearchTenCongDan", searchTenCongDan ?? "");
            cmd.Parameters.AddWithValue("@SearchCMND_CCCD", searchCMND_CCCD ?? "");
            cmd.Parameters.AddWithValue("@SearchIdTrangThai", searchIdTrangThai ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new HoSoInfo
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MaHoSo = reader["MaHoSo"].ToString(),
                        TieuDe = reader["TieuDe"].ToString(),
                        NgayTiepNhan = reader["NgayTiepNhan"] as DateTime?,
                        TenCongDan = reader["TenCongDan"].ToString(),
                        SoDienThoai = reader["SoDienThoai"].ToString(),
                        CMND_CCCD = reader["CMND_CCCD"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        GhiChu = reader["GhiChu"].ToString(),
                        MoTa = reader["MoTa"].ToString(),
                        HanXuLy = reader["HanXuLy"] as DateTime?,
                        IdPhongBan = reader["IdPhongBan"] as int?,
                        TenPhongBan = reader["TenPhongBan"].ToString(),
                        IdTrangThai = reader["IdTrangThai"] as int?,
                        IdLinhVuc = reader["IdLinhVuc"] as int?,
                        TenLinhVuc = reader["TenLinhVuc"].ToString(),
                        IdLoaiHoSo = reader["IdLoaiHoSo"] as int?,
                        TenLoaiHoSo = reader["TenLoaiHoSo"].ToString(),
                        IdCanBoTiepNhan = reader["IdCanBoTiepNhan"] as int?,
                        TenCanBoTiepNhan = reader["TenCanBoTiepNhan"].ToString(),
                        IdCanBoXuLy = reader["IdCanBoXuLy"] as int?,
                        TenCanBoXuLy = reader["TenCanBoXuLy"].ToString(),
                    });
                }
                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }
        return list;
    }

   

    public int AddHoSo(HoSo hoSo)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_AddHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MaHoSo", hoSo.MaHoSo ?? "");
            cmd.Parameters.AddWithValue("@TieuDe", hoSo.TieuDe ?? "");
            cmd.Parameters.AddWithValue("@MoTa", hoSo.MoTa ?? "");
            cmd.Parameters.AddWithValue("@NgayTiepNhan", hoSo.NgayTiepNhan ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@HanXuLy", hoSo.HanXuLy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", hoSo.GhiChu ?? "");
            cmd.Parameters.AddWithValue("@IdCanBoTiepNhan", hoSo.IdCanBoTiepNhan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdPhongBan", hoSo.IdPhongBan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLinhVuc", hoSo.IdLinhVuc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLoaiHoSo", hoSo.IdLoaiHoSo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdTrangThai", hoSo.IdTrangThai ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TenCongDan", hoSo.TenCongDan ?? "");
            cmd.Parameters.AddWithValue("@SoDienThoai", hoSo.SoDienThoai ?? "");
            cmd.Parameters.AddWithValue("@CMND_CCCD", hoSo.CMND_CCCD ?? "");
            cmd.Parameters.AddWithValue("@DiaChi", hoSo.DiaChi ?? "");
            cmd.Parameters.AddWithValue("@Email", hoSo.Email ?? "");
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }


    public void EditHoSo(HoSo hoSo)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_EditHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", hoSo.Id);
            cmd.Parameters.AddWithValue("@MaHoSo", hoSo.MaHoSo ?? "");
            cmd.Parameters.AddWithValue("@TieuDe", hoSo.TieuDe ?? "");
            cmd.Parameters.AddWithValue("@MoTa", hoSo.MoTa ?? "");
            cmd.Parameters.AddWithValue("@NgayTiepNhan", hoSo.NgayTiepNhan ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@HanXuLy", hoSo.HanXuLy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", hoSo.GhiChu ?? "");
            cmd.Parameters.AddWithValue("@IdCanBoTiepNhan", hoSo.IdCanBoTiepNhan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdPhongBan", hoSo.IdPhongBan ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLinhVuc", hoSo.IdLinhVuc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdLoaiHoSo", hoSo.IdLoaiHoSo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdTrangThai", hoSo.IdTrangThai ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TenCongDan", hoSo.TenCongDan ?? "");
            cmd.Parameters.AddWithValue("@SoDienThoai", hoSo.SoDienThoai ?? "");
            cmd.Parameters.AddWithValue("@CMND_CCCD", hoSo.CMND_CCCD ?? "");
            cmd.Parameters.AddWithValue("@DiaChi", hoSo.DiaChi ?? "");
            cmd.Parameters.AddWithValue("@Email", hoSo.Email ?? "");
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void AddQuaTrinhXuLyHoSo(int idHoSo, string buoc, string ghiChu, string fileDinhKem, int? idNguoiThucHien)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_AddQuaTrinhXuLyHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", idHoSo);
            cmd.Parameters.AddWithValue("@Buoc", buoc);
            cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? "");
            cmd.Parameters.AddWithValue("@FileDinhKem", fileDinhKem ?? "");
            cmd.Parameters.AddWithValue("@NgayThucHien", DateTime.Now);
            cmd.Parameters.AddWithValue("@IdNguoiThucHien", idNguoiThucHien ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }


    public bool PhanCongHoSo(int idHoSo, int idCanBoXuLy, string ghiChu, string fileDinhKem = null)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_PhanCongHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", idHoSo);
            cmd.Parameters.AddWithValue("@IdCanBoXuLy", idCanBoXuLy);
            cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? "");
            cmd.Parameters.AddWithValue("@FileDinhKem", fileDinhKem ?? "");
            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void XacNhanXuLyHoSo(int hoSoId, string ghiChuXuLy)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_XacNhanXuLyHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", hoSoId);
            cmd.Parameters.AddWithValue("@GhiChuXuLy", ghiChuXuLy ?? "");
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void DuyetKetQuaHoSo(int hoSoId, string ghiChuDuyet)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_DuyetKetQuaHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", hoSoId);
            cmd.Parameters.AddWithValue("@GhiChuDuyet", ghiChuDuyet ?? "");
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }



    public void TraKetQuaHoSo(int hoSoId, string ghiChuTraKetQua)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_TraKetQuaHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", hoSoId);
            cmd.Parameters.AddWithValue("@GhiChuTraKetQua", ghiChuTraKetQua ?? "");
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public List<QuaTrinhXuLyHoSoInfo> GetQuaTrinhXuLyByHoSoId(int hoSoId)
    {
        var list = new List<QuaTrinhXuLyHoSoInfo>();
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetQuaTrinhXuLyByHoSoId", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", hoSoId);
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new QuaTrinhXuLyHoSoInfo
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdHoSo = Convert.ToInt32(reader["IdHoSo"]),
                    Buoc = reader["Buoc"].ToString(),
                    GhiChu = reader["GhiChu"].ToString(),
                    FileDinhKem = reader["FileDinhKem"].ToString(),
                    NgayThucHien = Convert.ToDateTime(reader["NgayThucHien"]),
                    IdNguoiThucHien = reader["IdNguoiThucHien"] as int?,
                    TenNguoiThucHien = reader["TenNguoiThucHien"].ToString()
                });
            }
        }
        return list;
    }

    public List<TrangThaiHoSoInfo> GetTrangThaiHoSo()
    {
        var list = new List<TrangThaiHoSoInfo>();
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetTrangThaiHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TrangThaiHoSoInfo
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    TenTrangThai = reader["TenTrangThai"].ToString()
                });
            }
        }
        return list;
    }


    public List<HoSoInfo> GetHoSoDaXuLyByUser( int userId,string searchMaHoSo, string searchName,  string searchTenCongDan, string searchCMND_CCCD,  int? searchIdTrangThai,  int page, int pageSize,  out int totalRows)
    {
        var result = new List<HoSoInfo>();
        totalRows = 0;
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetHoSoDaXuLyByUser", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@SearchTenCongDan", searchTenCongDan ?? "");
            cmd.Parameters.AddWithValue("@SearchCMND_CCCD", searchCMND_CCCD ?? "");
            cmd.Parameters.AddWithValue("@SearchSapHetHan", (object)searchSapHetHan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SearchIdTrangThai", searchIdTrangThai ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new HoSoInfo
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MaHoSo = reader["MaHoSo"].ToString(),
                        TieuDe = reader["TieuDe"].ToString(),
                        NgayTiepNhan = reader["NgayTiepNhan"] != DBNull.Value ? (DateTime?)reader["NgayTiepNhan"] : null,
                        HanXuLy = reader["HanXuLy"] != DBNull.Value ? (DateTime?)reader["HanXuLy"] : null,
                        TenCongDan = reader["TenCongDan"].ToString(),
                        SoDienThoai = reader["SoDienThoai"].ToString(),
                        CMND_CCCD = reader["CMND_CCCD"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        IdTrangThai = reader["IdTrangThai"] != DBNull.Value ? (int?)Convert.ToInt32(reader["IdTrangThai"]) : null,
                        MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : null,
                        GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : null,
                        TenLinhVuc = reader["TenLinhVuc"] != DBNull.Value ? reader["TenLinhVuc"].ToString() : null,
                        TenLoaiHoSo = reader["TenLoaiHoSo"] != DBNull.Value ? reader["TenLoaiHoSo"].ToString() : null,
                        // Nếu muốn lấy MaLoaiHoSo, bạn có thể thêm property vào HoSoInfo hoặc dùng dynamic
                        MaLoaiHoSo = reader["MaLoaiHoSo"] != DBNull.Value ? reader["MaLoaiHoSo"].ToString() : null,
                    });
                }
                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }
        return result;
    }


    //lấy hồ sơ theo cán bộ xử lý
    public List<HoSoInfo> GetHoSoPhanCongXuLy(int? userId, string searchMaHoSo, string searchName, string searchTenCongDan, string searchCMND_CCCD, bool? searchSapHetHan, int? searchIdTrangThai, int page, int pageSize, out int totalRows)
    {
        var result = new List<HoSoInfo>();
        totalRows = 0;
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetHoSoPhanCongXuLy", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@SearchTenCongDan", searchTenCongDan ?? "");
            cmd.Parameters.AddWithValue("@SearchCMND_CCCD", searchCMND_CCCD ?? "");
            cmd.Parameters.AddWithValue("@SearchSapHetHan", (object)searchSapHetHan ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SearchIdTrangThai", searchIdTrangThai ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new HoSoInfo
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MaHoSo = reader["MaHoSo"].ToString(),
                        TieuDe = reader["TieuDe"].ToString(),
                        NgayTiepNhan = reader["NgayTiepNhan"] != DBNull.Value ? (DateTime?)reader["NgayTiepNhan"] : null,
                        HanXuLy = reader["HanXuLy"] != DBNull.Value ? (DateTime?)reader["HanXuLy"] : null,
                        TenCongDan = reader["TenCongDan"].ToString(),
                        SoDienThoai = reader["SoDienThoai"].ToString(),
                        CMND_CCCD = reader["CMND_CCCD"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        IdTrangThai = reader["IdTrangThai"] != DBNull.Value ? (int?)Convert.ToInt32(reader["IdTrangThai"]) : null,
                        MoTa = reader["MoTa"] != DBNull.Value ? reader["MoTa"].ToString() : null,
                        GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : null,
                        TenLinhVuc = reader["TenLinhVuc"] != DBNull.Value ? reader["TenLinhVuc"].ToString() : null,
                        TenLoaiHoSo = reader["TenLoaiHoSo"] != DBNull.Value ? reader["TenLoaiHoSo"].ToString() : null,
                        MaLoaiHoSo = reader["MaLoaiHoSo"] != DBNull.Value ? reader["MaLoaiHoSo"].ToString() : null,
                    });
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }
        return result;
    }




    public void DeleteHoSo(int id)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // Xóa file đính kèm trước
            using (var cmdFile = new SqlCommand("sp_DeleteAllFilesByHoSo", conn))
            {
                cmdFile.CommandType = CommandType.StoredProcedure;
                cmdFile.Parameters.AddWithValue("@IdHoSo", id);
                cmdFile.ExecuteNonQuery();
            }


            // Xóa hồ sơ
            using (var cmdHoSo = new SqlCommand("sp_DeleteHoSo", conn))
            {
                cmdHoSo.CommandType = CommandType.StoredProcedure;
                cmdHoSo.Parameters.AddWithValue("@Id", id);
                cmdHoSo.ExecuteNonQuery();
            }
        }
    }

    public void AddFileHoSo(int idHoSo, string tenFile, string duongDan, string loaiFile = null)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_AddFileHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", idHoSo);
            cmd.Parameters.AddWithValue("@TenFile", tenFile);
            cmd.Parameters.AddWithValue("@DuongDan", duongDan);
            cmd.Parameters.AddWithValue("@NgayUpload", DateTime.Now);
            cmd.Parameters.AddWithValue("@LoaiFile", loaiFile ?? (object)DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }


    public List<FileHoSo> GetFilesByHoSoId(int hoSoId)
    {
        var files = new List<FileHoSo>();
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetFilesByHoSoId", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", hoSoId);
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new FileHoSo
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    IdHoSo = Convert.ToInt32(reader["IdHoSo"]),
                    TenFile = reader["TenFile"].ToString(),
                    DuongDan = reader["DuongDan"].ToString()
                });
            }
        }
        return files;
    }
   

    public void DeleteFileHoSo(int idHoSo, string tenFile)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_DeleteFileHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", idHoSo);
            cmd.Parameters.AddWithValue("@TenFile", tenFile);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

}
