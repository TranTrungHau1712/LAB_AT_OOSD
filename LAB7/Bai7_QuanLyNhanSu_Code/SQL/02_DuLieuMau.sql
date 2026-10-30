/* Dữ liệu mẫu - chạy sau 01_QuanLyNhanSu.sql */
USE QuanLyNhanSu;
GO
INSERT INTO PhongBan(MaPB, TenPB, MoTa, NhuCauTuyenDung) VALUES
('PB01', N'Phòng Tài chính - Kế toán', N'Kế toán, quỹ, tiền lương', 1),
('PB02', N'Phòng Kỹ thuật', N'Pha chế màu, kiểm định chất lượng', 2),
('PB03', N'Phân xưởng Sản xuất', N'Sản xuất sơn', 3);

INSERT INTO ChucVu(MaCV, TenCV, PhuCapChucVu) VALUES
('CV01', N'Trưởng phòng', 1500000),
('CV02', N'Phó phòng', 800000),
('CV03', N'Tổ trưởng', 500000);

INSERT INTO NhanVien(MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, SoCCCD, NgayVaoLam, TrangThai, MaPB) VALUES
('NV01', N'Nguyễn Văn An',   '1988-03-12', N'Nam', N'Quận 1, TP.HCM',     '0901000001', '079088000001', '2026-01-05', N'Chính thức', 'PB01'),
('NV02', N'Trần Thị Bình',   '1990-07-25', N'Nữ',  N'Quận 3, TP.HCM',     '0901000002', '079090000002', '2026-01-05', N'Chính thức', 'PB02'),
('NV03', N'Lê Hoàng Cường',  '1985-11-02', N'Nam', N'Thủ Đức, TP.HCM',    '0901000003', '079085000003', '2026-01-05', N'Chính thức', 'PB03'),
('NV04', N'Phạm Thu Dung',   '1995-05-18', N'Nữ',  N'Bình Tân, TP.HCM',   '0901000004', '079095000004', '2026-01-05', N'Chính thức', 'PB03'),
('NV05', N'Võ Minh Đạt',     '2000-09-09', N'Nam', N'Quận 12, TP.HCM',    '0901000005', '079200000005', '2026-09-15', N'Thử việc',   'PB02'),
('NV06', N'Hoàng Thị Em',    '2001-01-30', N'Nữ',  N'Gò Vấp, TP.HCM',     '0901000006', '079201000006', '2026-10-01', N'Thử việc',   'PB01');

/* Quyết định tuyển dụng (thử việc) */
INSERT INTO QuyetDinh(SoQD, LoaiQD, NgayLap, NgayHieuLuc, NoiDung, NguoiKy, TrangThai, MaNV, MaPB) VALUES
('TD01','TUYENDUNG','2026-01-02','2026-01-05',N'Tuyển dụng thử việc NV01',N'Giám đốc',N'Đã ký','NV01','PB01'),
('TD02','TUYENDUNG','2026-01-02','2026-01-05',N'Tuyển dụng thử việc NV02',N'Giám đốc',N'Đã ký','NV02','PB02'),
('TD03','TUYENDUNG','2026-01-02','2026-01-05',N'Tuyển dụng thử việc NV03',N'Giám đốc',N'Đã ký','NV03','PB03'),
('TD04','TUYENDUNG','2026-01-02','2026-01-05',N'Tuyển dụng thử việc NV04',N'Giám đốc',N'Đã ký','NV04','PB03'),
('TD05','TUYENDUNG','2026-09-10','2026-09-15',N'Tuyển dụng thử việc NV05',N'Giám đốc',N'Đã ký','NV05','PB02'),
('TD06','TUYENDUNG','2026-09-28','2026-10-01',N'Tuyển dụng thử việc NV06',N'Giám đốc',N'Đã ký','NV06','PB01');
INSERT INTO QDTuyenDung(SoQD, ThoiGianThuViec, LuongThuViec, DieuKhoan) VALUES
('TD01',2,8000000,N'Hưởng 85% lương, thử việc tối đa 2 tháng. Hai bên báo trước 3 ngày nếu chấm dứt.'),
('TD02',2,7000000,N'Hưởng 85% lương, thử việc tối đa 2 tháng. Hai bên báo trước 3 ngày nếu chấm dứt.'),
('TD03',2,5500000,N'Hưởng 85% lương, thử việc tối đa 2 tháng. Hai bên báo trước 3 ngày nếu chấm dứt.'),
('TD04',2,5000000,N'Hưởng 85% lương, thử việc tối đa 2 tháng. Hai bên báo trước 3 ngày nếu chấm dứt.'),
('TD05',2,6500000,N'Thử việc 2 tháng tại phòng Kỹ thuật, tuân thủ nội quy công ty.'),
('TD06',2,6000000,N'Thử việc 2 tháng tại phòng Tài chính - Kế toán, tuân thủ nội quy công ty.');

/* Hợp đồng chính thức cho NV01-NV04 */
INSERT INTO HopDong(SoHD, LoaiHD, NgayKy, NgayBatDau, NgayKetThuc, LuongThoaThuan, MaNV) VALUES
('HD01', N'Dài hạn', '2026-03-01','2026-03-01', NULL,         15000000,'NV01'),
('HD02', N'Dài hạn', '2026-03-01','2026-03-01', NULL,         12000000,'NV02'),
('HD03', N'Ngắn hạn','2026-03-01','2026-03-01','2027-02-28',   9000000,'NV03'),
('HD04', N'Ngắn hạn','2026-03-01','2026-03-01','2027-02-28',   8500000,'NV04');
INSERT INTO HopDongDaiHan(SoHD, CoBaoHiem) VALUES ('HD01',1),('HD02',1);
INSERT INTO HopDongNganHan(SoHD, ThoiHan) VALUES ('HD03',12),('HD04',12);
INSERT INTO HoSoBaoHiem(MaBH, LoaiBH, SoSo, NgayThamGia, MucDong, SoHD) VALUES
('BH01',N'BHYT','YT0001','2026-03-01',1.5,'HD01'),
('BH02',N'BHXH','XH0001','2026-03-01',8,  'HD01'),
('BH03',N'BHYT','YT0002','2026-03-01',1.5,'HD02'),
('BH04',N'BHXH','XH0002','2026-03-01',8,  'HD02');

/* Quyết định lương, chức vụ */
INSERT INTO QuyetDinh(SoQD, LoaiQD, NgayLap, NgayHieuLuc, NoiDung, NguoiKy, TrangThai, MaNV, MaPB) VALUES
('LU01','LUONG','2026-02-25','2026-03-01',N'Quyết định lương NV01',N'Giám đốc',N'Đã ký','NV01',NULL),
('LU02','LUONG','2026-02-25','2026-03-01',N'Quyết định lương NV02',N'Giám đốc',N'Đã ký','NV02',NULL),
('LU03','LUONG','2026-02-25','2026-03-01',N'Quyết định lương NV03',N'Giám đốc',N'Đã ký','NV03',NULL),
('LU04','LUONG','2026-02-25','2026-03-01',N'Quyết định lương NV04',N'Giám đốc',N'Đã ký','NV04',NULL),
('CV01','CHUCVU','2026-02-25','2026-03-01',N'Bổ nhiệm Trưởng phòng Tài chính - Kế toán',N'Giám đốc',N'Đã ký','NV01',NULL);
INSERT INTO QDLuong(SoQD, LuongCoBan, PhuCapLuong, PhuCapDocHai) VALUES
('LU01',15000000,500000,0),('LU02',12000000,300000,0),('LU03',9000000,0,300000),('LU04',8500000,0,300000);
INSERT INTO QDChucVu(SoQD, MaCV, PhuCapChucVu) VALUES ('CV01','CV01',1500000);

/* Ca làm việc, kế hoạch phân công tuần */
INSERT INTO CaLamViec(MaCa, TenCa, GioBatDau, GioKetThuc) VALUES
('CA1', N'Ca sáng',  '07:00:00','15:00:00'),
('CA2', N'Ca chiều', '15:00:00','23:00:00');
INSERT INTO KeHoachPhanCong(MaKH, NgayBatDau, NgayKetThuc, MaPB, NguoiLap) VALUES
('KH2610W4','2026-10-19','2026-10-25','PB03',N'Lê Hoàng Cường');
INSERT INTO ChiTietPhanCong(MaKH, MaNV, Ngay, MaCa, CoPhuTroi) VALUES
('KH2610W4','NV03','2026-10-19','CA1',0),('KH2610W4','NV04','2026-10-19','CA1',0),
('KH2610W4','NV03','2026-10-20','CA2',1),('KH2610W4','NV04','2026-10-20','CA2',1);

/* Bảng chấm công tháng 10/2026 (đi làm thứ Hai - thứ Bảy, 8 giờ/ngày) */
INSERT INTO BangChamCong(MaBCC, Thang, Nam, MaPB, NguoiLap) VALUES
('CC2610PB01',10,2026,'PB01',N'Ban chấm công'),
('CC2610PB02',10,2026,'PB02',N'Ban chấm công'),
('CC2610PB03',10,2026,'PB03',N'Ban chấm công');
;WITH D AS (
    SELECT CAST('2026-10-01' AS DATE) AS d
    UNION ALL SELECT DATEADD(DAY, 1, d) FROM D WHERE d < '2026-10-31'
)
INSERT INTO ChiTietChamCong(MaBCC, MaNV, Ngay, SoGioLam, TrangThai)
SELECT b.MaBCC, nv.MaNV, D.d, 8, N'Đi làm'
FROM NhanVien nv
JOIN BangChamCong b ON b.MaPB = nv.MaPB AND b.Thang = 10 AND b.Nam = 2026
CROSS JOIN D
WHERE DATEDIFF(DAY, '19000107', D.d) % 7 <> 0;   -- bỏ Chủ nhật (07/01/1900 là Chủ nhật)

/* Phiếu phụ trội (giờ phụ trội = 1.5 lần giờ thường) */
INSERT INTO PhieuPhuTroi(SoPhieu, NgayPhuTroi, SoGioPhuTroi, HinhThuc, HeSo) VALUES
('PT2610001','2026-10-10',3,N'Tăng ca',1.5);
INSERT INTO ChiTietPhieuPhuTroi(SoPhieu, MaNV, SoGio) VALUES
('PT2610001','NV03',3),('PT2610001','NV04',3);

/* Khen thưởng, kỷ luật */
INSERT INTO QuyetDinh(SoQD, LoaiQD, NgayLap, NgayHieuLuc, NoiDung, NguoiKy, TrangThai, MaNV, MaPB) VALUES
('KT2610001','KHENTHUONG','2026-10-14','2026-10-15',N'Thưởng hoàn thành kế hoạch sản xuất',N'Giám đốc',N'Đã ký','NV03',NULL),
('KT2610002','KHENTHUONG','2026-10-14','2026-10-15',N'Thưởng tập thể phân xưởng đạt chỉ tiêu quý',N'Giám đốc',N'Đã ký',NULL,'PB03'),
('KL2610001','KYLUAT','2026-10-19','2026-10-20',N'Phạt đi làm trễ nhiều lần',N'Giám đốc',N'Đã ký','NV04',NULL);
INSERT INTO QDKhenThuong(SoQD, DoiTuong, HinhThuc, SoTienThuong, LyDo) VALUES
('KT2610001',N'Cá nhân',N'Thưởng thành tích',500000,N'Hoàn thành vượt kế hoạch sản xuất tháng 10'),
('KT2610002',N'Tập thể',N'Thưởng tập thể',1000000,N'Phân xưởng đạt chỉ tiêu quý 3');
INSERT INTO QDKyLuat(SoQD, DoiTuong, HinhThuc, SoTienPhat, LyDo) VALUES
('KL2610001',N'Cá nhân',N'Phạt tiền',200000,N'Đi làm trễ 5 lần trong tháng');
INSERT INTO PhieuChiKhenThuong(SoPhieuChi, SoQD, NgayChi, SoTien, HinhThucChi) VALUES
('PC2610001','KT2610001','2026-10-15',500000, N'Cộng vào lương'),
('PC2610002','KT2610002','2026-10-15',1000000,N'Tiền mặt');
INSERT INTO PhieuThuNopPhat(SoPhieuThu, SoQD, NgayThu, SoTien, HinhThucNop) VALUES
('TP2610001','KL2610001','2026-10-20',200000,N'Trừ lương');
GO

/* Thử tính lương tháng 10/2026 */
EXEC sp_TinhLuongThang @Thang = 10, @Nam = 2026, @NguoiLap = N'Kế toán';
GO
