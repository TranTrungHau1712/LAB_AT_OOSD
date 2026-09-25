package quanlykhachsan.services;

import quanlykhachsan.data.Db;
import quanlykhachsan.models.KetQuaXuLy;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.Date;
import java.util.List;
import java.util.Map;

public class DichVuService {

    public List<Map<String, Object>> layDanhSachDichVu() {
        return Db.query("SELECT MaDV, TenDV, DonViTinh, DonGia FROM DichVu");
    }

    public List<Map<String, Object>> layPhongDangO() {
        String sql = "SELECT ctdp.SoPhieuDat, ctdp.SoPhong, kh.HoTen AS TenKhach " +
                     "FROM ChiTietDatPhong ctdp " +
                     "JOIN PhieuDatPhong pdp ON ctdp.SoPhieuDat = pdp.SoPhieuDat " +
                     "JOIN KhachHang kh ON pdp.MaKhach = kh.MaKhach " +
                     "WHERE pdp.TrangThai = N'Đang ở'";
        return Db.query(sql);
    }

    public List<Map<String, Object>> layDichVuDaDungByPhieu(String soPhieuDat) {
        String sql = "SELECT p.SoPhieuSDDV, p.SoPhong, p.NgaySuDung, dv.TenDV, ct.SoLuong, ct.DonGia, ct.ThanhTien " +
                     "FROM PhieuSuDungDV p " +
                     "JOIN ChiTietPhieuSuDungDV ct ON p.SoPhieuSDDV = ct.SoPhieuSDDV " +
                     "JOIN DichVu dv ON ct.MaDV = dv.MaDV " +
                     "WHERE p.SoPhieuDat = ? ORDER BY p.NgaySuDung DESC";
        return Db.query(sql, soPhieuDat);
    }

    public KetQuaXuLy themDichVu(String soPhieuSDDV, String soPhieuDat, String soPhong, 
                                 String maDV, int soLuong, double donGia, String maNV) {
        if (soPhieuSDDV.isEmpty() || soPhieuDat.isEmpty() || soPhong.isEmpty() || maDV.isEmpty()) {
            return KetQuaXuLy.loi("Vui lòng điền đầy đủ thông tin!");
        }
        if (soLuong <= 0) {
            return KetQuaXuLy.loi("Số lượng dịch vụ phải lớn hơn 0!");
        }

        try (Connection cn = Db.open()) {
            cn.setAutoCommit(false);
            try {
                Object exist = Db.scalar(cn, "SELECT SoPhieuSDDV FROM PhieuSuDungDV WHERE SoPhieuSDDV = ?", soPhieuSDDV);
                if (exist == null) {
                    Db.execute(cn, "INSERT INTO PhieuSuDungDV(SoPhieuSDDV, SoPhieuDat, SoPhong, NgaySuDung, MaNV) VALUES(?,?,?,?,?)",
                            soPhieuSDDV, soPhieuDat, soPhong, new Date(), maNV);
                }

                Db.execute(cn, "INSERT INTO ChiTietPhieuSuDungDV(SoPhieuSDDV, MaDV, SoLuong, DonGia) VALUES(?,?,?,?)",
                        soPhieuSDDV, maDV, soLuong, donGia);

                cn.commit();
                return KetQuaXuLy.ok("Thêm dịch vụ thành công!");
            } catch (Exception ex) {
                cn.rollback();
                return KetQuaXuLy.loi("Lỗi ghi nhận dịch vụ: " + ex.getMessage());
            }
        } catch (SQLException e) {
            return KetQuaXuLy.loi("Lỗi kết nối CSDL: " + e.getMessage());
        }
    }
}