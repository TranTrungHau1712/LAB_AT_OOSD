namespace eShopping.Models
{
    public class ProductGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    /// <summary>Sản phẩm - dữ liệu lấy từ Hệ thống quản lý sản phẩm (hệ thống ngoài).</summary>
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Specs { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public int GroupId { get; set; }
    }
}