using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using QuanLyVanPhong.Common;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.BLL
{
    public static class AuthBll
    {
        public static string Hash(string s)
        {
            using (var sha = SHA256.Create())
            {
                var b = sha.ComputeHash(Encoding.UTF8.GetBytes(s ?? ""));
                var sb = new StringBuilder();
                foreach (var x in b) sb.Append(x.ToString("X2"));
                return sb.ToString();
            }
        }

        public static void Login(string user, string pass)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(user), "Vui lòng nhập tên đăng nhập.");
            Rule.Ensure(!string.IsNullOrEmpty(pass), "Vui lòng nhập mật khẩu.");
            var r = AuthDal.Login(user.Trim(), Hash(pass));
            Rule.Ensure(r != null, "Tên đăng nhập hoặc mật khẩu không đúng.");
            Session.UserId = Convert.ToInt32(r["MaTK"]);
            Session.UserName = r["TenDangNhap"].ToString();
            Session.FullName = r["HoTen"].ToString();
            Session.Role = r["VaiTro"].ToString();
            HopDongDal.RefreshStatuses();   // cập nhật hợp đồng hết hạn & trả phòng về trạng thái Trống
        }
    }

    public static class TangBll
    {
        public static DataTable Search(string kw) { return TangDal.Search(kw); }
        public static DataTable Lookup() { return TangDal.Lookup(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(v.Int("SoTang") >= 1, "Số tầng phải lớn hơn hoặc bằng 1.");
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("TenTang")), "Vui lòng nhập tên tầng.");
        }
        public static void Insert(Dictionary<string, object> v) { Check(v); TangDal.Insert(v.Int("SoTang"), v.Str("TenTang").Trim(), v.Str("MoTa")); }
        public static void Update(int id, Dictionary<string, object> v) { Check(v); TangDal.Update(id, v.Int("SoTang"), v.Str("TenTang").Trim(), v.Str("MoTa")); }
        public static void Delete(int id) { TangDal.Delete(id); }
    }

    public static class VanPhongBll
    {
        public static DataTable Search(string kw) { return VanPhongDal.Search(kw); }
        public static DataTable Lookup() { return VanPhongDal.Lookup(); }
        public static DataTable Available() { return VanPhongDal.Available(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("MaPhong")), "Vui lòng nhập mã phòng.");
            Rule.Ensure(v.Dec("DienTich") > 0, "Diện tích phải lớn hơn 0.");
            Rule.Ensure(v.Dec("GiaThue") > 0, "Giá thuê phải lớn hơn 0.");
        }

        public static void Insert(Dictionary<string, object> v)
        {
            Check(v);
            Rule.Ensure(v.Str("TrangThai") != "Đang thuê", "Trạng thái 'Đang thuê' do hệ thống tự cập nhật khi lập hợp đồng.");
            VanPhongDal.Insert(v.Str("MaPhong").Trim().ToUpper(), v.Int("MaTang"), v.Dec("DienTich"), v.Str("Huong"), v.Dec("GiaThue"), v.Str("TrangThai"));
        }

        public static void Update(int id, Dictionary<string, object> v)
        {
            Check(v);
            string cur = VanPhongDal.GetStatus(id);
            if (cur == "Đang thuê")
                Rule.Ensure(v.Str("TrangThai") == "Đang thuê", "Văn phòng đang được thuê, không thể đổi trạng thái thủ công.");
            else
                Rule.Ensure(v.Str("TrangThai") != "Đang thuê", "Trạng thái 'Đang thuê' do hệ thống tự cập nhật khi lập hợp đồng.");
            VanPhongDal.Update(id, v.Str("MaPhong").Trim().ToUpper(), v.Int("MaTang"), v.Dec("DienTich"), v.Str("Huong"), v.Dec("GiaThue"), v.Str("TrangThai"));
        }

        public static void Delete(int id)
        {
            Rule.Ensure(VanPhongDal.GetStatus(id) != "Đang thuê", "Không thể xóa văn phòng đang được thuê.");
            VanPhongDal.Delete(id);
        }
    }

    public static class CongTyBll
    {
        public static DataTable Search(string kw) { return CongTyDal.Search(kw); }
        public static DataTable Lookup() { return CongTyDal.Lookup(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("TenCT")), "Vui lòng nhập tên công ty.");
            Rule.Ensure(System.Text.RegularExpressions.Regex.IsMatch(v.Str("MaSoThue") ?? "", @"^\d{10}(\d{3})?$"), "Mã số thuế gồm 10 hoặc 13 chữ số.");
            Rule.Phone(v.Str("SDT"), "Số điện thoại");
            Rule.Email(v.Str("Email"));
        }
        public static void Insert(Dictionary<string, object> v)
        {
            Check(v);
            CongTyDal.Insert(v.Str("TenCT").Trim(), v.Str("MaSoThue").Trim(), v.Str("DiaChi"), v.Str("SDT"), v.Str("Email"), v.Str("NguoiDaiDien"));
        }
        public static void Update(int id, Dictionary<string, object> v)
        {
            Check(v);
            CongTyDal.Update(id, v.Str("TenCT").Trim(), v.Str("MaSoThue").Trim(), v.Str("DiaChi"), v.Str("SDT"), v.Str("Email"), v.Str("NguoiDaiDien"));
        }
        public static void Delete(int id) { CongTyDal.Delete(id); }
    }

    public static class KhachHangBll
    {
        public static DataTable Search(string kw) { return KhachHangDal.Search(kw); }
        public static DataTable Lookup() { return KhachHangDal.Lookup(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("HoTen")), "Vui lòng nhập họ tên khách hàng.");
            Rule.IdCard(v.Str("CCCD"));
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("SDT")), "Vui lòng nhập số điện thoại.");
            Rule.Phone(v.Str("SDT"), "Số điện thoại");
            Rule.Email(v.Str("Email"));
        }
        public static void Insert(Dictionary<string, object> v)
        {
            Check(v);
            KhachHangDal.Insert(v.Str("HoTen").Trim(), v.Str("CCCD").Trim(), v.Str("SDT"), v.Str("Email"), v.Str("DiaChi"), v.IntN("MaCT"));
        }
        public static void Update(int id, Dictionary<string, object> v)
        {
            Check(v);
            KhachHangDal.Update(id, v.Str("HoTen").Trim(), v.Str("CCCD").Trim(), v.Str("SDT"), v.Str("Email"), v.Str("DiaChi"), v.IntN("MaCT"));
        }
        public static void Delete(int id) { KhachHangDal.Delete(id); }
    }

    public static class DichVuBll
    {
        public static DataTable Search(string kw) { return DichVuDal.Search(kw); }
        public static DataTable All() { return DichVuDal.All(); }

        private static void Check(Dictionary<string, object> v)
        {
            Rule.Ensure(!string.IsNullOrWhiteSpace(v.Str("TenDV")), "Vui lòng nhập tên dịch vụ.");
            Rule.Ensure(v.Dec("DonGia") >= 0, "Đơn giá không được âm.");
        }
        public static void Insert(Dictionary<string, object> v) { Check(v); DichVuDal.Insert(v.Str("TenDV").Trim(), v.Str("DonVi"), v.Dec("DonGia"), v.Str("LoaiTinh")); }
        public static void Update(int id, Dictionary<string, object> v) { Check(v); DichVuDal.Update(id, v.Str("TenDV").Trim(), v.Str("DonVi"), v.Dec("DonGia"), v.Str("LoaiTinh")); }
        public static void Delete(int id) { DichVuDal.Delete(id); }

        /// <summary>Tính tiền theo chỉ số: (mới - cũ) * đơn giá.</summary>
        public static decimal Calc(decimal cu, decimal moi, decimal donGia)
        {
            Rule.Ensure(moi >= cu, "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.");
            return Math.Round((moi - cu) * donGia, 0);
        }
    }
}
