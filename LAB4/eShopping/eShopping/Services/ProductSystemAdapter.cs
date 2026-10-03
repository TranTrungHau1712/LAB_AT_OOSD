using System.Collections.Generic;
using eShopping.Data;
using eShopping.Models;

namespace eShopping.Adapters
{
    /// <summary>Cổng kết nối tới Hệ thống quản lý sản phẩm (hệ thống ngoài).</summary>
    public interface IProductSystemAdapter
    {
        List<ProductGroup> GetGroups();
        List<Product> GetProducts(int groupId);
        Product GetProduct(int productId);
    }

    public class ProductSystemAdapter : IProductSystemAdapter
    {
        private readonly ProductDao _dao = new ProductDao();
        public List<ProductGroup> GetGroups() => _dao.GetGroups();
        public List<Product> GetProducts(int groupId) => _dao.GetByGroup(groupId);
        public Product GetProduct(int productId) => _dao.GetById(productId);
    }
}