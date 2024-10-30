using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IBrandService
    {
        Task<IEnumerable<Brand>> GetAll(bool isExcept);
        Task<PagedList<Brand>> GetAll(string? sortBy, int page, int itemsPerPage);
        Task<Brand?> GetById(string id);

        Task<bool> Create(Brand brand);
        Task<bool> Update(Brand brand);
        Task<bool> Delete(string id);
    }
}
