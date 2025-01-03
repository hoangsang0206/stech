using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class ProductUtils
    {
        public static IQueryable<Product> SelectProduct(this IQueryable<Product> products)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                WarehouseProducts = p.WarehouseProducts,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
                SaleProducts = p.SaleProducts
                    .Where(sp => sp.Sale.IsActive == true
                            && sp.Sale.StartDate <= DateTime.Now
                            && sp.Sale.EndDate > DateTime.Now)
                    .Take(1).ToList(),
                Reviews = p.Reviews
                    .Where(r => r.IsProceeded == true)
                    .Select(r => new Review
                    {
                        Rating = r.Rating
                    }).ToList(),
            });
        }

        public static IQueryable<Product> SelectProductWithSold(this IQueryable<Product> products)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                WarehouseProducts = p.WarehouseProducts,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
                InvoiceDetails = p.InvoiceDetails,
                SaleProducts = p.SaleProducts
                    .Where(sp => sp.Sale.IsActive == true
                            && sp.Sale.StartDate <= DateTime.Now
                            && sp.Sale.EndDate > DateTime.Now)
                    .Take(1).ToList(),
                Reviews = p.Reviews
                    .Where(r => r.IsProceeded == true)
                    .Select(r => new Review
                    {
                        Rating = r.Rating
                    }).ToList(),
            });
        }

        public static IQueryable<Product> SelectProductWithSpecs(this IQueryable<Product> products)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                ProductSpecifications = p.ProductSpecifications,
                WarehouseProducts = p.WarehouseProducts,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
                SaleProducts = p.SaleProducts
                    .Where(sp => sp.Sale.IsActive == true
                            && sp.Sale.StartDate <= DateTime.Now
                            && sp.Sale.EndDate > DateTime.Now)
                    .Take(1).ToList(),
                Reviews = p.Reviews
                    .Where(r => r.IsProceeded == true)
                    .Select(r => new Review
                    {
                        Rating = r.Rating
                    }).ToList(),
            });
        }

        public static IQueryable<Product> SelectProduct(this IQueryable<Product> products, string? warehouseId)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                WarehouseProducts = warehouseId == null ? p.WarehouseProducts : p.WarehouseProducts
                    .Where(wp => wp.WarehouseId == warehouseId).ToList(),
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
                SaleProducts = p.SaleProducts
                    .Where(sp => sp.Sale.IsActive == true
                            && sp.Sale.StartDate <= DateTime.Now
                            && sp.Sale.EndDate > DateTime.Now)
                    .Take(1).ToList(),
                Reviews = p.Reviews
                    .Where(r => r.IsProceeded == true)
                    .Select(r => new Review
                    {
                        Rating = r.Rating
                    }).ToList(),
            });
        }

        public static IQueryable<Product> Sort(this IQueryable<Product> products, string? value)
        {
            if(value == null)
            {
                return products;
            }
            
            switch (value)
            {
                case "price-ascending":
                    return products.OrderBy(p => p.Price);
                case "price-descending":
                    return products.OrderByDescending(p => p.Price);
                case "name-az":
                    return products.OrderBy(p => p.ProductName);
                case "name-za":
                    return products.OrderByDescending(p => p.ProductName);
                default:
                    return products;
            }
        }

        public static IQueryable<Product> Filter(this IQueryable<Product> products, string? brands, string? categories, 
            string? status, string? price_range)
        {
            string[] filter_brands = brands?.Split(',') ?? [];
            string[] filter_categories = categories?.Split(',') ?? [];
            string[] filter_price_range = price_range?.Split(',') ?? [];

            if (filter_brands.Length > 0)
            {
                products = products.Where(p => filter_brands.Contains(p.BrandId));
            }

            if (filter_categories.Length > 0)
            {
                products = products.Where(p => filter_categories.Contains(p.CategoryId));
            }

            if(filter_price_range.Length >= 2)
            {
                products = products.Where(p => p.Price >= Convert.ToDecimal(filter_price_range[0]) 
                                               && p.Price <= Convert.ToDecimal(filter_price_range[1]));
            }

            switch (status)
            {
                case "in-stock":
                    products = products.Where(p => p.IsActive == true 
                                                   && p.WarehouseProducts.Sum(wp => wp.Quantity) > 0);
                    break;

                case "out-of-stock-soon":
                    products = products
                        .Where(p => p.IsActive == true)
                        .Where(p => p.WarehouseProducts.Sum(wp => wp.Quantity) > 0 
                                    && p.WarehouseProducts.Sum(wp => wp.Quantity) <= 5);
                    break;

                case "out-of-stock":
                    products = products.Where(p => p.IsActive == true 
                                                   && p.WarehouseProducts.Sum(wp => wp.Quantity) <= 0);
                    break;

                case "inactive":
                    products = products.Where(p => p.IsActive == false && p.IsDeleted == false);
                    break;

                case "activated":
                    products = products.Where(p => p.IsActive == true);
                    break;

                case "deleted":
                    products = products.Where(p => p.IsDeleted == true);
                    break;
                default:
                    products = products.Where(p => p.IsActive == true);
                    break;
            }

            return products;
        }
    }
}
