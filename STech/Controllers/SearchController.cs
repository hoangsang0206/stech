using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;

namespace STech.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string q, string? sort, int page = 1)
        {
            IEnumerable<Product> products = await _productService.SearchByName(q);

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(ProductUtils.productsPerPage)));

            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tìm kiếm", ""),
                new Breadcrumb(q, "")
            };

            ViewBag.Search = q;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;

            return View(new Tuple<IEnumerable<Product>, List<Breadcrumb>>(products.Sort(sort).Pagnigate(page), breadcrumbs));
        }
    }
}
