using STech.Data.Models;

namespace STech.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(bool isExcept);
        Task<IEnumerable<Category>> GetRandomWithProducts(int numCategories, int numProducts);
        Task<(IEnumerable<Category>, int)> GetAllWithProducts(string? sort_by, int page = 1);
        Task<Category?> GetOne(string id);

        Task<bool> Create(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(string id);
    }
}
