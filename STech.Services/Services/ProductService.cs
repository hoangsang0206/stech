using Azure;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly StechDbContext _context;
        public ProductService(StechDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products
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
        }



        public async Task<IEnumerable<Product>> SearchByName(string q)
        {
            if(q == null)
            {
                return new List<Product>();
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return await _context.Products
                .Where(p => keywords.All(key =>  p.ProductName.Contains(key)))
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
        }

        public async Task<IEnumerable<Product>> GetByCategory(string categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
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
        }

        public async Task<Product?> GetProduct(string id)
        {
            return await _context.Products
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
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product?> GetProductWithBasicInfo(string id)
        {
            return await _context.Products
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
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<bool> CheckOutOfStock(string id)
        {
            Product product = await _context.Products
                .Select(p => new Product {
                    ProductId = p.ProductId,
                    WarehouseProducts = p.WarehouseProducts 
                })
                .FirstOrDefaultAsync(p => p.ProductId == id) 
                ?? new Product();

            int totalQty = product.WarehouseProducts.Sum(p => p.Quantity);

            return totalQty <= 0;
        }

        public async Task<int> GetTotalQty(string id)
        {
            Product product = await _context.Products
                .Select(p => new Product {
                    ProductId = p.ProductId,
                    WarehouseProducts = p.WarehouseProducts
                })
                .FirstOrDefaultAsync(p => p.ProductId == id)
                ?? new Product();

            return product.WarehouseProducts
                .Sum(p => p.Quantity);
        }
    }
}
