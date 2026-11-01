using System.Data;

namespace QuanLyVanPhong.DAL
{
    public static class AuthDal
    {
        public static DataRow Login(string user, string hash)
        {
            var dt = Db.Query("SELECT MaTK, TenDangNhap, HoTen, VaiTro FROM TaiKhoan WHERE TenDangNhap=@u AND MatKhau=@p AND HoatDong=1", "@u", user, "@p", hash);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }
    }

    public static class TangDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query("SELECT MaTang, SoTang, TenTang, MoTa FROM Tang WHERE TenTang LIKE @k OR CAST(SoTang AS NVARCHAR(10)) LIKE @k OR MoTa LIKE @k ORDER BY SoTang", "@k", Db.Like(kw));
        }
        public static DataTable Lookup() { return Db.Query("SELECT MaTang, TenTang FROM Tang ORDER BY SoTang"); }
        public static void Insert(int so, string ten, string mota)
        {
            Db.Exec("INSERT INTO Tang(SoTang,TenTang,MoTa) VALUES(@s,@t,@m)", "@s", so, "@t", ten, "@m", mota);
        }
        public static void Update(int id, int so, string ten, string mota)
        {
            Db.Exec("UPDATE Tang SET SoTang=@s, TenTang=@t, MoTa=@m WHERE MaTang=@id", "@s", so, "@t", ten, "@m", mota, "@id", id);
        }
        public static void Delete(int id) { Db.Exec("DELETE FROM Tang WHERE MaTang=@id", "@id", id); }
    }

    public static class VanPhongDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT v.MaVP, v.MaPhong, v.MaTang, t.TenTang, v.DienTich, v.Huong, v.GiaThue, v.TrangThai
                FROM VanPhong v JOIN Tang t ON t.MaTang=v.MaTang
                WHERE v.MaPhong LIKE @k OR t.TenTang LIKE @k OR v.Huong LIKE @k OR v.TrangThai LIKE @k
                ORDER BY t.SoTang, v.MaPhong", "@k", Db.Like(kw));
        }
        public static DataTable Lookup()
        {
            return Db.Query(@"SELECT v.MaVP, v.MaPhong + N' (' + t.TenTang + N')' AS Ten
                FROM VanPhong v JOIN Tang t ON t.MaTang=v.MaTang ORDER BY t.SoTang, v.MaPhong");
        }
        public static DataTable Available()
        {
            return Db.Query(@"SELECT v.MaVP, v.MaPhong, t.TenTang, v.DienTich, v.Huong, v.GiaThue
                FROM VanPhong v JOIN Tang t ON t.MaTang=v.MaTang
                WHERE v.TrangThai=N'Trống' ORDER BY t.SoTang, v.MaPhong");
        }
        public static string GetStatus(int id)
        {
            object o = Db.Scalar("SELECT TrangThai FROM VanPhong WHERE MaVP=@id", "@id", id);
            return o == null ? null : o.ToString();
        }
        public static void Insert(string ma, int tang, decimal dt, string huong, decimal gia, string tt)
        {
            Db.Exec("INSERT INTO VanPhong(MaPhong,MaTang,DienTich,Huong,GiaThue,TrangThai) VALUES(@m,@t,@d,@h,@g,@s)",
                "@m", ma, "@t", tang, "@d", dt, "@h", huong, "@g", gia, "@s", tt);
        }
        public static void Update(int id, string ma, int tang, decimal dt, string huong, decimal gia, string tt)
        {
            Db.Exec("UPDATE VanPhong SET MaPhong=@m, MaTang=@t, DienTich=@d, Huong=@h, GiaThue=@g, TrangThai=@s WHERE MaVP=@id",
                "@m", ma, "@t", tang, "@d", dt, "@h", huong, "@g", gia, "@s", tt, "@id", id);
        }
        public static void Delete(int id) { Db.Exec("DELETE FROM VanPhong WHERE MaVP=@id", "@id", id); }
    }

    public static class CongTyDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT MaCT, TenCT, MaSoThue, DiaChi, SDT, Email, NguoiDaiDien FROM CongTy
                WHERE TenCT LIKE @k OR MaSoThue LIKE @k OR NguoiDaiDien LIKE @k OR SDT LIKE @k ORDER BY TenCT", "@k", Db.Like(kw));
        }
        public static DataTable Lookup() { return Db.Query("SELECT MaCT, TenCT FROM CongTy ORDER BY TenCT"); }
        public static void Insert(string ten, string mst, string dc, string sdt, string email, string dd)
        {
            Db.Exec("INSERT INTO CongTy(TenCT,MaSoThue,DiaChi,SDT,Email,NguoiDaiDien) VALUES(@a,@b,@c,@d,@e,@f)",
                "@a", ten, "@b", mst, "@c", dc, "@d", sdt, "@e", email, "@f", dd);
        }
        public static void Update(int id, string ten, string mst, string dc, string sdt, string email, string dd)
        {
            Db.Exec("UPDATE CongTy SET TenCT=@a, MaSoThue=@b, DiaChi=@c, SDT=@d, Email=@e, NguoiDaiDien=@f WHERE MaCT=@id",
                "@a", ten, "@b", mst, "@c", dc, "@d", sdt, "@e", email, "@f", dd, "@id", id);
        }
        public static void Delete(int id) { Db.Exec("DELETE FROM CongTy WHERE MaCT=@id", "@id", id); }
    }

    public static class KhachHangDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query(@"SELECT k.MaKH, k.HoTen, k.CCCD, k.SDT, k.Email, k.DiaChi, k.MaCT, c.TenCT
                FROM KhachHang k LEFT JOIN CongTy c ON c.MaCT=k.MaCT
                WHERE k.HoTen LIKE @k OR k.CCCD LIKE @k OR k.SDT LIKE @k OR c.TenCT LIKE @k ORDER BY k.HoTen", "@k", Db.Like(kw));
        }
        public static DataTable Lookup()
        {
            return Db.Query(@"SELECT k.MaKH, k.HoTen + CASE WHEN c.TenCT IS NULL THEN N'' ELSE N' - ' + c.TenCT END AS Ten
                FROM KhachHang k LEFT JOIN CongTy c ON c.MaCT=k.MaCT ORDER BY k.HoTen");
        }
        public static void Insert(string ten, string cccd, string sdt, string email, string dc, object maCT)
        {
            Db.Exec("INSERT INTO KhachHang(HoTen,CCCD,SDT,Email,DiaChi,MaCT) VALUES(@a,@b,@c,@d,@e,@f)",
                "@a", ten, "@b", cccd, "@c", sdt, "@d", email, "@e", dc, "@f", maCT);
        }
        public static void Update(int id, string ten, string cccd, string sdt, string email, string dc, object maCT)
        {
            Db.Exec("UPDATE KhachHang SET HoTen=@a, CCCD=@b, SDT=@c, Email=@d, DiaChi=@e, MaCT=@f WHERE MaKH=@id",
                "@a", ten, "@b", cccd, "@c", sdt, "@d", email, "@e", dc, "@f", maCT, "@id", id);
        }
        public static void Delete(int id) { Db.Exec("DELETE FROM KhachHang WHERE MaKH=@id", "@id", id); }
    }

    public static class DichVuDal
    {
        public static DataTable Search(string kw)
        {
            return Db.Query("SELECT MaDV, TenDV, DonVi, DonGia, LoaiTinh FROM DichVu WHERE TenDV LIKE @k OR LoaiTinh LIKE @k ORDER BY MaDV", "@k", Db.Like(kw));
        }
        public static DataTable All() { return Db.Query("SELECT MaDV, TenDV, DonVi, DonGia, LoaiTinh FROM DichVu ORDER BY MaDV"); }
        public static void Insert(string ten, string dv, decimal gia, string loai)
        {
            Db.Exec("INSERT INTO DichVu(TenDV,DonVi,DonGia,LoaiTinh) VALUES(@a,@b,@c,@d)", "@a", ten, "@b", dv, "@c", gia, "@d", loai);
        }
        public static void Update(int id, string ten, string dv, decimal gia, string loai)
        {
            Db.Exec("UPDATE DichVu SET TenDV=@a, DonVi=@b, DonGia=@c, LoaiTinh=@d WHERE MaDV=@id", "@a", ten, "@b", dv, "@c", gia, "@d", loai, "@id", id);
        }
        public static void Delete(int id) { Db.Exec("DELETE FROM DichVu WHERE MaDV=@id", "@id", id); }
    }
}
