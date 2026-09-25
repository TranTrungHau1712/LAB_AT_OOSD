# QuanLyKhachSan (bản Java) — Hướng dẫn chạy trong VS Code

Bản này chuyển từ thiết kế C#/WinForms trong đề sang **Java thuần + JDBC**, giữ nguyên toàn bộ
nghiệp vụ (Db, KetQuaXuLy, 6 Service). **Chưa làm giao diện (Swing GUI)** — phần Form sẽ làm ở
bước sau, sau khi Data + Service chạy đúng. Hiện tại kiểm tra bằng `Main.java` in kết quả ra console.

## 1. Yêu cầu môi trường (bạn đã có VS Code + Java rồi thì bỏ qua bước cài)

Trong VS Code, mở **Extensions** (Ctrl+Shift+X), kiểm tra đã cài:
- **Extension Pack for Java** (của Microsoft) — bắt buộc, gồm Language Support, Debugger, Test Runner, Maven for Java.
- Nếu bấm mở file `.java` mà VS Code không tự nhận project Maven, cài thêm cực nhanh: mở Command Palette (Ctrl+Shift+P) → gõ `Java: Configure Java Runtime` để kiểm tra JDK.

Cần **JDK 17 trở lên** (kiểm tra: mở terminal, gõ `java -version`). Nếu chưa có, tải tại
https://adoptium.net (chọn bản Temurin 17 LTS).

## 2. Mở project trong VS Code

1. Giải nén thư mục `QuanLyKhachSan-Java` ra vị trí bạn muốn.
2. Trong VS Code: **File → Open Folder...** → chọn thư mục `QuanLyKhachSan-Java`.
3. VS Code sẽ tự nhận đây là project Maven (nhờ có `pom.xml`) và tải driver `mssql-jdbc` về —
   đợi thanh trạng thái phía dưới chạy xong (lần đầu có thể mất vài phút để tải dependency).

## 3. Cấu hình kết nối SQL Server

Mở file `src/main/java/quanlykhachsan/data/Db.java`, sửa dòng `URL` cho khớp SQL Server của bạn:

```java
private static final String URL =
    "jdbc:sqlserver://localhost:1433;databaseName=QuanLyKhachSan;" +
    "user=sa;password=YOUR_PASSWORD;encrypt=true;trustServerCertificate=true";
```

- Nếu dùng **SQL Server Authentication**: sửa `user` và `password` cho đúng tài khoản của bạn.
- Nếu dùng **Windows Authentication**: đổi URL sang dạng có `integratedSecurity=true` (xem chú thích
  ngay trong file `Db.java`) — cách này cần thêm file `mssql-jdbc_auth-<version>-x64.dll` vào PATH,
  nên nếu không quen thì **cứ dùng SQL Authentication cho đơn giản** (mở SSMS → Security → Logins →
  tạo/kiểm tra user `sa` hoặc user riêng, bật SQL Server + Windows Authentication mode).

Database `QuanLyKhachSan` và toàn bộ bảng/script SQL vẫn dùng đúng file `.sql` đã có trong hướng dẫn
trước — chạy script đó trong SSMS trước khi chạy Java.

## 4. Chạy thử (chưa cần GUI)

Cách 1 — dùng nút Run có sẵn trong VS Code: mở file `Main.java`, bấm nút **Run** (tam giác xanh)
phía trên hàm `main`.

Cách 2 — dùng terminal:
```
mvn compile exec:java
```

Nếu thấy in ra danh sách khu vực/nhân viên từ database → kết nối đã đúng, Service đã chạy được.
Nếu báo lỗi kết nối, kiểm tra lại: SQL Server đã bật TCP/IP (SQL Server Configuration Manager),
đúng port 1433, đúng tên database, đúng user/password.

## 5. Cấu trúc project

```
QuanLyKhachSan-Java/
├── pom.xml
├── .gitignore
├── README.md
└── src/main/java/quanlykhachsan/
    ├── Main.java                  (chạy thử, không GUI)
    ├── data/Db.java                (kết nối + query/execute/scalar, thay cho Data/Db.cs)
    ├── models/
    │   ├── KetQuaXuLy.java         (kết quả xử lý — Ok/Fail)
    │   ├── PhongDatItem.java       (dòng phòng chọn khi đặt phòng)
    │   └── DenBuItem.java          (dòng tiện nghi đền bù)
    └── services/
        ├── DanhMucService.java     (khu vực, nhân viên, loại TN, dịch vụ, quy định đền bù)
        ├── PhongTienNghiService.java (phòng, tiện nghi, phiếu lắp đặt — BR04)
        ├── DatPhongService.java    (khách hàng, đặt/nhận phòng, người lưu trú — BR02, BR05, BR06)
        ├── DichVuService.java      (ghi nhận dịch vụ, cộng dồn cùng ngày — BR07)
        ├── TraPhongService.java    (đền bù, hóa đơn, thanh toán, trả phòng — BR08, BR09, BR10)
        └── ThongKeService.java     (thống kê tổng hợp + theo dịch vụ)
```

Mỗi Service đều giữ đúng chữ ký hàm và logic (transaction, kiểm tra sức chứa/trùng lịch/cộng dồn...)
như bản C# trong đề — chỉ khác cú pháp JDBC thay cho ADO.NET, và `List<Map<String,Object>>` thay cho
`DataTable` (vì Java không có sẵn kiểu DataTable).

## 6. Bước tiếp theo (làm sau, chưa cần bây giờ)

Khi Service đã chạy ổn qua `Main.java`, phần giao diện có thể làm bằng **Java Swing** (tương đương
WinForms): mỗi Form trong đề (FrmMain, FrmDanhMuc, FrmPhongTienNghi, FrmDatPhong, FrmDichVu,
FrmTraPhong, FrmThongKe) sẽ là một `JFrame`, dùng `JTable` thay cho `DataGridView`, `JComboBox`
thay cho ComboBox. Khi bạn sẵn sàng làm phần này, nhắn mình để mình hướng dẫn tiếp — có thể dùng
NetBeans (có GUI Builder kéo-thả) chỉ riêng cho phần thiết kế Form, còn logic vẫn dùng lại các
Service đã viết ở đây.

## 7. Push lên GitHub

Mở terminal tại thư mục `QuanLyKhachSan-Java` (nơi có file `pom.xml`):

```
git init
git add .
git commit -m "Bai 3: He thong quan ly khach san - ban Java (Data + Service, chua co GUI)"
git branch -M main
git remote add origin https://github.com/<username>/QuanLyKhachSan-Java.git
git push -u origin main
```

Thay `<username>` bằng tài khoản GitHub của bạn (`TranTrungHau1712`). Nếu bạn muốn gộp vào repo
`LAB_AT_OOSD` đang có sẵn (thay vì tạo repo riêng), `git pull` repo đó trước, đặt thư mục
`QuanLyKhachSan-Java/` (hoặc đổi tên thành `LAB3_QuanLyKhachSan/`) làm thư mục con trong repo,
rồi `git add . && git commit && git push` như bình thường — không cần `git init` mới.

`.gitignore` đã có sẵn trong project, loại trừ `target/` (file build Maven) và `.vscode/` — không
cần chỉnh gì thêm.
