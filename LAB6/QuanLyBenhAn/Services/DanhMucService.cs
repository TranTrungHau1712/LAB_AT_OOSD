using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Danh mục: khoa, bác sĩ, nhân viên CSKH, thuốc, xét nghiệm.</summary>
    public static class DanhMucService
    {
        public static readonly string[] Bang = { "Khoa", "BacSi", "NhanVienCSKH", "Thuoc", "XetNghiem" };

        private static void Check(string table)
        {
            if (Array.IndexOf(Bang, table) < 0) throw new BusinessException("Bảng danh mục không hợp lệ.");
        }

        public static DataTable Load(string table)
        {
            Check(table);
            return Db.Query("SELECT * FROM " + table);
        }

        public static void Save(string table, DataTable dt)
        {
            Check(table);
            using (var cn = Db.Open())
            using (var da = new SqlDataAdapter("SELECT * FROM " + table, cn))
            using (var cb = new SqlCommandBuilder(da))
            {
                da.Update(dt);
            }
        }

        public static DataTable Khoa() { return Db.Query("SELECT maKhoa, tenKhoa FROM Khoa ORDER BY tenKhoa"); }

        public static DataTable BacSi(string maKhoa)
        {
            return Db.Query("SELECT maBS, hoTen FROM BacSi WHERE maKhoa=@k ORDER BY hoTen", "@k", maKhoa ?? "");
        }

        public static DataTable NhanVien() { return Db.Query("SELECT maNV, hoTen FROM NhanVienCSKH ORDER BY hoTen"); }

        public static DataTable Thuoc(string kw)
        {
            return Db.Query(
                @"SELECT maThuoc, tenThuoc, tenThuoc + N' (' + donViTinh + N')' AS ten, chiDinh, tacDungPhu
                  FROM Thuoc WHERE @kw = '' OR tenThuoc LIKE N'%' + @kw + '%' OR maThuoc LIKE '%' + @kw + '%'
                  ORDER BY tenThuoc", "@kw", (kw ?? "").Trim());
        }

        public static DataTable XetNghiem()
        {
            return Db.Query("SELECT maXN, tenXN, tenXN + N' - ' + FORMAT(donGia, 'N0') + N'đ' AS ten FROM XetNghiem ORDER BY tenXN");
        }
    }
}
