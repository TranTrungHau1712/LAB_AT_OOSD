using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    /// <summary>Nghiệp vụ: thanh toán viện phí sau khi khám / xuất viện.</summary>
    public static class HoaDonService
    {
        public static readonly string[] HinhThuc = { "Tiền mặt", "Chuyển khoản", "Thẻ" };

        public static DataTable List()
        {
            return Db.Query(
                @"SELECT pk.maPK, bn.hoTen, pk.ngayGioKham, pk.loaiKham, ISNULL(SUM(x.donGia), 0) AS tienXN,
                         hd.maHD, hd.tongVienPhi, hd.hinhThuc,
                         CASE WHEN hd.maHD IS NULL THEN N'Chưa thanh toán' ELSE N'Đã thanh toán' END AS trangThai
                  FROM PhieuKham pk
                  JOIN HoSoBenhAn hs ON hs.maHS = pk.maHS JOIN BenhNhan bn ON bn.maBN = hs.maBN
                  LEFT JOIN ChiTietXetNghiem c ON c.maPK = pk.maPK
                  LEFT JOIN XetNghiem x ON x.maXN = c.maXN
                  LEFT JOIN HoaDon hd ON hd.maPK = pk.maPK
                  GROUP BY pk.maPK, bn.hoTen, pk.ngayGioKham, pk.loaiKham, hd.maHD, hd.tongVienPhi, hd.hinhThuc
                  ORDER BY pk.ngayGioKham DESC");
        }

        public static string Pay(string maPK, decimal tong, string hinhThuc)
        {
            if (string.IsNullOrEmpty(maPK)) throw new BusinessException("Hãy chọn phiếu khám cần thanh toán.");
            if (tong < 0) throw new BusinessException("Tổng viện phí không được âm.");
            if (Array.IndexOf(HinhThuc, hinhThuc) < 0) throw new BusinessException("Hình thức thanh toán không hợp lệ.");
            string ma = null;
            Db.InTransaction((cn, tx) =>
            {
                object loai = Db.Scalar(cn, tx, "SELECT loaiKham FROM PhieuKham WHERE maPK=@m", "@m", maPK);
                if (loai == null) throw new BusinessException("Không tìm thấy phiếu khám.");
                if (Convert.ToInt32(Db.Scalar(cn, tx, "SELECT COUNT(*) FROM HoaDon WHERE maPK=@m", "@m", maPK)) > 0)
                    throw new BusinessException("Phiếu khám này đã được thanh toán.");
                if ((string)loai == PhieuKhamService.DinhKy && tong > 0)
                    throw new BusinessException("Khám sức khỏe định kỳ được miễn phí, tổng viện phí phải bằng 0.");
                ma = Db.NextId(cn, tx, "HoaDon", "maHD", "HD", 4);
                Db.Exec(cn, tx, "INSERT HoaDon(maHD, maPK, ngayThanhToan, tongVienPhi, hinhThuc) VALUES(@h, @p, @n, @t, @f)",
                    "@h", ma, "@p", maPK, "@n", DateTime.Today, "@t", tong, "@f", hinhThuc);
            });
            return ma;
        }
    }
}
