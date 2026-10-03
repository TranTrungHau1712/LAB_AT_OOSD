using System;
using System.Linq;
using System.Text.RegularExpressions;
using eShopping.Data;
using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Đăng nhập + Đăng ký tài khoản khách hàng.</summary>
    public class AuthService
    {
        private readonly CustomerDao _dao = new CustomerDao();

        /// <summary>Trả về Customer nếu đúng, null nếu sai tên đăng nhập/mật khẩu.</summary>
        public Customer Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;
            var c = _dao.GetByUsername(username.Trim());
            if (c == null) return null;
            return c.PasswordHash == PasswordHasher.Hash(password) ? c : null;
        }

        /// <summary>Trả về null nếu thành công, ngược lại là thông báo lỗi.</summary>
        public string Register(Customer c, string password, string confirm)
        {
            if (string.IsNullOrWhiteSpace(c.FullName)) return "Vui lòng nhập họ tên.";
            if (c.BirthDate.Date >= DateTime.Today) return "Ngày sinh không hợp lệ.";
            if (c.BirthDate.Date < DateTime.Today.AddYears(-120)) return "Ngày sinh không hợp lệ.";
            if (string.IsNullOrWhiteSpace(c.IdNumber) || !Regex.IsMatch(c.IdNumber.Trim(), @"^[A-Za-z0-9]{8,12}$"))
                return "Số CMND/Passport gồm 8-12 ký tự chữ hoặc số.";
            if (string.IsNullOrWhiteSpace(c.Address)) return "Vui lòng nhập địa chỉ.";
            if (string.IsNullOrWhiteSpace(c.Phone) || !Regex.IsMatch(c.Phone.Trim(), @"^\d{9,11}$"))
                return "Số điện thoại gồm 9-11 chữ số.";
            if (string.IsNullOrWhiteSpace(c.Username) || !Regex.IsMatch(c.Username.Trim(), @"^[A-Za-z0-9_]{4,30}$"))
                return "Tên đăng nhập gồm 4-30 ký tự chữ/số/gạch dưới, không dấu cách.";
            if (string.IsNullOrEmpty(password) || password.Length < 6) return "Mật khẩu tối thiểu 6 ký tự.";
            if (password != confirm) return "Mật khẩu nhập lại không khớp.";
            if (!string.IsNullOrWhiteSpace(c.Email) &&
                !Regex.IsMatch(c.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Địa chỉ email không hợp lệ.";

            c.Username = c.Username.Trim();
            c.IdNumber = c.IdNumber.Trim();
            if (_dao.UsernameExists(c.Username)) return "Tên đăng nhập đã tồn tại.";
            if (_dao.IdNumberExists(c.IdNumber)) return "Số CMND/Passport đã được đăng ký.";

            c.PasswordHash = PasswordHasher.Hash(password);
            c.Id = _dao.Insert(c);
            return null;
        }
    }
}