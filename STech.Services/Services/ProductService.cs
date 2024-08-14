using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly StechDbContext _context;

        private readonly int NumOfProductPerPage = 40;

        public ProductService(StechDbContext context) => _context = context;

        public async Task<(IEnumerable<Product>, int)> GetAll(int page, string? sort)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p => p.IsActive == true)
                .Select(p => new Product()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                })
                .ToListAsync();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products.Sort(sort).Pagnigate(page, NumOfProductPerPage), totalPage);
        }



        public async Task<(IEnumerable<Product>, int)> SearchByName(string q, int page, string? sort)
        {
            if(string.IsNullOrEmpty(q))
            {
                return (new List<Product>(), 1);
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<Product> products = await _context.Products
                .Where(p => keywords.All(key =>  p.ProductName.Contains(key)) && p.IsActive == true)
                .Select(p => new Product()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                })
                .ToListAsync();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products, totalPage);
        }

        public async Task<(IEnumerable<Product>, int)> SearchByName(string q, int page, string? sort, string warehouseId)
        {
            if (string.IsNullOrEmpty(q))
            {
                return (new List<Product>(), 1);
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<Product> products = await _context.Products
                .Where(p => keywords.All(key => p.ProductName.Contains(key)) && p.IsActive == true 
                    && p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId))
                .Select(p => new Product()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                })
                .ToListAsync();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products, totalPage);
        }

        public async Task<(IEnumerable<Product>, int)> GetByCategory(string categoryId, int page, string? sort)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive == true)
                .Select(p => new Product()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                })
                .ToListAsync();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products.Sort(sort).Pagnigate(page, NumOfProductPerPage), totalPage);
        }

        public async Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive == true)
                .OrderBy(p => Guid.NewGuid())
                .Select(p => new Product()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                })
                .Take(numToTake)
                .ToListAsync();
        }

        public async Task<Product?> GetProduct(string id)
        {
            return await _context.Products
                .Where(p => p.ProductId == id && p.IsActive == true)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages,
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    ProductSpecifications = p.ProductSpecifications,
                    Category = p.Category,
                    ManufacturedYear = p.ManufacturedYear,
                    Warranty = p.Warranty,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Product?> GetProductWithBasicInfo(string id)
        {
            return await _context.Products
                .Where(p => p.ProductId == id && p.IsActive == true)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                    Category = p.Category,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Product?> GetProductWithBasicInfo(string id, string warehouseId)
        {
            return await _context.Products
                .Where(p => p.ProductId == id && p.IsActive == true 
                        && p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId))
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    WarehouseProducts = p.WarehouseProducts,
                    Brand = p.Brand,
                    Category = p.Category,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckOutOfStock(string id)
        {
            Product product = await _context.Products
                .Where(p => p.ProductId == id && p.IsActive == true)
                .Select(p => new Product {
                    ProductId = p.ProductId,
                    WarehouseProducts = p.WarehouseProducts 
                })
                .FirstOrDefaultAsync() 
                ?? new Product();

            int totalQty = product.WarehouseProducts.Sum(p => p.Quantity);

            return totalQty <= 0;
        }

        public async Task<int> GetTotalQty(string id)
        {
            Product product = await _context.Products
                .Where(p => p.ProductId == id && p.IsActive == true)
                .Select(p => new Product {
                    ProductId = p.ProductId,
                    WarehouseProducts = p.WarehouseProducts
                })
                .FirstOrDefaultAsync()
                ?? new Product();

            return product.WarehouseProducts
                .Sum(p => p.Quantity);
        }
    }
}
