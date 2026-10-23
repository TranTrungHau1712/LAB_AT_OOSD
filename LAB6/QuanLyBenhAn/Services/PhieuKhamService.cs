using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: lập phiếu khám (kể cả khám định kỳ) và nhập kết quả xét nghiệm.</summary>
    public static class PhieuKhamService
    {
        public const string Dieutri = "Điều trị";
        public const string DinhKy = "Định kỳ";

        public static string Create(PhieuKhamInput x)
        {
            if (string.IsNullOrWhiteSpace(x.MaHS)) throw new BusinessException("Hãy nhập mã hồ sơ bệnh án.");
            if (x.Loai != Dieutri && x.Loai != DinhKy) throw new BusinessException("Loại khám không hợp lệ.");
            if (string.IsNullOrEmpty(x.MaKhoa)) throw new BusinessException("Hãy chọn khoa điều trị.");
            if (string.IsNullOrEmpty(x.MaBS)) throw new BusinessException("Hãy chọn bác sĩ khám.");
            if (string.IsNullOrEmpty(x.MaNV)) throw new BusinessException("Hãy chọn nhân viên ghi nhận.");
            if (x.Loai == Dieutri && string.IsNullOrWhiteSpace(x.ChanDoan))
                throw new BusinessException("Khám điều trị phải có chẩn đoán.");
            if (x.NgayGio > DateTime.Now.AddMinutes(5)) throw new BusinessException("Ngày giờ khám không được ở tương lai.");

            string chanDoan = string.IsNullOrWhiteSpace(x.ChanDoan) ? "Không phát hiện bệnh" : x.ChanDoan.Trim();
            var xnSeen = new System.Collections.Generic.HashSet<string>();
            foreach (var t in x.TrieuChung)
                if (string.IsNullOrWhiteSpace(t.Ten)) throw new BusinessException("Triệu chứng không được để trống tên.");
            foreach (var t in x.XetNghiem)
                if (!xnSeen.Add(t.MaXN)) throw new BusinessException("Một xét nghiệm không được chỉ định hai lần.");

            string maPK = null;
            Db.InTransaction((cn, tx) =>
            {
                object het = Db.Scalar(cn, tx, "SELECT ngayHetHan FROM HoSoBenhAn WHERE maHS=@h", "@h", x.MaHS.Trim());
                if (het == null) throw new BusinessException("Không tìm thấy hồ sơ bệnh án " + x.MaHS + ".");
                if (((DateTime)het) < x.NgayGio.Date)
                    throw new BusinessException("Hồ sơ đã hết hạn ngày " + ((DateTime)het).ToString("dd/MM/yyyy") +
                                                ". Bệnh nhân cần đóng lệ phí năm mới.");

                if (Convert.ToInt32(Db.Scalar(cn, tx, "SELECT COUNT(*) FROM BacSi WHERE maBS=@b AND maKhoa=@k",
                        "@b", x.MaBS, "@k", x.MaKhoa)) == 0)
                    throw new BusinessException("Bác sĩ được chọn không thuộc khoa điều trị đã chọn.");

                if (x.Loai == DinhKy)
                {
                    int daKham = Convert.ToInt32(Db.Scalar(cn, tx,
                        @"SELECT COUNT(*) FROM PhieuKham WHERE maHS=@h AND loaiKham=N'Định kỳ'
                          AND YEAR(ngayGioKham)=@y AND DATEPART(QUARTER, ngayGioKham)=@q",
                        "@h", x.MaHS, "@y", x.NgayGio.Year, "@q", (x.NgayGio.Month - 1) / 3 + 1));
                    if (daKham > 0)
                        throw new BusinessException("Bệnh nhân đã khám định kỳ trong quý này (mỗi quý được miễn phí 1 lần).");
                }

                if (!string.IsNullOrEmpty(x.MaLH))
                {
                    int ok = Convert.ToInt32(Db.Scalar(cn, tx,
                        @"SELECT COUNT(*) FROM LichHen lh JOIN HoSoBenhAn hs ON hs.maBN = lh.maBN
                          WHERE lh.maLH=@l AND hs.maHS=@h AND lh.trangThai = N'Chờ khám'", "@l", x.MaLH, "@h", x.MaHS));
                    if (ok == 0) throw new BusinessException("Lịch hẹn không hợp lệ hoặc không còn ở trạng thái 'Chờ khám'.");
                }

                maPK = Db.NextId(cn, tx, "PhieuKham", "maPK", "PK", 4);
                Db.Exec(cn, tx,
                    @"INSERT PhieuKham(maPK, maHS, maBS, maKhoa, maNV, maLH, ngayGioKham, loaiKham, chanDoan, ketLuan)
                      VALUES(@pk, @hs, @bs, @k, @nv, @lh, @ng, @lo, @cd, @kl)",
                    "@pk", maPK, "@hs", x.MaHS.Trim(), "@bs", x.MaBS, "@k", x.MaKhoa, "@nv", x.MaNV,
                    "@lh", string.IsNullOrEmpty(x.MaLH) ? null : x.MaLH, "@ng", x.NgayGio, "@lo", x.Loai,
                    "@cd", chanDoan, "@kl", x.KetLuan);

                foreach (var t in x.TrieuChung)
                {
                    string maTC = Db.NextId(cn, tx, "TrieuChung", "maTC", "TC", 4);
                    Db.Exec(cn, tx, "INSERT TrieuChung(maTC, maPK, tenTrieuChung, giaTri, donVi) VALUES(@tc, @pk, @t, @g, @d)",
                        "@tc", maTC, "@pk", maPK, "@t", t.Ten.Trim(), "@g", t.GiaTri, "@d", t.DonVi);
                }
                foreach (var t in x.XetNghiem)
                {
                    bool coKq = !string.IsNullOrWhiteSpace(t.KetQua);
                    Db.Exec(cn, tx, "INSERT ChiTietXetNghiem(maPK, maXN, ngayXN, ketQua, ghiChu) VALUES(@pk, @xn, @n, @k, @g)",
                        "@pk", maPK, "@xn", t.MaXN, "@n", coKq ? (object)DateTime.Now : null,
                        "@k", coKq ? t.KetQua.Trim() : null, "@g", t.GhiChu);
                }
                if (!string.IsNullOrEmpty(x.MaLH))
                    Db.Exec(cn, tx, "UPDATE LichHen SET trangThai = N'Đã khám' WHERE maLH=@l", "@l", x.MaLH);
            });
            return maPK;
        }

        // ---- Kết quả xét nghiệm ----
        public static DataTable LoadXetNghiem(string maPK)
        {
            return Db.Query(
                @"SELECT c.maXN, x.tenXN, c.ngayXN, c.ketQua, c.ghiChu
                  FROM ChiTietXetNghiem c JOIN XetNghiem x ON x.maXN = c.maXN
                  WHERE c.maPK = @m ORDER BY x.tenXN", "@m", (maPK ?? "").Trim());
        }

        public static void SaveKetQua(string maPK, DataTable dt)
        {
            Db.InTransaction((cn, tx) =>
            {
                foreach (DataRow r in dt.Rows)
                {
                    string kq = r["ketQua"] == DBNull.Value ? null : r["ketQua"].ToString().Trim();
                    if (kq == "") kq = null;
                    string gc = r["ghiChu"] == DBNull.Value ? null : r["ghiChu"].ToString().Trim();
                    if (gc == "") gc = null;
                    Db.Exec(cn, tx,
                        @"UPDATE ChiTietXetNghiem SET ketQua=@k, ghiChu=@g,
                              ngayXN = CASE WHEN @k IS NOT NULL AND ngayXN IS NULL THEN GETDATE() ELSE ngayXN END
                          WHERE maPK=@pk AND maXN=@xn", "@k", kq, "@g", gc, "@pk", maPK, "@xn", r["maXN"].ToString());
                }
            });
        }
    }
}
