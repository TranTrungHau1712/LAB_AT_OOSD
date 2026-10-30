# Bài 7 - Quản lý nhân sự (C# WinForms + SQL Server)

## Cấu trúc
- `SQL/01_QuanLyNhanSu.sql`  tạo CSDL, 24 bảng, view `vw_ThuViec`, thủ tục `sp_TinhLuongThang`
- `SQL/02_DuLieuMau.sql`     dữ liệu mẫu và chạy thử tính lương tháng 10/2026
- `NhanSu/`                  project Visual Studio (.NET Framework 4.7.2), giao diện dựng hoàn toàn bằng code
- `Dac_ta_Use_Case_Quan_ly_nhan_su.xlsx`  đặc tả use case

## Cách chạy
1. SSMS: mở và chạy `01_QuanLyNhanSu.sql`, rồi `02_DuLieuMau.sql`.
2. Mở `NhanSu/QuanLyNhanSu.csproj` bằng Visual Studio 2019/2022.
3. Sửa `Data Source` trong `App.config` cho đúng máy (`.`, `.\SQLEXPRESS` hoặc `(localdb)\MSSQLLocalDB`).
4. Bấm F5.

## Chức năng
- CRUD cho tất cả bảng (menu Danh mục, Tuyển dụng - Hợp đồng, Quyết định, Chấm công - Phụ trội, Lương - Thu chi).
- Thử việc và ký hợp đồng chính thức (Câu 4, 5): dài hạn tự lập hồ sơ BHYT, BHXH; tạo quyết định lương; chuyển nhân viên sang Chính thức.
- Lập quyết định khen thưởng cá nhân/tập thể và giám đốc ký duyệt, tự lập phiếu chi (Câu 6).
- Tính lương hằng tháng bằng `sp_TinhLuongThang`.

## Chưa có
Đăng nhập/phân quyền theo actor, màn hình báo cáo, màn hình đề xuất của trưởng phòng.
