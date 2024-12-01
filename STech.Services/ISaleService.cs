using STech.Data.Models;

namespace STech.Services
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetActiveSales();
    }
}
