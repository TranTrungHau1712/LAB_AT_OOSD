using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QuanLyVanPhong.Common;

namespace QuanLyVanPhong.DAL
{
    /// <summary>Lớp truy cập SQL Server dùng chung. Tham số truyền theo cặp: "@ten", giá trị.</summary>
    public static class Db
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["QLVP"].ConnectionString; }
        }

        public static string Like(string kw) { return "%" + (kw ?? "").Trim() + "%"; }

        public static SqlConnection Open()
        {
            var c = new SqlConnection(ConnectionString);
            c.Open();
            return c;
        }

        private static SqlCommand Build(SqlConnection c, SqlTransaction t, string sql, object[] p)
        {
            var cmd = new SqlCommand(sql, c, t);
            if (p != null)
                for (int i = 0; i + 1 < p.Length; i += 2)
                    cmd.Parameters.AddWithValue((string)p[i], p[i + 1] ?? DBNull.Value);
            return cmd;
        }

        // ---- không transaction ----
        public static DataTable Query(string sql, params object[] p)
        {
            using (var c = Open())
                return Query(c, null, sql, p);
        }
        public static int Exec(string sql, params object[] p)
        {
            using (var c = Open())
                return Exec(c, null, sql, p);
        }
        public static object Scalar(string sql, params object[] p)
        {
            using (var c = Open())
                return Scalar(c, null, sql, p);
        }

        // ---- có transaction ----
        public static DataTable Query(SqlConnection c, SqlTransaction t, string sql, params object[] p)
        {
            var dt = new DataTable();
            using (var cmd = Build(c, t, sql, p))
            using (var da = new SqlDataAdapter(cmd))
                da.Fill(dt);
            return dt;
        }
        public static int Exec(SqlConnection c, SqlTransaction t, string sql, params object[] p)
        {
            using (var cmd = Build(c, t, sql, p))
                return cmd.ExecuteNonQuery();
        }
        public static object Scalar(SqlConnection c, SqlTransaction t, string sql, params object[] p)
        {
            using (var cmd = Build(c, t, sql, p))
            {
                object o = cmd.ExecuteScalar();
                return o == DBNull.Value ? null : o;
            }
        }

        /// <summary>Chạy một nhóm lệnh trong 1 transaction: lỗi bất kỳ => Rollback toàn bộ.</summary>
        public static void InTransaction(Action<SqlConnection, SqlTransaction> work)
        {
            using (var c = Open())
            using (var t = c.BeginTransaction())
            {
                try
                {
                    work(c, t);
                    t.Commit();
                }
                catch
                {
                    try { t.Rollback(); } catch { }
                    throw;
                }
            }
        }

        public static T InTransaction<T>(Func<SqlConnection, SqlTransaction, T> work)
        {
            T result = default(T);
            InTransaction(delegate (SqlConnection c, SqlTransaction t) { result = work(c, t); });
            return result;
        }

        public static string Friendly(Exception ex)
        {
            if (ex is BusinessException) return ex.Message;
            var se = ex as SqlException;
            if (se != null)
            {
                foreach (SqlError e in se.Errors)
                {
                    if (e.Number == 2627 || e.Number == 2601)
                        return "Dữ liệu bị trùng (giá trị duy nhất đã tồn tại).\n" + e.Message;
                    if (e.Number == 547)
                        return "Vi phạm ràng buộc dữ liệu (dữ liệu đang được sử dụng ở nơi khác hoặc giá trị không hợp lệ).\n" + e.Message;
                }
                if (se.Number == -1 || se.Number == 53 || se.Number == 2 || se.Number == 4060)
                    return "Không kết nối được SQL Server. Kiểm tra chuỗi kết nối trong App.config và đã chạy file QuanLyVanPhong.sql chưa.\n" + se.Message;
            }
            return ex.Message;
        }
    }
}
