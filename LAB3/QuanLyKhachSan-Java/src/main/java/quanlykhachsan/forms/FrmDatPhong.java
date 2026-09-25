package quanlykhachsan.forms;

import quanlykhachsan.services.DanhMucService;
import quanlykhachsan.services.DatPhongService;
import quanlykhachsan.services.DatPhongService.PhongDatItem;
import quanlykhachsan.models.KetQuaXuLy;
import quanlykhachsan.util.TableUtils;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;
import java.awt.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class FrmDatPhong extends JDialog {
    private final DatPhongService service = new DatPhongService();
    private final DanhMucService danhMuc = new DanhMucService();

    // ===== Tab Khách hàng =====
    private JTextField txtMaKH = new JTextField(8), txtTenKH = new JTextField(14),
            txtCMND = new JTextField(10), txtQuocTich = new JTextField(10), txtSDT = new JTextField(10);
    private JTable dgvKhach = new JTable();

    // ===== Tab Đặt phòng =====
    private JTextField txtSoPhieu = new JTextField(8), txtTienCoc = new JTextField(8);
    private JComboBox<ComboItem> cboKhach = new JComboBox<>();
    private JComboBox<ComboItem> cboNV = new JComboBox<>();
    private JComboBox<String> cboKenh = new JComboBox<>(new String[]{"Điện thoại", "Website", "Trực tiếp"});
    private JSpinner spNgayLap = new JSpinner(new SpinnerDateModel());
    private JSpinner spNgayNhan = new JSpinner(new SpinnerDateModel());
    private JSpinner spNgayTra = new JSpinner(new SpinnerDateModel());
    private JTable dgvPhong = new JTable();           // toàn bộ phòng
    private JTextField txtSoNguoi = new JTextField(4);
    private DefaultTableModel dgvChonModel = new DefaultTableModel(new String[]{"SoPhong", "SoNguoi"}, 0) {
        @Override public boolean isCellEditable(int r, int c) { return false; }
    };
    private JTable dgvChon = new JTable(dgvChonModel);
    private JTable dgvPhieu = new JTable();            // danh sách phiếu đã lập

    // ===== Tab Nhận phòng / Người lưu trú =====
    private JTextField txtPhieuChon = new JTextField(8);
    private JTextField txtNguoiPhong = new JTextField(6), txtNguoiTen = new JTextField(12),
            txtNguoiCMND = new JTextField(10), txtNguoiQT = new JTextField(10);
    private JTable dgvChiTiet = new JTable();
    private JTable dgvNguoi = new JTable();

    public FrmDatPhong(Frame owner) {
        super(owner, "Khách hàng - Đặt phòng - Nhận phòng", true);
        // TĂNG KÍCH THƯỚC CỬA SỔ ĐỂ TRÁNH BỊ KHUẤT NÚT
        setSize(1100, 700);
        setLocationRelativeTo(owner);

        String pattern = "dd/MM/yyyy";
        spNgayLap.setEditor(new JSpinner.DateEditor(spNgayLap, pattern));
        spNgayNhan.setEditor(new JSpinner.DateEditor(spNgayNhan, pattern));
        spNgayTra.setEditor(new JSpinner.DateEditor(spNgayTra, pattern));

        JTabbedPane tabs = new JTabbedPane();
        tabs.addTab("Khách hàng", tabKhachHang());
        tabs.addTab("Đặt phòng", tabDatPhong());
        tabs.addTab("Nhận phòng / Người lưu trú", tabNhanPhong());

        add(tabs);
        napComboBox();
        tai();
    }

    // ===== Combo item nhỏ để hiện Text nhưng lấy được mã =====
    private static class ComboItem {
        String ma, hienThi;
        ComboItem(String ma, String hienThi) { this.ma = ma; this.hienThi = hienThi; }
        @Override public String toString() { return hienThi; }
    }

    private void napComboBox() {
        cboKhach.removeAllItems();
        for (Map<String, Object> row : service.layKhach()) {
            cboKhach.addItem(new ComboItem(String.valueOf(row.get("MaKhach")), String.valueOf(row.get("HoTen"))));
        }
        cboNV.removeAllItems();
        for (Map<String, Object> row : danhMuc.layNhanVien()) {
            cboNV.addItem(new ComboItem(String.valueOf(row.get("MaNV")), String.valueOf(row.get("HoTen"))));
        }
    }

    // ===================== TAB 1: KHÁCH HÀNG =====================
    private JPanel tabKhachHang() {
        // Sử dụng GridLayout để chia đều form nhập liệu
        JPanel gridFields = new JPanel(new GridLayout(2, 6, 10, 10));
        gridFields.add(new JLabel("Mã KH:")); gridFields.add(txtMaKH);
        gridFields.add(new JLabel("Họ tên:")); gridFields.add(txtTenKH);
        gridFields.add(new JLabel("CMND:")); gridFields.add(txtCMND);
        gridFields.add(new JLabel("Quốc tịch:")); gridFields.add(txtQuocTich);
        gridFields.add(new JLabel("SĐT:")); gridFields.add(txtSDT);
        gridFields.add(new JLabel("")); gridFields.add(new JLabel("")); // Căn ô trống

        JButton btn = new JButton("Thêm khách");
        btn.addActionListener(e -> {
            KetQuaXuLy k = service.themKhach(txtMaKH.getText().trim(), txtTenKH.getText().trim(),
                    txtCMND.getText().trim(), txtQuocTich.getText().trim(), txtSDT.getText().trim());
            JOptionPane.showMessageDialog(this, k.thongBao);
            if (k.thanhCong) { tai(); napComboBox(); }
        });

        JPanel pnlBtn = new JPanel(new FlowLayout(FlowLayout.RIGHT));
        pnlBtn.add(btn);

        JPanel top = new JPanel(new BorderLayout(5, 5));
        top.setBorder(BorderFactory.createEmptyBorder(15, 15, 10, 15));
        top.add(gridFields, BorderLayout.CENTER);
        top.add(pnlBtn, BorderLayout.SOUTH);

        return panelWithTable(top, dgvKhach);
    }

    // ===================== TAB 2: ĐẶT PHÒNG =====================
    private JPanel tabDatPhong() {
        JPanel top = new JPanel(new GridLayout(2, 4, 15, 10));
        top.setBorder(BorderFactory.createEmptyBorder(15, 15, 15, 15));
        top.add(new JLabel("Số phiếu:")); top.add(txtSoPhieu);
        top.add(new JLabel("Khách:")); top.add(cboKhach);
        top.add(new JLabel("Lễ tân:")); top.add(cboNV);
        top.add(new JLabel("Kênh đặt:")); top.add(cboKenh);
        top.add(new JLabel("Ngày lập:")); top.add(spNgayLap);
        top.add(new JLabel("Ngày nhận:")); top.add(spNgayNhan);
        top.add(new JLabel("Ngày trả dự kiến:")); top.add(spNgayTra);
        top.add(new JLabel("Tiền cọc:")); top.add(txtTienCoc);

        JPanel middle = new JPanel(new GridLayout(1, 2, 10, 0));
        middle.setBorder(BorderFactory.createEmptyBorder(0, 15, 10, 15));
        
        JPanel leftPanel = new JPanel(new BorderLayout(0, 5));
        leftPanel.add(new JLabel("Danh sách phòng (chọn 1 dòng rồi bấm Thêm)"), BorderLayout.NORTH);
        leftPanel.add(new JScrollPane(dgvPhong), BorderLayout.CENTER);
        JPanel addRow = new JPanel(new FlowLayout(FlowLayout.LEFT));
        addRow.add(new JLabel("Số người:")); addRow.add(txtSoNguoi);
        JButton btnThemPhong = new JButton("Thêm phòng vào phiếu");
        btnThemPhong.addActionListener(e -> themPhongVaoDanhSach());
        addRow.add(btnThemPhong);
        leftPanel.add(addRow, BorderLayout.SOUTH);

        JPanel rightPanel = new JPanel(new BorderLayout(0, 5));
        rightPanel.add(new JLabel("Phòng đã chọn cho phiếu"), BorderLayout.NORTH);
        rightPanel.add(new JScrollPane(dgvChon), BorderLayout.CENTER);
        JPanel pnlBoPhong = new JPanel(new FlowLayout(FlowLayout.LEFT));
        JButton btnBoPhong = new JButton("Bỏ phòng khỏi danh sách");
        btnBoPhong.addActionListener(e -> {
            int row = dgvChon.getSelectedRow();
            if (row >= 0) dgvChonModel.removeRow(row);
        });
        pnlBoPhong.add(btnBoPhong);
        rightPanel.add(pnlBoPhong, BorderLayout.SOUTH);

        middle.add(leftPanel);
        middle.add(rightPanel);

        JPanel pnlLapPhieu = new JPanel(new FlowLayout(FlowLayout.CENTER));
        JButton btnLapPhieu = new JButton("LẬP PHIẾU ĐẶT PHÒNG");
        btnLapPhieu.setPreferredSize(new Dimension(200, 35));
        btnLapPhieu.addActionListener(e -> lapPhieu());
        pnlLapPhieu.add(btnLapPhieu);

        JPanel bottom = new JPanel(new BorderLayout(0, 5));
        bottom.setBorder(BorderFactory.createEmptyBorder(0, 15, 15, 15));
        bottom.add(new JLabel("Danh sách phiếu đặt đã lập"), BorderLayout.NORTH);
        bottom.add(new JScrollPane(dgvPhieu), BorderLayout.CENTER);

        JPanel main = new JPanel(new BorderLayout());
        JPanel topWrap = new JPanel(new BorderLayout());
        topWrap.add(top, BorderLayout.NORTH);
        topWrap.add(middle, BorderLayout.CENTER);
        topWrap.add(pnlLapPhieu, BorderLayout.SOUTH);

        main.add(topWrap, BorderLayout.NORTH);
        main.add(bottom, BorderLayout.CENTER);
        return main;
    }

    private void themPhongVaoDanhSach() {
        int row = dgvPhong.getSelectedRow();
        if (row < 0) {
            JOptionPane.showMessageDialog(this, "Chọn 1 phòng trong danh sách trước.");
            return;
        }
        String soPhong = String.valueOf(dgvPhong.getValueAt(row, dgvPhong.getColumn("SoPhong").getModelIndex()));
        for (int i = 0; i < dgvChonModel.getRowCount(); i++) {
            if (dgvChonModel.getValueAt(i, 0).equals(soPhong)) {
                JOptionPane.showMessageDialog(this, "Phòng đã có trong danh sách.");
                return;
            }
        }
        int soNguoi;
        try {
            soNguoi = Integer.parseInt(txtSoNguoi.getText().trim());
        } catch (NumberFormatException ex) {
            JOptionPane.showMessageDialog(this, "Số người không hợp lệ.");
            return;
        }
        dgvChonModel.addRow(new Object[]{soPhong, soNguoi});
    }

    private void lapPhieu() {
        List<PhongDatItem> ds = new ArrayList<>();
        for (int i = 0; i < dgvChonModel.getRowCount(); i++) {
            String soPhong = String.valueOf(dgvChonModel.getValueAt(i, 0));
            int soNguoi = ((Number) dgvChonModel.getValueAt(i, 1)).intValue();
            ds.add(new PhongDatItem(soPhong, soNguoi));
        }
        if (ds.isEmpty()) {
            JOptionPane.showMessageDialog(this, "Chưa chọn phòng nào.");
            return;
        }
        double coc;
        try {
            coc = Double.parseDouble(txtTienCoc.getText().trim());
        } catch (NumberFormatException ex) {
            JOptionPane.showMessageDialog(this, "Tiền cọc không hợp lệ.");
            return;
        }

        ComboItem khach = (ComboItem) cboKhach.getSelectedItem();
        ComboItem nv = (ComboItem) cboNV.getSelectedItem();
        if (khach == null || nv == null) {
            JOptionPane.showMessageDialog(this, "Chưa có khách hàng hoặc nhân viên.");
            return;
        }

        java.util.Date ngayLap = (java.util.Date) spNgayLap.getValue();
        java.util.Date ngayNhan = (java.util.Date) spNgayNhan.getValue();
        java.util.Date ngayTra = (java.util.Date) spNgayTra.getValue();

        KetQuaXuLy k = service.taoDatPhong(txtSoPhieu.getText().trim(), khach.ma, nv.ma,
                ngayLap, ngayNhan, ngayTra, coc, (String) cboKenh.getSelectedItem(), ds);
        JOptionPane.showMessageDialog(this, k.thongBao);
        if (k.thanhCong) {
            dgvChonModel.setRowCount(0);
            tai();
        }
    }

    // ===================== TAB 3: NHẬN PHÒNG / NGƯỜI LƯU TRÚ =====================
    private JPanel tabNhanPhong() {
        // Thanh công cụ phía trên
        JPanel top = new JPanel(new FlowLayout(FlowLayout.LEFT, 15, 10));
        top.setBorder(BorderFactory.createEmptyBorder(10, 10, 5, 10));
        top.add(new JLabel("Số phiếu đặt:")); top.add(txtPhieuChon);
        JButton btnXemChiTiet = new JButton("Xem chi tiết");
        btnXemChiTiet.addActionListener(e -> xemChiTietPhieu());
        top.add(btnXemChiTiet);

        JButton btnNhanPhong = new JButton("Nhận phòng");
        btnNhanPhong.addActionListener(e -> {
            KetQuaXuLy k = service.nhanPhong(txtPhieuChon.getText().trim());
            JOptionPane.showMessageDialog(this, k.thongBao);
            if (k.thanhCong) { tai(); xemChiTietPhieu(); }
        });
        JButton btnNoShow = new JButton("Đánh dấu No-show");
        btnNoShow.addActionListener(e -> {
            KetQuaXuLy k = service.danhDauNoShow(txtPhieuChon.getText().trim());
            JOptionPane.showMessageDialog(this, k.thongBao);
            if (k.thanhCong) tai();
        });
        top.add(btnNhanPhong);
        top.add(btnNoShow);

        // Bảng chi tiết bên trái
        JPanel chiTietPanel = new JPanel(new BorderLayout(0, 5));
        chiTietPanel.setBorder(BorderFactory.createEmptyBorder(0, 15, 15, 5));
        chiTietPanel.add(new JLabel("Chi tiết phòng trong phiếu"), BorderLayout.NORTH);
        chiTietPanel.add(new JScrollPane(dgvChiTiet), BorderLayout.CENTER);

        // Form thêm người bên phải
        JPanel pnlNguoiFields = new JPanel(new GridLayout(2, 4, 10, 10));
        pnlNguoiFields.add(new JLabel("Phòng:")); pnlNguoiFields.add(txtNguoiPhong);
        pnlNguoiFields.add(new JLabel("Họ tên:")); pnlNguoiFields.add(txtNguoiTen);
        pnlNguoiFields.add(new JLabel("CMND:")); pnlNguoiFields.add(txtNguoiCMND);
        pnlNguoiFields.add(new JLabel("Quốc tịch:")); pnlNguoiFields.add(txtNguoiQT);

        JButton btnThemNguoi = new JButton("Thêm người lưu trú");
        btnThemNguoi.addActionListener(e -> {
            KetQuaXuLy k = service.themNguoiLuuTru(txtPhieuChon.getText().trim(), txtNguoiPhong.getText().trim(),
                    txtNguoiTen.getText().trim(), txtNguoiCMND.getText().trim(), txtNguoiQT.getText().trim());
            JOptionPane.showMessageDialog(this, k.thongBao);
            if (k.thanhCong) xemChiTietPhieu();
        });
        JPanel pnlNguoiBtn = new JPanel(new FlowLayout(FlowLayout.RIGHT));
        pnlNguoiBtn.add(btnThemNguoi);

        JPanel nguoiForm = new JPanel(new BorderLayout());
        nguoiForm.add(pnlNguoiFields, BorderLayout.CENTER);
        nguoiForm.add(pnlNguoiBtn, BorderLayout.SOUTH);

        JPanel nguoiPanel = new JPanel(new BorderLayout(0, 5));
        nguoiPanel.setBorder(BorderFactory.createEmptyBorder(0, 5, 15, 15));
        nguoiPanel.add(nguoiForm, BorderLayout.NORTH);
        nguoiPanel.add(new JScrollPane(dgvNguoi), BorderLayout.CENTER);

        JPanel center = new JPanel(new GridLayout(1, 2, 10, 0));
        center.add(chiTietPanel);
        center.add(nguoiPanel);

        JPanel main = new JPanel(new BorderLayout());
        main.add(top, BorderLayout.NORTH);
        main.add(center, BorderLayout.CENTER);
        return main;
    }

    private void xemChiTietPhieu() {
        String so = txtPhieuChon.getText().trim();
        if (so.isEmpty()) return;
        TableUtils.fill(dgvChiTiet, service.layChiTiet(so));
        TableUtils.fill(dgvNguoi, service.layNguoiLuuTru(so));
    }

    // ===================== DÙNG CHUNG =====================
    private JPanel panelWithTable(JPanel top, JTable table) {
        JPanel p = new JPanel(new BorderLayout(0, 10));
        p.setBorder(BorderFactory.createEmptyBorder(0, 15, 15, 15));
        p.add(top, BorderLayout.NORTH);
        p.add(new JScrollPane(table), BorderLayout.CENTER);
        return p;
    }

    private void tai() {
        try {
            TableUtils.fill(dgvKhach, service.layKhach());
            TableUtils.fill(dgvPhong, service.layPhong());
            TableUtils.fill(dgvPhieu, service.layPhieuDat());
        } catch (RuntimeException ex) {
            JOptionPane.showMessageDialog(this, "Lỗi tải dữ liệu: " + ex.getMessage());
        }
    }
}