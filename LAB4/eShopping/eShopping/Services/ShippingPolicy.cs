using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Strategy: mỗi loại phiếu đặt hàng có một cách tính phí giao hàng.</summary>
    public interface IShippingPolicy
    {
        OrderType Type { get; }
        bool IsFree(decimal itemsTotal);
        decimal Fee(ShippingZone zone, decimal itemsTotal);
    }

    public class NormalShippingPolicy : IShippingPolicy
    {
        public OrderType Type => OrderType.Normal;
        public bool IsFree(decimal itemsTotal) => false;
        public decimal Fee(ShippingZone z, decimal itemsTotal) => z.NormalFee;
    }

    /// <summary>Miễn phí khi tổng trị giá hàng từ 1.000.000 đ.</summary>
    public class ExpressShippingPolicy : IShippingPolicy
    {
        public const decimal FreeFrom = 1000000m;
        public OrderType Type => OrderType.Express;
        public bool IsFree(decimal itemsTotal) => itemsTotal >= FreeFrom;
        public decimal Fee(ShippingZone z, decimal itemsTotal) => IsFree(itemsTotal) ? 0m : z.ExpressFee;
    }

    /// <summary>Miễn phí khi tổng trị giá hàng từ 5.000.000 đ.</summary>
    public class SameDayShippingPolicy : IShippingPolicy
    {
        public const decimal FreeFrom = 5000000m;
        public OrderType Type => OrderType.SameDay;
        public bool IsFree(decimal itemsTotal) => itemsTotal >= FreeFrom;
        public decimal Fee(ShippingZone z, decimal itemsTotal) => IsFree(itemsTotal) ? 0m : z.SameDayFee;
    }

    public static class ShippingPolicyFactory
    {
        public static IShippingPolicy For(OrderType t)
        {
            switch (t)
            {
                case OrderType.Express: return new ExpressShippingPolicy();
                case OrderType.SameDay: return new SameDayShippingPolicy();
                default: return new NormalShippingPolicy();
            }
        }
    }
}