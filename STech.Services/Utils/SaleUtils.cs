using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class SaleUtils
    {
        public static IQueryable<Sale> SelectSale(this IQueryable<Sale> sale)
        {
            return sale.Select(s => new Sale
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
                        Reviews = sp.Product.Reviews
                            .Where(r => r.IsProceeded == true)
                            .Select(r => new Review
                            {
                                Rating = r.Rating
                            }).ToList(),
                    }
                }).ToList()
            });
        }

        public static IQueryable<Sale> SelectSale(this IQueryable<Sale> sale, string? sortProduct)
        {
            return sale.Select(s => new Sale
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
                        Reviews = sp.Product.Reviews
                            .Where(r => r.IsProceeded == true)
                            .Select(r => new Review
                            {
                                Rating = r.Rating
                            }).ToList(),
                    }
                }).ToList().Sort(sortProduct)
            });
        }

        public static ICollection<SaleProduct> Sort(this ICollection<SaleProduct> saleProducts, string? value)
        {
            if (value == null)
            {
                return saleProducts;
            }

            switch (value)
            {
                case "price-ascending":
                    return saleProducts.OrderBy(p => p.SalePrice).ToList();
                case "price-descending":
                    return saleProducts.OrderByDescending(p => p.SalePrice).ToList();
                case "name-az":
                    return saleProducts.OrderBy(p => p.Product.ProductName).ToList();
                case "name-za":
                    return saleProducts.OrderByDescending(p => p.Product.ProductName).ToList();
                default:
                    return saleProducts;
            }
        }
    }
}
