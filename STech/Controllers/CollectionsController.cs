using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;
using Stripe.Climate;
using Product = STech.Data.Models.Product;

namespace STech.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        
        private readonly int _itemsPerPage = 40;

        public CollectionsController(ICategoryService categoryService, IBrandService brandService, IProductService productService)
        {
            _categoryService = categoryService;
            _brandService = brandService;
            _productService = productService;
        }
        
        [Route("/collections/all", Order = 1)]
        public async Task<IActionResult> GetAllProducts(string? brands, string? categories, string? price_range, string? sort, int page = 1)
        {
            PagedList<Product> products = await _productService
                .GetProducts(brands, categories, null, price_range, null, sort, page, _itemsPerPage);
            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tất cả sản phẩm", "")
            };
            
            IEnumerable<Category> categoryList = await _categoryService.GetAll(true);
            IEnumerable<Brand> brandList = await _brandService.GetAll(true);

            ViewData["Title"] = "Tất cả sản phẩm - STech";
            ViewBag.Sort = sort;
            ViewBag.SelectedBrands = brands?.Split(',') ?? [];
            ViewBag.SelectedCategories = categories?.Split(',') ?? [];

            return View("Index", new CollectionPageData
            {
                Breadcrumbs = breadcrumbs,
                Products = products,
                Brands = brandList,
                Categories = categoryList
            });
        }

        [Route("/collections/{id}", Order = 2)]
        public async Task<IActionResult> Index(string id, string? brands, string? price_range, string? sort, int page = 1)
        {
            Category? category = await _categoryService.GetOne(id);
            if(category == null)
            {
                return NotFound();
            }
            
            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Danh sách sản phẩm", "/collections/all"),
                new Breadcrumb(category.CategoryName, "")
            };
            IEnumerable<Brand> brandList = await _brandService.GetByCategory(category.CategoryId);

            PagedList<Product> products = await _productService.GetByCategory(id, brands, price_range, page, _itemsPerPage, sort);
  
            string title = "Danh sách " + category.CategoryName + " - STech";

            ViewBag.Sort = sort;
            ViewBag.SelectedBrands = brands?.Split(',') ?? [];

            ViewData["Title"] = title;
            return View(new CollectionPageData
            {
                Breadcrumbs = breadcrumbs,
                Products = products,
                Brands = brandList
            });
        }
    }
}
