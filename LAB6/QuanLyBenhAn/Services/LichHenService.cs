using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: đặt lịch hẹn khám (bao gồm khám sức khỏe định kỳ).</summary>
    public static class LichHenService
    {
        public static readonly string[] LoaiHen = { "Khám bệnh", "Định kỳ" };
        public static readonly string[] TrangThai = { "Chờ khám", "Đã khám", "Đã hủy" };

        public static DataTable List(string kw)
        {
            kw = (kw ?? "").Trim();
            return Db.Query(
                @"SELECT lh.maLH, lh.maBN, bn.hoTen, lh.ngayGioHen, lh.loaiHen, lh.trangThai, nv.hoTen AS nhanVien
                  FROM LichHen lh JOIN BenhNhan bn ON bn.maBN = lh.maBN JOIN NhanVienCSKH nv ON nv.maNV = lh.maNV
                  WHERE @kw = '' OR lh.maLH LIKE '%' + @kw + '%' OR lh.maBN LIKE '%' + @kw + '%' OR bn.hoTen LIKE N'%' + @kw + '%'
                  ORDER BY lh.ngayGioHen DESC", "@kw", kw);
        }

        /// <summary>Các lịch hẹn đang chờ khám của bệnh nhân sở hữu hồ sơ maHS.</summary>
        public static DataTable PendingByHoSo(string maHS)
        {
            return Db.Query(
                @"SELECT lh.maLH, lh.maLH + N' - ' + CONVERT(NVARCHAR(16), lh.ngayGioHen, 103) + N' ' +
                         CONVERT(NVARCHAR(5), lh.ngayGioHen, 108) + N' - ' + lh.loaiHen AS moTa
                  FROM LichHen lh JOIN HoSoBenhAn hs ON hs.maBN = lh.maBN
                  WHERE hs.maHS = @h AND lh.trangThai = N'Chờ khám' ORDER BY lh.ngayGioHen", "@h", (maHS ?? "").Trim());
        }

        public static string Add(string maBN, string maNV, DateTime ngayGio, string loai)
        {
            if (string.IsNullOrWhiteSpace(maBN)) throw new BusinessException("Hãy nhập mã bệnh nhân.");
            if (string.IsNullOrEmpty(maNV)) throw new BusinessException("Hãy chọn nhân viên tiếp nhận.");
            if (Array.IndexOf(LoaiHen, loai) < 0) throw new BusinessException("Loại hẹn không hợp lệ.");
            if (ngayGio <= DateTime.Now) throw new BusinessException("Ngày giờ hẹn phải ở tương lai.");
            if (ngayGio.Hour < 7 || ngayGio.Hour >= 17) throw new BusinessException("Giờ hẹn phải trong khoảng 07:00 - 17:00.");
            string ma = null;
            Db.InTransaction((cn, tx) =>
            {
                object het = Db.Scalar(cn, tx, "SELECT ngayHetHan FROM HoSoBenhAn WHERE maBN=@b", "@b", maBN.Trim());
                if (het == null) throw new BusinessException("Bệnh nhân chưa có hồ sơ bệnh án, hãy đăng ký trước.");
                if ((DateTime)het < ngayGio.Date)
                    throw new BusinessException("Hồ sơ hết hạn ngày " + ((DateTime)het).ToString("dd/MM/yyyy") + ", không thể hẹn sau ngày này.");
                if (Convert.ToInt32(Db.Scalar(cn, tx,
                        "SELECT COUNT(*) FROM LichHen WHERE maBN=@b AND ngayGioHen=@n AND trangThai<>N'Đã hủy'",
                        "@b", maBN.Trim(), "@n", ngayGio)) > 0)
                    throw new BusinessException("Bệnh nhân đã có lịch hẹn vào đúng thời điểm này.");
                ma = Db.NextId(cn, tx, "LichHen", "maLH", "LH", 4);
                Db.Exec(cn, tx, "INSERT LichHen(maLH, maBN, maNV, ngayGioHen, loaiHen, trangThai) VALUES(@m, @b, @n, @g, @l, N'Chờ khám')",
                    "@m", ma, "@b", maBN.Trim(), "@n", maNV, "@g", ngayGio, "@l", loai);
            });
            return ma;
        }

        public static void UpdateStatus(string maLH, string trangThai)
        {
            if (string.IsNullOrEmpty(maLH)) throw new BusinessException("Hãy chọn lịch hẹn.");
            if (Array.IndexOf(TrangThai, trangThai) < 0) throw new BusinessException("Trạng thái không hợp lệ.");
            Db.Exec("UPDATE LichHen SET trangThai=@t WHERE maLH=@m", "@t", trangThai, "@m", maLH);
        }

        public static void Delete(string maLH)
        {
            if (string.IsNullOrEmpty(maLH)) throw new BusinessException("Hãy chọn lịch hẹn.");
            if (Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM PhieuKham WHERE maLH=@m", "@m", maLH)) > 0)
                throw new BusinessException("Lịch hẹn đã gắn với phiếu khám nên không thể xóa. Hãy đổi trạng thái.");
            Db.Exec("DELETE FROM LichHen WHERE maLH=@m", "@m", maLH);
        }
    }
}
