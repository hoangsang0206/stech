using STech.Data.Models;

namespace STech.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetActiveSales();
        Task<Sale?> GetSale(string id, string? sortProducts);
    }
}
