using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NhanSu
{
    public static class Db
    {
        public static string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["QuanLyNhanSu"].ConnectionString; }
        }

        public static SqlParameter P(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public static DataTable Schema(string sql)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var ad = new SqlDataAdapter(sql, cn))
            {
                var dt = new DataTable();
                ad.FillSchema(dt, SchemaType.Source);
                return dt;
            }
        }

        public static int Exec(string sql, params SqlParameter[] ps)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(ps);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (var cn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(ps);
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>Chạy nhiều lệnh trong một transaction; lỗi thì rollback.</summary>
        public static void Tran(Action<SqlConnection, SqlTransaction> work)
        {
            using (var cn = new SqlConnection(ConnStr))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        work(cn, tx);
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void Run(SqlConnection cn, SqlTransaction tx, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.AddRange(ps);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
