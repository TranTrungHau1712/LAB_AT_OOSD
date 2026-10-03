using System;
using eShopping.Data;

namespace eShopping.Adapters
{
    /// <summary>Cổng kết nối tới Dịch vụ email (hệ thống ngoài).</summary>
    public interface IEmailService
    {
        bool Send(int orderId, string to, string subject, string body);
    }

    /// <summary>
    /// Bản giả lập: không gửi thật mà ghi vào bảng EmailLog để kiểm thử/ truy vết.
    /// Muốn gửi thật: thay thân hàm bằng System.Net.Mail.SmtpClient.
    /// </summary>
    public class LoggingEmailService : IEmailService
    {
        private readonly ReferenceDao _ref = new ReferenceDao();

        public bool Send(int orderId, string to, string subject, string body)
        {
            bool ok = true;
            try
            {
                // TODO (nếu cần gửi thật): new SmtpClient(...).Send(from, to, subject, body);
            }
            catch (Exception)
            {
                ok = false;
            }
            _ref.LogEmail(orderId, to, subject, body, ok);
            return ok;
        }
    }
}