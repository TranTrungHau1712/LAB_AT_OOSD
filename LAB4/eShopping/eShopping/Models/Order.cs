using System;
using System.Collections.Generic;

namespace eShopping.Models
{
    public class CreditCardInfo
    {
        public CardType Type { get; set; }
        public string Number { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string Holder { get; set; }
        public string Csv { get; set; }

        public string Last4
        {
            get
            {
                var n = Number ?? "";
                return n.Length >= 4 ? n.Substring(n.Length - 4) : n.PadLeft(4, '0');
            }
        }
    }

    public class ShippingZone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal NormalFee { get; set; }
        public decimal ExpressFee { get; set; }
        public decimal SameDayFee { get; set; }
        public override string ToString() => Name;
    }

    public class FeeBreakdown
    {
        public decimal ItemsTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal CardFee { get; set; }
        public bool ShippingFree { get; set; }
        public decimal Total => ItemsTotal + ShippingFee + CardFee;
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public Receiver Receiver { get; set; }
        public int ZoneId { get; set; }
        public CardType CardType { get; set; }
        public string CardLast4 { get; set; }
        public string CardHolder { get; set; }
        public string AuthCode { get; set; }
        public decimal ItemsTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal CardFee { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    /// <summary>Dữ liệu UI gửi xuống tầng Service khi bấm "Đặt hàng".</summary>
    public class OrderRequest
    {
        public Customer Customer { get; set; }
        public ShoppingCart Cart { get; set; }
        public OrderType OrderType { get; set; }
        public ShippingZone Zone { get; set; }
        public Receiver Receiver { get; set; }
        public CreditCardInfo Card { get; set; }
    }

    public class OrderResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public bool EmailSent { get; set; }
        public bool EmailAttempted { get; set; }
    }

    public class PaymentResult
    {
        public bool Approved { get; set; }
        public string AuthCode { get; set; }
        public string Message { get; set; }
    }
}