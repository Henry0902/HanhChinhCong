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

    public List<HoSo> SearchHoSoWithPaging(string searchName, int page, int pageSize, out int totalRows)
    {
        var list = new List<HoSo>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchHoSoWithPaging", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new HoSo
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MaHoSo = reader["MaHoSo"].ToString(),
                        TieuDe = reader["TieuDe"].ToString(),
                        MoTa = reader["MoTa"].ToString(),
                        NgayTiepNhan = reader["NgayTiepNhan"] as DateTime?,
                        HanXuLy = reader["HanXuLy"] as DateTime?,
                        NgayHoanThanh = reader["NgayHoanThanh"] as DateTime?,
                        GhiChu = reader["GhiChu"].ToString(),
                        IdCanBoTiepNhan = reader["IdCanBoTiepNhan"] as int?,
                        IdPhongBan = reader["IdPhongBan"] as int?,
                        IdLinhVuc = reader["IdLinhVuc"] as int?,
                        IdLoaiHoSo = reader["IdLoaiHoSo"] as int?,
                        IdTrangThai = reader["IdTrangThai"] as int?,
                        TenCongDan = reader["TenCongDan"].ToString(),
                        SoDienThoai = reader["SoDienThoai"].ToString(),
                        CMND_CCCD = reader["CMND_CCCD"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString()
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
        using (var cmd = new SqlCommand("sp_DeleteHoSo", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
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
}
