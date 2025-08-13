using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HanhChinhCong.Models;

public class LoaiHoSoRepository
{
    private readonly string connectionString;

    public LoaiHoSoRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DbConnectContext"].ConnectionString;
    }

    public List<LoaiHoSoViewModel> SearchLoaiHoSoWithPaging(string searchName, int page, int pageSize, out int totalRows)
    {
        var loaiHoSoList = new List<LoaiHoSoViewModel>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchLoaiHoSoWithPaging", conn))
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
                    loaiHoSoList.Add(new LoaiHoSoViewModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MaLoaiHoSo = reader["MaLoaiHoSo"].ToString(),
                        TenLoaiHoSo = reader["TenLoaiHoSo"].ToString(),
                        IdLinhVuc = Convert.ToInt32(reader["IdLinhVuc"]),
                        TenLinhVuc = reader["TenLinhVuc"].ToString()
                    });
                }
                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }

        return loaiHoSoList;
    }

}
