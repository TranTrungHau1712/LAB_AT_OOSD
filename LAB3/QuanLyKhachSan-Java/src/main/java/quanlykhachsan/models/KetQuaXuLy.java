package quanlykhachsan.models;

public class KetQuaXuLy {
    public boolean thanhCong;
    public String thongBao;

    public KetQuaXuLy(boolean thanhCong, String thongBao) {
        this.thanhCong = thanhCong;
        this.thongBao = thongBao;
    }

    // Khởi tạo thành công (Hỗ trợ cả ok và success)
    public static KetQuaXuLy ok(String thongBao) {
        return new KetQuaXuLy(true, thongBao);
    }

    public static KetQuaXuLy success(String thongBao) {
        return ok(thongBao);
    }

    // Khởi tạo thất bại (Hỗ trợ cả loi và fail)
    public static KetQuaXuLy loi(String thongBao) {
        return new KetQuaXuLy(false, thongBao);
    }

    public static KetQuaXuLy fail(String thongBao) {
        return loi(thongBao);
    }
}