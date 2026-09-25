package quanlykhachsan;

import quanlykhachsan.services.DanhMucService;

import java.util.List;
import java.util.Map;

/**
 * Điểm chạy thử KHÔNG cần giao diện (Swing GUI sẽ làm sau khi phần Data/Service chạy ổn).
 * Chạy: mvn compile exec:java   (hoặc bấm Run trên VS Code)
 */
public class Main {
    public static void main(String[] args) {
        DanhMucService dm = new DanhMucService();

        System.out.println("== Danh sách khu vực ==");
        List<Map<String, Object>> khuVuc = dm.layKhuVuc();
        for (Map<String, Object> row : khuVuc) {
            System.out.println(row);
        }

        System.out.println("== Danh sách nhân viên ==");
        for (Map<String, Object> row : dm.layNhanVien()) {
            System.out.println(row);
        }
    }
}
