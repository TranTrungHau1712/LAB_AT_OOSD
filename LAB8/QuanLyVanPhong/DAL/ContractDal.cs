using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyVanPhong.DAL
{
    public static class HopDongDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT h.MaHD, h.SoHD, h.MaKH, k.HoTen AS TenKH, ISNULL(c.TenCT, N'') AS TenCT,
                    h.NgayKy, h.NgayBatDau, h.SoThang, h.NgayKetThuc,
                    (SELECT STRING_AGG(v.MaPhong, N', ') FROM ChiTietHopDong ct JOIN VanPhong v ON v.MaVP=ct.MaVP WHERE ct.MaHD=h.MaHD) AS DanhSachPhong,
                    (SELECT SUM(GiaThue) FROM ChiTietHopDong WHERE MaHD=h.MaHD) AS TongThueThang,
                    h.TrangThai, h.GhiChu
                FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH LEFT JOIN CongTy c ON c.MaCT=k.MaCT
                WHERE h.SoHD LIKE @k OR k.HoTen LIKE @k OR c.TenCT LIKE @k OR h.TrangThai LIKE @k
                ORDER BY h.MaHD DESC", "@k", Db.Like(kw));
        }

        public static DataTable Rooms(int maHD)
        {
            return Db.Query(@"SELECT ct.MaVP, v.MaPhong, t.TenTang, v.DienTich, v.Huong, ct.GiaThue
                FROM ChiTietHopDong ct JOIN VanPhong v ON v.MaVP=ct.MaVP JOIN Tang t ON t.MaTang=v.MaTang
                WHERE ct.MaHD=@id ORDER BY v.MaPhong", "@id", maHD);
        }

        public static DataTable ActiveLookup()
        {
            return Db.Query(@"SELECT h.MaHD, h.SoHD + N' - ' + k.HoTen AS Ten
                FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH WHERE h.TrangThai=N'Hiệu lực' ORDER BY h.MaHD DESC");
        }

        public static DataTable AllLookup()
        {
            return Db.Query(@"SELECT h.MaHD, h.SoHD + N' - ' + k.HoTen AS Ten
                FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH ORDER BY h.MaHD DESC");
        }

        // ---------- các hàm dùng trong transaction ----------
        public static DataTable LockRoom(SqlConnection c, SqlTransaction t, int maVP)
        {
            return Db.Query(c, t, "SELECT MaPhong, TrangThai FROM VanPhong WITH (UPDLOCK, ROWLOCK) WHERE MaVP=@id", "@id", maVP);
        }

        public static string NextSoHD(SqlConnection c, SqlTransaction t, int year)
        {
            object o = Db.Scalar(c, t,
                "SELECT ISNULL(MAX(CAST(RIGHT(SoHD,3) AS INT)),0)+1 FROM HopDong WITH (UPDLOCK) WHERE SoHD LIKE @p",
                "@p", "HD-" + year + "-%");
            return "HD-" + year + "-" + Convert.ToInt32(o).ToString("000");
        }

        public static int InsertHeader(SqlConnection c, SqlTransaction t, string soHD, int maKH, DateTime ngayKy,
            DateTime batDau, int soThang, DateTime ketThuc, string ghiChu)
        {
            object o = Db.Scalar(c, t,
                @"INSERT INTO HopDong(SoHD,MaKH,NgayKy,NgayBatDau,SoThang,NgayKetThuc,TrangThai,GhiChu)
                  VALUES(@so,@kh,@ky,@bd,@st,@kt,N'Hiệu lực',@gc);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);",
                "@so", soHD, "@kh", maKH, "@ky", ngayKy, "@bd", batDau, "@st", soThang, "@kt", ketThuc, "@gc", ghiChu);
            return Convert.ToInt32(o);
        }

        public static void InsertDetail(SqlConnection c, SqlTransaction t, int maHD, int maVP)
        {
            Db.Exec(c, t, "INSERT INTO ChiTietHopDong(MaHD,MaVP,GiaThue) SELECT @hd, MaVP, GiaThue FROM VanPhong WHERE MaVP=@vp",
                "@hd", maHD, "@vp", maVP);
        }

        public static void SetRoomsStatus(SqlConnection c, SqlTransaction t, int maHD, string status)
        {
            Db.Exec(c, t, "UPDATE VanPhong SET TrangThai=@s WHERE MaVP IN (SELECT MaVP FROM ChiTietHopDong WHERE MaHD=@hd)",
                "@s", status, "@hd", maHD);
        }

        public static void GenerateSchedule(SqlConnection c, SqlTransaction t, int maHD)
        {
            using (var cmd = new SqlCommand("dbo.sp_PhatSinhLichThanhToan", c, t))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                cmd.ExecuteNonQuery();
            }
        }

        public static void PayPeriod(SqlConnection c, SqlTransaction t, int maHD, int kyThu, DateTime date)
        {
            Db.Exec(c, t, "UPDATE ThanhToan SET TrangThai=N'Đã thanh toán', NgayThanhToan=@d WHERE MaHD=@hd AND KyThu=@k",
                "@d", date, "@hd", maHD, "@k", kyThu);
        }

        public static DataRow GetRow(SqlConnection c, SqlTransaction t, int maHD)
        {
            var dt = Db.Query(c, t, "SELECT MaHD, SoHD, TrangThai, NgayKetThuc FROM HopDong WITH (UPDLOCK) WHERE MaHD=@id", "@id", maHD);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static decimal CongNo(SqlConnection c, SqlTransaction t, int maHD)
        {
            return Convert.ToDecimal(Db.Scalar(c, t, "SELECT dbo.fn_CongNo(@id)", "@id", maHD));
        }

        public static int CountDependents(int maHD)
        {
            return Convert.ToInt32(Db.Scalar(@"SELECT
                (SELECT COUNT(*) FROM ThanhToan WHERE MaHD=@id AND TrangThai=N'Đã thanh toán')
              + (SELECT COUNT(*) FROM HoaDonDichVu WHERE MaHD=@id)
              + (SELECT COUNT(*) FROM PhieuGiaHan WHERE MaHD=@id)
              + (SELECT COUNT(*) FROM ViPham WHERE MaHD=@id)", "@id", maHD));
        }

        public static void DeleteUnpaidFuture(SqlConnection c, SqlTransaction t, int maHD)
        {
            Db.Exec(c, t, "DELETE FROM ThanhToan WHERE MaHD=@id AND TrangThai=N'Chưa thanh toán' AND HanThanhToan > CAST(GETDATE() AS DATE)", "@id", maHD);
        }

        public static void SetStatus(SqlConnection c, SqlTransaction t, int maHD, string status)
        {
            Db.Exec(c, t, "UPDATE HopDong SET TrangThai=@s WHERE MaHD=@id", "@s", status, "@id", maHD);
        }

        public static void DeleteAll(SqlConnection c, SqlTransaction t, int maHD)
        {
            Db.Exec(c, t, "DELETE FROM ThanhToan WHERE MaHD=@id", "@id", maHD);
            Db.Exec(c, t, "DELETE FROM HopDong WHERE MaHD=@id", "@id", maHD);   // ChiTietHopDong xóa theo CASCADE
        }

        public static void UpdateNote(int maHD, string ghiChu)
        {
            Db.Exec("UPDATE HopDong SET GhiChu=@g WHERE MaHD=@id", "@g", ghiChu, "@id", maHD);
        }

        public static void RefreshStatuses()
        {
            Db.InTransaction(delegate (SqlConnection c, SqlTransaction t)
            {
                Db.Exec(c, t, @"UPDATE VanPhong SET TrangThai=N'Trống' WHERE TrangThai=N'Đang thuê' AND MaVP IN
                    (SELECT ct.MaVP FROM ChiTietHopDong ct JOIN HopDong h ON h.MaHD=ct.MaHD
                     WHERE h.TrangThai=N'Hiệu lực' AND h.NgayKetThuc < CAST(GETDATE() AS DATE))");
                Db.Exec(c, t, "UPDATE HopDong SET TrangThai=N'Hết hạn' WHERE TrangThai=N'Hiệu lực' AND NgayKetThuc < CAST(GETDATE() AS DATE)");
            });
        }
    }

    public static class GiaHanDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT p.MaPG, p.SoPhieu, h.SoHD, k.HoTen AS TenKH,
                    (SELECT STRING_AGG(v.MaPhong, N', ') FROM ChiTietGiaHan g JOIN VanPhong v ON v.MaVP=g.MaVP WHERE g.MaPG=p.MaPG) AS DanhSachPhong,
                    p.NgayLap, p.SoThangGiaHan, p.NgayKetThucCu, p.NgayKetThucMoi, p.NguoiLap, p.GhiChu
                FROM PhieuGiaHan p JOIN HopDong h ON h.MaHD=p.MaHD JOIN KhachHang k ON k.MaKH=h.MaKH
                WHERE p.SoPhieu LIKE @k OR h.SoHD LIKE @k OR k.HoTen LIKE @k ORDER BY p.MaPG DESC", "@k", Db.Like(kw));
        }

        private const string InfoSql = @"SELECT h.MaHD, h.SoHD, k.HoTen, h.NgayBatDau, h.NgayKetThuc, h.SoThang, h.TrangThai,
                ISNULL((SELECT SUM(SoThangGiaHan) FROM PhieuGiaHan WHERE MaHD=h.MaHD),0) AS DaGiaHan,
                dbo.fn_CongNo(h.MaHD) AS CongNo,
                (SELECT STRING_AGG(v.MaPhong, N', ') FROM ChiTietHopDong ct JOIN VanPhong v ON v.MaVP=ct.MaVP WHERE ct.MaHD=h.MaHD) AS Phong
            FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH WHERE h.MaHD=@id";

        public static DataRow GetInfo(int maHD)
        {
            var dt = Db.Query(InfoSql, "@id", maHD);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static DataRow GetInfo(SqlConnection c, SqlTransaction t, int maHD)
        {
            var dt = Db.Query(c, t, InfoSql, "@id", maHD);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static string NextSoPhieu(SqlConnection c, SqlTransaction t, int year)
        {
            object o = Db.Scalar(c, t,
                "SELECT ISNULL(MAX(CAST(RIGHT(SoPhieu,3) AS INT)),0)+1 FROM PhieuGiaHan WITH (UPDLOCK) WHERE SoPhieu LIKE @p",
                "@p", "GH-" + year + "-%");
            return "GH-" + year + "-" + Convert.ToInt32(o).ToString("000");
        }

        public static int Insert(SqlConnection c, SqlTransaction t, string soPhieu, int maHD, DateTime ngayLap, int soThang,
            DateTime cu, DateTime moi, string nguoiLap, string ghiChu)
        {
            object o = Db.Scalar(c, t,
                @"INSERT INTO PhieuGiaHan(SoPhieu,MaHD,NgayLap,SoThangGiaHan,NgayKetThucCu,NgayKetThucMoi,NguoiLap,GhiChu)
                  VALUES(@a,@b,@c,@d,@e,@f,@g,@h);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);",
                "@a", soPhieu, "@b", maHD, "@c", ngayLap, "@d", soThang, "@e", cu, "@f", moi, "@g", nguoiLap, "@h", ghiChu);
            int maPG = Convert.ToInt32(o);
            Db.Exec(c, t, "INSERT INTO ChiTietGiaHan(MaPG,MaVP) SELECT @pg, MaVP FROM ChiTietHopDong WHERE MaHD=@hd", "@pg", maPG, "@hd", maHD);
            Db.Exec(c, t, "UPDATE HopDong SET NgayKetThuc=@kt WHERE MaHD=@hd", "@kt", moi, "@hd", maHD);
            return maPG;
        }
    }

    public static class ThanhToanDal
    {
        public static DataTable Search(string kw, int? maHD, string status)
        {
            return Db.Query(@"SELECT t.MaTT, t.MaHD, h.SoHD, k.HoTen AS TenKH, t.KyThu,
                    CASE WHEN t.TuThang=t.DenThang THEN N'Tháng ' + CAST(t.TuThang AS NVARCHAR(5))
                         ELSE N'Tháng ' + CAST(t.TuThang AS NVARCHAR(5)) + N' - ' + CAST(t.DenThang AS NVARCHAR(5)) END AS KyThanhToan,
                    t.SoTien, t.HanThanhToan, t.NgayThanhToan, t.TrangThai
                FROM ThanhToan t JOIN HopDong h ON h.MaHD=t.MaHD JOIN KhachHang k ON k.MaKH=h.MaKH
                WHERE (@hd IS NULL OR t.MaHD=@hd) AND (@tt IS NULL OR t.TrangThai=@tt)
                  AND (h.SoHD LIKE @k OR k.HoTen LIKE @k)
                ORDER BY t.MaHD DESC, t.KyThu",
                "@hd", maHD.HasValue ? (object)maHD.Value : null, "@tt", status, "@k", Db.Like(kw));
        }

        public static DataRow Get(SqlConnection c, SqlTransaction t, int maTT)
        {
            var dt = Db.Query(c, t, @"SELECT t.MaTT, t.MaHD, t.KyThu, t.TrangThai, h.TrangThai AS TrangThaiHD
                FROM ThanhToan t WITH (UPDLOCK) JOIN HopDong h ON h.MaHD=t.MaHD WHERE t.MaTT=@id", "@id", maTT);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static int CountUnpaidBefore(SqlConnection c, SqlTransaction t, int maHD, int ky)
        {
            return Convert.ToInt32(Db.Scalar(c, t, "SELECT COUNT(*) FROM ThanhToan WHERE MaHD=@h AND KyThu<@k AND TrangThai=N'Chưa thanh toán'", "@h", maHD, "@k", ky));
        }

        public static int CountPaidAfter(SqlConnection c, SqlTransaction t, int maHD, int ky)
        {
            return Convert.ToInt32(Db.Scalar(c, t, "SELECT COUNT(*) FROM ThanhToan WHERE MaHD=@h AND KyThu>@k AND TrangThai=N'Đã thanh toán'", "@h", maHD, "@k", ky));
        }

        public static void SetPaid(SqlConnection c, SqlTransaction t, int maTT, DateTime date)
        {
            Db.Exec(c, t, "UPDATE ThanhToan SET TrangThai=N'Đã thanh toán', NgayThanhToan=@d WHERE MaTT=@id", "@d", date, "@id", maTT);
        }

        public static void SetUnpaid(SqlConnection c, SqlTransaction t, int maTT)
        {
            Db.Exec(c, t, "UPDATE ThanhToan SET TrangThai=N'Chưa thanh toán', NgayThanhToan=NULL WHERE MaTT=@id", "@id", maTT);
        }
    }
}
