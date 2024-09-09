using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly StechDbContext _context;

        private readonly int CategoriesPerPage = 30;

        public CategoryService(StechDbContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAll(bool isExcept)
        {
            if(isExcept)
            {
                return await _context.Categories.Where(c => c.CategoryId != "khac").OrderBy(c => c.CategoryName).ToListAsync();
            }

            return await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetRandomWithProducts(int numCategories, int numProducts)
        {
            if(numCategories == 0 || numProducts == 0) { return Enumerable.Empty<Category>(); }

            return await _context.Categories.Where(c => c.Products.Count >= numProducts && c.CategoryId != "khac")
                .OrderBy(c => Guid.NewGuid())
                .Select(c => new Category()
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Products = c.Products.Where(p => p.IsActive == true).OrderBy(p => Guid.NewGuid()).Select(p => new Product()
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        OriginalPrice = p.OriginalPrice,
                        Price = p.Price,
                        ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                        WarehouseProducts = p.WarehouseProducts,
                        Brand = p.Brand

                    }).Take(numProducts).ToList(),
                })
                .Take(numCategories)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Category>, int)> GetAllWithProducts(string? sort_by, int page = 1)
        {
            IEnumerable<Category> categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .Include(c => c.Products)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling(categories.Count() / (double)CategoriesPerPage);
            categories = categories.Sort(sort_by).Paginate(page, CategoriesPerPage);

            return (categories, totalPages);
        }

        public async Task<Category?> GetOne(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }
    }
}
