package quanlykhachsan.forms;

import quanlykhachsan.services.ThongKeService;
import quanlykhachsan.util.TableUtils;

import javax.swing.*;
import java.awt.*;

public class FrmThongKe extends JDialog {
    private final ThongKeService service = new ThongKeService();

    private JSpinner spNam = new JSpinner(new SpinnerNumberModel(2026, 2020, 2030, 1));
    private JTable dgvDoanhThu = new JTable();
    private JTable dgvDichVu = new JTable();
    private JTable dgvPhong = new JTable();

    public FrmThongKe(Frame owner) {
        super(owner, "Báo Cáo & Thống Kê Doanh Thu", true);
        setSize(950, 600);
        setLocationRelativeTo(owner);

        JTabbedPane tabs = new JTabbedPane();
        tabs.addTab("Doanh thu theo Tháng", tabDoanhThu());
        tabs.addTab("Thống kê Dịch vụ", tabDichVu());
        tabs.addTab("Tần suất Đặt phòng", tabPhong());

        JPanel mainWrap = new JPanel(new BorderLayout());
        mainWrap.setBorder(BorderFactory.createEmptyBorder(10, 10, 10, 10));
        mainWrap.add(tabs, BorderLayout.CENTER);
        
        add(mainWrap);
        taiThongKe();
    }

    private JPanel tabDoanhThu() {
        JPanel p = new JPanel(new BorderLayout(0, 10));
        p.setBorder(BorderFactory.createEmptyBorder(15, 15, 15, 15));
        
        JPanel top = new JPanel(new FlowLayout(FlowLayout.LEFT, 15, 5));
        spNam.setPreferredSize(new Dimension(100, 25));
        top.add(new JLabel("Chọn Năm Báo Cáo:")); 
        top.add(spNam);
        
        JButton btn = new JButton("Xem thống kê");
        btn.addActionListener(e -> taiThongKeDoanhThu());
        top.add(btn);

        p.add(top, BorderLayout.NORTH);
        p.add(new JScrollPane(dgvDoanhThu), BorderLayout.CENTER);
        return p;
    }

    private JPanel tabDichVu() {
        JPanel p = new JPanel(new BorderLayout());
        p.setBorder(BorderFactory.createEmptyBorder(15, 15, 15, 15));
        p.add(new JScrollPane(dgvDichVu), BorderLayout.CENTER);
        return p;
    }

    private JPanel tabPhong() {
        JPanel p = new JPanel(new BorderLayout());
        p.setBorder(BorderFactory.createEmptyBorder(15, 15, 15, 15));
        p.add(new JScrollPane(dgvPhong), BorderLayout.CENTER);
        return p;
    }

    private void taiThongKe() {
        taiThongKeDoanhThu();
        TableUtils.fill(dgvDichVu, service.thongKeDichVuBanChay());
        TableUtils.fill(dgvPhong, service.thongKeTanSuatPhong());
    }

    private void taiThongKeDoanhThu() {
        int nam = (Integer) spNam.getValue();
        TableUtils.fill(dgvDoanhThu, service.thongKeDoanhThuThang(nam));
    }
}