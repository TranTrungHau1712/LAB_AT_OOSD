package quanlykhachsan.services;

import quanlykhachsan.data.Db;
import quanlykhachsan.models.KetQuaXuLy;

import java.sql.Date;
import java.util.List;
import java.util.Map;

public class PhongTienNghiService {

    public List<Map<String, Object>> layPhong() {
        return Db.query(
            "SELECT p.*, k.TenKhuVuc FROM Phong p " +
            "JOIN KhuVuc k ON p.MaKhuVuc = k.MaKhuVuc " +
            "ORDER BY p.SoPhong");
    }

    public List<Map<String, Object>> layTienNghi() {
        return Db.query(
            "SELECT t.*, l.TenLoaiTN FROM TienNghi t " +
            "JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN " +
            "ORDER BY t.MaTienNghi");
    }

    public List<Map<String, Object>> layLapDat() {
        return Db.query(
            "SELECT p.*, l.TenLoaiTN FROM PhieuLapDat p " +
            "JOIN TienNghi t ON p.MaTienNghi = t.MaTienNghi " +
            "JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN " +
            "ORDER BY p.NgayLap DESC");
    }

    public KetQuaXuLy themPhong(String soPhong, String maKhuVuc, int soNguoiToiDa, double donGia) {
        if (isBlank(soPhong) || isBlank(maKhuVuc) || soNguoiToiDa <= 0 || donGia < 0)
            return KetQuaXuLy.fail("Thông tin phòng không hợp lệ.");
        try {
            Db.execute(
                "INSERT INTO Phong(SoPhong,MaKhuVuc,SoNguoiToiDa,DonGiaNgay,TrangThai) " +
                "VALUES(?,?,?,?,N'Trống')",
                soPhong, maKhuVuc, soNguoiToiDa, donGia);
            return KetQuaXuLy.ok("Đã thêm phòng.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    public KetQuaXuLy themTienNghi(String maTienNghi, String maLoaiTN, int soThuTu, String tinhTrang) {
        if (isBlank(maTienNghi) || isBlank(maLoaiTN) || soThuTu <= 0)
            return KetQuaXuLy.fail("Thông tin tiện nghi không hợp lệ.");
        try {
            Db.execute(
                "INSERT INTO TienNghi VALUES(?,?,?,?)",
                maTienNghi, maLoaiTN, soThuTu, tinhTrang);
            return KetQuaXuLy.ok("Đã thêm tiện nghi.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    /** BR04: một thiết bị chỉ được trang bị cho một phòng trong một ngày (UNIQUE MaTienNghi,NgayLap). */
    public KetQuaXuLy lapDat(String soPhieuLapDat, String maTienNghi, String soPhong,
                              Date ngayLap, String tinhTrang, String maNV, String ghiChu) {
        if (isBlank(soPhieuLapDat) || isBlank(maTienNghi) || isBlank(soPhong)
                || isBlank(tinhTrang) || isBlank(maNV))
            return KetQuaXuLy.fail("Phiếu lắp đặt chưa đủ thông tin.");
        try {
            Db.execute(
                "INSERT INTO PhieuLapDat VALUES(?,?,?,?,?,?,?)",
                soPhieuLapDat, maTienNghi, soPhong, ngayLap, tinhTrang, maNV, ghiChu);
            // cập nhật tình trạng hiện tại của tiện nghi
            Db.execute(
                "UPDATE TienNghi SET TinhTrangHienTai=? WHERE MaTienNghi=?",
                tinhTrang, maTienNghi);
            return KetQuaXuLy.ok("Đã lập phiếu lắp đặt.");
        } catch (RuntimeException e) {
            Throwable cause = e.getCause();
            if (cause instanceof java.sql.SQLException) {
                java.sql.SQLException sqlEx = (java.sql.SQLException) cause;
                if (sqlEx.getErrorCode() == 2627 || sqlEx.getErrorCode() == 2601) {
                    return KetQuaXuLy.fail("Thiết bị này đã được lắp cho một phòng khác trong ngày đã chọn.");
                }
            }
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    private boolean isBlank(String s) {
        return s == null || s.trim().isEmpty();
    }
}