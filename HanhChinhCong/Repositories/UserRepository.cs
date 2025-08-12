using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HanhChinhCong.Models;

public class UserRepository
{
    private readonly string connectionString;

    public UserRepository()
    {
        connectionString = ConfigurationManager.ConnectionStrings["DbConnectContext"].ConnectionString;
    }

    public List<UserViewModel> SearchUsersWithPaging(string searchName, int? searchRole, int page, int pageSize, out int totalRows)
    {
        var users = new List<UserViewModel>();
        totalRows = 0;

        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_SearchUsersWithPaging", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchName", searchName ?? "");
            cmd.Parameters.AddWithValue("@SearchRole", (object)searchRole ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                // First result: paged data
                while (reader.Read())
                {
                    users.Add(new UserViewModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        HoTen = reader["HoTen"].ToString(),
                        Role = Convert.ToInt32(reader["Role"]),
                        //UserName = reader["UserName"].ToString()
                    });
                }
                // Second result: total count
                if (reader.NextResult() && reader.Read())
                {
                    totalRows = Convert.ToInt32(reader["TotalRows"]);
                }
            }
        }

        return users;
    }

    public List<UserInfo> GetAllCanBoXuLy(int vaiTro)
    {
        var list = new List<UserInfo>();
        using (var conn = new SqlConnection(connectionString))
        using (var cmd = new SqlCommand("sp_GetAllCanBoXuLy", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Role", vaiTro);
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new UserInfo
                {
                    Id = reader.GetInt32(0),
                    HoTen = reader.GetString(1),
                    Role = reader.GetInt32(2)
                });
            }
        }
        return list;
    }


}
