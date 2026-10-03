using eShopping.Adapters;
using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Điểm khởi tạo dịch vụ + trạng thái phiên làm việc (khách đang đăng nhập, giỏ hàng hiện tại).</summary>
    public static class AppServices
    {
        public static readonly IProductSystemAdapter ProductSystem = new ProductSystemAdapter();
        public static readonly CatalogService Catalog = new CatalogService(ProductSystem);
        public static readonly AuthService Auth = new AuthService();
        public static readonly OrderService Orders =
            new OrderService(ProductSystem, new MockPaymentGateway(), new LoggingEmailService());
    }

    public static class Session
    {
        public static Customer CurrentCustomer { get; set; }
        public static ShoppingCart Cart { get; } = new ShoppingCart();
        public static bool IsLoggedIn => CurrentCustomer != null;
    }
}