package quanlykhachsan.services;

import quanlykhachsan.data.Db;
import quanlykhachsan.models.KetQuaXuLy;

import java.util.List;
import java.util.Map;

public class DanhMucService {

    public List<Map<String, Object>> layKhuVuc() {
        return Db.query("SELECT * FROM KhuVuc ORDER BY MaKhuVuc");
    }

    public List<Map<String, Object>> layNhanVien() {
        return Db.query("SELECT * FROM NhanVien ORDER BY MaNV");
    }

    public List<Map<String, Object>> layLoaiTienNghi() {
        return Db.query("SELECT * FROM LoaiTienNghi ORDER BY MaLoaiTN");
    }

    public List<Map<String, Object>> layDichVu() {
        return Db.query("SELECT * FROM DichVu ORDER BY MaDV");
    }

    public List<Map<String, Object>> layQuyDinhDenBu() {
        return Db.query(
            "SELECT q.*, l.TenLoaiTN FROM QuyDinhDenBu q " +
            "JOIN LoaiTienNghi l ON q.MaLoaiTN=l.MaLoaiTN ORDER BY q.MaQuyDinh");
    }

    public KetQuaXuLy themKhu(String ma, String ten) {
        if (isBlank(ma) || isBlank(ten)) return KetQuaXuLy.fail("Mã khu vực và tên khu vực không được để trống.");
        try {
            Db.execute("INSERT INTO KhuVuc VALUES(?,?)", ma, ten);
            return KetQuaXuLy.ok("Đã thêm khu vực.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(rootMessage(e));
        }
    }

    public KetQuaXuLy themNhanVien(String ma, String ten, String vaiTro, String sdt) {
        if (isBlank(ma) || isBlank(ten) || isBlank(vaiTro)) return KetQuaXuLy.fail("Thông tin nhân viên chưa đầy đủ.");
        try {
            Db.execute("INSERT INTO NhanVien VALUES(?,?,?,?)", ma, ten, vaiTro, sdt);
            return KetQuaXuLy.ok("Đã thêm nhân viên.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(rootMessage(e));
        }
    }

    public KetQuaXuLy themLoaiTN(String ma, String ten) {
        if (isBlank(ma) || isBlank(ten)) return KetQuaXuLy.fail("Thông tin loại tiện nghi chưa đủ.");
        try {
            Db.execute("INSERT INTO LoaiTienNghi VALUES(?,?)", ma, ten);
            return KetQuaXuLy.ok("Đã thêm loại tiện nghi.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(rootMessage(e));
        }
    }

    public KetQuaXuLy themDichVu(String ma, String ten, String dvt, double gia) {
        if (isBlank(ma) || isBlank(ten) || isBlank(dvt) || gia < 0) return KetQuaXuLy.fail("Thông tin dịch vụ không hợp lệ.");
        try {
            Db.execute("INSERT INTO DichVu VALUES(?,?,?,?)", ma, ten, dvt, gia);
            return KetQuaXuLy.ok("Đã thêm dịch vụ.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(rootMessage(e));
        }
    }

    public KetQuaXuLy themQuyDinh(String ma, String maLoaiTN, String mucDo, double tien) {
        if (isBlank(ma) || isBlank(maLoaiTN) || isBlank(mucDo) || tien < 0) return KetQuaXuLy.fail("Quy định đền bù không hợp lệ.");
        try {
            Db.execute("INSERT INTO QuyDinhDenBu VALUES(?,?,?,?)", ma, maLoaiTN, mucDo, tien);
            return KetQuaXuLy.ok("Đã thêm quy định đền bù.");
        } catch (RuntimeException e) {
            return KetQuaXuLy.fail(rootMessage(e));
        }
    }

    static String rootMessage(RuntimeException e) {
        Throwable c = e.getCause();
        return c != null ? c.getMessage() : e.getMessage();
    }

    private boolean isBlank(String s) {
        return s == null || s.trim().isEmpty();
    }
}
