using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly StechDbContext _context;
        public CategoryService(StechDbContext context) => _context = context;

        public async Task<IEnumerable<Category>> GetAll(bool isExcept)
        {
            if(isExcept)
            {
                return await _context.Categories.Where(c => c.CategoryId != "khac").ToListAsync();
            }

            return await _context.Categories.ToListAsync();
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
                    Products = c.Products.OrderBy(p => Guid.NewGuid()).Select(p => new Product()
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        OriginalPrice = p.OriginalPrice,
                        Price = p.Price,
                        ProductImages = p.ProductImages.Take(1).ToList(),
                        WarehouseProducts = p.WarehouseProducts,

                    }).Take(numProducts).ToList(),
                })
                .Take(numCategories)
                .ToListAsync();
        }

        public async Task<Category> GetOne(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id) ?? new Category();
        }
    }
}
