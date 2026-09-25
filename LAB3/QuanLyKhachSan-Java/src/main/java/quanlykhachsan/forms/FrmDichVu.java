package quanlykhachsan.forms;

import quanlykhachsan.models.KetQuaXuLy;
import quanlykhachsan.services.DanhMucService;
import quanlykhachsan.services.DichVuService;
import quanlykhachsan.util.TableUtils;

import javax.swing.*;
import java.awt.*;
import java.util.Map;

public class FrmDichVu extends JDialog {
    private final DichVuService service = new DichVuService();
    private final DanhMucService danhMuc = new DanhMucService();

    private JTextField txtSoPhieuSDDV = new JTextField(10);
    private JComboBox<ComboItem> cboPhongDangO = new JComboBox<>();
    private JComboBox<ComboItem> cboDichVu = new JComboBox<>();
    private JComboBox<ComboItem> cboNV = new JComboBox<>();
    private JTextField txtSoLuong = new JTextField("1", 5);
    private JTextField txtDonGia = new JTextField(8);

    private JTable dgvChiTietDV = new JTable();

    public FrmDichVu(Frame owner) {
        super(owner, "Ghi nhận Sử dụng Dịch vụ", true);
        setSize(950, 600);
        setLocationRelativeTo(owner);

        setLayout(new BorderLayout(0, 10));
        add(topPanel(), BorderLayout.NORTH);
        
        JPanel pnlTable = new JPanel(new BorderLayout());
        pnlTable.setBorder(BorderFactory.createEmptyBorder(0, 15, 15, 15));
        pnlTable.add(new JScrollPane(dgvChiTietDV), BorderLayout.CENTER);
        add(pnlTable, BorderLayout.CENTER);

        napDataCombo();
    }

    private static class ComboItem {
        String id, text, extra;
        ComboItem(String id, String text) { this(id, text, ""); }
        ComboItem(String id, String text, String extra) { this.id = id; this.text = text; this.extra = extra; }
        @Override public String toString() { return text; }
    }

    private JPanel topPanel() {
        JPanel p = new JPanel(new GridLayout(3, 4, 15, 15));
        p.setBorder(BorderFactory.createTitledBorder("Thông tin dịch vụ sử dụng"));

        p.add(new JLabel("Mã phiếu SDDV:")); p.add(txtSoPhieuSDDV);
        p.add(new JLabel("Phòng (Đang ở):")); p.add(cboPhongDangO);
        p.add(new JLabel("Chọn Dịch vụ:")); p.add(cboDichVu);
        p.add(new JLabel("Đơn giá:")); p.add(txtDonGia);
        p.add(new JLabel("Số lượng:")); p.add(txtSoLuong);
        p.add(new JLabel("Nhân viên ghi nhận:")); p.add(cboNV);

        cboDichVu.addActionListener(e -> {
            ComboItem item = (ComboItem) cboDichVu.getSelectedItem();
            if (item != null) txtDonGia.setText(item.extra);
        });

        cboPhongDangO.addActionListener(e -> taiChiTietDichVuByPhong());

        JButton btnThem = new JButton("Thêm Dịch Vụ");
        btnThem.setPreferredSize(new Dimension(150, 35));
        btnThem.addActionListener(e -> themDichVu());
        
        JPanel pnlBtn = new JPanel(new FlowLayout(FlowLayout.RIGHT));
        pnlBtn.add(btnThem);

        JPanel topContainer = new JPanel(new BorderLayout(5, 5));
        topContainer.setBorder(BorderFactory.createEmptyBorder(15, 15, 5, 15));
        topContainer.add(p, BorderLayout.CENTER);
        topContainer.add(pnlBtn, BorderLayout.SOUTH);
        
        return topContainer;
    }

    private void napDataCombo() {
        cboPhongDangO.removeAllItems();
        for (Map<String, Object> r : service.layPhongDangO()) {
            String val = r.get("SoPhieuDat") + "|" + r.get("SoPhong");
            String display = "Phòng " + r.get("SoPhong") + " (" + r.get("TenKhach") + ")";
            cboPhongDangO.addItem(new ComboItem(val, display, String.valueOf(r.get("SoPhieuDat"))));
        }

        cboDichVu.removeAllItems();
        for (Map<String, Object> r : service.layDanhSachDichVu()) {
            cboDichVu.addItem(new ComboItem(String.valueOf(r.get("MaDV")), 
                    String.valueOf(r.get("TenDV")), String.valueOf(r.get("DonGia"))));
        }

        cboNV.removeAllItems();
        for (Map<String, Object> r : danhMuc.layNhanVien()) {
            cboNV.addItem(new ComboItem(String.valueOf(r.get("MaNV")), String.valueOf(r.get("HoTen"))));
        }
    }

    private void taiChiTietDichVuByPhong() {
        ComboItem selectedPhong = (ComboItem) cboPhongDangO.getSelectedItem();
        if (selectedPhong != null) {
            String soPhieuDat = selectedPhong.extra;
            TableUtils.fill(dgvChiTietDV, service.layDichVuDaDungByPhieu(soPhieuDat));
        }
    }

    private void themDichVu() {
        ComboItem phong = (ComboItem) cboPhongDangO.getSelectedItem();
        ComboItem dv = (ComboItem) cboDichVu.getSelectedItem();
        ComboItem nv = (ComboItem) cboNV.getSelectedItem();

        if (phong == null || dv == null || nv == null) {
            JOptionPane.showMessageDialog(this, "Vui lòng chọn đầy đủ thông tin phòng, dịch vụ, nhân viên!");
            return;
        }

        String[] parts = phong.id.split("\\|");
        String soPhieuDat = parts[0];
        String soPhong = parts[1];

        int soLuong;
        double donGia;
        try {
            soLuong = Integer.parseInt(txtSoLuong.getText().trim());
            donGia = Double.parseDouble(txtDonGia.getText().trim());
        } catch (NumberFormatException ex) {
            JOptionPane.showMessageDialog(this, "Số lượng hoặc đơn giá không hợp lệ.");
            return;
        }

        KetQuaXuLy k = service.themDichVu(txtSoPhieuSDDV.getText().trim(), soPhieuDat, soPhong, dv.id, soLuong, donGia, nv.id);
        JOptionPane.showMessageDialog(this, k.thongBao);
        if (k.thanhCong) {
            taiChiTietDichVuByPhong();
        }
    }
}