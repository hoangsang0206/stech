using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly int _itemsPerPage = 20;
        
        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task<IActionResult> Index(string? sort_by, int page = 1)
        {
            if(page <= 0) 
            {
                page = 1;
            }

            PagedList<Brand> brands = await _brandService.GetAll(sort_by, page, _itemsPerPage);

            ViewBag.ActiveSidebar = "categories-brands";
            
            return View(brands);
        }
    }
}
