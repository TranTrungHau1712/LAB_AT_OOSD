using System;
using System.Data;
using System.Text.RegularExpressions;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: bệnh nhân, phiếu đăng ký, đóng lệ phí, cấp/gia hạn hồ sơ bệnh án.</summary>
    public static class BenhNhanService
    {
        private static readonly Regex Phone = new Regex(@"^0\d{9,10}$");

        public static DataTable Search(string kw)
        {
            kw = (kw ?? "").Trim();
            return Db.Query(
                @"SELECT bn.maBN, bn.hoTen, bn.ngaySinh, bn.dienThoai, bn.diaChi, hs.maHS
                  FROM BenhNhan bn LEFT JOIN HoSoBenhAn hs ON hs.maBN = bn.maBN
                  WHERE @kw = '' OR bn.maBN LIKE '%' + @kw + '%' OR bn.hoTen LIKE N'%' + @kw + '%'
                        OR bn.dienThoai LIKE '%' + @kw + '%' OR hs.maHS LIKE '%' + @kw + '%'
                  ORDER BY bn.maBN", "@kw", kw);
        }

        public static byte[] GetPhoto(string maBN)
        {
            return Db.Scalar("SELECT anh3x4 FROM BenhNhan WHERE maBN = @m", "@m", maBN) as byte[];
        }

        private static void Validate(string hoTen, string dienThoai, DateTime? ngaySinh, byte[] anh)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) throw new BusinessException("Họ tên không được để trống.");
            if (!string.IsNullOrWhiteSpace(dienThoai) && !Phone.IsMatch(dienThoai.Trim()))
                throw new BusinessException("Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.");
            if (ngaySinh.HasValue && ngaySinh.Value.Date > DateTime.Today)
                throw new BusinessException("Ngày sinh không được lớn hơn ngày hiện tại.");
            if (anh != null && anh.Length > 2 * 1024 * 1024)
                throw new BusinessException("Ảnh 3x4 tối đa 2 MB.");
        }

        public static string Insert(string hoTen, string diaChi, DateTime? ngaySinh, string dienThoai, byte[] anh)
        {
            Validate(hoTen, dienThoai, ngaySinh, anh);
            string ma = null;
            Db.InTransaction((cn, tx) =>
            {
                ma = Db.NextId(cn, tx, "BenhNhan", "maBN", "BN", 4);
                Db.Exec(cn, tx,
                    "INSERT BenhNhan(maBN, hoTen, diaChi, ngaySinh, dienThoai, anh3x4) VALUES(@m, @t, @d, @s, @p, @anh)",
                    "@m", ma, "@t", hoTen.Trim(), "@d", diaChi, "@s", ngaySinh, "@p", dienThoai, "@anh", anh);
            });
            return ma;
        }

        public static void Update(string maBN, string hoTen, string diaChi, DateTime? ngaySinh, string dienThoai, byte[] anh)
        {
            if (string.IsNullOrEmpty(maBN)) throw new BusinessException("Hãy chọn bệnh nhân cần cập nhật.");
            Validate(hoTen, dienThoai, ngaySinh, anh);
            Db.Exec("UPDATE BenhNhan SET hoTen=@t, diaChi=@d, ngaySinh=@s, dienThoai=@p, anh3x4=@anh WHERE maBN=@m",
                "@m", maBN, "@t", hoTen.Trim(), "@d", diaChi, "@s", ngaySinh, "@p", dienThoai, "@anh", anh);
        }

        public static void Delete(string maBN)
        {
            if (string.IsNullOrEmpty(maBN)) throw new BusinessException("Hãy chọn bệnh nhân cần xóa.");
            if (Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM HoSoBenhAn WHERE maBN=@m", "@m", maBN)) > 0)
                throw new BusinessException("Bệnh nhân đã có hồ sơ bệnh án nên không thể xóa.");
            if (Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM LichHen WHERE maBN=@m", "@m", maBN)) > 0)
                throw new BusinessException("Bệnh nhân còn lịch hẹn nên không thể xóa.");
            Db.InTransaction((cn, tx) =>
            {
                Db.Exec(cn, tx, "DELETE FROM PhieuDangKy WHERE maBN=@m", "@m", maBN);
                Db.Exec(cn, tx, "DELETE FROM BenhNhan WHERE maBN=@m", "@m", maBN);
            });
        }

        /// <summary>Lập phiếu đăng ký + thu lệ phí năm; cấp hồ sơ mới hoặc gia hạn hồ sơ hiện có. Trả về mã hồ sơ.</summary>
        public static string DangKyHoSo(string maBN, decimal lePhi, int namHieuLuc)
        {
            if (string.IsNullOrEmpty(maBN)) throw new BusinessException("Hãy chọn bệnh nhân cần đăng ký.");
            if (lePhi <= 0) throw new BusinessException("Lệ phí hằng năm phải lớn hơn 0.");
            if (namHieuLuc < DateTime.Today.Year) throw new BusinessException("Năm hiệu lực không được nhỏ hơn năm hiện tại.");
            string maHS = null;
            Db.InTransaction((cn, tx) =>
            {
                if (Convert.ToInt32(Db.Scalar(cn, tx, "SELECT COUNT(*) FROM BenhNhan WHERE maBN=@m", "@m", maBN)) == 0)
                    throw new BusinessException("Không tìm thấy bệnh nhân.");
                DateTime het = new DateTime(namHieuLuc, 12, 31);
                object cur = Db.Scalar(cn, tx, "SELECT ngayHetHan FROM HoSoBenhAn WHERE maBN=@m", "@m", maBN);
                if (cur != null && (DateTime)cur >= het)
                    throw new BusinessException("Hồ sơ đã có hiệu lực đến " + ((DateTime)cur).ToString("dd/MM/yyyy") +
                        ", không cần đóng thêm lệ phí cho năm " + namHieuLuc + ".");

                string maPhieu = Db.NextId(cn, tx, "PhieuDangKy", "maPhieu", "PD", 4);
                Db.Exec(cn, tx,
                    "INSERT PhieuDangKy(maPhieu, maBN, ngayDangKy, namHieuLuc, lePhiNam, daThanhToan) VALUES(@p, @b, @n, @y, @l, 1)",
                    "@p", maPhieu, "@b", maBN, "@n", DateTime.Today, "@y", namHieuLuc, "@l", lePhi);

                if (cur == null)
                {
                    maHS = Db.NextId(cn, tx, "HoSoBenhAn", "maHS", "HS", 4);
                    Db.Exec(cn, tx,
                        "INSERT HoSoBenhAn(maHS, maBN, maPhieu, ngayLap, ngayHetHan, trangThai) VALUES(@h, @b, @p, @n, @e, N'Còn hiệu lực')",
                        "@h", maHS, "@b", maBN, "@p", maPhieu, "@n", DateTime.Today, "@e", het);
                }
                else
                {
                    maHS = (string)Db.Scalar(cn, tx, "SELECT maHS FROM HoSoBenhAn WHERE maBN=@b", "@b", maBN);
                    Db.Exec(cn, tx,
                        "UPDATE HoSoBenhAn SET maPhieu=@p, ngayHetHan=@e, trangThai=N'Còn hiệu lực' WHERE maHS=@h",
                        "@p", maPhieu, "@e", het, "@h", maHS);
                }
            });
            return maHS;
        }
    }
}
