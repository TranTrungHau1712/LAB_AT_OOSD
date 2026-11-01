using System;
using System.Collections.Generic;

namespace QuanLyVanPhong.Common
{
    /// <summary>Lỗi vi phạm quy tắc nghiệp vụ (hiển thị thẳng cho người dùng).</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    public static class Rule
    {
        public static void Ensure(bool ok, string message)
        {
            if (!ok) throw new BusinessException(message);
        }

        public static void Phone(string s, string name)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            Ensure(System.Text.RegularExpressions.Regex.IsMatch(s, @"^\d{9,11}$"), name + " phải gồm 9-11 chữ số.");
        }

        public static void Email(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            Ensure(System.Text.RegularExpressions.Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"), "Email không hợp lệ.");
        }

        public static void IdCard(string s)
        {
            Ensure(System.Text.RegularExpressions.Regex.IsMatch(s ?? "", @"^(\d{9}|\d{12})$"), "CCCD/CMND phải gồm 9 hoặc 12 chữ số.");
        }
    }

    public static class Session
    {
        public static int UserId;
        public static string UserName;
        public static string FullName;
        public static string Role;
    }

    public static class Vals
    {
        public static bool IsNull(this Dictionary<string, object> v, string k)
        {
            object o;
            return !v.TryGetValue(k, out o) || o == null || o == DBNull.Value;
        }
        public static string Str(this Dictionary<string, object> v, string k) { return v.IsNull(k) ? null : Convert.ToString(v[k]); }
        public static int Int(this Dictionary<string, object> v, string k) { return Convert.ToInt32(v[k]); }
        public static int? IntN(this Dictionary<string, object> v, string k) { return v.IsNull(k) ? (int?)null : Convert.ToInt32(v[k]); }
        public static decimal Dec(this Dictionary<string, object> v, string k) { return Convert.ToDecimal(v[k]); }
        public static DateTime Date(this Dictionary<string, object> v, string k) { return Convert.ToDateTime(v[k]).Date; }
        public static bool Bool(this Dictionary<string, object> v, string k) { return !v.IsNull(k) && Convert.ToBoolean(v[k]); }
    }
}
