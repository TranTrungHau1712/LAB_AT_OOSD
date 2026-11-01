using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyVanPhong.DAL
{
    public static class HoaDonDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT d.MaHDDV, d.MaHD, h.SoHD, k.HoTen AS TenKH, d.Thang, d.Nam, d.NgayLap, d.TongTien, d.NgayThanhToan, d.TrangThai
                FROM HoaDonDichVu d JOIN HopDong h ON h.MaHD=d.MaHD JOIN KhachHang k ON k.MaKH=h.MaKH
                WHERE h.SoHD LIKE @k OR k.HoTen LIKE @k OR d.TrangThai LIKE @k
                ORDER BY d.Nam DESC, d.Thang DESC, d.MaHDDV DESC", "@k", Db.Like(kw));
        }

        public static DataTable Details(int maHDDV)
        {
            return Db.Query(@"SELECT s.TenDV, s.DonVi, c.ChiSoCu, c.ChiSoMoi, c.SoLuong, c.DonGia, c.ThanhTien
                FROM ChiTietHoaDon c JOIN DichVu s ON s.MaDV=c.MaDV WHERE c.MaHDDV=@id ORDER BY s.MaDV", "@id", maHDDV);
        }

        public static decimal? LastReading(int maHD, int maDV, int thang, int nam)
        {
            object o = Db.Scalar(@"SELECT TOP 1 c.ChiSoMoi FROM ChiTietHoaDon c JOIN HoaDonDichVu h ON h.MaHDDV=c.MaHDDV
                WHERE h.MaHD=@hd AND c.MaDV=@dv AND c.ChiSoMoi IS NOT NULL AND (h.Nam*12+h.Thang) < (@nam*12+@thang)
                ORDER BY h.Nam DESC, h.Thang DESC", "@hd", maHD, "@dv", maDV, "@nam", nam, "@thang", thang);
            return o == null ? (decimal?)null : Convert.ToDecimal(o);
        }

        public static DataRow ContractPeriod(SqlConnection c, SqlTransaction t, int maHD)
        {
            var dt = Db.Query(c, t, "SELECT NgayBatDau, NgayKetThuc, TrangThai FROM HopDong WITH (UPDLOCK) WHERE MaHD=@id", "@id", maHD);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static bool Exists(SqlConnection c, SqlTransaction t, int maHD, int thang, int nam)
        {
            return Convert.ToInt32(Db.Scalar(c, t, "SELECT COUNT(*) FROM HoaDonDichVu WHERE MaHD=@h AND Thang=@t AND Nam=@n", "@h", maHD, "@t", thang, "@n", nam)) > 0;
        }

        public static int InsertHeader(SqlConnection c, SqlTransaction t, int maHD, int thang, int nam, DateTime ngayLap, decimal tong)
        {
            object o = Db.Scalar(c, t,
                @"INSERT INTO HoaDonDichVu(MaHD,Thang,Nam,NgayLap,TongTien,TrangThai) VALUES(@h,@t,@n,@d,@tong,N'Chưa thanh toán');
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", "@h", maHD, "@t", thang, "@n", nam, "@d", ngayLap, "@tong", tong);
            return Convert.ToInt32(o);
        }

        public static void InsertDetail(SqlConnection c, SqlTransaction t, int maHDDV, int maDV, decimal? cu, decimal? moi, decimal sl, decimal gia, decimal tien)
        {
            Db.Exec(c, t, "INSERT INTO ChiTietHoaDon(MaHDDV,MaDV,ChiSoCu,ChiSoMoi,SoLuong,DonGia,ThanhTien) VALUES(@a,@b,@c,@d,@e,@f,@g)",
                "@a", maHDDV, "@b", maDV, "@c", cu.HasValue ? (object)cu.Value : null, "@d", moi.HasValue ? (object)moi.Value : null,
                "@e", sl, "@f", gia, "@g", tien);
        }

        public static string GetStatus(int maHDDV)
        {
            object o = Db.Scalar("SELECT TrangThai FROM HoaDonDichVu WHERE MaHDDV=@id", "@id", maHDDV);
            return o == null ? null : o.ToString();
        }

        public static void Pay(int maHDDV, DateTime d)
        {
            Db.Exec("UPDATE HoaDonDichVu SET TrangThai=N'Đã thanh toán', NgayThanhToan=@d WHERE MaHDDV=@id", "@d", d, "@id", maHDDV);
        }

        public static void Delete(int maHDDV) { Db.Exec("DELETE FROM HoaDonDichVu WHERE MaHDDV=@id", "@id", maHDDV); }
    }

    public static class NhanVienDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT n.MaNV, n.HoTen, n.NgaySinh, n.GioiTinh, n.CCCD, n.SDT, n.MaCT, c.TenCT, n.MaVP, v.MaPhong, t.TenTang,
                    n.ChucVu, n.HinhAnh, n.TrangThai
                FROM NhanVien n JOIN CongTy c ON c.MaCT=n.MaCT JOIN VanPhong v ON v.MaVP=n.MaVP JOIN Tang t ON t.MaTang=v.MaTang
                WHERE n.MaNV LIKE @k OR n.HoTen LIKE @k OR c.TenCT LIKE @k OR v.MaPhong LIKE @k OR n.ChucVu LIKE @k
                ORDER BY n.MaNV", "@k", Db.Like(kw));
        }

        public static DataTable Lookup()
        {
            return Db.Query("SELECT MaNV, MaNV + N' - ' + HoTen AS Ten FROM NhanVien WHERE TrangThai=N'Đang làm' ORDER BY MaNV");
        }

        public static int CountRoomOfCompany(int maVP, int maCT)
        {
            return Convert.ToInt32(Db.Scalar(@"SELECT COUNT(*) FROM ChiTietHopDong ct JOIN HopDong h ON h.MaHD=ct.MaHD
                JOIN KhachHang k ON k.MaKH=h.MaKH WHERE ct.MaVP=@vp AND k.MaCT=@ct AND h.TrangThai=N'Hiệu lực'", "@vp", maVP, "@ct", maCT));
        }

        public static void Insert(string ma, string ten, DateTime ns, string gt, string cccd, string sdt, int ct, int vp, string cv, string anh, string tt)
        {
            Db.Exec(@"INSERT INTO NhanVien(MaNV,HoTen,NgaySinh,GioiTinh,CCCD,SDT,MaCT,MaVP,ChucVu,HinhAnh,TrangThai)
                VALUES(@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k)",
                "@a", ma, "@b", ten, "@c", ns, "@d", gt, "@e", cccd, "@f", sdt, "@g", ct, "@h", vp, "@i", cv, "@j", anh, "@k", tt);
        }

        public static void Update(string ma, string ten, DateTime ns, string gt, string cccd, string sdt, int ct, int vp, string cv, string anh, string tt)
        {
            Db.Exec(@"UPDATE NhanVien SET HoTen=@b, NgaySinh=@c, GioiTinh=@d, CCCD=@e, SDT=@f, MaCT=@g, MaVP=@h, ChucVu=@i, HinhAnh=@j, TrangThai=@k WHERE MaNV=@a",
                "@a", ma, "@b", ten, "@c", ns, "@d", gt, "@e", cccd, "@f", sdt, "@g", ct, "@h", vp, "@i", cv, "@j", anh, "@k", tt);
        }

        public static void Delete(string ma) { Db.Exec("DELETE FROM NhanVien WHERE MaNV=@a", "@a", ma); }
    }

    public static class TheXeDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT g.MaThe, g.MaNV, n.HoTen, c.TenCT, g.NgayCap, g.TrangThai, p.LoaiXe, p.BienSo, p.HangXe, p.MauSac
                FROM TheGiuXe g JOIN NhanVien n ON n.MaNV=g.MaNV JOIN CongTy c ON c.MaCT=n.MaCT JOIN PhuongTien p ON p.MaThe=g.MaThe
                WHERE g.MaThe LIKE @k OR n.HoTen LIKE @k OR p.BienSo LIKE @k OR c.TenCT LIKE @k ORDER BY g.MaThe", "@k", Db.Like(kw));
        }

        public static string StaffStatus(SqlConnection c, SqlTransaction t, string maNV)
        {
            object o = Db.Scalar(c, t, "SELECT TrangThai FROM NhanVien WHERE MaNV=@a", "@a", maNV);
            return o == null ? null : o.ToString();
        }

        public static void Insert(SqlConnection c, SqlTransaction t, string maThe, string maNV, DateTime ngayCap, string tt,
            string loai, string bienSo, string hang, string mau)
        {
            Db.Exec(c, t, "INSERT INTO TheGiuXe(MaThe,MaNV,NgayCap,TrangThai) VALUES(@a,@b,@c,@d)", "@a", maThe, "@b", maNV, "@c", ngayCap, "@d", tt);
            Db.Exec(c, t, "INSERT INTO PhuongTien(MaThe,LoaiXe,BienSo,HangXe,MauSac) VALUES(@a,@b,@c,@d,@e)", "@a", maThe, "@b", loai, "@c", bienSo, "@d", hang, "@e", mau);
        }

        public static void Update(SqlConnection c, SqlTransaction t, string maThe, DateTime ngayCap, string tt,
            string loai, string bienSo, string hang, string mau)
        {
            Db.Exec(c, t, "UPDATE TheGiuXe SET NgayCap=@c, TrangThai=@d WHERE MaThe=@a", "@a", maThe, "@c", ngayCap, "@d", tt);
            Db.Exec(c, t, "UPDATE PhuongTien SET LoaiXe=@b, BienSo=@c, HangXe=@d, MauSac=@e WHERE MaThe=@a", "@a", maThe, "@b", loai, "@c", bienSo, "@d", hang, "@e", mau);
        }

        public static void Delete(string maThe) { Db.Exec("DELETE FROM TheGiuXe WHERE MaThe=@a", "@a", maThe); }
    }

    public static class KiemTraDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT k.MaPKT, k.MaVP, v.MaPhong, t.TenTang, k.NgayKiemTra, k.NguoiKiemTra, k.TinhTrang, k.CoViPham, k.GhiChu
                FROM PhieuKiemTra k JOIN VanPhong v ON v.MaVP=k.MaVP JOIN Tang t ON t.MaTang=v.MaTang
                WHERE v.MaPhong LIKE @k OR k.NguoiKiemTra LIKE @k OR k.TinhTrang LIKE @k OR CAST(YEAR(k.NgayKiemTra) AS VARCHAR(4)) LIKE @k
                ORDER BY k.NgayKiemTra DESC, k.MaPKT DESC", "@k", Db.Like(kw));
        }

        public static DataTable DueRooms()
        {
            return Db.Query(@"SELECT v.MaVP, v.MaPhong, t.TenTang, v.TrangThai AS TrangThaiPhong,
                    (SELECT MAX(NgayKiemTra) FROM PhieuKiemTra WHERE MaVP=v.MaVP) AS LanKiemTraCuoi
                FROM VanPhong v JOIN Tang t ON t.MaTang=v.MaTang
                WHERE NOT EXISTS (SELECT 1 FROM PhieuKiemTra k WHERE k.MaVP=v.MaVP AND k.NgayKiemTra > DATEADD(YEAR,-1,CAST(GETDATE() AS DATE)))
                ORDER BY t.SoTang, v.MaPhong");
        }

        public static void Insert(int maVP, DateTime ngay, string nguoi, string tinhTrang, bool coVP, string ghiChu)
        {
            Db.Exec("INSERT INTO PhieuKiemTra(MaVP,NgayKiemTra,NguoiKiemTra,TinhTrang,CoViPham,GhiChu) VALUES(@a,@b,@c,@d,@e,@f)",
                "@a", maVP, "@b", ngay, "@c", nguoi, "@d", tinhTrang, "@e", coVP, "@f", ghiChu);
        }

        public static void Update(int id, int maVP, DateTime ngay, string nguoi, string tinhTrang, bool coVP, string ghiChu)
        {
            Db.Exec("UPDATE PhieuKiemTra SET MaVP=@a, NgayKiemTra=@b, NguoiKiemTra=@c, TinhTrang=@d, CoViPham=@e, GhiChu=@f WHERE MaPKT=@id",
                "@a", maVP, "@b", ngay, "@c", nguoi, "@d", tinhTrang, "@e", coVP, "@f", ghiChu, "@id", id);
        }

        public static void Delete(int id) { Db.Exec("DELETE FROM PhieuKiemTra WHERE MaPKT=@id", "@id", id); }

        public static int CountViolations(int maPKT)
        {
            return Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM ViPham WHERE MaPKT=@id", "@id", maPKT));
        }
    }

    public static class ViPhamDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT p.MaVPh, p.MaPKT, v.MaPhong, h.SoHD, p.NgayLap, p.NoiDung, p.MucPhat, p.TrangThai, p.NgayNop
                FROM ViPham p JOIN PhieuKiemTra k ON k.MaPKT=p.MaPKT JOIN VanPhong v ON v.MaVP=k.MaVP JOIN HopDong h ON h.MaHD=p.MaHD
                WHERE v.MaPhong LIKE @k OR h.SoHD LIKE @k OR p.NoiDung LIKE @k OR p.TrangThai LIKE @k
                ORDER BY p.MaVPh DESC", "@k", Db.Like(kw));
        }

        public static DataTable InspectionLookup()
        {
            return Db.Query(@"SELECT k.MaPKT, N'PKT' + CAST(k.MaPKT AS NVARCHAR(10)) + N' - ' + v.MaPhong + N' - ' + CONVERT(NVARCHAR(10), k.NgayKiemTra, 103) AS Ten
                FROM PhieuKiemTra k JOIN VanPhong v ON v.MaVP=k.MaVP WHERE k.CoViPham=1 ORDER BY k.NgayKiemTra DESC");
        }

        public static bool InspectionHasViolationFlag(int maPKT)
        {
            return Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM PhieuKiemTra WHERE MaPKT=@id AND CoViPham=1", "@id", maPKT)) > 0;
        }

        /// <summary>Hợp đồng đang thuê phòng được kiểm tra tại ngày kiểm tra.</summary>
        public static int? ContractOfInspection(int maPKT)
        {
            object o = Db.Scalar(@"SELECT TOP 1 h.MaHD FROM PhieuKiemTra k
                JOIN ChiTietHopDong ct ON ct.MaVP=k.MaVP JOIN HopDong h ON h.MaHD=ct.MaHD
                WHERE k.MaPKT=@id AND h.NgayBatDau<=k.NgayKiemTra AND h.NgayKetThuc>=k.NgayKiemTra AND h.TrangThai<>N'Đã thanh lý'
                ORDER BY h.MaHD DESC", "@id", maPKT);
            return o == null ? (int?)null : Convert.ToInt32(o);
        }

        public static string GetStatus(int id)
        {
            object o = Db.Scalar("SELECT TrangThai FROM ViPham WHERE MaVPh=@id", "@id", id);
            return o == null ? null : o.ToString();
        }

        public static void Insert(int maPKT, int maHD, DateTime ngay, string noiDung, decimal phat, string tt, object ngayNop)
        {
            Db.Exec("INSERT INTO ViPham(MaPKT,MaHD,NgayLap,NoiDung,MucPhat,TrangThai,NgayNop) VALUES(@a,@b,@c,@d,@e,@f,@g)",
                "@a", maPKT, "@b", maHD, "@c", ngay, "@d", noiDung, "@e", phat, "@f", tt, "@g", ngayNop);
        }

        public static void Update(int id, int maPKT, int maHD, DateTime ngay, string noiDung, decimal phat, string tt, object ngayNop)
        {
            Db.Exec("UPDATE ViPham SET MaPKT=@a, MaHD=@b, NgayLap=@c, NoiDung=@d, MucPhat=@e, TrangThai=@f, NgayNop=@g WHERE MaVPh=@id",
                "@a", maPKT, "@b", maHD, "@c", ngay, "@d", noiDung, "@e", phat, "@f", tt, "@g", ngayNop, "@id", id);
        }

        public static void Delete(int id) { Db.Exec("DELETE FROM ViPham WHERE MaVPh=@id", "@id", id); }

        public static void Pay(int id, DateTime d)
        {
            Db.Exec("UPDATE ViPham SET TrangThai=N'Đã nộp', NgayNop=@d WHERE MaVPh=@id", "@d", d, "@id", id);
        }
    }

    public static class ReportDal
    {
        public static DataRow Dashboard()
        {
            var dt = Db.Query(@"SELECT
                (SELECT COUNT(*) FROM VanPhong) AS TongPhong,
                (SELECT COUNT(*) FROM VanPhong WHERE TrangThai=N'Đang thuê') AS DangThue,
                (SELECT COUNT(*) FROM VanPhong WHERE TrangThai=N'Trống') AS PhongTrong,
                (SELECT COUNT(*) FROM HopDong WHERE TrangThai=N'Hiệu lực') AS HopDongHieuLuc,
                (SELECT ISNULL(SUM(dbo.fn_CongNo(MaHD)),0) FROM HopDong WHERE TrangThai<>N'Đã thanh lý') AS CongNo");
            return dt.Rows[0];
        }

        public static DataTable ExpiringContracts(int days)
        {
            return Db.Query(@"SELECT h.SoHD, k.HoTen AS TenKH, h.NgayKetThuc, DATEDIFF(DAY, CAST(GETDATE() AS DATE), h.NgayKetThuc) AS ConLaiNgay
                FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH
                WHERE h.TrangThai=N'Hiệu lực' AND h.NgayKetThuc BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY,@d,CAST(GETDATE() AS DATE))
                ORDER BY h.NgayKetThuc", "@d", days);
        }

        public static DataTable DueRentPayments()
        {
            return Db.Query(@"SELECT h.SoHD, k.HoTen AS TenKH, t.KyThu, t.SoTien, t.HanThanhToan
                FROM ThanhToan t JOIN HopDong h ON h.MaHD=t.MaHD JOIN KhachHang k ON k.MaKH=h.MaKH
                WHERE t.TrangThai=N'Chưa thanh toán' AND t.HanThanhToan <= DATEADD(DAY,7,CAST(GETDATE() AS DATE)) AND h.TrangThai=N'Hiệu lực'
                ORDER BY t.HanThanhToan");
        }

        public static DataTable Revenue(int year)
        {
            return Db.Query(@"SELECT Thang, SUM(TienThue) AS TienThue, SUM(TienDichVu) AS TienDichVu, SUM(TienPhat) AS TienPhat,
                    SUM(TienThue + TienDichVu + TienPhat) AS TongThu
                FROM (
                    SELECT MONTH(NgayThanhToan) AS Thang, CAST(SoTien AS DECIMAL(18,0)) AS TienThue, CAST(0 AS DECIMAL(18,0)) AS TienDichVu, CAST(0 AS DECIMAL(18,0)) AS TienPhat
                        FROM ThanhToan WHERE TrangThai=N'Đã thanh toán' AND YEAR(NgayThanhToan)=@y
                    UNION ALL
                    SELECT MONTH(NgayThanhToan), CAST(0 AS DECIMAL(18,0)), TongTien, CAST(0 AS DECIMAL(18,0))
                        FROM HoaDonDichVu WHERE TrangThai=N'Đã thanh toán' AND YEAR(NgayThanhToan)=@y
                    UNION ALL
                    SELECT MONTH(NgayNop), CAST(0 AS DECIMAL(18,0)), CAST(0 AS DECIMAL(18,0)), MucPhat
                        FROM ViPham WHERE TrangThai=N'Đã nộp' AND YEAR(NgayNop)=@y
                ) x GROUP BY Thang ORDER BY Thang", "@y", year);
        }

        public static DataTable RoomsByFloor()
        {
            return Db.Query(@"SELECT t.TenTang, COUNT(v.MaVP) AS TongPhong,
                    SUM(CASE WHEN v.TrangThai=N'Đang thuê' THEN 1 ELSE 0 END) AS DangThue,
                    SUM(CASE WHEN v.TrangThai=N'Trống' THEN 1 ELSE 0 END) AS PhongTrong,
                    SUM(CASE WHEN v.TrangThai=N'Bảo trì' THEN 1 ELSE 0 END) AS BaoTri,
                    CASE WHEN COUNT(v.MaVP)=0 THEN 0 ELSE CAST(100.0*SUM(CASE WHEN v.TrangThai=N'Đang thuê' THEN 1 ELSE 0 END)/COUNT(v.MaVP) AS DECIMAL(5,1)) END AS TyLeLapDay
                FROM Tang t LEFT JOIN VanPhong v ON v.MaTang=t.MaTang GROUP BY t.TenTang, t.SoTang ORDER BY t.SoTang");
        }

        public static DataTable Debts()
        {
            return Db.Query(@"SELECT h.SoHD, k.HoTen AS TenKH, ISNULL(c.TenCT, N'') AS TenCT, h.TrangThai, dbo.fn_CongNo(h.MaHD) AS CongNo
                FROM HopDong h JOIN KhachHang k ON k.MaKH=h.MaKH LEFT JOIN CongTy c ON c.MaCT=k.MaCT
                WHERE dbo.fn_CongNo(h.MaHD) > 0 ORDER BY CongNo DESC");
        }
    }
}
