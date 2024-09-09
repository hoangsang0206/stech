using STech.Data.Models;

namespace STech.Services
{
    public interface IBrandService
    {
        Task<IEnumerable<Brand>> GetAll(bool isExcept);
        Task<(IEnumerable<Brand>, int)> GetAll(string? sort_by, int page = 1);
        Task<Brand?> GetById(string id);
    }
}
