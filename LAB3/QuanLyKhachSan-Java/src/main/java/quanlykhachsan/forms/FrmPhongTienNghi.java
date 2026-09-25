package quanlykhachsan.forms;

import quanlykhachsan.services.PhongTienNghiService;
import quanlykhachsan.models.KetQuaXuLy;
import quanlykhachsan.util.TableUtils;

import javax.swing.*;
import java.awt.*;
import java.sql.Date;

public class FrmPhongTienNghi extends JDialog {
    private final PhongTienNghiService service = new PhongTienNghiService();

    // Tab Phòng
    private JTextField txtSoPhong = new JTextField(8), txtMaKhu = new JTextField(6),
            txtSoNguoiToiDa = new JTextField(5), txtDonGia = new JTextField(8);
    private JTable dgvPhong = new JTable();

    // Tab Tiện nghi
    private JTextField txtMaTN = new JTextField(8), txtMaLoaiTN = new JTextField(6),
            txtSoThuTu = new JTextField(5), txtTinhTrangTN = new JTextField(10);
    private JTable dgvTN = new JTable();

    // Tab Lắp đặt
    private JTextField txtSoPhieuLD = new JTextField(8), txtTienNghiLD = new JTextField(8),
            txtPhongLD = new JTextField(6), txtTinhTrangLD = new JTextField(10),
            txtNVLD = new JTextField(6), txtGhiChuLD = new JTextField(12);
    private JSpinner spNgayLap = new JSpinner(new SpinnerDateModel());
    private JTable dgvLD = new JTable();

    public FrmPhongTienNghi(Frame owner) {
        super(owner, "Phòng - Tiện nghi - Phiếu lắp đặt", true);
        setSize(820, 500);
        setLocationRelativeTo(owner);

        spNgayLap.setEditor(new JSpinner.DateEditor(spNgayLap, "dd/MM/yyyy"));

        JTabbedPane tabs = new JTabbedPane();
        tabs.addTab("Phòng", tabPhong());
        tabs.addTab("Tiện nghi", tabTienNghi());
        tabs.addTab("Lắp đặt / luân chuyển", tabLapDat());

        add(tabs);
        tai();
    }

    private JPanel tabPhong() {
        JPanel top = new JPanel();
        top.add(new JLabel("Số phòng:")); top.add(txtSoPhong);
        top.add(new JLabel("Mã khu vực:")); top.add(txtMaKhu);
        top.add(new JLabel("Số người tối đa:")); top.add(txtSoNguoiToiDa);
        top.add(new JLabel("Đơn giá/ngày:")); top.add(txtDonGia);
        JButton btn = new JButton("Thêm phòng");
        btn.addActionListener(e -> {
            try {
                int max = Integer.parseInt(txtSoNguoiToiDa.getText().trim());
                double gia = Double.parseDouble(txtDonGia.getText().trim());
                hienThi(service.themPhong(txtSoPhong.getText().trim(), txtMaKhu.getText().trim(), max, gia));
            } catch (NumberFormatException ex) {
                JOptionPane.showMessageDialog(this, "Số người tối đa / đơn giá không hợp lệ.");
            }
        });
        top.add(btn);
        return panelWithTable(top, dgvPhong);
    }

    private JPanel tabTienNghi() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã tiện nghi:")); top.add(txtMaTN);
        top.add(new JLabel("Mã loại TN:")); top.add(txtMaLoaiTN);
        top.add(new JLabel("Số thứ tự:")); top.add(txtSoThuTu);
        top.add(new JLabel("Tình trạng:")); top.add(txtTinhTrangTN);
        JButton btn = new JButton("Thêm tiện nghi");
        btn.addActionListener(e -> {
            try {
                int stt = Integer.parseInt(txtSoThuTu.getText().trim());
                hienThi(service.themTienNghi(txtMaTN.getText().trim(), txtMaLoaiTN.getText().trim(),
                        stt, txtTinhTrangTN.getText().trim()));
            } catch (NumberFormatException ex) {
                JOptionPane.showMessageDialog(this, "Số thứ tự không hợp lệ.");
            }
        });
        top.add(btn);
        return panelWithTable(top, dgvTN);
    }

    private JPanel tabLapDat() {
        JPanel top = new JPanel(new GridLayout(0, 4, 6, 6));
        top.add(new JLabel("Số phiếu:")); top.add(txtSoPhieuLD);
        top.add(new JLabel("Mã tiện nghi:")); top.add(txtTienNghiLD);
        top.add(new JLabel("Số phòng:")); top.add(txtPhongLD);
        top.add(new JLabel("Ngày lập:")); top.add(spNgayLap);
        top.add(new JLabel("Tình trạng:")); top.add(txtTinhTrangLD);
        top.add(new JLabel("Mã NV:")); top.add(txtNVLD);
        top.add(new JLabel("Ghi chú:")); top.add(txtGhiChuLD);

        JButton btn = new JButton("Lập phiếu");
        btn.addActionListener(e -> {
            java.util.Date d = (java.util.Date) spNgayLap.getValue();
            Date ngay = new Date(d.getTime());
            hienThi(service.lapDat(txtSoPhieuLD.getText().trim(), txtTienNghiLD.getText().trim(),
                    txtPhongLD.getText().trim(), ngay, txtTinhTrangLD.getText().trim(),
                    txtNVLD.getText().trim(), txtGhiChuLD.getText().trim()));
        });

        JPanel wrap = new JPanel(new BorderLayout());
        wrap.add(top, BorderLayout.NORTH);
        wrap.add(btn, BorderLayout.CENTER);
        return panelWithTable(wrap, dgvLD);
    }

    private JPanel panelWithTable(JPanel top, JTable table) {
        JPanel p = new JPanel(new BorderLayout());
        p.add(top, BorderLayout.NORTH);
        p.add(new JScrollPane(table), BorderLayout.CENTER);
        return p;
    }

    private void hienThi(KetQuaXuLy k) {
        JOptionPane.showMessageDialog(this, k.thongBao);
        if (k.thanhCong) tai();
    }

    private void tai() {
        try {
            TableUtils.fill(dgvPhong, service.layPhong());
            TableUtils.fill(dgvTN, service.layTienNghi());
            TableUtils.fill(dgvLD, service.layLapDat());
        } catch (RuntimeException ex) {
            JOptionPane.showMessageDialog(this, "Lỗi tải dữ liệu: " + ex.getMessage());
        }
    }
}