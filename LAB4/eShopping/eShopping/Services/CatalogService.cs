using System.Collections.Generic;
using eShopping.Adapters;
using eShopping.Models;

namespace eShopping.Services
{
    /// <summary>Xem nhóm sản phẩm, danh sách sản phẩm, chi tiết sản phẩm (qua adapter).</summary>
    public class CatalogService
    {
        private readonly IProductSystemAdapter _products;
        public CatalogService(IProductSystemAdapter products) { _products = products; }

        public List<ProductGroup> GetGroups() => _products.GetGroups();
        public List<Product> GetProducts(int groupId) => _products.GetProducts(groupId);
        public Product GetProduct(int id) => _products.GetProduct(id);
    }
}