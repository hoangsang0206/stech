using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class ProductUtils
    {
        public static IEnumerable<Product> Sort(this IEnumerable<Product> products, string? value)
        {
            if(value == null)
            {
                return products;
            }

            IEnumerable<Product> sortedProduct = new List<Product>();

            if (value == "price-ascending")
            {
                sortedProduct = products.OrderBy(t => t.Price).ToList();
            }
            else if (value == "price-descending")
            {
                sortedProduct = products.OrderByDescending(t => t.Price).ToList();
            }
            else if (value == "name-az")
            {
                sortedProduct = products.OrderBy(t => t.ProductName).ToList();
            }
            else if (value == "name-za")
            {
                sortedProduct = products.OrderByDescending(t => t.ProductName).ToList();
            }
            else
            {
                sortedProduct = products.OrderBy(sp => Guid.NewGuid()).ToList();
            }

            return sortedProduct;
        }

        public static IEnumerable<Product> Pagnigate(this IEnumerable<Product> products, int page, int numToTake)
        {
            if(page <= 0)
            {
                page = 1;
            }

            int noOfProductToSkip = (page - 1) * numToTake;

            products = products.Skip(noOfProductToSkip).Take(numToTake).ToList();

            return products;
        }

        public static IEnumerable<Product> Filter(this IEnumerable<Product> products, string? filter_type, string? filter_value)
        {
            switch(filter_type)
            {
                case "specs":
                    string[] specs = filter_value?.Split(",") ?? [] ;
                    
                    break;

                case "price":
                    string[]? priceRange = filter_value?.Split(",");
                    if(priceRange != null && priceRange.Length == 2)
                    {
                        decimal _minPrice = Convert.ToDecimal(priceRange[0]);
                        decimal _maxPrice = Convert.ToDecimal(priceRange[1]);

                        products = products.Where(p => p.Price >= _minPrice && p.Price <= _maxPrice).ToList();
                    }

                    break;

                case "brands":
                    string[] brands = filter_value?.Split(",") ?? [];
                    products = products.Where(p => brands.Contains(p.BrandId)).ToList();
                    break;

                case "categories":
                    string[]? categories= filter_value?.Split(",") ?? [];
                    products = products.Where(p => categories.Contains(p.CategoryId)).ToList();
                    break;
            }

            return products;
        }
    }
}
