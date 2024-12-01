using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class SaleService : ISaleService
    {
        private readonly StechDbContext _context;

        public SaleService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetActiveSales()
        {
            return await _context.Sales
                .Where(s => s.IsActive == true && s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now)
                .Select(s => new Sale 
                { 
                    SaleId = s.SaleId,
                    SaleName = s.SaleName,
                    BackgroundColor = s.BackgroundColor,
                    HeaderTextColor = s.HeaderTextColor,
                    IsActive = s.IsActive,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    SaleProducts = s.SaleProducts.Select(sp => new SaleProduct
                    {
                        ProductId = sp.ProductId,
                        SalePrice = sp.SalePrice,
                        SaleQuantity = sp.SaleQuantity,
                        Product = new Product
                        {
                            ProductId = sp.Product.ProductId,
                            ProductName = sp.Product.ProductName,
                            OriginalPrice = sp.Product.OriginalPrice,
                            Price = sp.Product.Price,
                            ProductImages = sp.Product.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                            WarehouseProducts = sp.Product.WarehouseProducts,
                            BrandId = sp.Product.BrandId,
                            CategoryId = sp.Product.CategoryId,
                        }
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
