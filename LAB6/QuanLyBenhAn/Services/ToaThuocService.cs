using System;
using System.Collections.Generic;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: lập / sửa toa thuốc cho một phiếu khám.</summary>
    public static class ToaThuocService
    {
        public static DataRow PhieuInfo(string maPK)
        {
            var dt = Db.Query(
                @"SELECT pk.maPK, bn.hoTen, pk.ngayGioKham, pk.loaiKham, pk.chanDoan
                  FROM PhieuKham pk JOIN HoSoBenhAn hs ON hs.maHS = pk.maHS JOIN BenhNhan bn ON bn.maBN = hs.maBN
                  WHERE pk.maPK = @m", "@m", (maPK ?? "").Trim());
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public static DataTable GetChiTiet(string maPK)
        {
            return Db.Query(
                @"SELECT c.maThuoc, t.tenThuoc, c.soLuong, c.lieuDung, c.cachDung
                  FROM ToaThuoc tt JOIN ChiTietToaThuoc c ON c.maToa = tt.maToa JOIN Thuoc t ON t.maThuoc = c.maThuoc
                  WHERE tt.maPK = @m ORDER BY t.tenThuoc", "@m", maPK);
        }

        public static string GetLoiDan(string maPK)
        {
            return Db.Scalar("SELECT loiDan FROM ToaThuoc WHERE maPK=@m", "@m", maPK) as string;
        }

        /// <summary>Lưu toa (tạo mới hoặc ghi đè toa hiện có). Trả về mã toa.</summary>
        public static string Save(string maPK, string loiDan, List<ChiTietToaInput> ds)
        {
            if (ds == null || ds.Count == 0) throw new BusinessException("Toa thuốc phải có ít nhất một loại thuốc.");
            var seen = new HashSet<string>();
            foreach (var c in ds)
            {
                if (c.SoLuong <= 0) throw new BusinessException("Số lượng thuốc phải lớn hơn 0.");
                if (string.IsNullOrWhiteSpace(c.LieuDung)) throw new BusinessException("Hãy nhập liều dùng cho từng loại thuốc.");
                if (!seen.Add(c.MaThuoc)) throw new BusinessException("Một loại thuốc không được kê hai lần trong cùng toa.");
            }
            string maToa = null;
            Db.InTransaction((cn, tx) =>
            {
                if (Convert.ToInt32(Db.Scalar(cn, tx, "SELECT COUNT(*) FROM PhieuKham WHERE maPK=@m", "@m", maPK)) == 0)
                    throw new BusinessException("Không tìm thấy phiếu khám " + maPK + ".");

                maToa = Db.Scalar(cn, tx, "SELECT maToa FROM ToaThuoc WHERE maPK=@m", "@m", maPK) as string;
                if (maToa == null)
                {
                    maToa = Db.NextId(cn, tx, "ToaThuoc", "maToa", "TO", 4);
                    Db.Exec(cn, tx, "INSERT ToaThuoc(maToa, maPK, ngayKe, loiDan) VALUES(@t, @p, @n, @l)",
                        "@t", maToa, "@p", maPK, "@n", DateTime.Today, "@l", loiDan);
                }
                else
                {
                    Db.Exec(cn, tx, "UPDATE ToaThuoc SET loiDan=@l, ngayKe=@n WHERE maToa=@t",
                        "@l", loiDan, "@n", DateTime.Today, "@t", maToa);
                    Db.Exec(cn, tx, "DELETE FROM ChiTietToaThuoc WHERE maToa=@t", "@t", maToa);
                }
                foreach (var c in ds)
                    Db.Exec(cn, tx,
                        "INSERT ChiTietToaThuoc(maToa, maThuoc, soLuong, lieuDung, cachDung) VALUES(@t, @m, @s, @l, @c)",
                        "@t", maToa, "@m", c.MaThuoc, "@s", c.SoLuong, "@l", c.LieuDung.Trim(), "@c", c.CachDung);
            });
            return maToa;
        }
    }
}
