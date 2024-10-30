using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;

namespace STech.Controllers
{
    [Route("/collections/{id}")]
    public class CollectionsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        
        private readonly int _itemsPerPage = 40;

        public CollectionsController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string id, string? brands, string? price_range, string? sort, int page = 1)
        {
            if(id == null)
            {
                return NotFound();
            }

            PagedList<Product> products = new PagedList<Product>();
            Category? category;
            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();
            string title = "STech";

            if (id == "all")
            {
                products = await _productService
                    .GetProducts(brands, null, null, price_range, null, sort, page, _itemsPerPage);
                breadcrumbs.Add(new Breadcrumb("Tất cả sản phẩm", ""));
                title = "Tất cả sản phẩm - STech";
            }
            else
            {
                category = await _categoryService.GetOne(id);
                if(category == null)
                {
                    return NotFound();
                }

                products = await _productService.GetByCategory(id, page, _itemsPerPage, sort);
                breadcrumbs.Add(new Breadcrumb("Danh sách sản phẩm", "/collections/all"));
                breadcrumbs.Add(new Breadcrumb(category.CategoryName, ""));
                title = "Danh sách " + category.CategoryName;
            }

            ViewBag.Sort = sort;

            ViewData["Title"] = title;
            return View(new Tuple<PagedList<Product>, List<Breadcrumb>>(products, breadcrumbs));
        }
    }
}
