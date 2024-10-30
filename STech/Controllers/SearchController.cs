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
        private readonly int _itemsPerPage = 40;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string q, string? sort, int page = 1)
        {
            PagedList<Product> products = await _productService.SearchByName(q, page, _itemsPerPage, sort);

            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tìm kiếm", ""),
                new Breadcrumb(q, "")
            };

            ViewBag.Search = q;
            ViewBag.Sort = sort;

            return View(new Tuple<PagedList<Product>, List<Breadcrumb>>(products, breadcrumbs));
        }
    }
}
