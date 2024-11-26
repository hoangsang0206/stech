using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly StechDbContext _context;
        
        public ProductService(StechDbContext context) => _context = context;
        
        #region GET
        public async Task<PagedList<Product>> GetProducts(string? brands, string? categories, string? status, 
            string? priceRange, string? warehouseId, string? sort, int page, int itemsPerPage)
        {
            IQueryable<Product> products = _context.Products
                .Where(p => warehouseId == null || p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId))
                .Filter(brands, categories, status, priceRange)
                .Sort(sort)
                .SelectProduct(warehouseId);
            
            
            return await products.ToPagedListAsync(page, itemsPerPage);
        }



        public async Task<PagedList<Product>> SearchByName(string q, int page, int itemsPerPage, string? sort)
        {
            if(string.IsNullOrEmpty(q))
            {
                return new PagedList<Product>();
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Product> products = _context.Products
                .Where(p => keywords.All(key =>  p.ProductName.Contains(key)) && p.IsActive == true)
                .Sort(sort)
                .SelectProduct();
            
            return await products.ToPagedListAsync(page, itemsPerPage);
        }

        public async Task<PagedList<Product>> SearchProducts(string q, int page, int itemsPerPage, string? sort, string? warehouseId)
        {
            if (string.IsNullOrEmpty(q))
            {
                return new PagedList<Product>();
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Product> products = _context.Products
                .Where(p => (p.ProductId == q || keywords.All(key => p.ProductName.Contains(key)))
                            && (warehouseId == null || p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId)))
                .Sort(sort)
                .SelectProduct(warehouseId);

            return await products.ToPagedListAsync(page, itemsPerPage);
        }

        public async Task<PagedList<Product>> GetByCategory(string categoryId, string? brands,  
            string? priceRange, int page, int itemsPerPage, string? sort)
        {
            IQueryable<Product> products = _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive == true)
                .Sort(sort)
                .Filter(brands, null, null, priceRange)
                .SelectProduct();

            return await products.ToPagedListAsync(page, itemsPerPage);
        }

        public async Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive == true && p.WarehouseProducts.Sum(wp => wp.Quantity) > 0)
                .OrderBy(p => Guid.NewGuid())
                .SelectProduct()
                .Take(numToTake)
                .ToListAsync();
        }

        public async Task<Product?> GetProduct(string id)
        {
            return await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OriginalPrice = p.OriginalPrice,
                    Price = p.Price,
                    ProductImages = p.ProductImages,
                    WarehouseProducts = p.WarehouseProducts,
                    BrandId = p.BrandId,
                    Brand = p.Brand,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    ProductSpecifications = p.ProductSpecifications,
                    CategoryId = p.CategoryId,
                    Category = p.Category,
                    ManufacturedYear = p.ManufacturedYear,
                    Warranty = p.Warranty,
                    IsActive = p.IsActive,
                    IsDeleted = p.IsDeleted,
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
                .Select(p => new Product
                {
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
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    WarehouseProducts = p.WarehouseProducts
                })
                .FirstOrDefaultAsync()
                ?? new Product();

            return product.WarehouseProducts
                .Sum(p => p.Quantity);
        }

        #endregion GET


        #region POST

        public async Task<bool> CreateProduct(ProductVM product)
        {
            Product _product = new Product()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName.Trim(),
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Warranty = product.Warranty,
                ManufacturedYear = product.ManufacturedYear,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                IsActive = false,
                IsDeleted = false,
                DateAdded = DateTime.Now,        
            };

            if (product.Specifications != null && product.Specifications.Count > 0 && !product.Specifications.Any(s => s == null))
            {
                foreach (ProductVM.Specification spec in product.Specifications)
                {
                    _product.ProductSpecifications.Add(new ProductSpecification
                    {
                        SpecName = spec.Name,
                        SpecValue = spec.Value
                    });
                }
            }

            if (product.Images != null && product.Images.Count > 0 && !product.Images.Any(i => i == null))
            {
                foreach (ProductVM.Image image in product.Images)
                {
                    _product.ProductImages.Add(new ProductImage
                    {
                        ImageSrc = image.ImageSrc
                    });
                }
            }

            await _context.Products.AddAsync(_product);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion POST


        #region PUT

        public async Task<bool> UpdateProduct(ProductVM product)
        {
            Product? _product = await _context.Products.FindAsync(product.ProductId);
            if (_product == null)
            {
                return false;
            }

            _product.ProductName = product.ProductName.Trim();
            _product.Price = product.Price;
            _product.OriginalPrice = product.OriginalPrice;
            _product.Warranty = product.Warranty;
            _product.ManufacturedYear = product.ManufacturedYear;
            _product.BrandId = product.BrandId;
            _product.CategoryId = product.CategoryId;
            _product.ShortDescription = product.ShortDescription;
            _product.Description = product.Description;

            List<ProductSpecification> specifications = await _context.ProductSpecifications
                .Where(ps => ps.ProductId == product.ProductId)
                .ToListAsync();

            _context.ProductSpecifications.RemoveRange(specifications);
            specifications.Clear();
            if (product.Specifications != null && product.Specifications.Count > 0 && !product.Specifications.Any(s => s == null))
            {
                foreach (ProductVM.Specification spec in product.Specifications)
                {
                    specifications.Add(new ProductSpecification
                    {
                        ProductId = product.ProductId,
                        SpecName = spec.Name,
                        SpecValue = spec.Value
                    });
                }

                _context.ProductSpecifications.AddRange(specifications);
            }

            if(product.Images != null && product.Images.Count > 0 && !product.Images.Any(i => i == null))
            {
                foreach(ProductVM.Image image in product.Images)
                {
                    if(image.Id != null)
                    {
                        if (image.Status == "deleted")
                        {
                            ProductImage? pImage = await _context.ProductImages.FindAsync(image.Id);
                            if (pImage != null)
                            {
                                _context.ProductImages.Remove(pImage);
                            }
                        }
                    } 
                    else
                    {
                        await _context.ProductImages.AddAsync(new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageSrc = image.ImageSrc
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }

            _context.Products.Update(_product);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion PUT


        #region DELETE

        public async Task<bool> DeleteProduct(string id)
        {
            Product? product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return false;
            }

            product.IsActive = false;
            product.IsDeleted = true;
            product.DateDeleted = DateTime.Now;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProducts(string[] ids)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p => ids.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count() == 0)
            {
                return false;
            }

            foreach(Product product in products)
            {
                product.IsActive = false;
                product.IsDeleted = true;
                product.DateDeleted = DateTime.Now;
            }

            _context.Products.UpdateRange(products);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> PermanentlyDeleteProduct(string id)
        {
            Product? product = await _context.Products
                .Where(p => p.ProductId == id && p.IsDeleted == true)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> PermanentlyDeleteProducts(string[] ids)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p => ids.Contains(p.ProductId) && p.IsDeleted == true)
                .ToListAsync();

            if (products.Count() == 0)
            {
                return false;
            }

            _context.Products.RemoveRange(products);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion DELETE


        #region RESTORE

        public async Task<bool> RestoreProduct(string id)
        {
            Product? product = await _context.Products
                .Where(p => p.ProductId == id && p.IsDeleted == true)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            product.IsDeleted = false;
            product.DateDeleted = null;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RestoreProducts(string[] ids)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p => ids.Contains(p.ProductId) && p.IsDeleted == true)
                .ToListAsync();

            if (products.Count() == 0)
            {
                return false;
            }

            foreach(Product product in products)
            {
                product.IsDeleted = false;
                product.DateDeleted = null;
            }

            _context.Products.UpdateRange(products);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion RESTORE

        #region ACTIVATE

        public async Task<bool> ActivateProduct(string id)
        {
            Product? product = await _context.Products
               .Where(p => p.ProductId == id && p.IsActive == false && p.IsDeleted != true)
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            product.IsActive = true;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> ActivateProducts(string[] ids)
        {

            IEnumerable<Product> products = await _context.Products
                .Where(p => ids.Contains(p.ProductId) && p.IsActive == false && p.IsDeleted != true)
                .ToListAsync();

            if (products.Count() == 0)
            {
                return false;
            }

            foreach (Product product in products)
            {
                product.IsActive = true;
            }

            _context.Products.UpdateRange(products);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion ACTIVATE

        #region DEACTIVATE

        public async Task<bool> DeActivateProduct(string id)
        {
            Product? product = await _context.Products
               .Where(p => p.ProductId == id && p.IsActive == true && p.IsDeleted != true)
               .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            product.IsActive = false;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeActivateProducts(string[] ids)
        {

            IEnumerable<Product> products = await _context.Products
                .Where(p => ids.Contains(p.ProductId) && p.IsActive == true && p.IsDeleted != true)
                .ToListAsync();

            if (products.Count() == 0)
            {
                return false;
            }

            foreach (Product product in products)
            {
                product.IsActive = false;
            }

            _context.Products.UpdateRange(products);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion DEACTIVATE
    }
}
