using STech.Data.Models;

namespace STech.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product>, int)> GetAll(int page, string? sort);
        Task<(IEnumerable<Product>, int)> SearchByName(string q, int page, string? sort);
        Task<(IEnumerable<Product>, int)> GetByCategory(string categoryId, int page, string? sort);
        Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake);
        Task<Product?> GetProduct(string id);
        Task<Product?> GetProductWithBasicInfo(string id);
        Task<bool> CheckOutOfStock(string id);
        Task<int> GetTotalQty(string id);
    }
}
