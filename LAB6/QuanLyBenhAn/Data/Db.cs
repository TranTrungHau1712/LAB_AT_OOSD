using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyBenhAn.Data
{
    /// <summary>Lớp truy cập dữ liệu dùng chung (tầng Data). Tham số truyền theo cặp: tên, giá trị.</summary>
    public static class Db
    {
        // Sửa Server cho phù hợp máy của bạn, ví dụ: .\SQLEXPRESS hoặc (localdb)\MSSQLLocalDB
        public static string ConnectionString =
            @"Server=.;Database=QuanLyBenhAn;Integrated Security=true;TrustServerCertificate=true;";

        public static string TestConnection()
        {
            try { using (var cn = new SqlConnection(ConnectionString)) { cn.Open(); } return null; }
            catch (Exception ex) { return ex.Message; }
        }

        public static SqlConnection Open()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();
            return cn;
        }

        private static SqlCommand Build(SqlConnection cn, SqlTransaction tx, string sql, object[] p)
        {
            var cmd = new SqlCommand(sql, cn, tx);
            if (p != null)
            {
                for (int i = 0; i + 1 < p.Length; i += 2)
                {
                    string name = (string)p[i];
                    object v = p[i + 1];
                    SqlParameter prm;
                    if (name == "@anh")
                    {
                        prm = new SqlParameter(name, SqlDbType.VarBinary, -1);
                        prm.Value = v == null ? (object)DBNull.Value : v;
                    }
                    else prm = new SqlParameter(name, v ?? DBNull.Value);
                    cmd.Parameters.Add(prm);
                }
            }
            return cmd;
        }

        public static DataTable Query(string sql, params object[] p)
        {
            using (var cn = Open())
            using (var cmd = Build(cn, null, sql, p))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Exec(string sql, params object[] p)
        {
            using (var cn = Open())
            using (var cmd = Build(cn, null, sql, p))
                return cmd.ExecuteNonQuery();
        }

        public static object Scalar(string sql, params object[] p)
        {
            using (var cn = Open())
            using (var cmd = Build(cn, null, sql, p))
            {
                object o = cmd.ExecuteScalar();
                return o == DBNull.Value ? null : o;
            }
        }

        // ---- Các hàm dùng trong giao dịch ----
        public static int Exec(SqlConnection cn, SqlTransaction tx, string sql, params object[] p)
        {
            using (var cmd = Build(cn, tx, sql, p)) return cmd.ExecuteNonQuery();
        }

        public static object Scalar(SqlConnection cn, SqlTransaction tx, string sql, params object[] p)
        {
            using (var cmd = Build(cn, tx, sql, p))
            {
                object o = cmd.ExecuteScalar();
                return o == DBNull.Value ? null : o;
            }
        }

        public static void InTransaction(Action<SqlConnection, SqlTransaction> work)
        {
            using (var cn = Open())
            using (var tx = cn.BeginTransaction())
            {
                try { work(cn, tx); tx.Commit(); }
                catch { tx.Rollback(); throw; }
            }
        }

        /// <summary>Sinh mã kế tiếp, ví dụ NextId(..., "BenhNhan", "maBN", "BN", 4) -> BN0006.</summary>
        public static string NextId(SqlConnection cn, SqlTransaction tx, string table, string col, string prefix, int digits)
        {
            string sql = "SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(" + col + ", @len + 1, 20) AS INT)), 0) + 1 " +
                         "FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE " + col + " LIKE @pre + '%'";
            object o = Scalar(cn, tx, sql, "@len", prefix.Length, "@pre", prefix);
            int n = Convert.ToInt32(o);
            return prefix + n.ToString("D" + digits);
        }
    }
}
