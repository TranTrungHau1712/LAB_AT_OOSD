package quanlykhachsan.services;

import quanlykhachsan.data.Db;

import java.util.List;
import java.util.Map;

public class ThongKeService {

    public List<Map<String, Object>> thongKeDoanhThuThang(int nam) {
        String sql = "SELECT MONTH(NgayThanhToan) AS Thang, COUNT(MaThanhToan) AS SoLuotThanhToan, SUM(SoTien) AS TongDoanhThu " +
                     "FROM ThanhToan " +
                     "WHERE YEAR(NgayThanhToan) = ? " +
                     "GROUP BY MONTH(NgayThanhToan) ORDER BY Thang";
        return Db.query(sql, nam);
    }

    public List<Map<String, Object>> thongKeDichVuBanChay() {
        String sql = "SELECT dv.TenDV, dv.DonViTinh, SUM(ct.SoLuong) AS TongSoLuong, SUM(ct.ThanhTien) AS TongDoanhThu " +
                     "FROM ChiTietPhieuSuDungDV ct " +
                     "JOIN DichVu dv ON ct.MaDV = dv.MaDV " +
                     "GROUP BY dv.TenDV, dv.DonViTinh ORDER BY TongDoanhThu DESC";
        return Db.query(sql);
    }

    public List<Map<String, Object>> thongKeTanSuatPhong() {
        String sql = "SELECT p.SoPhong, p.TrangThai, COUNT(ct.SoPhieuDat) AS SoLuotDat " +
                     "FROM Phong p " +
                     "LEFT JOIN ChiTietDatPhong ct ON p.SoPhong = ct.SoPhong " +
                     "GROUP BY p.SoPhong, p.TrangThai ORDER BY SoLuotDat DESC";
        return Db.query(sql);
    }
}