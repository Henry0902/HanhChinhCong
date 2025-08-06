using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HanhChinhCong.Models;

public class PhongBanRepository
{
    private readonly string connectionString;

    public PhongBanRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DbConnectContext"].ConnectionString;
    }

    public List<PhongBan> SearchPhongBanWithPaging(string searchName, int page, int pageSize, out int totalRows)
    {
        var phongBans = new List<PhongBan>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchPhongBanWithPaging", conn))
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
                    phongBans.Add(new PhongBan
                    {
                        Id = Convert.ToInt32(reader["Id"]),
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
        return phongBans;
    }

    // Có thể bổ sung các hàm Add, Edit, Delete nếu muốn thao tác trực tiếp qua repository
}