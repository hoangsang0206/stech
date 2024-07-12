using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) => _productService = productService;

        [Route("product/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Product? product = await _productService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();

            Category? pCategory = product.Category;
            if(pCategory != null)
            {
                breadcrumbs.Add(new Breadcrumb(pCategory.CategoryName, $"/collections/{pCategory.CategoryId}"));
            }
            breadcrumbs.Add(new Breadcrumb(product.ProductName, ""));

            return View(new Tuple<Product, List<Breadcrumb>>(product, breadcrumbs));
        }
    }
}
