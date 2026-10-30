using System.Collections.Generic;

namespace NhanSu
{
    public class Lookup
    {
        public string Table, Value, Display, Where;
        public Lookup(string table, string value, string display, string where = null)
        {
            Table = table; Value = value; Display = display; Where = where;
        }
    }

    /// <summary>Mô tả một màn hình quản lý (CRUD) trên một bảng hoặc một cặp bảng cha - con.</summary>
    public class EntityDef
    {
        public string Title;
        public string[] Tables;                  // bảng cha trước, bảng con sau (cùng khóa)
        public string[] Keys;                    // các cột khóa chính
        public string Where;                     // điều kiện lọc, dùng bí danh t0
        public Dictionary<string, object> Fixed = new Dictionary<string, object>();
        public Dictionary<string, Lookup> Lookups = new Dictionary<string, Lookup>();
        public Dictionary<string, string[]> Choices = new Dictionary<string, string[]>();
        public HashSet<string> Hidden = new HashSet<string>();
    }

    public static class Labels
    {
        static readonly Dictionary<string, string> map = new Dictionary<string, string>
        {
            {"MaPB","Mã phòng ban"},{"TenPB","Tên phòng ban"},{"MoTa","Mô tả"},{"NhuCauTuyenDung","Nhu cầu tuyển dụng"},
            {"MaCV","Mã chức vụ"},{"TenCV","Tên chức vụ"},{"PhuCapChucVu","Phụ cấp chức vụ"},
            {"MaNV","Mã nhân viên"},{"HoTen","Họ tên"},{"NgaySinh","Ngày sinh"},{"GioiTinh","Giới tính"},{"DiaChi","Địa chỉ"},
            {"SoDienThoai","Số điện thoại"},{"SoCCCD","Số CCCD"},{"NgayVaoLam","Ngày vào làm"},{"TrangThai","Trạng thái"},
            {"SoQD","Số quyết định"},{"LoaiQD","Loại quyết định"},{"NgayLap","Ngày lập"},{"NgayHieuLuc","Ngày hiệu lực"},
            {"NoiDung","Nội dung"},{"NguoiKy","Người ký"},
            {"ThoiGianThuViec","Thời gian thử việc (tháng)"},{"LuongThuViec","Lương thử việc"},{"DieuKhoan","Điều khoản hai bên"},
            {"LuongCoBan","Lương cơ bản"},{"PhuCapLuong","Phụ cấp lương"},{"PhuCapDocHai","Phụ cấp độc hại"},
            {"DoiTuong","Đối tượng"},{"HinhThuc","Hình thức"},{"SoTienThuong","Số tiền thưởng"},{"SoTienPhat","Số tiền phạt"},{"LyDo","Lý do"},
            {"SoHD","Số hợp đồng"},{"LoaiHD","Loại hợp đồng"},{"NgayKy","Ngày ký"},{"NgayBatDau","Ngày bắt đầu"},{"NgayKetThuc","Ngày kết thúc"},
            {"LuongThoaThuan","Lương thỏa thuận"},{"ThoiHan","Thời hạn (tháng)"},{"CoBaoHiem","Có bảo hiểm"},
            {"MaBH","Mã hồ sơ BH"},{"LoaiBH","Loại bảo hiểm"},{"SoSo","Số sổ"},{"NgayThamGia","Ngày tham gia"},{"MucDong","Mức đóng (%)"},
            {"MaCa","Mã ca"},{"TenCa","Tên ca"},{"GioBatDau","Giờ bắt đầu (hh:mm:ss)"},{"GioKetThuc","Giờ kết thúc (hh:mm:ss)"},
            {"MaKH","Mã kế hoạch"},{"Ngay","Ngày"},{"CoPhuTroi","Có phụ trội"},
            {"MaBCC","Mã bảng chấm công"},{"Thang","Tháng"},{"Nam","Năm"},{"SoGioLam","Số giờ làm"},
            {"SoPhieu","Số phiếu"},{"NgayPhuTroi","Ngày phụ trội"},{"SoGioPhuTroi","Số giờ phụ trội"},{"HeSo","Hệ số"},{"SoGio","Số giờ"},
            {"MaBL","Mã bảng lương"},{"TongPhuCap","Tổng phụ cấp"},{"TienPhuTroi","Tiền phụ trội"},{"TienThuong","Tiền thưởng"},
            {"KhauTruBH","Khấu trừ BH"},{"TienPhat","Tiền phạt"},{"ThucLanh","Thực lãnh"},
            {"SoPhieuChi","Số phiếu chi"},{"NgayChi","Ngày chi"},{"SoTien","Số tiền"},{"HinhThucChi","Hình thức chi"},
            {"SoPhieuThu","Số phiếu thu"},{"NgayThu","Ngày thu"},{"HinhThucNop","Hình thức nộp"},
            {"TenDoiTuong","Tên đối tượng"},{"NgayBatDauThuViec","Ngày bắt đầu thử việc"},{"NgayHetThuViec","Ngày hết thử việc"}
        };

        public static string Of(string col)
        {
            string s;
            return map.TryGetValue(col, out s) ? s : col;
        }
    }

    /// <summary>Danh sách các màn hình CRUD của hệ thống.</summary>
    public static class Entities
    {
        static Lookup NV() { return new Lookup("NhanVien", "MaNV", "HoTen"); }
        static Lookup PB() { return new Lookup("PhongBan", "MaPB", "TenPB"); }
        static readonly string[] TrangThaiQD = { "Chờ duyệt", "Đã ký", "Từ chối" };
        static readonly string[] CaNhanTapThe = { "Cá nhân", "Tập thể" };

        static EntityDef Single(string title, string table, params string[] keys)
        {
            return new EntityDef { Title = title, Tables = new[] { table }, Keys = keys };
        }

        static EntityDef QD(string title, string loai, string child)
        {
            var d = new EntityDef
            {
                Title = title,
                Tables = new[] { "QuyetDinh", child },
                Keys = new[] { "SoQD" },
                Where = "t0.LoaiQD='" + loai + "'"
            };
            d.Fixed["LoaiQD"] = loai;
            d.Hidden.Add("LoaiQD");
            d.Lookups["MaNV"] = NV();
            d.Lookups["MaPB"] = PB();
            d.Choices["TrangThai"] = TrangThaiQD;
            return d;
        }

        static EntityDef HD(string title, string loai, string child)
        {
            var d = new EntityDef
            {
                Title = title,
                Tables = new[] { "HopDong", child },
                Keys = new[] { "SoHD" },
                Where = "t0.LoaiHD=N'" + loai + "'"
            };
            d.Fixed["LoaiHD"] = loai;
            d.Hidden.Add("LoaiHD");
            d.Lookups["MaNV"] = NV();
            return d;
        }

        public static EntityDef PhongBan() { return Single("Phòng ban", "PhongBan", "MaPB"); }
        public static EntityDef ChucVu() { return Single("Chức vụ", "ChucVu", "MaCV"); }
        public static EntityDef NhanVien()
        {
            var d = Single("Nhân viên", "NhanVien", "MaNV");
            d.Lookups["MaPB"] = PB();
            d.Choices["GioiTinh"] = new[] { "Nam", "Nữ" };
            d.Choices["TrangThai"] = new[] { "Thử việc", "Chính thức", "Thôi việc" };
            return d;
        }
        public static EntityDef CaLamViec() { return Single("Ca làm việc", "CaLamViec", "MaCa"); }

        public static EntityDef QDTuyenDung() { return QD("Quyết định tuyển dụng (thử việc)", "TUYENDUNG", "QDTuyenDung"); }
        public static EntityDef QDChucVu()
        {
            var d = QD("Quyết định chức vụ", "CHUCVU", "QDChucVu");
            d.Lookups["MaCV"] = new Lookup("ChucVu", "MaCV", "TenCV");
            return d;
        }
        public static EntityDef QDLuong() { return QD("Quyết định lương", "LUONG", "QDLuong"); }
        public static EntityDef QDKhenThuong()
        {
            var d = QD("Quyết định khen thưởng", "KHENTHUONG", "QDKhenThuong");
            d.Choices["DoiTuong"] = CaNhanTapThe;
            return d;
        }
        public static EntityDef QDKyLuat()
        {
            var d = QD("Quyết định kỷ luật", "KYLUAT", "QDKyLuat");
            d.Choices["DoiTuong"] = CaNhanTapThe;
            return d;
        }

        public static EntityDef HopDongDaiHan() { return HD("Hợp đồng dài hạn", "Dài hạn", "HopDongDaiHan"); }
        public static EntityDef HopDongNganHan() { return HD("Hợp đồng ngắn hạn", "Ngắn hạn", "HopDongNganHan"); }
        public static EntityDef HoSoBaoHiem()
        {
            var d = Single("Hồ sơ BHYT, BHXH", "HoSoBaoHiem", "MaBH");
            d.Lookups["SoHD"] = new Lookup("HopDong", "SoHD", "MaNV");
            d.Choices["LoaiBH"] = new[] { "BHYT", "BHXH" };
            return d;
        }

        public static EntityDef KeHoachPhanCong()
        {
            var d = Single("Kế hoạch phân công tuần", "KeHoachPhanCong", "MaKH");
            d.Lookups["MaPB"] = PB();
            return d;
        }
        public static EntityDef ChiTietPhanCong()
        {
            var d = Single("Chi tiết phân công", "ChiTietPhanCong", "MaKH", "MaNV", "Ngay");
            d.Lookups["MaKH"] = new Lookup("KeHoachPhanCong", "MaKH", "MaPB");
            d.Lookups["MaNV"] = NV();
            d.Lookups["MaCa"] = new Lookup("CaLamViec", "MaCa", "TenCa");
            return d;
        }
        public static EntityDef BangChamCong()
        {
            var d = Single("Bảng chấm công", "BangChamCong", "MaBCC");
            d.Lookups["MaPB"] = PB();
            return d;
        }
        public static EntityDef ChiTietChamCong()
        {
            var d = Single("Chi tiết chấm công", "ChiTietChamCong", "MaBCC", "MaNV", "Ngay");
            d.Lookups["MaBCC"] = new Lookup("BangChamCong", "MaBCC", "MaPB");
            d.Lookups["MaNV"] = NV();
            d.Choices["TrangThai"] = new[] { "Đi làm", "Nghỉ phép", "Vắng" };
            return d;
        }
        public static EntityDef PhieuPhuTroi()
        {
            var d = Single("Phiếu ghi nhận giờ phụ trội", "PhieuPhuTroi", "SoPhieu");
            d.Choices["HinhThuc"] = new[] { "Tăng ca", "Ngày lễ", "Ngày nghỉ" };
            return d;
        }
        public static EntityDef ChiTietPhieuPhuTroi()
        {
            var d = Single("Chi tiết phiếu phụ trội", "ChiTietPhieuPhuTroi", "SoPhieu", "MaNV");
            d.Lookups["SoPhieu"] = new Lookup("PhieuPhuTroi", "SoPhieu", "HinhThuc");
            d.Lookups["MaNV"] = NV();
            return d;
        }
        public static EntityDef PhieuChiKhenThuong()
        {
            var d = Single("Phiếu chi khen thưởng", "PhieuChiKhenThuong", "SoPhieuChi");
            d.Lookups["SoQD"] = new Lookup("QuyetDinh", "SoQD", "NoiDung", "LoaiQD='KHENTHUONG'");
            d.Choices["HinhThucChi"] = new[] { "Tiền mặt", "Cộng vào lương" };
            return d;
        }
        public static EntityDef PhieuThuNopPhat()
        {
            var d = Single("Phiếu thu nộp phạt", "PhieuThuNopPhat", "SoPhieuThu");
            d.Lookups["SoQD"] = new Lookup("QuyetDinh", "SoQD", "NoiDung", "LoaiQD='KYLUAT'");
            d.Choices["HinhThucNop"] = new[] { "Nộp trực tiếp", "Trừ lương" };
            return d;
        }
    }
}
