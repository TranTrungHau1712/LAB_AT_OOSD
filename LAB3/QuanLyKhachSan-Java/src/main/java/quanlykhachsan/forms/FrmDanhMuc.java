package quanlykhachsan.forms;

import quanlykhachsan.services.DanhMucService;
import quanlykhachsan.models.KetQuaXuLy;
import quanlykhachsan.util.TableUtils;

import javax.swing.*;
import java.awt.*;

public class FrmDanhMuc extends JDialog {
    private final DanhMucService service = new DanhMucService();

    private JTextField txtKhuMa = new JTextField(8), txtKhuTen = new JTextField(15);
    private JTable dgvKhu = new JTable();

    private JTextField txtNVMa = new JTextField(8), txtNVTen = new JTextField(15),
            txtNVVaiTro = new JTextField(10), txtNVSDT = new JTextField(10);
    private JTable dgvNV = new JTable();

    private JTextField txtLoaiMa = new JTextField(8), txtLoaiTen = new JTextField(15);
    private JTable dgvLoaiTN = new JTable();

    private JTextField txtDVMa = new JTextField(8), txtDVTen = new JTextField(15),
            txtDVDVT = new JTextField(8), txtDVGia = new JTextField(8);
    private JTable dgvDV = new JTable();

    private JTextField txtQDMa = new JTextField(8), txtQDLoai = new JTextField(8),
            txtQDMucDo = new JTextField(12), txtQDTien = new JTextField(8);
    private JTable dgvQD = new JTable();

    public FrmDanhMuc(Frame owner) {
        super(owner, "Danh mục khách sạn", true);
        setSize(720, 480);
        setLocationRelativeTo(owner);

        JTabbedPane tabs = new JTabbedPane();
        tabs.addTab("Khu vực", tabKhuVuc());
        tabs.addTab("Nhân viên", tabNhanVien());
        tabs.addTab("Loại tiện nghi", tabLoaiTN());
        tabs.addTab("Dịch vụ", tabDichVu());
        tabs.addTab("Quy định đền bù", tabQuyDinh());

        add(tabs);
        tai();
    }

    private JPanel tabKhuVuc() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã:")); top.add(txtKhuMa);
        top.add(new JLabel("Tên:")); top.add(txtKhuTen);
        JButton btn = new JButton("Thêm");
        btn.addActionListener(e ->
            hienThi(service.themKhu(txtKhuMa.getText().trim(), txtKhuTen.getText().trim())));
        top.add(btn);
        return panelWithTable(top, dgvKhu);
    }

    private JPanel tabNhanVien() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã:")); top.add(txtNVMa);
        top.add(new JLabel("Tên:")); top.add(txtNVTen);
        top.add(new JLabel("Vai trò:")); top.add(txtNVVaiTro);
        top.add(new JLabel("SĐT:")); top.add(txtNVSDT);
        JButton btn = new JButton("Thêm");
        btn.addActionListener(e -> hienThi(service.themNhanVien(
                txtNVMa.getText().trim(), txtNVTen.getText().trim(),
                txtNVVaiTro.getText().trim(), txtNVSDT.getText().trim())));
        top.add(btn);
        return panelWithTable(top, dgvNV);
    }

    private JPanel tabLoaiTN() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã:")); top.add(txtLoaiMa);
        top.add(new JLabel("Tên:")); top.add(txtLoaiTen);
        JButton btn = new JButton("Thêm");
        btn.addActionListener(e ->
            hienThi(service.themLoaiTN(txtLoaiMa.getText().trim(), txtLoaiTen.getText().trim())));
        top.add(btn);
        return panelWithTable(top, dgvLoaiTN);
    }

    private JPanel tabDichVu() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã:")); top.add(txtDVMa);
        top.add(new JLabel("Tên:")); top.add(txtDVTen);
        top.add(new JLabel("Đơn vị:")); top.add(txtDVDVT);
        top.add(new JLabel("Đơn giá:")); top.add(txtDVGia);
        JButton btn = new JButton("Thêm");
        btn.addActionListener(e -> {
            try {
                double gia = Double.parseDouble(txtDVGia.getText().trim());
                hienThi(service.themDichVu(txtDVMa.getText().trim(), txtDVTen.getText().trim(),
                        txtDVDVT.getText().trim(), gia));
            } catch (NumberFormatException ex) {
                JOptionPane.showMessageDialog(this, "Đơn giá không hợp lệ.");
            }
        });
        top.add(btn);
        return panelWithTable(top, dgvDV);
    }

    private JPanel tabQuyDinh() {
        JPanel top = new JPanel();
        top.add(new JLabel("Mã QĐ:")); top.add(txtQDMa);
        top.add(new JLabel("Mã loại TN:")); top.add(txtQDLoai);
        top.add(new JLabel("Mức độ:")); top.add(txtQDMucDo);
        top.add(new JLabel("Mức đền bù:")); top.add(txtQDTien);
        JButton btn = new JButton("Thêm");
        btn.addActionListener(e -> {
            try {
                double tien = Double.parseDouble(txtQDTien.getText().trim());
                hienThi(service.themQuyDinh(txtQDMa.getText().trim(), txtQDLoai.getText().trim(),
                        txtQDMucDo.getText().trim(), tien));
            } catch (NumberFormatException ex) {
                JOptionPane.showMessageDialog(this, "Mức đền bù không hợp lệ.");
            }
        });
        top.add(btn);
        return panelWithTable(top, dgvQD);
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
            TableUtils.fill(dgvKhu, service.layKhuVuc());
            TableUtils.fill(dgvNV, service.layNhanVien());
            TableUtils.fill(dgvLoaiTN, service.layLoaiTienNghi());
            TableUtils.fill(dgvDV, service.layDichVu());
            TableUtils.fill(dgvQD, service.layQuyDinhDenBu());
        } catch (RuntimeException ex) {
            JOptionPane.showMessageDialog(this, "Lỗi tải dữ liệu: " + ex.getMessage());
        }
    }
}