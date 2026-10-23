/* =====================================================================
   LAB 6 - BAI 5: QUAN LY HO SO BENH AN  (SQL Server)
   Chay toan bo file trong SSMS: tao CSDL + bang + du lieu mau + view
   ===================================================================== */
IF DB_ID(N'QuanLyBenhAn') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyBenhAn SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyBenhAn;
END
GO
CREATE DATABASE QuanLyBenhAn COLLATE Vietnamese_CI_AS;
GO
USE QuanLyBenhAn;
GO

/* ---------------- DANH MUC ---------------- */
CREATE TABLE Khoa(
    maKhoa  VARCHAR(10)   NOT NULL CONSTRAINT PK_Khoa PRIMARY KEY,
    tenKhoa NVARCHAR(100) NOT NULL,
    moTa    NVARCHAR(200) NULL
);
CREATE TABLE BacSi(
    maBS      VARCHAR(10)   NOT NULL CONSTRAINT PK_BacSi PRIMARY KEY,
    maKhoa    VARCHAR(10)   NOT NULL CONSTRAINT FK_BacSi_Khoa REFERENCES Khoa(maKhoa),
    hoTen     NVARCHAR(100) NOT NULL,
    chuyenMon NVARCHAR(100) NULL,
    dienThoai VARCHAR(15)   NULL
);
CREATE TABLE NhanVienCSKH(
    maNV      VARCHAR(10)   NOT NULL CONSTRAINT PK_NhanVienCSKH PRIMARY KEY,
    hoTen     NVARCHAR(100) NOT NULL,
    dienThoai VARCHAR(15)   NULL
);
CREATE TABLE Thuoc(
    maThuoc    VARCHAR(10)   NOT NULL CONSTRAINT PK_Thuoc PRIMARY KEY,
    tenThuoc   NVARCHAR(100) NOT NULL,
    donViTinh  NVARCHAR(30)  NOT NULL,
    chiDinh    NVARCHAR(200) NULL,
    tacDungPhu NVARCHAR(200) NULL
);
CREATE TABLE XetNghiem(
    maXN   VARCHAR(10)   NOT NULL CONSTRAINT PK_XetNghiem PRIMARY KEY,
    tenXN  NVARCHAR(100) NOT NULL,
    moTa   NVARCHAR(200) NULL,
    donGia DECIMAL(12,0) NOT NULL CONSTRAINT DF_XN_DonGia DEFAULT 0 CONSTRAINT CK_XN_DonGia CHECK (donGia >= 0)
);

/* ---------------- BENH NHAN - DANG KY - HO SO ---------------- */
CREATE TABLE BenhNhan(
    maBN      VARCHAR(10)   NOT NULL CONSTRAINT PK_BenhNhan PRIMARY KEY,
    hoTen     NVARCHAR(100) NOT NULL,
    diaChi    NVARCHAR(200) NULL,
    ngaySinh  DATE          NULL,
    dienThoai VARCHAR(15)   NULL,
    anh3x4    VARBINARY(MAX) NULL
);
CREATE TABLE PhieuDangKy(
    maPhieu       VARCHAR(10)   NOT NULL CONSTRAINT PK_PhieuDangKy PRIMARY KEY,
    maBN          VARCHAR(10)   NOT NULL CONSTRAINT FK_PDK_BN REFERENCES BenhNhan(maBN),
    ngayDangKy    DATE          NOT NULL CONSTRAINT DF_PDK_Ngay DEFAULT CAST(GETDATE() AS DATE),
    namHieuLuc    INT           NOT NULL,
    lePhiNam      DECIMAL(12,0) NOT NULL CONSTRAINT CK_PDK_LePhi CHECK (lePhiNam >= 0),
    daThanhToan   BIT           NOT NULL CONSTRAINT DF_PDK_TT DEFAULT 0
);
CREATE TABLE HoSoBenhAn(
    maHS       VARCHAR(10)  NOT NULL CONSTRAINT PK_HoSo PRIMARY KEY,
    maBN       VARCHAR(10)  NOT NULL CONSTRAINT FK_HS_BN REFERENCES BenhNhan(maBN) CONSTRAINT UQ_HS_BN UNIQUE,
    maPhieu    VARCHAR(10)  NOT NULL CONSTRAINT FK_HS_PDK REFERENCES PhieuDangKy(maPhieu),
    ngayLap    DATE         NOT NULL,
    ngayHetHan DATE         NOT NULL,
    trangThai  NVARCHAR(30) NOT NULL CONSTRAINT DF_HS_TT DEFAULT N'Còn hiệu lực',
    CONSTRAINT CK_HS_Ngay CHECK (ngayHetHan >= ngayLap)
);

/* ---------------- LICH HEN - PHIEU KHAM ---------------- */
CREATE TABLE LichHen(
    maLH       VARCHAR(10)  NOT NULL CONSTRAINT PK_LichHen PRIMARY KEY,
    maBN       VARCHAR(10)  NOT NULL CONSTRAINT FK_LH_BN REFERENCES BenhNhan(maBN),
    maNV       VARCHAR(10)  NOT NULL CONSTRAINT FK_LH_NV REFERENCES NhanVienCSKH(maNV),
    ngayGioHen DATETIME     NOT NULL,
    loaiHen    NVARCHAR(40) NOT NULL CONSTRAINT DF_LH_Loai DEFAULT N'Khám bệnh',
    trangThai  NVARCHAR(30) NOT NULL CONSTRAINT DF_LH_TT DEFAULT N'Chờ khám'
);
CREATE TABLE PhieuKham(
    maPK        VARCHAR(10)   NOT NULL CONSTRAINT PK_PhieuKham PRIMARY KEY,
    maHS        VARCHAR(10)   NOT NULL CONSTRAINT FK_PK_HS REFERENCES HoSoBenhAn(maHS) ON DELETE CASCADE,
    maBS        VARCHAR(10)   NOT NULL CONSTRAINT FK_PK_BS REFERENCES BacSi(maBS),
    maKhoa      VARCHAR(10)   NOT NULL CONSTRAINT FK_PK_Khoa REFERENCES Khoa(maKhoa),
    maNV        VARCHAR(10)   NOT NULL CONSTRAINT FK_PK_NV REFERENCES NhanVienCSKH(maNV),
    maLH        VARCHAR(10)   NULL CONSTRAINT FK_PK_LH REFERENCES LichHen(maLH),
    ngayGioKham DATETIME      NOT NULL,
    loaiKham    NVARCHAR(40)  NOT NULL CONSTRAINT DF_PK_Loai DEFAULT N'Điều trị',
    chanDoan    NVARCHAR(300) NULL,
    ketLuan     NVARCHAR(300) NULL,
    CONSTRAINT CK_PK_Loai CHECK (loaiKham IN (N'Điều trị', N'Định kỳ'))
);
CREATE TABLE TrieuChung(
    maTC          VARCHAR(10)   NOT NULL CONSTRAINT PK_TrieuChung PRIMARY KEY,
    maPK          VARCHAR(10)   NOT NULL CONSTRAINT FK_TC_PK REFERENCES PhieuKham(maPK) ON DELETE CASCADE,
    tenTrieuChung NVARCHAR(100) NOT NULL,
    giaTri        NVARCHAR(100) NULL,
    donVi         NVARCHAR(20)  NULL
);
CREATE TABLE ChiTietXetNghiem(
    maPK    VARCHAR(10)   NOT NULL CONSTRAINT FK_CTXN_PK REFERENCES PhieuKham(maPK) ON DELETE CASCADE,
    maXN    VARCHAR(10)   NOT NULL CONSTRAINT FK_CTXN_XN REFERENCES XetNghiem(maXN),
    ngayXN  DATETIME      NULL,
    ketQua  NVARCHAR(300) NULL,
    ghiChu  NVARCHAR(200) NULL,
    CONSTRAINT PK_CTXN PRIMARY KEY (maPK, maXN)
);
CREATE TABLE ToaThuoc(
    maToa  VARCHAR(10)   NOT NULL CONSTRAINT PK_ToaThuoc PRIMARY KEY,
    maPK   VARCHAR(10)   NOT NULL CONSTRAINT FK_Toa_PK REFERENCES PhieuKham(maPK) ON DELETE CASCADE CONSTRAINT UQ_Toa_PK UNIQUE,
    ngayKe DATE          NOT NULL CONSTRAINT DF_Toa_Ngay DEFAULT CAST(GETDATE() AS DATE),
    loiDan NVARCHAR(300) NULL
);
CREATE TABLE ChiTietToaThuoc(
    maToa    VARCHAR(10)   NOT NULL CONSTRAINT FK_CTT_Toa REFERENCES ToaThuoc(maToa) ON DELETE CASCADE,
    maThuoc  VARCHAR(10)   NOT NULL CONSTRAINT FK_CTT_Thuoc REFERENCES Thuoc(maThuoc),
    soLuong  INT           NOT NULL CONSTRAINT CK_CTT_SL CHECK (soLuong > 0),
    lieuDung NVARCHAR(100) NOT NULL,
    cachDung NVARCHAR(200) NULL,
    CONSTRAINT PK_CTT PRIMARY KEY (maToa, maThuoc)
);
CREATE TABLE HoaDon(
    maHD          VARCHAR(10)   NOT NULL CONSTRAINT PK_HoaDon PRIMARY KEY,
    maPK          VARCHAR(10)   NOT NULL CONSTRAINT FK_HD_PK REFERENCES PhieuKham(maPK) ON DELETE CASCADE CONSTRAINT UQ_HD_PK UNIQUE,
    ngayThanhToan DATE          NOT NULL CONSTRAINT DF_HD_Ngay DEFAULT CAST(GETDATE() AS DATE),
    tongVienPhi   DECIMAL(12,0) NOT NULL CONSTRAINT CK_HD_Tien CHECK (tongVienPhi >= 0),
    hinhThuc      NVARCHAR(30)  NOT NULL CONSTRAINT DF_HD_HT DEFAULT N'Tiền mặt'
);
GO
CREATE INDEX IX_BenhNhan_HoTen     ON BenhNhan(hoTen);
CREATE INDEX IX_BenhNhan_DienThoai ON BenhNhan(dienThoai);
CREATE INDEX IX_PhieuKham_HS       ON PhieuKham(maHS, ngayGioKham);
GO

/* ---------------- DU LIEU MAU ---------------- */
INSERT Khoa VALUES
('K01',N'Nội tổng quát',N'Khám và điều trị nội khoa'),
('K02',N'Ngoại',N'Phẫu thuật và chấn thương'),
('K03',N'Nhi',N'Khám bệnh trẻ em'),
('K04',N'Tim mạch',N'Chuyên khoa tim và huyết áp');
INSERT BacSi VALUES
('BS001','K01',N'Nguyễn Văn An',N'Nội tổng quát','0901000001'),
('BS002','K02',N'Trần Thị Bình',N'Ngoại tổng quát','0901000002'),
('BS003','K03',N'Lê Minh Châu',N'Nhi khoa','0901000003'),
('BS004','K04',N'Phạm Quốc Dũng',N'Tim mạch','0901000004');
INSERT NhanVienCSKH VALUES
('NV001',N'Hoàng Thị Lan','0902000001'),
('NV002',N'Võ Thanh Sơn','0902000002');
INSERT Thuoc VALUES
('T001',N'Paracetamol 500mg',N'Viên',N'Hạ sốt, giảm đau',N'Hiếm: dị ứng da, tổn thương gan khi quá liều'),
('T002',N'Amoxicillin 500mg',N'Viên',N'Kháng sinh nhóm penicillin',N'Tiêu chảy, nổi mề đay'),
('T003',N'Ibuprofen 400mg',N'Viên',N'Giảm đau, kháng viêm',N'Đau dạ dày, buồn nôn'),
('T004',N'Amlodipine 5mg',N'Viên',N'Hạ huyết áp',N'Phù mắt cá chân, chóng mặt'),
('T005',N'Omeprazole 20mg',N'Viên',N'Giảm tiết acid dạ dày',N'Đau đầu, tiêu chảy'),
('T006',N'Vitamin C 500mg',N'Viên',N'Bổ sung vitamin C',N'Rối loạn tiêu hóa nhẹ'),
('T007',N'Oresol',N'Gói',N'Bù nước và điện giải',NULL),
('T008',N'Cetirizine 10mg',N'Viên',N'Chống dị ứng',N'Buồn ngủ');
INSERT XetNghiem VALUES
('XN01',N'Công thức máu',N'Đếm tế bào máu',90000),
('XN02',N'Đường huyết lúc đói',N'Glucose máu',50000),
('XN03',N'Mỡ máu (lipid)',N'Cholesterol, triglyceride',120000),
('XN04',N'Tổng phân tích nước tiểu',N'Nước tiểu 10 thông số',60000),
('XN05',N'Điện tim (ECG)',N'Điện tâm đồ 12 chuyển đạo',150000);
INSERT BenhNhan(maBN,hoTen,diaChi,ngaySinh,dienThoai) VALUES
('BN0001',N'Trần Văn Hùng',N'12 Lê Lợi, Quận 1, TP.HCM','1985-03-12','0911000001'),
('BN0002',N'Nguyễn Thị Mai',N'45 Nguyễn Huệ, Quận 1, TP.HCM','1992-07-25','0911000002'),
('BN0003',N'Lê Quang Huy',N'8 Trần Hưng Đạo, Quận 5, TP.HCM','1978-11-03','0911000003'),
('BN0004',N'Phạm Ngọc Anh',N'99 Võ Văn Tần, Quận 3, TP.HCM','2015-01-18','0911000004'),
('BN0005',N'Đỗ Thanh Tùng',N'27 Hai Bà Trưng, Quận 3, TP.HCM','1965-09-30','0911000005');
INSERT PhieuDangKy VALUES
('PD0001','BN0001','2026-01-10',2026,500000,1),
('PD0002','BN0002','2026-02-15',2026,500000,1),
('PD0003','BN0003','2026-03-05',2026,500000,1),
('PD0004','BN0004','2026-04-20',2026,500000,1),
('PD0005','BN0005','2026-05-02',2026,500000,1);
INSERT HoSoBenhAn VALUES
('HS0001','BN0001','PD0001','2026-01-10','2026-12-31',N'Còn hiệu lực'),
('HS0002','BN0002','PD0002','2026-02-15','2026-12-31',N'Còn hiệu lực'),
('HS0003','BN0003','PD0003','2026-03-05','2026-12-31',N'Còn hiệu lực'),
('HS0004','BN0004','PD0004','2026-04-20','2026-12-31',N'Còn hiệu lực'),
('HS0005','BN0005','PD0005','2026-05-02','2026-12-31',N'Còn hiệu lực');
INSERT LichHen VALUES
('LH0001','BN0001','NV001','2026-04-10 08:30',N'Định kỳ',N'Đã khám'),
('LH0002','BN0003','NV002','2026-11-05 09:00',N'Định kỳ',N'Chờ khám'),
('LH0003','BN0005','NV001','2026-11-06 14:00',N'Khám bệnh',N'Chờ khám');
INSERT PhieuKham VALUES
('PK0001','HS0001','BS001','K01','NV001',NULL,'2026-02-02 09:15',N'Điều trị',N'Viêm họng cấp',N'Điều trị ngoại trú 5 ngày'),
('PK0002','HS0001','BS001','K01','NV001','LH0001','2026-04-10 08:40',N'Định kỳ',N'Không phát hiện bệnh',N'Sức khỏe bình thường'),
('PK0003','HS0005','BS004','K04','NV002',NULL,'2026-06-12 10:00',N'Điều trị',N'Tăng huyết áp độ 1',N'Dùng thuốc hạ áp, tái khám sau 1 tháng'),
('PK0004','HS0004','BS003','K03','NV001',NULL,'2026-05-20 15:30',N'Điều trị',N'Tiêu chảy cấp',N'Bù nước, theo dõi tại nhà');
INSERT TrieuChung VALUES
('TC0001','PK0001',N'Đau họng',N'Có',NULL),
('TC0002','PK0001',N'Thân nhiệt',N'38.2',N'°C'),
('TC0003','PK0002',N'Huyết áp',N'120/80',N'mmHg'),
('TC0004','PK0003',N'Nhức đầu',N'Có',NULL),
('TC0005','PK0003',N'Huyết áp',N'150/95',N'mmHg'),
('TC0006','PK0004',N'Tiêu chảy',N'5 lần/ngày',NULL),
('TC0007','PK0004',N'Thân nhiệt',N'37.8',N'°C');
INSERT ChiTietXetNghiem VALUES
('PK0001','XN01','2026-02-02 09:40',N'Bạch cầu tăng nhẹ',NULL),
('PK0002','XN01','2026-04-10 09:00',N'Bình thường',NULL),
('PK0002','XN02','2026-04-10 09:00',N'5.0 mmol/L',N'Bình thường'),
('PK0003','XN03','2026-06-12 10:30',N'Cholesterol hơi cao',NULL),
('PK0003','XN05','2026-06-12 10:45',N'Nhịp xoang đều',NULL);
INSERT ToaThuoc VALUES
('TO0001','PK0001','2026-02-02',N'Uống sau ăn, tái khám nếu sốt quá 3 ngày'),
('TO0002','PK0003','2026-06-12',N'Đo huyết áp mỗi sáng'),
('TO0003','PK0004','2026-05-20',N'Uống nhiều nước');
INSERT ChiTietToaThuoc VALUES
('TO0001','T001',15,N'1 viên x 3 lần/ngày',N'Uống sau ăn'),
('TO0001','T002',15,N'1 viên x 3 lần/ngày',N'Uống sau ăn, đủ 5 ngày'),
('TO0002','T004',30,N'1 viên x 1 lần/ngày',N'Uống buổi sáng'),
('TO0003','T007',10,N'1 gói x 4 lần/ngày',N'Pha 200 ml nước');
INSERT HoaDon VALUES
('HD0001','PK0001','2026-02-02',250000,N'Tiền mặt'),
('HD0002','PK0003','2026-06-12',480000,N'Chuyển khoản');
GO

/* ---------------- VIEW TIEN ICH ---------------- */
CREATE VIEW vw_LichSuKham AS
SELECT pk.maPK, pk.maHS, bn.maBN, bn.hoTen AS tenBenhNhan, pk.ngayGioKham, pk.loaiKham,
       bs.hoTen AS tenBacSi, k.tenKhoa, pk.chanDoan, pk.ketLuan
FROM PhieuKham pk
JOIN HoSoBenhAn hs ON hs.maHS = pk.maHS
JOIN BenhNhan bn   ON bn.maBN = hs.maBN
JOIN BacSi bs      ON bs.maBS = pk.maBS
JOIN Khoa k        ON k.maKhoa = pk.maKhoa;
GO
CREATE VIEW vw_KhamDinhKyTheoQuy AS
SELECT hs.maHS, bn.hoTen, YEAR(pk.ngayGioKham) AS nam, DATEPART(QUARTER, pk.ngayGioKham) AS quy, COUNT(*) AS soLan
FROM PhieuKham pk
JOIN HoSoBenhAn hs ON hs.maHS = pk.maHS
JOIN BenhNhan bn   ON bn.maBN = hs.maBN
WHERE pk.loaiKham = N'Định kỳ'
GROUP BY hs.maHS, bn.hoTen, YEAR(pk.ngayGioKham), DATEPART(QUARTER, pk.ngayGioKham);
GO
PRINT N'Tao CSDL QuanLyBenhAn thanh cong.';
