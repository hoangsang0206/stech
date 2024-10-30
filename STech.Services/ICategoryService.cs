using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(bool isExcept);
        Task<IEnumerable<Category>> GetRandomWithProducts(int numCategories, int numProducts);
        Task<PagedList<Category>> GetAllWithProducts(string? sort_by, int page, int itemsPerPage);
        Task<Category?> GetOne(string id);

        Task<bool> Create(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(string id);
    }
}
