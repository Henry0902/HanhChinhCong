using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HanhChinhCong.Models;

public class LinhVucRepository
{
    private readonly string connectionString;

    public LinhVucRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DbConnectContext"].ConnectionString;
    }

    public List<LinhVucViewModel> SearchLinhVucWithPaging(string searchName, int page, int pageSize, out int totalRows)
    {
        var linhvuc = new List<LinhVucViewModel>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchLinhVucWithPaging", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                // First result: paged data
                while (reader.Read())
                {
                    linhvuc.Add(new LinhVucViewModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TenLinhVuc = reader["TenLinhVuc"].ToString(),
                        IdPhongBan = Convert.ToInt32(reader["IdPhongBan"]),
                        TenPhongBan = reader["TenPhongBan"].ToString()
                    });
                }
                // Second result: total count
                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }

        return linhvuc;
    }

}
