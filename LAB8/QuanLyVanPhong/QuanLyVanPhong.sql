/* =====================================================================
   BÀI 4 - QUẢN LÝ CHO THUÊ VĂN PHÒNG CAO ỐC  (SQL Server 2017+)
   Chạy toàn bộ file này trong SSMS (hoặc sqlcmd) bằng một lần Execute.
   Tài khoản mẫu: admin / admin123   |   letan / 123456
   ===================================================================== */
USE master;
GO
IF DB_ID(N'QuanLyVanPhong') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyVanPhong SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyVanPhong;
END
GO
CREATE DATABASE QuanLyVanPhong;
GO
USE QuanLyVanPhong;
GO
SET DATEFORMAT ymd;
GO

/* ------------------------------ BẢNG ------------------------------ */
CREATE TABLE TaiKhoan(
    MaTK        INT IDENTITY(1,1) CONSTRAINT PK_TaiKhoan PRIMARY KEY,
    TenDangNhap VARCHAR(30)   NOT NULL CONSTRAINT UQ_TaiKhoan_Ten UNIQUE,
    MatKhau     VARCHAR(64)   NOT NULL,                    -- SHA-256 (hex, chữ hoa)
    HoTen       NVARCHAR(100) NOT NULL,
    VaiTro      NVARCHAR(20)  NOT NULL CONSTRAINT CK_TaiKhoan_VaiTro CHECK (VaiTro IN (N'Quản trị', N'Nhân viên')),
    HoatDong    BIT           NOT NULL CONSTRAINT DF_TaiKhoan_HD DEFAULT 1
);

CREATE TABLE Tang(
    MaTang  INT IDENTITY(1,1) CONSTRAINT PK_Tang PRIMARY KEY,
    SoTang  INT           NOT NULL CONSTRAINT UQ_Tang_So UNIQUE CONSTRAINT CK_Tang_So CHECK (SoTang >= 1),
    TenTang NVARCHAR(50)  NOT NULL CONSTRAINT UQ_Tang_Ten UNIQUE,
    MoTa    NVARCHAR(200) NULL
);

CREATE TABLE VanPhong(
    MaVP      INT IDENTITY(1,1) CONSTRAINT PK_VanPhong PRIMARY KEY,
    MaPhong   VARCHAR(10)   NOT NULL CONSTRAINT UQ_VanPhong_Ma UNIQUE,
    MaTang    INT           NOT NULL CONSTRAINT FK_VanPhong_Tang REFERENCES Tang(MaTang),
    DienTich  DECIMAL(10,2) NOT NULL CONSTRAINT CK_VanPhong_DT CHECK (DienTich > 0),
    Huong     NVARCHAR(20)  NOT NULL,
    GiaThue   DECIMAL(18,0) NOT NULL CONSTRAINT CK_VanPhong_Gia CHECK (GiaThue > 0),   -- giá thuê / tháng
    TrangThai NVARCHAR(20)  NOT NULL CONSTRAINT DF_VanPhong_TT DEFAULT N'Trống'
              CONSTRAINT CK_VanPhong_TT CHECK (TrangThai IN (N'Trống', N'Đang thuê', N'Bảo trì'))
);

CREATE TABLE CongTy(
    MaCT         INT IDENTITY(1,1) CONSTRAINT PK_CongTy PRIMARY KEY,
    TenCT        NVARCHAR(150) NOT NULL,
    MaSoThue     VARCHAR(20)   NOT NULL CONSTRAINT UQ_CongTy_MST UNIQUE,
    DiaChi       NVARCHAR(200) NULL,
    SDT          VARCHAR(15)   NULL,
    Email        NVARCHAR(100) NULL,
    NguoiDaiDien NVARCHAR(100) NULL
);

CREATE TABLE KhachHang(
    MaKH   INT IDENTITY(1,1) CONSTRAINT PK_KhachHang PRIMARY KEY,
    HoTen  NVARCHAR(100) NOT NULL,
    CCCD   VARCHAR(12)   NOT NULL CONSTRAINT UQ_KhachHang_CCCD UNIQUE,
    SDT    VARCHAR(15)   NOT NULL,
    Email  NVARCHAR(100) NULL,
    DiaChi NVARCHAR(200) NULL,
    MaCT   INT           NULL CONSTRAINT FK_KhachHang_CongTy REFERENCES CongTy(MaCT)   -- NULL = khách cá nhân
);

CREATE TABLE HopDong(
    MaHD        INT IDENTITY(1,1) CONSTRAINT PK_HopDong PRIMARY KEY,
    SoHD        VARCHAR(20)  NOT NULL CONSTRAINT UQ_HopDong_So UNIQUE,
    MaKH        INT          NOT NULL CONSTRAINT FK_HopDong_KH REFERENCES KhachHang(MaKH),
    NgayKy      DATE         NOT NULL,
    NgayBatDau  DATE         NOT NULL,
    SoThang     INT          NOT NULL CONSTRAINT CK_HopDong_SoThang CHECK (SoThang >= 6),   -- thời gian thuê ban đầu
    NgayKetThuc DATE         NOT NULL,
    TrangThai   NVARCHAR(20) NOT NULL CONSTRAINT DF_HopDong_TT DEFAULT N'Hiệu lực'
                CONSTRAINT CK_HopDong_TT CHECK (TrangThai IN (N'Hiệu lực', N'Hết hạn', N'Đã thanh lý')),
    GhiChu      NVARCHAR(300) NULL,
    CONSTRAINT CK_HopDong_Ngay1 CHECK (NgayBatDau >= NgayKy),
    CONSTRAINT CK_HopDong_Ngay2 CHECK (NgayKetThuc > NgayBatDau)
);

CREATE TABLE ChiTietHopDong(
    MaHD    INT           NOT NULL CONSTRAINT FK_CTHD_HD REFERENCES HopDong(MaHD) ON DELETE CASCADE,
    MaVP    INT           NOT NULL CONSTRAINT FK_CTHD_VP REFERENCES VanPhong(MaVP),
    GiaThue DECIMAL(18,0) NOT NULL CONSTRAINT CK_CTHD_Gia CHECK (GiaThue > 0),   -- giá chốt tại thời điểm ký
    CONSTRAINT PK_ChiTietHopDong PRIMARY KEY (MaHD, MaVP)
);

CREATE TABLE PhieuGiaHan(
    MaPG           INT IDENTITY(1,1) CONSTRAINT PK_PhieuGiaHan PRIMARY KEY,
    SoPhieu        VARCHAR(20)  NOT NULL CONSTRAINT UQ_PhieuGiaHan_So UNIQUE,
    MaHD           INT          NOT NULL CONSTRAINT FK_PGH_HD REFERENCES HopDong(MaHD),
    NgayLap        DATE         NOT NULL,
    SoThangGiaHan  INT          NOT NULL CONSTRAINT CK_PGH_Thang CHECK (SoThangGiaHan > 0),
    NgayKetThucCu  DATE         NOT NULL,
    NgayKetThucMoi DATE         NOT NULL,
    NguoiLap       NVARCHAR(100) NULL,
    GhiChu         NVARCHAR(300) NULL,
    CONSTRAINT CK_PGH_Ngay CHECK (NgayKetThucMoi > NgayKetThucCu)
);

CREATE TABLE ChiTietGiaHan(
    MaPG INT NOT NULL CONSTRAINT FK_CTGH_PG REFERENCES PhieuGiaHan(MaPG) ON DELETE CASCADE,
    MaVP INT NOT NULL CONSTRAINT FK_CTGH_VP REFERENCES VanPhong(MaVP),
    CONSTRAINT PK_ChiTietGiaHan PRIMARY KEY (MaPG, MaVP)
);

CREATE TABLE ThanhToan(                       -- lịch & kết quả thanh toán tiền thuê
    MaTT          INT IDENTITY(1,1) CONSTRAINT PK_ThanhToan PRIMARY KEY,
    MaHD          INT          NOT NULL CONSTRAINT FK_TT_HD REFERENCES HopDong(MaHD),
    KyThu         INT          NOT NULL CONSTRAINT CK_TT_Ky CHECK (KyThu >= 1),   -- kỳ 1 = trả trước 6 tháng đầu
    TuThang       INT          NOT NULL,
    DenThang      INT          NOT NULL,
    SoTien        DECIMAL(18,0) NOT NULL CONSTRAINT CK_TT_Tien CHECK (SoTien > 0),
    HanThanhToan  DATE         NOT NULL,
    NgayThanhToan DATE         NULL,
    TrangThai     NVARCHAR(20) NOT NULL CONSTRAINT DF_TT_TT DEFAULT N'Chưa thanh toán'
                  CONSTRAINT CK_TT_TT CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),
    CONSTRAINT UQ_TT_HD_Ky UNIQUE (MaHD, KyThu),
    CONSTRAINT CK_TT_Thang CHECK (DenThang >= TuThang),
    CONSTRAINT CK_TT_Ngay CHECK ((TrangThai = N'Đã thanh toán' AND NgayThanhToan IS NOT NULL)
                              OR (TrangThai = N'Chưa thanh toán' AND NgayThanhToan IS NULL))
);

CREATE TABLE DichVu(
    MaDV     INT IDENTITY(1,1) CONSTRAINT PK_DichVu PRIMARY KEY,
    TenDV    NVARCHAR(100) NOT NULL CONSTRAINT UQ_DichVu_Ten UNIQUE,
    DonVi    NVARCHAR(20)  NOT NULL,
    DonGia   DECIMAL(18,0) NOT NULL CONSTRAINT CK_DichVu_Gia CHECK (DonGia >= 0),
    LoaiTinh NVARCHAR(20)  NOT NULL CONSTRAINT CK_DichVu_Loai CHECK (LoaiTinh IN (N'Theo chỉ số', N'Cố định'))
);

CREATE TABLE HoaDonDichVu(
    MaHDDV        INT IDENTITY(1,1) CONSTRAINT PK_HoaDonDV PRIMARY KEY,
    MaHD          INT          NOT NULL CONSTRAINT FK_HDDV_HD REFERENCES HopDong(MaHD),
    Thang         INT          NOT NULL CONSTRAINT CK_HDDV_Thang CHECK (Thang BETWEEN 1 AND 12),
    Nam           INT          NOT NULL CONSTRAINT CK_HDDV_Nam CHECK (Nam BETWEEN 2000 AND 2100),
    NgayLap       DATE         NOT NULL,
    TongTien      DECIMAL(18,0) NOT NULL CONSTRAINT CK_HDDV_Tong CHECK (TongTien >= 0),
    NgayThanhToan DATE         NULL,
    TrangThai     NVARCHAR(20) NOT NULL CONSTRAINT DF_HDDV_TT DEFAULT N'Chưa thanh toán'
                  CONSTRAINT CK_HDDV_TT CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),
    CONSTRAINT UQ_HDDV_Ky UNIQUE (MaHD, Thang, Nam),
    CONSTRAINT CK_HDDV_Ngay CHECK ((TrangThai = N'Đã thanh toán' AND NgayThanhToan IS NOT NULL)
                                OR (TrangThai = N'Chưa thanh toán' AND NgayThanhToan IS NULL))
);

CREATE TABLE ChiTietHoaDon(
    MaHDDV   INT           NOT NULL CONSTRAINT FK_CTHDDV_HD REFERENCES HoaDonDichVu(MaHDDV) ON DELETE CASCADE,
    MaDV     INT           NOT NULL CONSTRAINT FK_CTHDDV_DV REFERENCES DichVu(MaDV),
    ChiSoCu  DECIMAL(12,2) NULL,
    ChiSoMoi DECIMAL(12,2) NULL,
    SoLuong  DECIMAL(12,2) NOT NULL CONSTRAINT CK_CTHDDV_SL CHECK (SoLuong >= 0),
    DonGia   DECIMAL(18,0) NOT NULL,
    ThanhTien DECIMAL(18,0) NOT NULL CONSTRAINT CK_CTHDDV_TT CHECK (ThanhTien >= 0),
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY (MaHDDV, MaDV),
    CONSTRAINT CK_CTHDDV_CS CHECK (ChiSoMoi IS NULL OR ChiSoCu IS NULL OR ChiSoMoi >= ChiSoCu)
);

CREATE TABLE NhanVien(                        -- nhân viên làm việc tại các công ty thuê văn phòng
    MaNV     VARCHAR(10)   CONSTRAINT PK_NhanVien PRIMARY KEY,
    HoTen    NVARCHAR(100) NOT NULL,
    NgaySinh DATE          NOT NULL,
    GioiTinh NVARCHAR(5)   NOT NULL CONSTRAINT CK_NV_GT CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    CCCD     VARCHAR(12)   NOT NULL CONSTRAINT UQ_NV_CCCD UNIQUE,
    SDT      VARCHAR(15)   NULL,
    MaCT     INT           NOT NULL CONSTRAINT FK_NV_CT REFERENCES CongTy(MaCT),
    MaVP     INT           NOT NULL CONSTRAINT FK_NV_VP REFERENCES VanPhong(MaVP),   -- phòng (và tầng) làm việc
    ChucVu   NVARCHAR(50)  NOT NULL,
    HinhAnh  NVARCHAR(260) NULL,
    TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_NV_TT DEFAULT N'Đang làm'
              CONSTRAINT CK_NV_TT CHECK (TrangThai IN (N'Đang làm', N'Đã nghỉ'))
);

CREATE TABLE TheGiuXe(
    MaThe     VARCHAR(10) CONSTRAINT PK_TheGiuXe PRIMARY KEY,
    MaNV      VARCHAR(10) NOT NULL CONSTRAINT UQ_The_NV UNIQUE CONSTRAINT FK_The_NV REFERENCES NhanVien(MaNV),
    NgayCap   DATE        NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL CONSTRAINT DF_The_TT DEFAULT N'Hoạt động'
              CONSTRAINT CK_The_TT CHECK (TrangThai IN (N'Hoạt động', N'Đã khóa'))
);

CREATE TABLE PhuongTien(
    MaPT   INT IDENTITY(1,1) CONSTRAINT PK_PhuongTien PRIMARY KEY,
    MaThe  VARCHAR(10) NOT NULL CONSTRAINT UQ_PT_The UNIQUE
           CONSTRAINT FK_PT_The REFERENCES TheGiuXe(MaThe) ON DELETE CASCADE,
    LoaiXe NVARCHAR(20) NOT NULL CONSTRAINT CK_PT_Loai CHECK (LoaiXe IN (N'Xe máy', N'Ô tô')),
    BienSo VARCHAR(15)  NOT NULL CONSTRAINT UQ_PT_BienSo UNIQUE,
    HangXe NVARCHAR(50) NULL,
    MauSac NVARCHAR(30) NULL
);

CREATE TABLE PhieuKiemTra(
    MaPKT         INT IDENTITY(1,1) CONSTRAINT PK_PhieuKiemTra PRIMARY KEY,
    MaVP          INT           NOT NULL CONSTRAINT FK_PKT_VP REFERENCES VanPhong(MaVP),
    NgayKiemTra   DATE          NOT NULL,
    Nam           AS YEAR(NgayKiemTra) PERSISTED,
    NguoiKiemTra  NVARCHAR(100) NOT NULL,
    TinhTrang     NVARCHAR(30)  NOT NULL CONSTRAINT CK_PKT_TT CHECK (TinhTrang IN (N'Tốt', N'Cần sửa chữa', N'Hư hỏng')),
    CoViPham      BIT           NOT NULL CONSTRAINT DF_PKT_VP DEFAULT 0,   -- tự ý thay đổi kết cấu
    GhiChu        NVARCHAR(300) NULL,
    CONSTRAINT UQ_PKT_VP_Nam UNIQUE (MaVP, Nam)                          -- mỗi phòng kiểm tra 1 lần / năm
);

CREATE TABLE ViPham(
    MaVPh     INT IDENTITY(1,1) CONSTRAINT PK_ViPham PRIMARY KEY,
    MaPKT     INT           NOT NULL CONSTRAINT FK_VPh_PKT REFERENCES PhieuKiemTra(MaPKT),
    MaHD      INT           NOT NULL CONSTRAINT FK_VPh_HD REFERENCES HopDong(MaHD),
    NgayLap   DATE          NOT NULL,
    NoiDung   NVARCHAR(300) NOT NULL,
    MucPhat   DECIMAL(18,0) NOT NULL CONSTRAINT CK_VPh_Phat CHECK (MucPhat > 0),
    TrangThai NVARCHAR(20)  NOT NULL CONSTRAINT DF_VPh_TT DEFAULT N'Chưa nộp'
              CONSTRAINT CK_VPh_TT CHECK (TrangThai IN (N'Chưa nộp', N'Đã nộp')),
    NgayNop   DATE          NULL,
    CONSTRAINT CK_VPh_Ngay CHECK ((TrangThai = N'Đã nộp' AND NgayNop IS NOT NULL)
                               OR (TrangThai = N'Chưa nộp' AND NgayNop IS NULL))
);
GO

CREATE INDEX IX_VanPhong_Tang   ON VanPhong(MaTang);
CREATE INDEX IX_HopDong_KH      ON HopDong(MaKH);
CREATE INDEX IX_ThanhToan_HD    ON ThanhToan(MaHD, TrangThai);
CREATE INDEX IX_NhanVien_CT     ON NhanVien(MaCT);
GO

/* --------------------- HÀM & THỦ TỤC NGHIỆP VỤ --------------------- */
-- Công nợ của một hợp đồng = tiền thuê đã đến hạn chưa trả + hóa đơn dịch vụ chưa trả + tiền phạt chưa nộp
CREATE FUNCTION dbo.fn_CongNo(@MaHD INT) RETURNS DECIMAL(18,0)
AS
BEGIN
    DECLARE @t DECIMAL(18,0) = 0;
    SELECT @t = @t + ISNULL(SUM(SoTien), 0)  FROM ThanhToan
        WHERE MaHD = @MaHD AND TrangThai = N'Chưa thanh toán' AND HanThanhToan <= CAST(GETDATE() AS DATE);
    SELECT @t = @t + ISNULL(SUM(TongTien), 0) FROM HoaDonDichVu
        WHERE MaHD = @MaHD AND TrangThai = N'Chưa thanh toán';
    SELECT @t = @t + ISNULL(SUM(MucPhat), 0)  FROM ViPham
        WHERE MaHD = @MaHD AND TrangThai = N'Chưa nộp';
    RETURN @t;
END
GO

-- Phát sinh lịch thanh toán: kỳ 1 = 6 tháng đầu (trả trước, hạn = ngày ký), từ tháng 7 mỗi tháng 1 kỳ.
-- Chạy lại sau khi gia hạn: chỉ bổ sung các tháng còn thiếu.
CREATE PROCEDURE dbo.sp_PhatSinhLichThanhToan @MaHD INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @BatDau DATE, @KetThuc DATE, @Ky DATE, @Tong INT, @Da INT, @KyThu INT, @Thue DECIMAL(18,0);
    SELECT @BatDau = NgayBatDau, @KetThuc = NgayKetThuc, @Ky = NgayKy FROM HopDong WHERE MaHD = @MaHD;
    IF @BatDau IS NULL RETURN;
    SELECT @Thue = SUM(GiaThue) FROM ChiTietHopDong WHERE MaHD = @MaHD;
    SET @Tong = DATEDIFF(MONTH, @BatDau, DATEADD(DAY, 1, @KetThuc));
    SELECT @Da = ISNULL(MAX(DenThang), 0), @KyThu = ISNULL(MAX(KyThu), 0) FROM ThanhToan WHERE MaHD = @MaHD;

    IF @Da = 0
    BEGIN
        DECLARE @Den INT = CASE WHEN @Tong < 6 THEN @Tong ELSE 6 END;
        SET @KyThu = 1;
        INSERT ThanhToan(MaHD, KyThu, TuThang, DenThang, SoTien, HanThanhToan)
        VALUES (@MaHD, @KyThu, 1, @Den, @Thue * @Den, @Ky);
        SET @Da = @Den;
    END

    WHILE @Da < @Tong
    BEGIN
        SET @Da = @Da + 1;
        SET @KyThu = @KyThu + 1;
        INSERT ThanhToan(MaHD, KyThu, TuThang, DenThang, SoTien, HanThanhToan)
        VALUES (@MaHD, @KyThu, @Da, @Da, @Thue, DATEADD(MONTH, @Da - 1, @BatDau));
    END
END
GO

/* ---------------------------- DỮ LIỆU MẪU ---------------------------- */
INSERT TaiKhoan(TenDangNhap, MatKhau, HoTen, VaiTro) VALUES
 ('admin', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), N'Trần Trung Hậu', N'Quản trị'),
 ('letan', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', '123456'), 2),   N'Nguyễn Thị Lan',  N'Nhân viên');

INSERT Tang(SoTang, TenTang, MoTa) VALUES
 (1, N'Tầng 1', N'Sảnh, quầy lễ tân, văn phòng nhỏ'),
 (2, N'Tầng 2', N'Khối văn phòng thấp tầng'),
 (3, N'Tầng 3', N'Khối văn phòng trung tầng'),
 (4, N'Tầng 4', N'Khối văn phòng trung tầng'),
 (5, N'Tầng 5', N'Khối văn phòng cao tầng, view đẹp');

INSERT VanPhong(MaPhong, MaTang, DienTich, Huong, GiaThue, TrangThai) VALUES
 ('P101', 1, 40.00, N'Đông',      12000000, N'Trống'),
 ('P102', 1, 55.50, N'Tây',       16000000, N'Trống'),
 ('P201', 2, 60.00, N'Nam',       20000000, N'Đang thuê'),
 ('P202', 2, 75.00, N'Bắc',       24000000, N'Đang thuê'),
 ('P203', 2, 45.00, N'Đông Nam',  15000000, N'Trống'),
 ('P301', 3, 90.00, N'Đông',      32000000, N'Đang thuê'),
 ('P302', 3, 50.00, N'Tây Nam',   18000000, N'Trống'),
 ('P303', 3, 65.00, N'Nam',       22000000, N'Trống'),
 ('P401', 4, 80.00, N'Bắc',       30000000, N'Đang thuê'),
 ('P402', 4, 120.00, N'Đông Bắc', 46000000, N'Trống'),
 ('P403', 4, 55.00, N'Tây',       20000000, N'Bảo trì'),
 ('P501', 5, 100.00, N'Đông',     42000000, N'Trống'),
 ('P502', 5, 150.00, N'Nam',      60000000, N'Trống'),
 ('P503', 5, 70.00, N'Tây Bắc',   28000000, N'Trống');

INSERT CongTy(TenCT, MaSoThue, DiaChi, SDT, Email, NguoiDaiDien) VALUES
 (N'Công ty TNHH Phần mềm Sao Việt',   '0312345678', N'12 Nguyễn Huệ, Q.1, TP.HCM',   '0283822111', N'info@saoviet.vn',   N'Lê Minh Tuấn'),
 (N'Công ty CP Thương mại An Phát',    '0309876543', N'45 Lê Lợi, Q.1, TP.HCM',       '0283933222', N'contact@anphat.vn', N'Phạm Thu Hà'),
 (N'Công ty TNHH Xây dựng Hòa Bình',   '0301122334', N'88 Hai Bà Trưng, Q.3, TP.HCM', '0283844333', N'hb@hoabinh.vn',     N'Võ Quốc Bảo');

INSERT KhachHang(HoTen, CCCD, SDT, Email, DiaChi, MaCT) VALUES
 (N'Lê Minh Tuấn', '079090001111', '0901000111', N'tuan@saoviet.vn',  N'Q.7, TP.HCM',       1),
 (N'Phạm Thu Hà',  '079091002222', '0902000222', N'ha@anphat.vn',     N'Q.3, TP.HCM',       2),
 (N'Võ Quốc Bảo',  '079092003333', '0903000333', N'bao@hoabinh.vn',   N'Q.Bình Thạnh',      3),
 (N'Đặng Văn Khoa','079093004444', '0904000444', N'khoa@gmail.com',   N'TP. Thủ Đức',       NULL);

-- HD1: 2 phòng, 12 tháng, đã trả 6 tháng đầu + tháng 7..10; còn hóa đơn T10 và tiền phạt chưa nộp  => CÓ CÔNG NỢ
INSERT HopDong(SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, TrangThai, GhiChu) VALUES
 ('HD-2026-001', 1, '20251220', '20260101', 12, '20261231', N'Hiệu lực', N'Thuê 2 phòng tầng 2');
-- HD2: 1 phòng, 12 tháng, đã trả trước 6 tháng đầu, tháng 7 (01/11) chưa đến hạn  => KHÔNG CÔNG NỢ (gia hạn được)
INSERT HopDong(SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, TrangThai, GhiChu) VALUES
 ('HD-2026-002', 2, '20260425', '20260501', 12, '20270430', N'Hiệu lực', N'Văn phòng tầng 3');
-- HD3: 1 phòng, 8 tháng, quá hạn tháng 7, 8  => CÓ CÔNG NỢ (không gia hạn được)
INSERT HopDong(SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, TrangThai, GhiChu) VALUES
 ('HD-2026-003', 3, '20260225', '20260301', 8, '20261031', N'Hiệu lực', N'Văn phòng tầng 4');
-- HD4: đã hết hạn (khách cá nhân)
INSERT HopDong(SoHD, MaKH, NgayKy, NgayBatDau, SoThang, NgayKetThuc, TrangThai, GhiChu) VALUES
 ('HD-2025-004', 4, '20250520', '20250601', 6, '20251130', N'Hết hạn', N'Hợp đồng đã kết thúc');

INSERT ChiTietHopDong(MaHD, MaVP, GiaThue) VALUES
 (1, 3, 20000000), (1, 4, 24000000),
 (2, 6, 32000000),
 (3, 9, 30000000),
 (4, 2, 16000000);

EXEC dbo.sp_PhatSinhLichThanhToan 1;
EXEC dbo.sp_PhatSinhLichThanhToan 2;
EXEC dbo.sp_PhatSinhLichThanhToan 3;
EXEC dbo.sp_PhatSinhLichThanhToan 4;

-- Thanh toán tiền thuê đã thu
UPDATE ThanhToan SET TrangThai = N'Đã thanh toán', NgayThanhToan = '20251220' WHERE MaHD = 1 AND KyThu = 1;
UPDATE ThanhToan SET TrangThai = N'Đã thanh toán', NgayThanhToan = DATEADD(DAY, -1, HanThanhToan) WHERE MaHD = 1 AND KyThu BETWEEN 2 AND 5;
UPDATE ThanhToan SET TrangThai = N'Đã thanh toán', NgayThanhToan = '20260425' WHERE MaHD = 2 AND KyThu = 1;
UPDATE ThanhToan SET TrangThai = N'Đã thanh toán', NgayThanhToan = '20260225' WHERE MaHD = 3 AND KyThu = 1;   -- tháng 7, 8 của HD3 chưa trả
UPDATE ThanhToan SET TrangThai = N'Đã thanh toán', NgayThanhToan = '20250520' WHERE MaHD = 4;

INSERT DichVu(TenDV, DonVi, DonGia, LoaiTinh) VALUES
 (N'Điện',                 N'kWh',   3500,   N'Theo chỉ số'),
 (N'Nước',                 N'm3',    18000,  N'Theo chỉ số'),
 (N'Internet',             N'tháng', 500000, N'Cố định'),
 (N'Vệ sinh văn phòng',    N'tháng', 300000, N'Cố định');

INSERT HoaDonDichVu(MaHD, Thang, Nam, NgayLap, TongTien, NgayThanhToan, TrangThai) VALUES
 (1, 9,  2026, '20260930', 2291000, '20261005', N'Đã thanh toán'),
 (1, 10, 2026, '20261029', 2309000, NULL,       N'Chưa thanh toán');
INSERT ChiTietHoaDon(MaHDDV, MaDV, ChiSoCu, ChiSoMoi, SoLuong, DonGia, ThanhTien) VALUES
 (1, 1, 1000, 1450, 450, 3500,   1575000),
 (1, 2, 50,   62,   12,  18000,  216000),
 (1, 3, NULL, NULL, 1,   500000, 500000),
 (2, 1, 1450, 1900, 450, 3500,   1575000),
 (2, 2, 62,   75,   13,  18000,  234000),
 (2, 3, NULL, NULL, 1,   500000, 500000);

INSERT NhanVien(MaNV, HoTen, NgaySinh, GioiTinh, CCCD, SDT, MaCT, MaVP, ChucVu) VALUES
 ('NV001', N'Nguyễn Hoàng Nam', '19950312', N'Nam', '079095000001', '0911000001', 1, 3, N'Lập trình viên'),
 ('NV002', N'Trần Thị Mai',     '19970825', N'Nữ',  '079097000002', '0911000002', 1, 3, N'Kế toán'),
 ('NV003', N'Lý Quốc Bình',     '19900105', N'Nam', '079090000003', '0911000003', 1, 4, N'Trưởng phòng'),
 ('NV004', N'Phạm Ngọc Anh',    '19980214', N'Nữ',  '079098000004', '0911000004', 2, 6, N'Nhân viên kinh doanh'),
 ('NV005', N'Đỗ Văn Long',      '19930630', N'Nam', '079093000005', '0911000005', 2, 6, N'Quản lý kho'),
 ('NV006', N'Hồ Thanh Tâm',     '19960919', N'Nữ',  '079096000006', '0911000006', 3, 9, N'Kỹ sư công trình');

INSERT TheGiuXe(MaThe, MaNV, NgayCap) VALUES
 ('T001', 'NV001', '20260105'), ('T002', 'NV004', '20260510'), ('T003', 'NV006', '20260305');
INSERT PhuongTien(MaThe, LoaiXe, BienSo, HangXe, MauSac) VALUES
 ('T001', N'Xe máy', '59X1-123.45', N'Honda Vision', N'Trắng'),
 ('T002', N'Ô tô',   '51H-678.90',  N'Toyota Vios',  N'Đen'),
 ('T003', N'Xe máy', '59P2-456.78', N'Yamaha Exciter', N'Xanh');

INSERT PhieuKiemTra(MaVP, NgayKiemTra, NguoiKiemTra, TinhTrang, CoViPham, GhiChu) VALUES
 (3, '20260315', N'Nguyễn Thị Lan', N'Tốt',          0, NULL),
 (4, '20260315', N'Nguyễn Thị Lan', N'Cần sửa chữa', 1, N'Đục tường ngăn phòng họp khi chưa xin phép'),
 (6, '20260610', N'Nguyễn Thị Lan', N'Tốt',          0, NULL),
 (9, '20250920', N'Nguyễn Thị Lan', N'Tốt',          0, NULL),
 (9, '20260212', N'Nguyễn Thị Lan', N'Hư hỏng',      1, N'Tự ý lắp thêm vách kính');

INSERT ViPham(MaPKT, MaHD, NgayLap, NoiDung, MucPhat, TrangThai, NgayNop) VALUES
 (2, 1, '20260316', N'Tự ý đục tường, thay đổi kết cấu phòng P202', 5000000, N'Chưa nộp', NULL),
 (5, 3, '20260213', N'Tự ý lắp thêm vách kính phòng P401',          3000000, N'Đã nộp',   '20260220');
GO

-- Đồng bộ trạng thái phòng với hợp đồng hiệu lực (kiểm tra nhanh dữ liệu mẫu)
SELECT N'Tổng phòng' AS ChiTieu, COUNT(*) AS GiaTri FROM VanPhong
UNION ALL SELECT N'Hợp đồng hiệu lực', COUNT(*) FROM HopDong WHERE TrangThai = N'Hiệu lực'
UNION ALL SELECT N'Công nợ HD1', dbo.fn_CongNo(1)
UNION ALL SELECT N'Công nợ HD2', dbo.fn_CongNo(2)
UNION ALL SELECT N'Công nợ HD3', dbo.fn_CongNo(3);
GO
