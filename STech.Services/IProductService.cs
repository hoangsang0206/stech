using STech.Data.Models;

namespace STech.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> SearchByName(string q);
        Task<IEnumerable<Product>> GetByCategory(string categoryId);
        Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake);
        Task<Product?> GetProduct(string id);
        Task<Product?> GetProductWithBasicInfo(string id);
        Task<bool> CheckOutOfStock(string id);
        Task<int> GetTotalQty(string id);
    }
}
