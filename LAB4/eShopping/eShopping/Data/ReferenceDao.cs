using System.Collections.Generic;
using System.Data;
using eShopping.Models;

namespace eShopping.Data
{
    /// <summary>Bảng tham chiếu (khu vực giao hàng, lệ phí thẻ) và các bảng log.</summary>
    public class ReferenceDao
    {
        public List<ShippingZone> GetZones()
        {
            var list = new List<ShippingZone>();
            foreach (DataRow r in Db.Query("SELECT * FROM dbo.ShippingZone ORDER BY ZoneId").Rows)
                list.Add(new ShippingZone
                {
                    Id = (int)r["ZoneId"],
                    Name = (string)r["ZoneName"],
                    NormalFee = (decimal)r["NormalFee"],
                    ExpressFee = (decimal)r["ExpressFee"],
                    SameDayFee = (decimal)r["SameDayFee"]
                });
            return list;
        }

        public decimal GetCardFee(CardType type)
        {
            var o = Db.Scalar("SELECT UsageFee FROM dbo.CardFee WHERE CardType=@t", Db.P("@t", (byte)type));
            return o == null ? 0m : (decimal)o;
        }

        public void LogPayment(int? orderId, CreditCardInfo card, decimal amount, PaymentResult res)
        {
            Db.Exec(@"INSERT dbo.PaymentTransaction(OrderId,CardType,CardLast4,Amount,Approved,Message)
                      VALUES(@o,@t,@l,@a,@ok,@m)",
                Db.P("@o", orderId), Db.P("@t", (byte)card.Type), Db.P("@l", card.Last4),
                Db.P("@a", amount), Db.P("@ok", res.Approved), Db.P("@m", res.Message));
        }

        public void LogEmail(int orderId, string to, string subject, string body, bool sent)
        {
            Db.Exec(@"INSERT dbo.EmailLog(OrderId,ToAddress,Subject,Body,Sent) VALUES(@o,@to,@s,@b,@ok)",
                Db.P("@o", orderId), Db.P("@to", to), Db.P("@s", subject), Db.P("@b", body), Db.P("@ok", sent));
        }
    }
}