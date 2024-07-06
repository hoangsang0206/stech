using STech.Data.Models;

namespace STech.Utils
{
    public static class ProductUtils
    {
        public static IEnumerable<Product> Sort(IEnumerable<Product> products, string value)
        {
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

        public static IEnumerable<Product> Pagnigate(IEnumerable<Product> products, int page, int productsPerPage)
        {
            int noOfProductToSkip = (page - 1) * productsPerPage;

            products = products.Skip(noOfProductToSkip).Take(productsPerPage).ToList();

            return products;
        }
    }
}
