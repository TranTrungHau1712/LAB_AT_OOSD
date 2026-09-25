package quanlykhachsan.forms;

import javax.swing.*;
import java.awt.*;

public class FrmMain extends JFrame {
    public FrmMain() {
        setTitle("Quản lý khách sạn");
        setSize(560, 360);
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setLocationRelativeTo(null);

        JLabel title = new JLabel("HỆ THỐNG QUẢN LÝ KHÁCH SẠN", SwingConstants.CENTER);
        title.setFont(new Font("Arial", Font.BOLD, 20));
        title.setForeground(new Color(20, 60, 130));

        JPanel grid = new JPanel(new GridLayout(3, 2, 12, 12));
        grid.setBorder(BorderFactory.createEmptyBorder(20, 40, 20, 40));

        JButton btnDanhMuc = new JButton("Danh mục");
        JButton btnPhong = new JButton("Phòng - Tiện nghi");
        JButton btnDatPhong = new JButton("Đặt / Nhận phòng");
        JButton btnDichVu = new JButton("Sử dụng dịch vụ");
        JButton btnTraPhong = new JButton("Trả phòng - Thanh toán");
        JButton btnThongKe = new JButton("Thống kê");

        grid.add(btnDanhMuc);
        grid.add(btnPhong);
        grid.add(btnDatPhong);
        grid.add(btnDichVu);
        grid.add(btnTraPhong);
        grid.add(btnThongKe);

        btnDanhMuc.addActionListener(e -> new FrmDanhMuc(this).setVisible(true));
        btnPhong.addActionListener(e -> new FrmPhongTienNghi(this).setVisible(true));
        btnDatPhong.addActionListener(e -> new FrmDatPhong(this).setVisible(true));
        // Mở form Sử dụng dịch vụ
        btnDichVu.addActionListener(e -> {
            new FrmDichVu(this).setVisible(true);
        });

        // Mở form Trả phòng - Thanh toán
        btnTraPhong.addActionListener(e -> {
            new FrmTraPhong(this).setVisible(true);
        });

        // Mở form Thống kê
        btnThongKe.addActionListener(e -> {
            new FrmThongKe(this).setVisible(true);
        });
        // btnPhong, btnDatPhong, btnDichVu, btnTraPhong, btnThongKe
        // sẽ được nối khi các form tương ứng được tạo ở các bước sau

        setLayout(new BorderLayout());
        add(title, BorderLayout.NORTH);
        add(grid, BorderLayout.CENTER);
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> new FrmMain().setVisible(true));
    }
}