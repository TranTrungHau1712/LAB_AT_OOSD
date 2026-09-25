package quanlykhachsan.forms;

import quanlykhachsan.models.KetQuaXuLy;
import quanlykhachsan.services.DanhMucService;
import quanlykhachsan.services.TraPhongService;

import javax.swing.*;
import java.awt.*;
import java.util.Map;

public class FrmTraPhong extends JDialog {
    private final TraPhongService service = new TraPhongService();
    private final DanhMucService danhMuc = new DanhMucService();

    private JComboBox<ComboItem> cboPhieu = new JComboBox<>();
    private JComboBox<ComboItem> cboNV = new JComboBox<>();

    private JTextField txtTienPhong = new JTextField("0", 10);
    private JTextField txtTienDV = new JTextField("0", 10);
    private JTextField txtTienDB = new JTextField("0", 10);
    private JTextField txtTienCoc = new JTextField("0", 10);
    private JTextField txtTongThanhToan = new JTextField("0", 10);

    private JTextField txtSoHoaDon = new JTextField(10);
    private JTextField txtMaThanhToan = new JTextField(10);
    private JComboBox<String> cboHinhThuc = new JComboBox<>(new String[]{"Tiền mặt", "Chuyển khoản", "Thẻ", "Ví điện tử"});

    private int soNgayTinhTien = 1;

    public FrmTraPhong(Frame owner) {
        super(owner, "Trả Phòng & Thanh Toán Hóa Đơn", true);
        setSize(850, 600);
        setLocationRelativeTo(owner);
        setLayout(new BorderLayout());

        txtTienPhong.setEditable(false);
        txtTienDV.setEditable(false);
        txtTienDB.setEditable(false);
        txtTienCoc.setEditable(false);
        txtTongThanhToan.setEditable(false);
        
        txtTongThanhToan.setFont(new Font("SansSerif", Font.BOLD, 14));
        txtTongThanhToan.setForeground(Color.RED);

        add(taoFormThanhToan(), BorderLayout.CENTER);
        napData();
    }

    private static class ComboItem {
        String id, text, extra;
        ComboItem(String id, String text) { this(id, text, ""); }
        ComboItem(String id, String text, String extra) { this.id = id; this.text = text; this.extra = extra; }
        @Override public String toString() { return text; }
    }

    private JPanel taoFormThanhToan() {
        JPanel main = new JPanel(new BorderLayout(15, 15));
        main.setBorder(BorderFactory.createEmptyBorder(15, 15, 15, 15));

        JPanel pTop = new JPanel(new GridLayout(2, 2, 15, 15));
        pTop.setBorder(BorderFactory.createTitledBorder("Chọn phiếu trả phòng"));
        pTop.add(new JLabel("Phiếu đặt phòng:")); pTop.add(cboPhieu);
        pTop.add(new JLabel("Nhân viên thu ngân:")); pTop.add(cboNV);

        cboPhieu.addActionListener(e -> tinhTien());

        JPanel pCalc = new JPanel(new GridLayout(2, 4, 15, 15));
        pCalc.setBorder(BorderFactory.createTitledBorder("Chi tiết tính tiền"));
        pCalc.add(new JLabel("Tiền phòng:")); pCalc.add(txtTienPhong);
        pCalc.add(new JLabel("Tiền dịch vụ:")); pCalc.add(txtTienDV);
        pCalc.add(new JLabel("Tiền đền bù:")); pCalc.add(txtTienDB);
        pCalc.add(new JLabel("Trừ Tiền cọc:")); pCalc.add(txtTienCoc);

        JPanel pPay = new JPanel(new GridLayout(2, 4, 15, 15));
        pPay.setBorder(BorderFactory.createTitledBorder("Thông tin Hóa đơn & Thanh toán"));
        pPay.add(new JLabel("Mã Hóa Đơn:")); pPay.add(txtSoHoaDon);
        pPay.add(new JLabel("Mã Giao Dịch:")); pPay.add(txtMaThanhToan);
        pPay.add(new JLabel("Hình thức:")); pPay.add(cboHinhThuc);
        pPay.add(new JLabel("TỔNG THANH TOÁN:")); pPay.add(txtTongThanhToan);

        JButton btnThanhToan = new JButton("XÁC NHẬN THANH TOÁN & TRẢ PHÒNG");
        btnThanhToan.setPreferredSize(new Dimension(300, 45));
        btnThanhToan.setFont(new Font("SansSerif", Font.BOLD, 14));
        btnThanhToan.setBackground(new Color(40, 167, 69));
        btnThanhToan.setForeground(Color.WHITE);
        btnThanhToan.addActionListener(e -> xuLyThanhToan());
        
        JPanel pnlBtn = new JPanel(new FlowLayout(FlowLayout.CENTER));
        pnlBtn.add(btnThanhToan);

        JPanel center = new JPanel(new GridLayout(3, 1, 10, 15));
        center.add(pTop);
        center.add(pCalc);
        center.add(pPay);

        main.add(center, BorderLayout.CENTER);
        main.add(pnlBtn, BorderLayout.SOUTH);

        return main;
    }

    private void napData() {
        cboPhieu.removeAllItems();
        for (Map<String, Object> r : service.layPhieuDangO()) {
            String text = r.get("SoPhieuDat") + " - " + r.get("TenKhach");
            cboPhieu.addItem(new ComboItem(String.valueOf(r.get("SoPhieuDat")), text, String.valueOf(r.get("TienCoc"))));
        }

        cboNV.removeAllItems();
        for (Map<String, Object> r : danhMuc.layNhanVien()) {
            cboNV.addItem(new ComboItem(String.valueOf(r.get("MaNV")), String.valueOf(r.get("HoTen"))));
        }

        tinhTien();
    }

    private void tinhTien() {
        ComboItem phieu = (ComboItem) cboPhieu.getSelectedItem();
        if (phieu == null) return;

        Map<String, Object> calc = service.tinhTienPhieu(phieu.id);
        double tPhong = ((Number) calc.get("TienPhong")).doubleValue();
        double tDV = ((Number) calc.get("TienDV")).doubleValue();
        double tDB = ((Number) calc.get("TienDenBu")).doubleValue();
        double tCoc = Double.parseDouble(phieu.extra);
        soNgayTinhTien = ((Number) calc.get("SoNgay")).intValue();

        txtTienPhong.setText(String.format("%.0f", tPhong));
        txtTienDV.setText(String.format("%.0f", tDV));
        txtTienDB.setText(String.format("%.0f", tDB));
        txtTienCoc.setText(String.format("%.0f", tCoc));

        double tong = (tPhong + tDV + tDB) - tCoc;
        txtTongThanhToan.setText(String.format("%.0f", Math.max(0, tong)));
    }

    private void xuLyThanhToan() {
        ComboItem phieu = (ComboItem) cboPhieu.getSelectedItem();
        ComboItem nv = (ComboItem) cboNV.getSelectedItem();

        if (phieu == null || nv == null) {
            JOptionPane.showMessageDialog(this, "Chưa chọn phiếu hoặc nhân viên.");
            return;
        }

        String hd = txtSoHoaDon.getText().trim();
        String tt = txtMaThanhToan.getText().trim();
        if (hd.isEmpty() || tt.isEmpty()) {
            JOptionPane.showMessageDialog(this, "Vui lòng nhập Mã hóa đơn và Mã thanh toán!");
            return;
        }

        double tPhong = Double.parseDouble(txtTienPhong.getText());
        double tDV = Double.parseDouble(txtTienDV.getText());
        double tongTT = Double.parseDouble(txtTongThanhToan.getText());

        KetQuaXuLy k = service.thanhToanVaTraPhong(hd, phieu.id, nv.id, soNgayTinhTien,
                tPhong, tDV, tt, (String) cboHinhThuc.getSelectedItem(), tongTT);

        JOptionPane.showMessageDialog(this, k.thongBao);
        if (k.thanhCong) {
            napData();
        }
    }
}