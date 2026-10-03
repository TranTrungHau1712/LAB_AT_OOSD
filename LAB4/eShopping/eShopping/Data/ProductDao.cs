using System.Collections.Generic;
using System.Data;
using eShopping.Models;

namespace eShopping.Data
{
    /// <summary>Đọc dữ liệu từ schema ext (giả lập Hệ thống quản lý sản phẩm). Chỉ được gọi qua ProductSystemAdapter.</summary>
    public class ProductDao
    {
        public List<ProductGroup> GetGroups()
        {
            var list = new List<ProductGroup>();
            foreach (DataRow r in Db.Query("SELECT GroupId, GroupName FROM ext.ProductGroup ORDER BY GroupName").Rows)
                list.Add(new ProductGroup { Id = (int)r["GroupId"], Name = (string)r["GroupName"] });
            return list;
        }

        public List<Product> GetByGroup(int groupId)
        {
            var list = new List<Product>();
            var dt = Db.Query("SELECT * FROM ext.Product WHERE GroupId=@g ORDER BY ProductName", Db.P("@g", groupId));
            foreach (DataRow r in dt.Rows) list.Add(Map(r));
            return list;
        }

        public Product GetById(int id)
        {
            var dt = Db.Query("SELECT * FROM ext.Product WHERE ProductId=@id", Db.P("@id", id));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        private static Product Map(DataRow r) => new Product
        {
            Id = (int)r["ProductId"],
            Code = (string)r["ProductCode"],
            Name = (string)r["ProductName"],
            Manufacturer = (string)r["Manufacturer"],
            ImageUrl = r["ImageUrl"] as string,
            Description = r["Description"] as string,
            Specs = r["Specs"] as string,
            Price = (decimal)r["Price"],
            InStock = (bool)r["InStock"],
            GroupId = (int)r["GroupId"]
        };
    }
}