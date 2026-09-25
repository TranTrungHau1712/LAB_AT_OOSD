package quanlykhachsan.services;

import quanlykhachsan.data.Db;
import quanlykhachsan.models.KetQuaXuLy;

import java.sql.Connection;
import java.sql.Date;
import java.sql.SQLException;
import java.util.List;
import java.util.Map;

public class DatPhongService {

    public List<Map<String, Object>> layKhach() {
        return Db.query("SELECT * FROM KhachHang ORDER BY HoTen");
    }

    public List<Map<String, Object>> layPhong() {
        return Db.query(
            "SELECT p.*, k.TenKhuVuc FROM Phong p " +
            "JOIN KhuVuc k ON p.MaKhuVuc = k.MaKhuVuc ORDER BY p.SoPhong");
    }

    public List<Map<String, Object>> layPhieuDat() {
        return Db.query(
            "SELECT d.*, k.HoTen FROM PhieuDatPhong d " +
            "JOIN KhachHang k ON d.MaKhach = k.MaKhach ORDER BY d.NgayLap DESC");
    }

    public List<Map<String, Object>> layChiTiet(String soPhieuDat) {
        return Db.query(
            "SELECT c.*, p.SoNguoiToiDa, p.DonGiaNgay FROM ChiTietDatPhong c " +
            "JOIN Phong p ON c.SoPhong = p.SoPhong WHERE c.SoPhieuDat=?",
            soPhieuDat);
    }

    public List<Map<String, Object>> layNguoiLuuTru(String soPhieuDat) {
        return Db.query(
            "SELECT * FROM NguoiLuuTru WHERE SoPhieuDat=? ORDER BY SoPhong,MaNguoiLT",
            soPhieuDat);
    }

    public KetQuaXuLy themKhach(String ma, String ten, String cmnd, String quocTich, String sdt) {
        if (isBlank(ma) || isBlank(ten) || isBlank(cmnd) || isBlank(quocTich))
            return KetQuaXuLy.fail("Thông tin khách chưa đầy đủ.");
        try {
            Db.execute("INSERT INTO KhachHang VALUES(?,?,?,?,?)", ma, ten, cmnd, quocTich, sdt);
            return KetQuaXuLy.ok("Đã lưu khách hàng.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    /** 1 phòng được chọn trong phiếu đặt: số phòng + số người ở. */
    public static class PhongDatItem {
        public String soPhong;
        public int soNguoi;
        public PhongDatItem(String soPhong, int soNguoi) {
            this.soPhong = soPhong;
            this.soNguoi = soNguoi;
        }
    }

    /**
     * Lập phiếu đặt phòng. Kiểm tra: ngày trả >= ngày nhận, sức chứa từng phòng,
     * và trùng lịch với các phiếu Đã đặt/Đang ở khác. Toàn bộ chạy trong 1 transaction.
     */
    public KetQuaXuLy taoDatPhong(String soPhieuDat, String maKhach, String maNVLeTan,
                                   java.util.Date ngayLap, java.util.Date ngayNhan, java.util.Date ngayTraDuKien,
                                   double tienCoc, String kenhDat, List<PhongDatItem> danhSach) {

        if (isBlank(soPhieuDat) || isBlank(maKhach) || isBlank(maNVLeTan)
                || danhSach == null || danhSach.isEmpty())
            return KetQuaXuLy.fail("Phiếu đặt phòng chưa đủ thông tin.");

        if (ngayTraDuKien.before(ngayNhan))
            return KetQuaXuLy.fail("Ngày trả dự kiến không được trước ngày nhận.");

        Date sqlLap = new Date(ngayLap.getTime());
        Date sqlNhan = new Date(ngayNhan.getTime());
        Date sqlTra = new Date(ngayTraDuKien.getTime());

        try (Connection cn = Db.open()) {
            cn.setAutoCommit(false);
            try {
                // 1) Kiểm tra sức chứa + trùng lịch cho từng phòng
                for (PhongDatItem x : danhSach) {
                    Object o = Db.scalar(cn, "SELECT SoNguoiToiDa FROM Phong WHERE SoPhong=?", x.soPhong);
                    if (o == null) {
                        cn.rollback();
                        return KetQuaXuLy.fail("Không tìm thấy phòng " + x.soPhong);
                    }
                    int max = ((Number) o).intValue();
                    if (x.soNguoi <= 0 || x.soNguoi > max) {
                        cn.rollback();
                        return KetQuaXuLy.fail("Số người của phòng " + x.soPhong + " vượt sức chứa.");
                    }

                    Object countObj = Db.scalar(cn,
                        "SELECT COUNT(*) FROM ChiTietDatPhong c " +
                        "JOIN PhieuDatPhong d ON c.SoPhieuDat = d.SoPhieuDat " +
                        "WHERE c.SoPhong=? AND d.TrangThai IN (N'Đã đặt',N'Đang ở') " +
                        "AND ? <= d.NgayTraDuKien AND ? >= d.NgayNhan",
                        x.soPhong, sqlNhan, sqlTra);
                    int trung = ((Number) countObj).intValue();
                    if (trung > 0) {
                        cn.rollback();
                        return KetQuaXuLy.fail("Phòng " + x.soPhong + " bị trùng lịch đặt.");
                    }
                }

                // 2) Lập đầu phiếu
                Db.execute(cn,
                    "INSERT INTO PhieuDatPhong(SoPhieuDat,MaKhach,MaNVLeTan,NgayLap,NgayNhan,NgayTraDuKien,TienCoc,KenhDat,TrangThai) " +
                    "VALUES(?,?,?,?,?,?,?,?,N'Đã đặt')",
                    soPhieuDat, maKhach, maNVLeTan, sqlLap, sqlNhan, sqlTra, tienCoc, kenhDat);

                // 3) Lưu chi tiết từng phòng + cập nhật trạng thái phòng
                for (PhongDatItem x : danhSach) {
                    Db.execute(cn, "INSERT INTO ChiTietDatPhong VALUES(?,?,?)",
                        soPhieuDat, x.soPhong, x.soNguoi);
                    Db.execute(cn, "UPDATE Phong SET TrangThai=N'Đã đặt' WHERE SoPhong=?", x.soPhong);
                }

                cn.commit();
                return KetQuaXuLy.ok("Đã lập phiếu đặt phòng.");
            } catch (RuntimeException ex) {
                cn.rollback();
                return KetQuaXuLy.fail(DanhMucService.rootMessage(ex));
            }
        } catch (SQLException e) {
            return KetQuaXuLy.fail(e.getMessage());
        }
    }

    public KetQuaXuLy themNguoiLuuTru(String soPhieuDat, String soPhong, String hoTen, String cmnd, String quocTich) {
        if (isBlank(soPhieuDat) || isBlank(soPhong) || isBlank(hoTen) || isBlank(cmnd) || isBlank(quocTich))
            return KetQuaXuLy.fail("Thông tin người lưu trú chưa đầy đủ.");
        try {
            Object o = Db.scalar(
                "SELECT SoNguoi FROM ChiTietDatPhong WHERE SoPhieuDat=? AND SoPhong=?",
                soPhieuDat, soPhong);
            if (o == null) return KetQuaXuLy.fail("Không tìm thấy phòng trong phiếu đặt này.");
            int max = ((Number) o).intValue();

            Object countObj = Db.scalar(
                "SELECT COUNT(*) FROM NguoiLuuTru WHERE SoPhieuDat=? AND SoPhong=?",
                soPhieuDat, soPhong);
            int dem = ((Number) countObj).intValue();
            if (dem >= max) return KetQuaXuLy.fail("Đã đủ số người đăng ký cho phòng này.");

            Db.execute(
                "INSERT INTO NguoiLuuTru(SoPhieuDat,SoPhong,HoTen,SoCMND,QuocTich) VALUES(?,?,?,?,?)",
                soPhieuDat, soPhong, hoTen, cmnd, quocTich);
            return KetQuaXuLy.ok("Đã thêm người lưu trú.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    public KetQuaXuLy nhanPhong(String soPhieuDat) {
        try (Connection cn = Db.open()) {
            cn.setAutoCommit(false);
            try {
                int rows = Db.execute(cn,
                    "UPDATE PhieuDatPhong SET TrangThai=N'Đang ở', NgayNhanThucTe=GETDATE() " +
                    "WHERE SoPhieuDat=? AND TrangThai=N'Đã đặt'",
                    soPhieuDat);
                if (rows == 0) {
                    cn.rollback();
                    return KetQuaXuLy.fail("Phiếu không ở trạng thái có thể nhận phòng.");
                }
                Db.execute(cn,
                    "UPDATE Phong SET TrangThai=N'Đang ở' WHERE SoPhong IN " +
                    "(SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=?)",
                    soPhieuDat);
                cn.commit();
                return KetQuaXuLy.ok("Đã nhận phòng.");
            } catch (RuntimeException ex) {
                cn.rollback();
                return KetQuaXuLy.fail(DanhMucService.rootMessage(ex));
            }
        } catch (SQLException e) {
            return KetQuaXuLy.fail(e.getMessage());
        }
    }

    public KetQuaXuLy danhDauNoShow(String soPhieuDat) {
        try {
            Db.execute(
                "UPDATE PhieuDatPhong SET TrangThai=N'No-show' WHERE SoPhieuDat=? AND TrangThai=N'Đã đặt'",
                soPhieuDat);
            Db.execute(
                "UPDATE Phong SET TrangThai=N'Trống' WHERE SoPhong IN " +
                "(SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=?)",
                soPhieuDat);
            return KetQuaXuLy.ok("Đã đánh dấu không nhận phòng.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(DanhMucService.rootMessage(e));
        }
    }

    private boolean isBlank(String s) {
        return s == null || s.trim().isEmpty();
    }
}