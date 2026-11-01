using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyVanPhong.Common;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.BLL
{
    public class ContractInput
    {
        public int MaKH;
        public DateTime NgayKy, NgayBatDau;
        public int SoThang;
        public List<int> Rooms = new List<int>();
        public bool ThuSauThangDau;
        public string GhiChu;
    }

    public static class ContractBll
    {
        public static DataTable Search(string kw) { return HopDongDal.Search(kw); }
        public static DataTable Rooms(int maHD) { return HopDongDal.Rooms(maHD); }
        public static DataTable KhachLookup() { return KhachHangBll.Lookup(); }
        public static DataTable ActiveLookup() { return HopDongDal.ActiveLookup(); }
        public static DataTable AllLookup() { return HopDongDal.AllLookup(); }

        public static DateTime EndDate(DateTime start, int months) { return start.AddMonths(months).AddDays(-1); }

        /// <summary>Lập hợp đồng: 1 transaction gồm hợp đồng + chi tiết + trạng thái phòng + lịch thanh toán (+ thu 6 tháng đầu).</summary>
        public static int Create(ContractInput c)
        {
            Rule.Ensure(c.MaKH > 0, "Vui lòng chọn khách hàng.");
            Rule.Ensure(c.Rooms.Count > 0, "Hợp đồng phải thuê ít nhất một văn phòng.");
            Rule.Ensure(c.SoThang >= 6, "Thời gian thuê tối thiểu là 6 tháng.");
            Rule.Ensure(c.NgayBatDau.Date >= c.NgayKy.Date, "Ngày bắt đầu thuê không được trước ngày ký hợp đồng.");

            return Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                foreach (int id in c.Rooms)
                {
                    DataTable r = HopDongDal.LockRoom(cn, tr, id);
                    Rule.Ensure(r.Rows.Count == 1, "Văn phòng không tồn tại.");
                    Rule.Ensure(r.Rows[0]["TrangThai"].ToString() == "Trống",
                        "Văn phòng " + r.Rows[0]["MaPhong"] + " không còn trống (" + r.Rows[0]["TrangThai"] + ").");
                }
                string so = HopDongDal.NextSoHD(cn, tr, c.NgayKy.Year);
                DateTime end = EndDate(c.NgayBatDau, c.SoThang);
                int maHD = HopDongDal.InsertHeader(cn, tr, so, c.MaKH, c.NgayKy.Date, c.NgayBatDau.Date, c.SoThang, end, c.GhiChu);
                foreach (int id in c.Rooms) HopDongDal.InsertDetail(cn, tr, maHD, id);
                HopDongDal.SetRoomsStatus(cn, tr, maHD, "Đang thuê");
                HopDongDal.GenerateSchedule(cn, tr, maHD);
                if (c.ThuSauThangDau) HopDongDal.PayPeriod(cn, tr, maHD, 1, c.NgayKy.Date);   // trả trước 6 tháng đầu
                return maHD;
            });
        }

        public static void UpdateNote(int maHD, string ghiChu) { HopDongDal.UpdateNote(maHD, ghiChu); }

        /// <summary>Thanh lý sớm: chỉ khi hết công nợ.</summary>
        public static void Terminate(int maHD)
        {
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                DataRow h = HopDongDal.GetRow(cn, tr, maHD);
                Rule.Ensure(h != null, "Không tìm thấy hợp đồng.");
                Rule.Ensure(h["TrangThai"].ToString() == "Hiệu lực", "Chỉ thanh lý được hợp đồng đang hiệu lực.");
                decimal no = HopDongDal.CongNo(cn, tr, maHD);
                Rule.Ensure(no == 0, "Hợp đồng còn công nợ " + no.ToString("N0") + " đ. Cần thanh toán đầy đủ trước khi thanh lý.");
                HopDongDal.DeleteUnpaidFuture(cn, tr, maHD);
                HopDongDal.SetRoomsStatus(cn, tr, maHD, "Trống");
                HopDongDal.SetStatus(cn, tr, maHD, "Đã thanh lý");
            });
        }

        /// <summary>Xóa hợp đồng nhập nhầm: chỉ khi chưa phát sinh thanh toán/hóa đơn/gia hạn/vi phạm.</summary>
        public static void Delete(int maHD)
        {
            Rule.Ensure(HopDongDal.CountDependents(maHD) == 0,
                "Hợp đồng đã phát sinh thanh toán, hóa đơn, gia hạn hoặc vi phạm nên không thể xóa. Hãy dùng chức năng Thanh lý.");
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                DataRow h = HopDongDal.GetRow(cn, tr, maHD);
                Rule.Ensure(h != null, "Không tìm thấy hợp đồng.");
                if (h["TrangThai"].ToString() == "Hiệu lực") HopDongDal.SetRoomsStatus(cn, tr, maHD, "Trống");
                HopDongDal.DeleteAll(cn, tr, maHD);
            });
        }
    }

    public class ExtensionInfo
    {
        public int MaHD, SoThangGoc, DaGiaHan, ToiDaConLai;
        public string SoHD, TenKH, Phong, TrangThai;
        public DateTime NgayBatDau, NgayKetThuc;
        public decimal CongNo;
    }

    public static class GiaHanBll
    {
        public static DataTable Search(string kw) { return GiaHanDal.Search(kw); }

        private static ExtensionInfo Map(DataRow r)
        {
            Rule.Ensure(r != null, "Không tìm thấy hợp đồng.");
            var i = new ExtensionInfo();
            i.MaHD = Convert.ToInt32(r["MaHD"]);
            i.SoHD = r["SoHD"].ToString();
            i.TenKH = r["HoTen"].ToString();
            i.Phong = r["Phong"] == DBNull.Value ? "" : r["Phong"].ToString();
            i.TrangThai = r["TrangThai"].ToString();
            i.NgayBatDau = Convert.ToDateTime(r["NgayBatDau"]);
            i.NgayKetThuc = Convert.ToDateTime(r["NgayKetThuc"]);
            i.SoThangGoc = Convert.ToInt32(r["SoThang"]);
            i.DaGiaHan = Convert.ToInt32(r["DaGiaHan"]);
            i.CongNo = Convert.ToDecimal(r["CongNo"]);
            i.ToiDaConLai = Math.Max(0, i.SoThangGoc / 2 - i.DaGiaHan);   // tổng gia hạn <= 1/2 thời gian thuê ban đầu
            return i;
        }

        public static ExtensionInfo GetInfo(int maHD) { return Map(GiaHanDal.GetInfo(maHD)); }

        /// <summary>Trả về danh sách vi phạm điều kiện (rỗng = đủ điều kiện gia hạn).</summary>
        public static List<string> Validate(ExtensionInfo i, int soThang)
        {
            var p = new List<string>();
            if (i.TrangThai != "Hiệu lực") p.Add("Hợp đồng không còn hiệu lực (" + i.TrangThai + ").");
            else if (i.NgayKetThuc.Date < DateTime.Today) p.Add("Hợp đồng đã quá ngày kết thúc, không thể gia hạn.");
            if (soThang < 1) p.Add("Thời gian gia hạn phải từ 1 tháng trở lên.");
            else if (soThang > i.ToiDaConLai)
                p.Add("Thời gian gia hạn vượt quy định: tối đa " + (i.SoThangGoc / 2) + " tháng (1/2 thời gian thuê ban đầu " + i.SoThangGoc
                    + " tháng), đã gia hạn " + i.DaGiaHan + " tháng, còn được gia hạn " + i.ToiDaConLai + " tháng.");
            if (i.CongNo > 0)
                p.Add("Còn công nợ " + i.CongNo.ToString("N0") + " đ (tiền thuê đến hạn, hóa đơn dịch vụ, tiền phạt). Phải thanh toán đầy đủ mới được gia hạn.");
            return p;
        }

        /// <summary>Lập phiếu gia hạn trong 1 transaction: kiểm tra lại điều kiện → phiếu + chi tiết phòng + ngày kết thúc mới + lịch thanh toán các tháng bổ sung.</summary>
        public static int Create(int maHD, int soThang, DateTime ngayLap, string ghiChu)
        {
            return Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                ExtensionInfo i = Map(GiaHanDal.GetInfo(cn, tr, maHD));
                List<string> problems = Validate(i, soThang);
                Rule.Ensure(problems.Count == 0, string.Join("\n", problems.ToArray()));
                DateTime moi = ContractBll.EndDate(i.NgayBatDau, i.SoThangGoc + i.DaGiaHan + soThang);
                string so = GiaHanDal.NextSoPhieu(cn, tr, ngayLap.Year);
                int maPG = GiaHanDal.Insert(cn, tr, so, maHD, ngayLap.Date, soThang, i.NgayKetThuc, moi, Session.FullName, ghiChu);
                HopDongDal.GenerateSchedule(cn, tr, maHD);
                return maPG;
            });
        }
    }

    public static class ThanhToanBll
    {
        public static DataTable Search(string kw, int? maHD, string status) { return ThanhToanDal.Search(kw, maHD, status); }

        public static void Pay(int maTT, DateTime date)
        {
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                DataRow r = ThanhToanDal.Get(cn, tr, maTT);
                Rule.Ensure(r != null, "Không tìm thấy kỳ thanh toán.");
                Rule.Ensure(r["TrangThai"].ToString() == "Chưa thanh toán", "Kỳ này đã được thanh toán.");
                Rule.Ensure(r["TrangThaiHD"].ToString() != "Đã thanh lý", "Hợp đồng đã thanh lý.");
                int maHD = Convert.ToInt32(r["MaHD"]), ky = Convert.ToInt32(r["KyThu"]);
                Rule.Ensure(ThanhToanDal.CountUnpaidBefore(cn, tr, maHD, ky) == 0,
                    "Phải thanh toán các kỳ trước (tiền thuê 6 tháng đầu, rồi lần lượt từng tháng) trước khi thanh toán kỳ này.");
                ThanhToanDal.SetPaid(cn, tr, maTT, date.Date);
            });
        }

        public static void Unpay(int maTT)
        {
            Db.InTransaction(delegate (SqlConnection cn, SqlTransaction tr)
            {
                DataRow r = ThanhToanDal.Get(cn, tr, maTT);
                Rule.Ensure(r != null, "Không tìm thấy kỳ thanh toán.");
                Rule.Ensure(r["TrangThai"].ToString() == "Đã thanh toán", "Kỳ này chưa thanh toán.");
                Rule.Ensure(ThanhToanDal.CountPaidAfter(cn, tr, Convert.ToInt32(r["MaHD"]), Convert.ToInt32(r["KyThu"])) == 0,
                    "Chỉ hủy được kỳ thanh toán gần nhất (các kỳ sau đã thanh toán).");
                ThanhToanDal.SetUnpaid(cn, tr, maTT);
            });
        }
    }
}
