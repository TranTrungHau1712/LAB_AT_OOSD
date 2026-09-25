package quanlykhachsan.models;

/** Một dòng phòng được chọn khi lập phiếu đặt phòng. */
public class PhongDatItem {
    public String soPhong;
    public int soNguoi;
    public double donGiaNgay;

    public PhongDatItem() {}

    public PhongDatItem(String soPhong, int soNguoi, double donGiaNgay) {
        this.soPhong = soPhong;
        this.soNguoi = soNguoi;
        this.donGiaNgay = donGiaNgay;
    }
}
