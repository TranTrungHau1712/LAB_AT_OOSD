using System;
using System.Data;
using System.Text;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: tra cứu hồ sơ bệnh án và lịch sử khám.</summary>
    public static class HoSoService
    {
        private const string SelectHoSo =
            @"SELECT hs.maHS, bn.maBN, bn.hoTen, bn.ngaySinh, bn.dienThoai, bn.diaChi, hs.ngayLap, hs.ngayHetHan,
                     CASE WHEN hs.ngayHetHan < CAST(GETDATE() AS DATE) THEN N'Hết hạn' ELSE N'Còn hiệu lực' END AS trangThai
              FROM HoSoBenhAn hs JOIN BenhNhan bn ON bn.maBN = hs.maBN ";

        public static DataTable Search(string kw)
        {
            kw = (kw ?? "").Trim();
            return Db.Query(SelectHoSo +
                @"WHERE @kw = '' OR hs.maHS LIKE '%' + @kw + '%' OR bn.maBN LIKE '%' + @kw + '%'
                        OR bn.hoTen LIKE N'%' + @kw + '%' OR bn.dienThoai LIKE '%' + @kw + '%'
                  ORDER BY hs.maHS", "@kw", kw);
        }

        public static DataRow Get(string maHS)
        {
            var dt = Db.Query(SelectHoSo + "WHERE hs.maHS = @m", "@m", (maHS ?? "").Trim());
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static DataTable Visits(string maHS)
        {
            return Db.Query(
                @"SELECT maPK, ngayGioKham, loaiKham, tenBacSi, tenKhoa, chanDoan, ketLuan
                  FROM vw_LichSuKham WHERE maHS = @m ORDER BY ngayGioKham DESC", "@m", maHS);
        }

        public static string VisitDetail(string maPK)
        {
            var sb = new StringBuilder();
            string nl = Environment.NewLine;

            sb.AppendLine("TRIỆU CHỨNG");
            var tc = Db.Query("SELECT tenTrieuChung, giaTri, donVi FROM TrieuChung WHERE maPK=@m ORDER BY maTC", "@m", maPK);
            if (tc.Rows.Count == 0) sb.AppendLine("  (không ghi nhận)");
            foreach (DataRow r in tc.Rows)
                sb.AppendLine("  - " + r["tenTrieuChung"] + ": " + r["giaTri"] + " " + r["donVi"]);

            sb.Append(nl).AppendLine("XÉT NGHIỆM");
            var xn = Db.Query(
                @"SELECT x.tenXN, c.ketQua, c.ghiChu FROM ChiTietXetNghiem c JOIN XetNghiem x ON x.maXN = c.maXN
                  WHERE c.maPK=@m ORDER BY x.tenXN", "@m", maPK);
            if (xn.Rows.Count == 0) sb.AppendLine("  (không có)");
            foreach (DataRow r in xn.Rows)
            {
                string kq = r["ketQua"] == DBNull.Value ? "chưa có kết quả" : r["ketQua"].ToString();
                sb.AppendLine("  - " + r["tenXN"] + ": " + kq);
            }

            sb.Append(nl).AppendLine("TOA THUỐC");
            var toa = Db.Query(
                @"SELECT t.tenThuoc, c.soLuong, t.donViTinh, c.lieuDung, c.cachDung, tt.loiDan
                  FROM ToaThuoc tt JOIN ChiTietToaThuoc c ON c.maToa = tt.maToa JOIN Thuoc t ON t.maThuoc = c.maThuoc
                  WHERE tt.maPK=@m ORDER BY t.tenThuoc", "@m", maPK);
            if (toa.Rows.Count == 0) sb.AppendLine("  (chưa lập toa)");
            foreach (DataRow r in toa.Rows)
                sb.AppendLine("  - " + r["tenThuoc"] + " x" + r["soLuong"] + " " + r["donViTinh"] + " | " + r["lieuDung"] +
                              (r["cachDung"] == DBNull.Value ? "" : " | " + r["cachDung"]));
            if (toa.Rows.Count > 0 && toa.Rows[0]["loiDan"] != DBNull.Value)
                sb.AppendLine("  Lời dặn: " + toa.Rows[0]["loiDan"]);
            return sb.ToString();
        }
    }
}
