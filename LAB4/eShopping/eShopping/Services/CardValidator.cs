using System;
using System.Linq;
using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Kiểm tra định dạng thẻ tín dụng (phía hệ thống), trước khi gọi dịch vụ thanh toán.</summary>
    public static class CardValidator
    {
        public static int NumberLength(CardType t) => t == CardType.AmericanExpress ? 15 : 16;
        public static int CsvLength(CardType t) => t == CardType.AmericanExpress ? 4 : 3;

        /// <summary>null = hợp lệ.</summary>
        public static string Validate(CreditCardInfo c, DateTime today)
        {
            if (c == null) return "Vui lòng nhập thông tin thẻ.";
            if (string.IsNullOrWhiteSpace(c.Holder)) return "Vui lòng nhập họ tên chủ thẻ.";

            string n = c.Number ?? "";
            if (n.Length == 0 || !n.All(char.IsDigit) || n.Length != NumberLength(c.Type))
                return $"Số thẻ {EnumText.Of(c.Type)} phải gồm đúng {NumberLength(c.Type)} chữ số.";

            string v = c.Csv ?? "";
            if (v.Length == 0 || !v.All(char.IsDigit) || v.Length != CsvLength(c.Type))
                return $"CSV của thẻ {EnumText.Of(c.Type)} phải gồm đúng {CsvLength(c.Type)} chữ số.";

            if (c.ExpMonth < 1 || c.ExpMonth > 12 || c.ExpYear < 2000) return "Ngày hết hạn không hợp lệ.";
            // thẻ dùng được đến hết tháng hết hạn
            var firstDayAfter = new DateTime(c.ExpYear, c.ExpMonth, 1).AddMonths(1);
            if (firstDayAfter <= today.Date) return "Thẻ đã hết hạn.";
            return null;
        }
    }
}