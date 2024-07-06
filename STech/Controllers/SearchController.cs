using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STech.Data.Models;
using STech.Services;
using STech.Utils;

namespace STech.Controllers
{
    public class SearchController : Controller
    {
        private readonly int productsPerPage = 40;
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string q, string sort, int page)
        {
            IEnumerable<Product> products = await _productService.SearchByName(q);

            if(!sort.IsNullOrEmpty())
            {
                products = ProductUtils.Sort(products, sort);
            }

            if(page <= 0)
            {
                page = 1;
            }

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(productsPerPage)));

            products = ProductUtils.Pagnigate(products, page, productsPerPage);

            ViewBag.Search = q;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;

            return View(products);
        }
    }
}
