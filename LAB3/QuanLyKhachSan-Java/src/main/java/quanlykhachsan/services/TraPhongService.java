package quanlykhachsan.services;

import quanlykhachsan.data.Db;
import quanlykhachsan.models.KetQuaXuLy;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.Date;
import java.util.List;
import java.util.Map;

public class TraPhongService {

    public List<Map<String, Object>> layPhieuDangO() {
        String sql = "SELECT pdp.SoPhieuDat, kh.HoTen AS TenKhach, pdp.NgayNhan, pdp.TienCoc " +
                     "FROM PhieuDatPhong pdp " +
                     "JOIN KhachHang kh ON pdp.MaKhach = kh.MaKhach " +
                     "WHERE pdp.TrangThai = N'Đang ở'";
        return Db.query(sql);
    }

    public Map<String, Object> tinhTienPhieu(String soPhieuDat) {
        String sqlPhong = "SELECT SUM(DATEDIFF(DAY, pdp.NgayNhan, GETDATE()) * p.DonGiaNgay) AS TienPhong, " +
                          "MAX(DATEDIFF(DAY, pdp.NgayNhan, GETDATE())) AS SoNgay " +
                          "FROM ChiTietDatPhong ct " +
                          "JOIN Phong p ON ct.SoPhong = p.SoPhong " +
                          "JOIN PhieuDatPhong pdp ON ct.SoPhieuDat = pdp.SoPhieuDat " +
                          "WHERE ct.SoPhieuDat = ?";
        List<Map<String, Object>> resPhong = Db.query(sqlPhong, soPhieuDat);

        String sqlDV = "SELECT ISNULL(SUM(ct.ThanhTien), 0) AS TienDV " +
                       "FROM PhieuSuDungDV p " +
                       "JOIN ChiTietPhieuSuDungDV ct ON p.SoPhieuSDDV = ct.SoPhieuSDDV " +
                       "WHERE p.SoPhieuDat = ?";
        List<Map<String, Object>> resDV = Db.query(sqlDV, soPhieuDat);

        String sqlDB = "SELECT ISNULL(SUM(TongTien), 0) AS TienDenBu FROM PhieuDenBu WHERE SoPhieuDat = ?";
        List<Map<String, Object>> resDB = Db.query(sqlDB, soPhieuDat);

        double tienPhong = 0;
        int soNgay = 1;
        if (!resPhong.isEmpty() && resPhong.get(0).get("TienPhong") != null) {
            tienPhong = Double.parseDouble(String.valueOf(resPhong.get(0).get("TienPhong")));
            soNgay = Integer.parseInt(String.valueOf(resPhong.get(0).get("SoNgay")));
            if (soNgay <= 0) soNgay = 1;
            if (tienPhong <= 0) {
                Object val = Db.scalar("SELECT SUM(p.DonGiaNgay) FROM ChiTietDatPhong ct JOIN Phong p ON ct.SoPhong = p.SoPhong WHERE ct.SoPhieuDat = ?", soPhieuDat);
                tienPhong = val != null ? Double.parseDouble(String.valueOf(val)) : 0;
            }
        }

        double tienDV = (!resDV.isEmpty() && resDV.get(0).get("TienDV") != null) ? Double.parseDouble(String.valueOf(resDV.get(0).get("TienDV"))) : 0;
        double tienDB = (!resDB.isEmpty() && resDB.get(0).get("TienDenBu") != null) ? Double.parseDouble(String.valueOf(resDB.get(0).get("TienDenBu"))) : 0;

        return Map.of("TienPhong", tienPhong, "SoNgay", soNgay, "TienDV", tienDV, "TienDenBu", tienDB);
    }

    public KetQuaXuLy thanhToanVaTraPhong(String soHoaDon, String soPhieuDat, String maNV, int soNgay,
                                          double tienPhong, double tienDV, String maThanhToan, String hinhThuc, double tongThanhToan) {
        try (Connection cn = Db.open()) {
            cn.setAutoCommit(false);
            try {
                Db.execute(cn, "INSERT INTO HoaDon(SoHoaDon, SoPhieuDat, NgayLap, MaNV, SoNgayTinhTien, TienPhong, TienDichVu, TrangThai) " +
                               "VALUES(?,?,?,?,?,?,?,N'Đã thanh toán')",
                        soHoaDon, soPhieuDat, new Date(), maNV, soNgay, tienPhong, tienDV);

                Db.execute(cn, "INSERT INTO ThanhToan(MaThanhToan, SoHoaDon, NgayThanhToan, HinhThuc, SoTien) VALUES(?,?,?,?,?)",
                        maThanhToan, soHoaDon, new Date(), hinhThuc, tongThanhToan);

                Db.execute(cn, "UPDATE PhieuDatPhong SET TrangThai = N'Đã trả', NgayTraThucTe = GETDATE() WHERE SoPhieuDat = ?", soPhieuDat);
                Db.execute(cn, "UPDATE Phong SET TrangThai = N'Trống' WHERE SoPhong IN (SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat = ?)", soPhieuDat);

                cn.commit();
                return KetQuaXuLy.ok("Thanh toán & Trả phòng thành công!");
            } catch (Exception ex) {
                cn.rollback();
                return KetQuaXuLy.loi("Lỗi thanh toán: " + ex.getMessage());
            }
        } catch (SQLException e) {
            return KetQuaXuLy.loi("Lỗi CSDL: " + e.getMessage());
        }
    }
}