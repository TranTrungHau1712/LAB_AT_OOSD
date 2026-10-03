using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace eShopping.Data
{
    /// <summary>Tiện ích truy cập SQL Server dùng chung cho các lớp DAO.</summary>
    public static class Db
    {
        public static string ConnStr => ConfigurationManager.ConnectionStrings["eShoppingDb"].ConnectionString;

        public static SqlConnection Open()
        {
            var c = new SqlConnection(ConnStr);
            c.Open();
            return c;
        }

        public static SqlParameter P(string name, object value)
            => new SqlParameter(name, value ?? DBNull.Value);

        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }

        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteScalar();
            }
        }

        public static int Exec(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}