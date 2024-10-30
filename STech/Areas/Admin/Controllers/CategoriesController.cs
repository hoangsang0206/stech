using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly int _itemsPerPage = 30;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? sort_by, int page = 1)
        {
            if(page <= 0)
            {
                page = 1;
            }

            PagedList<Category> categories = await _categoryService.GetAllWithProducts(sort_by, page, _itemsPerPage);

            ViewBag.ActiveSidebar = "categories-brands";
            ViewBag.SortBy = sort_by;
            
            return View(categories);
        }
    }
}
