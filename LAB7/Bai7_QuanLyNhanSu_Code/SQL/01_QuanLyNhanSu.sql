/* =====================================================================
   BÀI 7 - QUẢN LÝ NHÂN SỰ (Công ty sơn Đông Á)
   SQL Server - tạo CSDL, bảng, ràng buộc, thủ tục tính lương, dữ liệu mẫu
   Chạy toàn bộ file trong SSMS (có thể chạy lại nhiều lần).
   ===================================================================== */
USE master;
GO
IF DB_ID(N'QuanLyNhanSu') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyNhanSu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyNhanSu;
END
GO
CREATE DATABASE QuanLyNhanSu;
GO
USE QuanLyNhanSu;
GO

/* ---------- Tổ chức và nhân sự ---------- */
CREATE TABLE PhongBan (
    MaPB            VARCHAR(10)    NOT NULL PRIMARY KEY,
    TenPB           NVARCHAR(100)  NOT NULL,
    MoTa            NVARCHAR(255)  NULL,
    NhuCauTuyenDung INT            NOT NULL DEFAULT 0 CHECK (NhuCauTuyenDung >= 0)
);
CREATE TABLE ChucVu (
    MaCV          VARCHAR(10)    NOT NULL PRIMARY KEY,
    TenCV         NVARCHAR(100)  NOT NULL,
    PhuCapChucVu  DECIMAL(18,0)  NOT NULL DEFAULT 0 CHECK (PhuCapChucVu >= 0)
);
CREATE TABLE NhanVien (
    MaNV         VARCHAR(10)    NOT NULL PRIMARY KEY,
    HoTen        NVARCHAR(100)  NOT NULL,
    NgaySinh     DATE           NOT NULL,
    GioiTinh     NVARCHAR(5)    NOT NULL CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    DiaChi       NVARCHAR(200)  NULL,
    SoDienThoai  VARCHAR(15)    NULL,
    SoCCCD       VARCHAR(15)    NULL,
    NgayVaoLam   DATE           NOT NULL,
    TrangThai    NVARCHAR(30)   NOT NULL DEFAULT N'Thử việc'
                 CHECK (TrangThai IN (N'Thử việc', N'Chính thức', N'Thôi việc')),
    MaPB         VARCHAR(10)    NOT NULL REFERENCES PhongBan(MaPB)
);

/* ---------- Quyết định (lớp cha + 5 lớp con) ---------- */
CREATE TABLE QuyetDinh (
    SoQD        VARCHAR(20)    NOT NULL PRIMARY KEY,
    LoaiQD      VARCHAR(12)    NOT NULL
                CHECK (LoaiQD IN ('TUYENDUNG','CHUCVU','LUONG','KHENTHUONG','KYLUAT')),
    NgayLap     DATE           NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    NgayHieuLuc DATE           NOT NULL,
    NoiDung     NVARCHAR(500)  NULL,
    NguoiKy     NVARCHAR(100)  NULL,
    TrangThai   NVARCHAR(20)   NOT NULL DEFAULT N'Chờ duyệt'
                CHECK (TrangThai IN (N'Chờ duyệt', N'Đã ký', N'Từ chối')),
    MaNV        VARCHAR(10)    NULL REFERENCES NhanVien(MaNV),   -- quyết định cá nhân
    MaPB        VARCHAR(10)    NULL REFERENCES PhongBan(MaPB),   -- phòng ban thử việc / tập thể
    CHECK (MaNV IS NOT NULL OR MaPB IS NOT NULL)
);
CREATE TABLE QDTuyenDung (
    SoQD            VARCHAR(20)    NOT NULL PRIMARY KEY REFERENCES QuyetDinh(SoQD),
    ThoiGianThuViec INT            NOT NULL CHECK (ThoiGianThuViec > 0),   -- số tháng
    LuongThuViec    DECIMAL(18,0)  NOT NULL CHECK (LuongThuViec >= 0),
    DieuKhoan       NVARCHAR(1000) NULL                                    -- điều khoản hai bên
);
CREATE TABLE QDChucVu (
    SoQD          VARCHAR(20)    NOT NULL PRIMARY KEY REFERENCES QuyetDinh(SoQD),
    MaCV          VARCHAR(10)    NOT NULL REFERENCES ChucVu(MaCV),
    PhuCapChucVu  DECIMAL(18,0)  NOT NULL DEFAULT 0
);
CREATE TABLE QDLuong (
    SoQD          VARCHAR(20)    NOT NULL PRIMARY KEY REFERENCES QuyetDinh(SoQD),
    LuongCoBan    DECIMAL(18,0)  NOT NULL CHECK (LuongCoBan >= 0),
    PhuCapLuong   DECIMAL(18,0)  NOT NULL DEFAULT 0,
    PhuCapDocHai  DECIMAL(18,0)  NOT NULL DEFAULT 0
);
CREATE TABLE QDKhenThuong (
    SoQD          VARCHAR(20)    NOT NULL PRIMARY KEY REFERENCES QuyetDinh(SoQD),
    DoiTuong      NVARCHAR(10)   NOT NULL CHECK (DoiTuong IN (N'Cá nhân', N'Tập thể')),
    HinhThuc      NVARCHAR(100)  NOT NULL,      -- ngày lễ, thưởng cuối năm, lương tháng 13, ...
    SoTienThuong  DECIMAL(18,0)  NOT NULL CHECK (SoTienThuong >= 0),
    LyDo          NVARCHAR(300)  NULL
);
CREATE TABLE QDKyLuat (
    SoQD          VARCHAR(20)    NOT NULL PRIMARY KEY REFERENCES QuyetDinh(SoQD),
    DoiTuong      NVARCHAR(10)   NOT NULL CHECK (DoiTuong IN (N'Cá nhân', N'Tập thể')),
    HinhThuc      NVARCHAR(100)  NOT NULL,
    SoTienPhat    DECIMAL(18,0)  NOT NULL DEFAULT 0 CHECK (SoTienPhat >= 0),
    LyDo          NVARCHAR(300)  NULL
);

/* ---------- Hợp đồng và bảo hiểm ---------- */
CREATE TABLE HopDong (
    SoHD            VARCHAR(20)    NOT NULL PRIMARY KEY,
    LoaiHD          NVARCHAR(10)   NOT NULL CHECK (LoaiHD IN (N'Dài hạn', N'Ngắn hạn')),
    NgayKy          DATE           NOT NULL,
    NgayBatDau      DATE           NOT NULL,
    NgayKetThuc     DATE           NULL,          -- hợp đồng dài hạn không xác định thời hạn thì để trống
    LuongThoaThuan  DECIMAL(18,0)  NOT NULL CHECK (LuongThoaThuan >= 0),
    MaNV            VARCHAR(10)    NOT NULL REFERENCES NhanVien(MaNV)
);
CREATE TABLE HopDongNganHan (
    SoHD    VARCHAR(20) NOT NULL PRIMARY KEY REFERENCES HopDong(SoHD),
    ThoiHan INT         NOT NULL CHECK (ThoiHan > 0)      -- số tháng
);
CREATE TABLE HopDongDaiHan (
    SoHD      VARCHAR(20) NOT NULL PRIMARY KEY REFERENCES HopDong(SoHD),
    CoBaoHiem BIT         NOT NULL DEFAULT 1
);
CREATE TABLE HoSoBaoHiem (
    MaBH         VARCHAR(20)   NOT NULL PRIMARY KEY,
    LoaiBH       NVARCHAR(10)  NOT NULL CHECK (LoaiBH IN (N'BHYT', N'BHXH')),
    SoSo         VARCHAR(30)   NULL,
    NgayThamGia  DATE          NOT NULL,
    MucDong      DECIMAL(5,2)  NOT NULL CHECK (MucDong BETWEEN 0 AND 100),  -- % lương cơ bản nhân viên đóng
    SoHD         VARCHAR(20)   NOT NULL REFERENCES HopDong(SoHD)
);

/* ---------- Phân công, chấm công, phụ trội ---------- */
CREATE TABLE CaLamViec (
    MaCa       VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenCa      NVARCHAR(50)  NOT NULL,
    GioBatDau  TIME(0)       NOT NULL,
    GioKetThuc TIME(0)       NOT NULL
);
CREATE TABLE KeHoachPhanCong (
    MaKH        VARCHAR(20)   NOT NULL PRIMARY KEY,
    NgayBatDau  DATE          NOT NULL,
    NgayKetThuc DATE          NOT NULL,
    MaPB        VARCHAR(10)   NOT NULL REFERENCES PhongBan(MaPB),
    NguoiLap    NVARCHAR(100) NULL,
    CHECK (NgayKetThuc >= NgayBatDau)
);
CREATE TABLE ChiTietPhanCong (
    MaKH       VARCHAR(20)  NOT NULL REFERENCES KeHoachPhanCong(MaKH) ON DELETE CASCADE,
    MaNV       VARCHAR(10)  NOT NULL REFERENCES NhanVien(MaNV),
    Ngay       DATE         NOT NULL,
    MaCa       VARCHAR(10)  NOT NULL REFERENCES CaLamViec(MaCa),
    CoPhuTroi  BIT          NOT NULL DEFAULT 0,
    PRIMARY KEY (MaKH, MaNV, Ngay)
);
CREATE TABLE BangChamCong (
    MaBCC     VARCHAR(20)   NOT NULL PRIMARY KEY,
    Thang     INT           NOT NULL CHECK (Thang BETWEEN 1 AND 12),
    Nam       INT           NOT NULL,
    MaPB      VARCHAR(10)   NOT NULL REFERENCES PhongBan(MaPB),
    NguoiLap  NVARCHAR(100) NULL,
    UNIQUE (Thang, Nam, MaPB)
);
CREATE TABLE ChiTietChamCong (
    MaBCC      VARCHAR(20)   NOT NULL REFERENCES BangChamCong(MaBCC) ON DELETE CASCADE,
    MaNV       VARCHAR(10)   NOT NULL REFERENCES NhanVien(MaNV),
    Ngay       DATE          NOT NULL,
    SoGioLam   DECIMAL(4,1)  NOT NULL DEFAULT 8 CHECK (SoGioLam BETWEEN 0 AND 24),
    TrangThai  NVARCHAR(20)  NOT NULL DEFAULT N'Đi làm'
               CHECK (TrangThai IN (N'Đi làm', N'Nghỉ phép', N'Vắng')),
    PRIMARY KEY (MaBCC, MaNV, Ngay)
);
CREATE TABLE PhieuPhuTroi (
    SoPhieu       VARCHAR(20)   NOT NULL PRIMARY KEY,
    NgayPhuTroi   DATE          NOT NULL,
    SoGioPhuTroi  DECIMAL(5,1)  NOT NULL CHECK (SoGioPhuTroi > 0),
    HinhThuc      NVARCHAR(20)  NOT NULL CHECK (HinhThuc IN (N'Tăng ca', N'Ngày lễ', N'Ngày nghỉ')),
    HeSo          DECIMAL(3,1)  NOT NULL DEFAULT 1.5          -- giờ phụ trội = 1.5 lần giờ bình thường
);
CREATE TABLE ChiTietPhieuPhuTroi (
    SoPhieu  VARCHAR(20)   NOT NULL REFERENCES PhieuPhuTroi(SoPhieu) ON DELETE CASCADE,
    MaNV     VARCHAR(10)   NOT NULL REFERENCES NhanVien(MaNV),
    SoGio    DECIMAL(5,1)  NOT NULL CHECK (SoGio > 0),
    PRIMARY KEY (SoPhieu, MaNV)
);

/* ---------- Lương và thu chi ---------- */
CREATE TABLE BangLuong (
    MaBL      VARCHAR(20)   NOT NULL PRIMARY KEY,
    Thang     INT           NOT NULL CHECK (Thang BETWEEN 1 AND 12),
    Nam       INT           NOT NULL,
    NgayLap   DATE          NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    NguoiLap  NVARCHAR(100) NULL,
    UNIQUE (Thang, Nam)
);
CREATE TABLE ChiTietBangLuong (
    MaBL         VARCHAR(20)    NOT NULL REFERENCES BangLuong(MaBL) ON DELETE CASCADE,
    MaNV         VARCHAR(10)    NOT NULL REFERENCES NhanVien(MaNV),
    LuongCoBan   DECIMAL(18,0)  NOT NULL DEFAULT 0,
    TongPhuCap   DECIMAL(18,0)  NOT NULL DEFAULT 0,
    TienPhuTroi  DECIMAL(18,0)  NOT NULL DEFAULT 0,
    TienThuong   DECIMAL(18,0)  NOT NULL DEFAULT 0,
    KhauTruBH    DECIMAL(18,0)  NOT NULL DEFAULT 0,
    TienPhat     DECIMAL(18,0)  NOT NULL DEFAULT 0,
    ThucLanh     DECIMAL(18,0)  NOT NULL DEFAULT 0,
    PRIMARY KEY (MaBL, MaNV)
);
CREATE TABLE PhieuChiKhenThuong (
    SoPhieuChi  VARCHAR(20)    NOT NULL PRIMARY KEY,
    SoQD        VARCHAR(20)    NOT NULL UNIQUE REFERENCES QuyetDinh(SoQD),
    NgayChi     DATE           NOT NULL,
    SoTien      DECIMAL(18,0)  NOT NULL CHECK (SoTien >= 0),
    HinhThucChi NVARCHAR(20)   NOT NULL DEFAULT N'Tiền mặt'
                CHECK (HinhThucChi IN (N'Tiền mặt', N'Cộng vào lương'))
);
CREATE TABLE PhieuThuNopPhat (
    SoPhieuThu  VARCHAR(20)    NOT NULL PRIMARY KEY,
    SoQD        VARCHAR(20)    NOT NULL UNIQUE REFERENCES QuyetDinh(SoQD),
    NgayThu     DATE           NOT NULL,
    SoTien      DECIMAL(18,0)  NOT NULL CHECK (SoTien >= 0),
    HinhThucNop NVARCHAR(20)   NOT NULL DEFAULT N'Nộp trực tiếp'
                CHECK (HinhThucNop IN (N'Nộp trực tiếp', N'Trừ lương'))
);
GO

/* ---------- View hỗ trợ ---------- */
CREATE VIEW vw_ThuViec AS
SELECT nv.MaNV, nv.HoTen, nv.MaPB, pb.TenPB,
       q.SoQD, q.NgayHieuLuc AS NgayBatDauThuViec,
       t.ThoiGianThuViec, t.LuongThuViec,
       DATEADD(MONTH, t.ThoiGianThuViec, q.NgayHieuLuc) AS NgayHetThuViec
FROM NhanVien nv
JOIN PhongBan pb ON pb.MaPB = nv.MaPB
JOIN QuyetDinh q ON q.MaNV = nv.MaNV AND q.LoaiQD = 'TUYENDUNG' AND q.TrangThai = N'Đã ký'
JOIN QDTuyenDung t ON t.SoQD = q.SoQD
WHERE nv.TrangThai = N'Thử việc';
GO

/* ---------- Thủ tục tính lương tháng ----------
   Quy ước tính (ghi rõ trong báo cáo):
   - Giờ chuẩn 1 tháng = 208 giờ (26 ngày x 8 giờ).
   - Lương theo công = LuongCoBan x MIN(1, tổng giờ làm / 208).
   - Tiền phụ trội = tổng (SoGio x HeSo) x (LuongCoBan / 208).
   - Phụ cấp = phụ cấp lương + độc hại (QĐ lương mới nhất) + phụ cấp chức vụ (QĐ chức vụ mới nhất).
   - Khấu trừ BH = LuongCoBan x tổng MucDong% của hồ sơ BH thuộc hợp đồng dài hạn của nhân viên.
   - Thưởng chỉ cộng khi phiếu chi là 'Cộng vào lương' (tập thể chia đều theo số nhân viên phòng ban).
   - Phạt chỉ trừ khi phiếu thu là 'Trừ lương' (tập thể chia đều).
*/
CREATE PROCEDURE sp_TinhLuongThang
    @Thang INT, @Nam INT, @NguoiLap NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Dau DATE = DATEFROMPARTS(@Nam, @Thang, 1);
    DECLARE @Cuoi DATE = EOMONTH(@Dau);
    DECLARE @MaBL VARCHAR(20) = 'BL' + CONVERT(VARCHAR(6), @Dau, 112);  -- BLyyyyMM

    BEGIN TRAN;
    DELETE FROM BangLuong WHERE Thang = @Thang AND Nam = @Nam;          -- tính lại nếu đã có
    INSERT INTO BangLuong(MaBL, Thang, Nam, NgayLap, NguoiLap)
    VALUES (@MaBL, @Thang, @Nam, CAST(GETDATE() AS DATE), @NguoiLap);

    ;WITH TT AS (
        SELECT nv.MaNV, nv.MaPB,
               ISNULL(l.LuongCoBan, ISNULL(td.LuongThuViec, 0)) AS LuongCoBan,
               ISNULL(l.PhuCapLuong, 0) + ISNULL(l.PhuCapDocHai, 0) + ISNULL(cv.PhuCapChucVu, 0) AS TongPhuCap
        FROM NhanVien nv
        OUTER APPLY (SELECT TOP 1 x.LuongCoBan, x.PhuCapLuong, x.PhuCapDocHai
                     FROM QuyetDinh q JOIN QDLuong x ON x.SoQD = q.SoQD
                     WHERE q.MaNV = nv.MaNV AND q.TrangThai = N'Đã ký' AND q.NgayHieuLuc <= @Cuoi
                     ORDER BY q.NgayHieuLuc DESC) l
        OUTER APPLY (SELECT TOP 1 x.LuongThuViec
                     FROM QuyetDinh q JOIN QDTuyenDung x ON x.SoQD = q.SoQD
                     WHERE q.MaNV = nv.MaNV AND q.TrangThai = N'Đã ký' AND q.NgayHieuLuc <= @Cuoi
                     ORDER BY q.NgayHieuLuc DESC) td
        OUTER APPLY (SELECT TOP 1 x.PhuCapChucVu
                     FROM QuyetDinh q JOIN QDChucVu x ON x.SoQD = q.SoQD
                     WHERE q.MaNV = nv.MaNV AND q.TrangThai = N'Đã ký' AND q.NgayHieuLuc <= @Cuoi
                     ORDER BY q.NgayHieuLuc DESC) cv
        WHERE nv.TrangThai <> N'Thôi việc' AND nv.NgayVaoLam <= @Cuoi
    )
    INSERT INTO ChiTietBangLuong(MaBL, MaNV, LuongCoBan, TongPhuCap, TienPhuTroi, TienThuong, KhauTruBH, TienPhat, ThucLanh)
    SELECT @MaBL, t.MaNV, t.LuongCoBan, t.TongPhuCap, 0, 0, 0, 0, 0 FROM TT t;

    -- Tiền phụ trội
    UPDATE b SET TienPhuTroi = ROUND(ISNULL(p.Tong, 0) * (b.LuongCoBan / 208.0), 0)
    FROM ChiTietBangLuong b
    OUTER APPLY (SELECT SUM(ct.SoGio * pt.HeSo) AS Tong
                 FROM ChiTietPhieuPhuTroi ct JOIN PhieuPhuTroi pt ON pt.SoPhieu = ct.SoPhieu
                 WHERE ct.MaNV = b.MaNV AND pt.NgayPhuTroi BETWEEN @Dau AND @Cuoi) p
    WHERE b.MaBL = @MaBL;

    -- Khấu trừ bảo hiểm
    UPDATE b SET KhauTruBH = ROUND(b.LuongCoBan * ISNULL(bh.TongMuc, 0) / 100.0, 0)
    FROM ChiTietBangLuong b
    OUTER APPLY (SELECT SUM(h.MucDong) AS TongMuc
                 FROM HoSoBaoHiem h JOIN HopDong hd ON hd.SoHD = h.SoHD
                 WHERE hd.MaNV = b.MaNV AND h.NgayThamGia <= @Cuoi) bh
    WHERE b.MaBL = @MaBL;

    -- Thưởng (cộng vào lương)
    UPDATE b SET TienThuong = ISNULL(th.Tong, 0)
    FROM ChiTietBangLuong b
    JOIN NhanVien nv ON nv.MaNV = b.MaNV
    OUTER APPLY (
        SELECT SUM(CASE WHEN k.DoiTuong = N'Cá nhân' THEN k.SoTienThuong
                        ELSE k.SoTienThuong / NULLIF((SELECT COUNT(*) FROM NhanVien n2
                                                      WHERE n2.MaPB = q.MaPB AND n2.TrangThai <> N'Thôi việc'), 0) END) AS Tong
        FROM QuyetDinh q
        JOIN QDKhenThuong k ON k.SoQD = q.SoQD
        JOIN PhieuChiKhenThuong pc ON pc.SoQD = q.SoQD AND pc.HinhThucChi = N'Cộng vào lương'
        WHERE q.TrangThai = N'Đã ký' AND pc.NgayChi BETWEEN @Dau AND @Cuoi
          AND ((k.DoiTuong = N'Cá nhân' AND q.MaNV = b.MaNV) OR (k.DoiTuong = N'Tập thể' AND q.MaPB = nv.MaPB))
    ) th
    WHERE b.MaBL = @MaBL;

    -- Phạt (trừ lương)
    UPDATE b SET TienPhat = ISNULL(ph.Tong, 0)
    FROM ChiTietBangLuong b
    JOIN NhanVien nv ON nv.MaNV = b.MaNV
    OUTER APPLY (
        SELECT SUM(CASE WHEN k.DoiTuong = N'Cá nhân' THEN k.SoTienPhat
                        ELSE k.SoTienPhat / NULLIF((SELECT COUNT(*) FROM NhanVien n2
                                                    WHERE n2.MaPB = q.MaPB AND n2.TrangThai <> N'Thôi việc'), 0) END) AS Tong
        FROM QuyetDinh q
        JOIN QDKyLuat k ON k.SoQD = q.SoQD
        JOIN PhieuThuNopPhat pt ON pt.SoQD = q.SoQD AND pt.HinhThucNop = N'Trừ lương'
        WHERE q.TrangThai = N'Đã ký' AND pt.NgayThu BETWEEN @Dau AND @Cuoi
          AND ((k.DoiTuong = N'Cá nhân' AND q.MaNV = b.MaNV) OR (k.DoiTuong = N'Tập thể' AND q.MaPB = nv.MaPB))
    ) ph
    WHERE b.MaBL = @MaBL;

    -- Cuối cùng mới quy lương cơ bản theo số công thực tế (phụ trội và BH tính trên lương đầy đủ)
    UPDATE b SET LuongCoBan = ROUND(b.LuongCoBan *
        CASE WHEN g.TongGio >= 208 THEN 1.0 ELSE g.TongGio / 208.0 END, 0)
    FROM ChiTietBangLuong b
    CROSS APPLY (SELECT ISNULL(SUM(c.SoGioLam), 0) AS TongGio FROM ChiTietChamCong c
                 WHERE c.MaNV = b.MaNV AND c.TrangThai <> N'Vắng' AND c.Ngay BETWEEN @Dau AND @Cuoi) g
    WHERE b.MaBL = @MaBL;

    UPDATE ChiTietBangLuong
    SET ThucLanh = LuongCoBan + TongPhuCap + TienPhuTroi + TienThuong - KhauTruBH - TienPhat
    WHERE MaBL = @MaBL;
    COMMIT;

    SELECT b.MaBL, b.MaNV, nv.HoTen, b.LuongCoBan, b.TongPhuCap, b.TienPhuTroi, b.TienThuong,
           b.KhauTruBH, b.TienPhat, b.ThucLanh
    FROM ChiTietBangLuong b JOIN NhanVien nv ON nv.MaNV = b.MaNV
    WHERE b.MaBL = @MaBL ORDER BY b.MaNV;
END
GO
