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

    public List<HoSoInfo> SearchHoSoWithPaging(string searchName, string searchTenCongDan, string searchCMND_CCCD, int page, int pageSize, out int totalRows)
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
                        IdLinhVuc = reader["IdLinhVuc"] as int?,
                        TenLinhVuc = reader["TenLinhVuc"].ToString(),
                        IdLoaiHoSo = reader["IdLoaiHoSo"] as int?,
                        TenLoaiHoSo = reader["TenLoaiHoSo"].ToString()
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

    public void AddFileHoSo(int idHoSo, string tenFile, string duongDan)
    {
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_AddFileHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdHoSo", idHoSo);
            cmd.Parameters.AddWithValue("@TenFile", tenFile);
            cmd.Parameters.AddWithValue("@DuongDan", duongDan);
            cmd.Parameters.AddWithValue("@NgayUpload", DateTime.Now);
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
