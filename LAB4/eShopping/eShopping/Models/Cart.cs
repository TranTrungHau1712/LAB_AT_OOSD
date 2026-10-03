using System;
using System.Collections.Generic;
using System.Linq;

namespace eShopping.Models
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice => Product.Price;
        public decimal SubTotal => Product.Price * Quantity;
    }

    public class ShoppingCart
    {
        private readonly List<CartItem> _items = new List<CartItem>();

        public IReadOnlyList<CartItem> Items => _items;
        public int Count => _items.Sum(i => i.Quantity);
        public bool IsEmpty => _items.Count == 0;
        public decimal Total => _items.Sum(i => i.SubTotal);

        public void Add(Product p, int qty)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (qty <= 0) throw new ArgumentException("Số lượng phải > 0");
            var existing = _items.FirstOrDefault(i => i.Product.Id == p.Id);
            if (existing != null) existing.Quantity += qty;
            else _items.Add(new CartItem { Product = p, Quantity = qty });
        }

        public void Remove(int productId) => _items.RemoveAll(i => i.Product.Id == productId);

        public void UpdateQuantity(int productId, int qty)
        {
            if (qty <= 0) { Remove(productId); return; }
            var it = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (it != null) it.Quantity = qty;
        }

        public void Clear() => _items.Clear();
    }
}