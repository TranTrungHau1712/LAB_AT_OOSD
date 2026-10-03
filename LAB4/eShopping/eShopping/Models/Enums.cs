namespace eShopping.Models
{
    public enum OrderType { Normal = 1, Express = 2, SameDay = 3 }

    public enum CardType { Visa = 1, Master = 2, Discover = 3, AmericanExpress = 4 }

    public enum OrderStatus { Confirmed = 1, Processing = 2, Delivered = 3, Cancelled = 4 }

    public static class EnumText
    {
        public static string Of(OrderType t)
        {
            switch (t)
            {
                case OrderType.Express: return "Chuyển phát nhanh";
                case OrderType.SameDay: return "Chuyển phát nhanh trong ngày";
                default: return "Thường";
            }
        }

        public static string Of(CardType t)
        {
            switch (t)
            {
                case CardType.Visa: return "Visa";
                case CardType.Master: return "Master";
                case CardType.Discover: return "Discover";
                default: return "American Express";
            }
        }
    }
}