using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyVanPhong.Common;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.BLL
{
    public class InvoiceLine
    {
        public int MaDV;
        public decimal? ChiSoCu, ChiSoMoi;
        public decimal SoLuong, DonGia;
        public bool TheoChiSo;
    }

    public static class HoaDonBll
    {
        public static DataTable Search(string kw) { return HoaDonDal.Search(kw); }
        public static DataTable Details(int id) { return HoaDonDal.Details(id); }

        /// <summary>Bảng dịch vụ để nhập chỉ số; chỉ số cũ lấy từ chỉ số mới của kỳ trước.</summary>
        public static DataTable PrepareLines(int maHD, int thang, int nam)
        {
            var dt = new DataTable();
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("MaDV", typeof(int));
            dt.Columns.Add("TenDV", typeof(string));
            dt.Columns.Add("DonVi", typeof(string));
            dt.Columns.Add("LoaiTinh", typeof(string));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("ChiSoCu", typeof(decimal));
            dt.Columns.Add("ChiSoMoi", typeof(decimal));
            dt.Columns.Add("SoLuong", typeof(decimal));
            dt.Columns.Add("ThanhTien", typeof(decimal));
            foreach (DataRow s in DichVuBll.All().Rows)
            {
                DataRow r = dt.NewRow();
                bool meter = s["LoaiTinh"].ToString() == "Theo chỉ số";
                r["Chon"] = true;
                r["MaDV"] = s["MaDV"]; r["TenDV"] = s["TenDV"]; r["DonVi"] = s["DonVi"]; r["LoaiTinh"] = s["LoaiTinh"]; r["DonGia"] = s["DonGia"];
                if (meter)
                {
                    decimal? last = HoaDonDal.LastReading(maHD, Convert.ToInt32(s["MaDV"]), thang, nam);
                    r["ChiSoCu"] = last ?? 0m;
                    r["ChiSoMoi"] = last ?? 0m;
                    r["SoLuong"] = 0m;
                }
                else r["SoLuong"] = 1m;
                r["ThanhTien"] = Math.Round(Convert.ToDecimal(r["SoLuong"]) * Convert.ToDecimal(r["DonGia"]), 0);
                dt.Rows.Add(r);
            }
            return dt;
        }

        public static decimal LineAmount(InvoiceLine l) { return Math.Round(l.SoLuong * l.DonGia, 0); }

        public static int Create(int maHD, int thang, int nam, DateTime ngayLap, List<InvoiceLine> lines)
        {
            Rule.Ensure(maHD > 0, "Vui lòng chọn hợp đồng.");
            Rule.Ensure(lines.Count > 0, "Hóa đơn phải có ít nhất một dịch vụ.");
            foreach (var l in lines)
            {
                if (l.TheoChiSo)
                {
                    Rule.Ensure(l.ChiSoCu.HasValue && l.ChiSoMoi.HasValue, "Dịch vụ tính theo chỉ số phải nhập chỉ số cũ và mới.");
                    Rule.Ensure(l.ChiSoMoi.Value >= l.ChiSoCu.Value, "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.");
                    l.SoLuong = l.ChiSoMoi.Value - l.ChiSoCu.Value;
                }
            }
            var first = new DateTime(nam, thang, 1);
            var last = first.AddMonths(1).AddDays(-1);
            Rule.Ensure(ngayLap.Date >= first, "Ngày lập hóa đơn không được trước tháng tính tiền.");

            return Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                DataRow p = HoaDonDal.ContractPeriod(cn, tr, maHD);
                Rule.Ensure(p != null, "Không tìm thấy hợp đồng.");
                Rule.Ensure(Convert.ToDateTime(p["NgayBatDau"]) <= last && Convert.ToDateTime(p["NgayKetThuc"]) >= first,
                    "Tháng " + thang + "/" + nam + " nằm ngoài thời hạn hợp đồng.");
                Rule.Ensure(!HoaDonDal.Exists(cn, tr, maHD, thang, nam), "Hợp đồng này đã có hóa đơn dịch vụ tháng " + thang + "/" + nam + ".");
                decimal tong = 0;
                foreach (var l in lines) tong += LineAmount(l);
                int id = HoaDonDal.InsertHeader(cn, tr, maHD, thang, nam, ngayLap.Date, tong);
                foreach (var l in lines)
                    HoaDonDal.InsertDetail(cn, tr, id, l.MaDV, l.ChiSoCu, l.ChiSoMoi, l.SoLuong, l.DonGia, LineAmount(l));
                return id;
            });
        }

        public static void Pay(int id, DateTime d)
        {
            Rule.Ensure(HoaDonDal.GetStatus(id) == "Chưa thanh toán", "Hóa đơn đã được thanh toán.");
            HoaDonDal.Pay(id, d.Date);
        }

        public static void Delete(int id)
        {
            Rule.Ensure(HoaDonDal.GetStatus(id) == "Chưa thanh toán", "Không thể xóa hóa đơn đã thanh toán.");
            HoaDonDal.Delete(id);
        }
    }

    public static class NhanVienBll
    {
        public static DataTable Search(string kw) { return NhanVienDal.Search(kw); }
        public static DataTable Lookup() { return NhanVienDal.Lookup(); }

        private static void Check(Dictionary<string, object> v, bool isNew)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("MaNV")), "Vui lòng nhập mã nhân viên.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("HoTen")), "Vui lòng nhập họ tên.");
            Rule.Ensure(v.Date("NgaySinh") <= DateTime.Today.AddYears(-16), "Nhân viên phải đủ 16 tuổi.");
            Rule.IdCard(v.Str("CCCD"));
            Rule.Phone(v.Str("SDT"), "Số điện thoại");
            Rule.Ensure(NhanVienDal.CountRoomOfCompany(v.Int("MaVP"), v.Int("MaCT")) > 0,
                "Phòng làm việc phải là văn phòng mà công ty này đang thuê (hợp đồng còn hiệu lực).");
        }

        public static void Insert(Dictionary<string, object> v)
        {
            Check(v, true);
            NhanVienDal.Insert(v.Str("MaNV").Trim().ToUpper(), v.Str("HoTen").Trim(), v.Date("NgaySinh"), v.Str("GioiTinh"), v.Str("CCCD"), v.Str("SDT"),
                v.Int("MaCT"), v.Int("MaVP"), v.Str("ChucVu"), v.Str("HinhAnh"), v.Str("TrangThai"));
        }

        public static void Update(string id, Dictionary<string, object> v)
        {
            Check(v, false);
            NhanVienDal.Update(id, v.Str("HoTen").Trim(), v.Date("NgaySinh"), v.Str("GioiTinh"), v.Str("CCCD"), v.Str("SDT"),
                v.Int("MaCT"), v.Int("MaVP"), v.Str("ChucVu"), v.Str("HinhAnh"), v.Str("TrangThai"));
        }

        public static void Delete(string id) { NhanVienDal.Delete(id); }
    }

    public static class TheXeBll
    {
        public static DataTable Search(string kw) { return TheXeDal.Search(kw); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("MaThe")), "Vui lòng nhập mã thẻ.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("MaNV")), "Vui lòng chọn nhân viên được cấp thẻ.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("BienSo")), "Vui lòng nhập biển số xe.");
        }

        public static void Insert(Dictionary<string, object> v)
        {
            Check(v);
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                Rule.Ensure(TheXeDal.StaffStatus(cn, tr, v.Str("MaNV")) == "Đang làm", "Chỉ cấp thẻ cho nhân viên đang làm việc.");
                TheXeDal.Insert(cn, tr, v.Str("MaThe").Trim().ToUpper(), v.Str("MaNV"), v.Date("NgayCap"), v.Str("TrangThai"),
                    v.Str("LoaiXe"), v.Str("BienSo").Trim().ToUpper(), v.Str("HangXe"), v.Str("MauSac"));
            });
        }

        public static void Update(string maThe, Dictionary<string, object> v)
        {
            Check(v);
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                TheXeDal.Update(cn, tr, maThe, v.Date("NgayCap"), v.Str("TrangThai"), v.Str("LoaiXe"), v.Str("BienSo").Trim().ToUpper(), v.Str("HangXe"), v.Str("MauSac"));
            });
        }

        public static void Delete(string maThe) { TheXeDal.Delete(maThe); }
    }

    public static class KiemTraBll
    {
        public static DataTable Search(string kw) { return KiemTraDal.Search(kw); }
        public static DataTable DueRooms() { return KiemTraDal.DueRooms(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(v.Date("NgayKiemTra") <= DateTime.Today, "Ngày kiểm tra không được ở tương lai.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("NguoiKiemTra")), "Vui lòng nhập người kiểm tra.");
        }

        public static void Insert(Dictionary<string, object> v)
        {
            Check(v);
            KiemTraDal.Insert(v.Int("MaVP"), v.Date("NgayKiemTra"), v.Str("NguoiKiemTra").Trim(), v.Str("TinhTrang"), v.Bool("CoViPham"), v.Str("GhiChu"));
        }

        public static void Update(int id, Dictionary<string, object> v)
        {
            Check(v);
            if (!v.Bool("CoViPham"))
                Rule.Ensure(KiemTraDal.CountViolations(id) == 0, "Phiếu này đã có biên bản vi phạm, không thể bỏ đánh dấu vi phạm.");
            KiemTraDal.Update(id, v.Int("MaVP"), v.Date("NgayKiemTra"), v.Str("NguoiKiemTra").Trim(), v.Str("TinhTrang"), v.Bool("CoViPham"), v.Str("GhiChu"));
        }

        public static void Delete(int id)
        {
            Rule.Ensure(KiemTraDal.CountViolations(id) == 0, "Phiếu kiểm tra đã có biên bản vi phạm, không thể xóa.");
            KiemTraDal.Delete(id);
        }
    }

    public static class ViPhamBll
    {
        public static DataTable Search(string kw) { return ViPhamDal.Search(kw); }
        public static DataTable InspectionLookup() { return ViPhamDal.InspectionLookup(); }

        private static int Check(Dictionary<string, object> v)
        {
            int pkt = v.Int("MaPKT");
            Rule.Ensure(ViPhamDal.InspectionHasViolationFlag(pkt), "Phiếu kiểm tra này không ghi nhận vi phạm.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("NoiDung")), "Vui lòng nhập nội dung vi phạm.");
            Rule.Ensure(v.Dec("MucPhat") > 0, "Mức phạt phải lớn hơn 0.");
            int? hd = ViPhamDal.ContractOfInspection(pkt);
            Rule.Ensure(hd.HasValue, "Không xác định được hợp đồng thuê phòng này tại ngày kiểm tra.");
            return hd.Value;
        }

        private static object PaidDate(Dictionary<string, object> v)
        {
            if (v.Str("TrangThai") == "Đã nộp") return DateTime.Today;
            return null;
        }

        public static void Insert(Dictionary<string, object> v)
        {
            int hd = Check(v);
            ViPhamDal.Insert(v.Int("MaPKT"), hd, v.Date("NgayLap"), v.Str("NoiDung").Trim(), v.Dec("MucPhat"), v.Str("TrangThai"), PaidDate(v));
        }

        public static void Update(int id, Dictionary<string, object> v)
        {
            int hd = Check(v);
            object nop = PaidDate(v);
            ViPhamDal.Update(id, v.Int("MaPKT"), hd, v.Date("NgayLap"), v.Str("NoiDung").Trim(), v.Dec("MucPhat"), v.Str("TrangThai"), nop);
        }

        public static void Delete(int id)
        {
            Rule.Ensure(ViPhamDal.GetStatus(id) == "Chưa nộp", "Không thể xóa biên bản đã nộp phạt.");
            ViPhamDal.Delete(id);
        }

        public static void Pay(int id)
        {
            Rule.Ensure(ViPhamDal.GetStatus(id) == "Chưa nộp", "Biên bản này đã nộp phạt.");
            ViPhamDal.Pay(id, DateTime.Today);
        }
    }

    public static class ReportBll
    {
        public static DataRow Dashboard() { return ReportDal.Dashboard(); }
        public static DataTable Expiring(int days) { return ReportDal.ExpiringContracts(days); }
        public static DataTable DueRent() { return ReportDal.DueRentPayments(); }
        public static DataTable Revenue(int year) { return ReportDal.Revenue(year); }
        public static DataTable RoomsByFloor() { return ReportDal.RoomsByFloor(); }
        public static DataTable Debts() { return ReportDal.Debts(); }
    }
}
