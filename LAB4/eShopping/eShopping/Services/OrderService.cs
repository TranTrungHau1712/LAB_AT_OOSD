using System;
using System.Text;
using eShopping.Adapters;
using eShopping.Data;
using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Quy trình đặt mua hàng và tính tiền (use case "Đặt mua hàng").</summary>
    public class OrderService
    {
        private readonly IProductSystemAdapter _products;
        private readonly IPaymentGateway _gateway;
        private readonly IEmailService _email;
        private readonly OrderDao _orders = new OrderDao();
        private readonly ReferenceDao _ref = new ReferenceDao();

        public OrderService(IProductSystemAdapter products, IPaymentGateway gateway, IEmailService email)
        {
            _products = products;
            _gateway = gateway;
            _email = email;
        }

        public System.Collections.Generic.List<ShippingZone> GetZones() => _ref.GetZones();

        /// <summary>Tính tạm các khoản phí để hiển thị trên form (chưa ghi gì xuống CSDL).</summary>
        public FeeBreakdown CalculateFees(decimal itemsTotal, ShippingZone zone, OrderType type, CardType card)
        {
            var policy = ShippingPolicyFactory.For(type);
            return new FeeBreakdown
            {
                ItemsTotal = itemsTotal,
                ShippingFee = zone == null ? 0m : policy.Fee(zone, itemsTotal),
                ShippingFree = policy.IsFree(itemsTotal),
                CardFee = _ref.GetCardFee(card)
            };
        }

        public OrderResult Checkout(OrderRequest req)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (req.Customer == null) return Fail("Bạn cần đăng nhập trước khi đặt hàng.");
            if (req.Cart == null || req.Cart.IsEmpty) return Fail("Giỏ hàng đang trống.");
            if (req.Zone == null) return Fail("Vui lòng chọn khu vực giao hàng.");

            var r = req.Receiver;
            if (r == null || string.IsNullOrWhiteSpace(r.FullName) ||
                string.IsNullOrWhiteSpace(r.Address) || string.IsNullOrWhiteSpace(r.Phone))
                return Fail("Vui lòng nhập đầy đủ họ tên, địa chỉ, điện thoại người nhận.");

            string cardErr = CardValidator.Validate(req.Card, DateTime.Today);
            if (cardErr != null) return Fail(cardErr);

            // 2. Lấy lại giá hiện hành + tình trạng từ Hệ thống quản lý sản phẩm
            var order = new Order
            {
                CustomerId = req.Customer.Id,
                OrderDate = DateTime.Now,
                OrderType = req.OrderType,
                Receiver = r,
                ZoneId = req.Zone.Id,
                CardType = req.Card.Type,
                CardLast4 = req.Card.Last4,
                CardHolder = req.Card.Holder.Trim(),
                Status = OrderStatus.Confirmed
            };
            foreach (var it in req.Cart.Items)
            {
                var p = _products.GetProduct(it.Product.Id);
                if (p == null) return Fail($"Sản phẩm \"{it.Product.Name}\" không còn tồn tại.");
                if (!p.InStock) return Fail($"Sản phẩm \"{p.Name}\" đã hết hàng, vui lòng bỏ khỏi giỏ.");
                order.Items.Add(new OrderItem { ProductId = p.Id, ProductName = p.Name, Quantity = it.Quantity, UnitPrice = p.Price });
            }

            // 3. Tính tiền
            decimal itemsTotal = 0;
            foreach (var oi in order.Items) itemsTotal += oi.SubTotal;
            var fees = CalculateFees(itemsTotal, req.Zone, req.OrderType, req.Card.Type);
            order.ItemsTotal = fees.ItemsTotal;
            order.ShippingFee = fees.ShippingFee;
            order.CardFee = fees.CardFee;
            order.Total = fees.Total;

            // 4. Kiểm tra thẻ qua dịch vụ thanh toán trực tuyến
            PaymentResult pay;
            try { pay = _gateway.Authorize(req.Card, order.Total); }
            catch (Exception ex) { return Fail("Không kết nối được dịch vụ thanh toán: " + ex.Message); }

            if (!pay.Approved)
            {
                _ref.LogPayment(null, req.Card, order.Total, pay);
                return Fail("Thanh toán bị từ chối: " + pay.Message);
            }
            order.AuthCode = pay.AuthCode;

            // 5. Ghi nhận đơn đặt hàng
            try { order.Id = _orders.Insert(order); }
            catch (Exception ex) { return Fail("Không lưu được đơn hàng: " + ex.Message); }
            _ref.LogPayment(order.Id, req.Card, order.Total, pay);

            // 6. Gửi email xác nhận (nếu khách có email) - lỗi email không làm hỏng đơn
            var result = new OrderResult
            {
                Success = true,
                OrderId = order.Id,
                Total = order.Total,
                Message = "Đặt hàng thành công."
            };
            if (!string.IsNullOrWhiteSpace(req.Customer.Email))
            {
                result.EmailAttempted = true;
                try
                {
                    result.EmailSent = _email.Send(order.Id, req.Customer.Email.Trim(),
                        "Xác nhận đơn hàng #" + order.Id, BuildEmailBody(order, req.Customer, req.Zone));
                }
                catch (Exception) { result.EmailSent = false; }
            }
            return result;
        }

        /// <summary>Nội dung email: đầy đủ thông tin đơn, NGOẠI TRỪ thông tin thẻ tín dụng.</summary>
        public static string BuildEmailBody(Order o, Customer buyer, ShippingZone zone)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Cảm ơn " + buyer.FullName + " đã mua hàng tại e-Shopping.");
            sb.AppendLine("Mã đơn: " + o.Id + " | Thời điểm đặt: " + o.OrderDate.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("Hình thức giao hàng: " + EnumText.Of(o.OrderType) + " - " + zone.Name);
            sb.AppendLine("Người nhận: " + o.Receiver.FullName + " | " + o.Receiver.Phone + " | " + o.Receiver.Address);
            sb.AppendLine("Sản phẩm:");
            foreach (var i in o.Items)
                sb.AppendLine("  - " + i.ProductName + " x" + i.Quantity + " @ " + i.UnitPrice.ToString("N0") + " = " + i.SubTotal.ToString("N0"));
            sb.AppendLine("Tiền hàng: " + o.ItemsTotal.ToString("N0"));
            sb.AppendLine("Phí giao hàng: " + o.ShippingFee.ToString("N0"));
            sb.AppendLine("Lệ phí thanh toán thẻ: " + o.CardFee.ToString("N0"));
            sb.AppendLine("TỔNG CỘNG: " + o.Total.ToString("N0") + " đ");
            return sb.ToString();
        }

        private static OrderResult Fail(string msg) => new OrderResult { Success = false, Message = msg };
    }
}