using System;
using System.Linq;
using eShopping.Models;

namespace eShopping.Adapters
{
    /// <summary>Cổng kết nối tới Hệ thống dịch vụ thanh toán trực tuyến (hệ thống ngoài).</summary>
    public interface IPaymentGateway
    {
        PaymentResult Authorize(CreditCardInfo card, decimal amount);
    }

    /// <summary>
    /// Bản giả lập dùng cho prototype:
    ///  - Số thẻ phải đúng thuật toán Luhn và đúng đầu số theo loại thẻ.
    ///  - Thẻ test 4000000000000002 luôn bị từ chối (thẻ bị khóa).
    ///  - Số tiền vượt 200.000.000 đ bị từ chối (không đủ khả năng thanh toán).
    /// </summary>
    public class MockPaymentGateway : IPaymentGateway
    {
        public const decimal CreditLimit = 200000000m;
        public const string BlockedCard = "4000000000000002";

        public PaymentResult Authorize(CreditCardInfo card, decimal amount)
        {
            string n = card.Number ?? "";

            if (!PrefixMatches(card.Type, n) || !LuhnOk(n))
                return Fail("Thông tin thẻ không hợp lệ.");
            if (n == BlockedCard)
                return Fail("Thẻ bị từ chối (thẻ bị khóa).");
            if (amount > CreditLimit)
                return Fail("Thẻ không đủ khả năng thanh toán.");

            return new PaymentResult
            {
                Approved = true,
                AuthCode = "AUTH" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                Message = "Approved"
            };
        }

        private static PaymentResult Fail(string msg) => new PaymentResult { Approved = false, Message = msg };

        private static bool PrefixMatches(CardType t, string n)
        {
            switch (t)
            {
                case CardType.Visa: return n.StartsWith("4");
                case CardType.Master: return n.StartsWith("5") || n.StartsWith("2");
                case CardType.Discover: return n.StartsWith("6");
                default: return n.StartsWith("34") || n.StartsWith("37");
            }
        }

        public static bool LuhnOk(string number)
        {
            if (string.IsNullOrEmpty(number) || !number.All(char.IsDigit)) return false;
            int sum = 0;
            bool dbl = false;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                int d = number[i] - '0';
                if (dbl) { d *= 2; if (d > 9) d -= 9; }
                sum += d;
                dbl = !dbl;
            }
            return sum % 10 == 0;
        }
    }
}