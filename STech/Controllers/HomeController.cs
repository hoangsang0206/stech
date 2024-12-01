using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ISliderService _sliderService;
        private readonly ISaleService _saleService;

        public HomeController(IProductService productService, ICategoryService categoryService, 
            IBrandService brandService, ISliderService sliderService,
            ISaleService saleService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _sliderService = sliderService;
            _saleService = saleService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll(true);
            IEnumerable<Brand> brands = await _brandService.GetAll(true);
            IEnumerable<Category> randomCategories = await _categoryService.GetRandomWithProducts(8, 15);
            IEnumerable<Slider> sliders = await _sliderService.GetAll();
            IEnumerable<Sale> sales = await _saleService.GetActiveSales();
            PagedList<Product> bestSellingProducts = await _productService.GetBestSellingProducts(1, 20);
            PagedList<Product> newProducts = await _productService.GetNewestProducts(1, 20);

            HomePageData data = new HomePageData
            {
                Categories = categories,
                Brands = brands,
                RandomCategories = randomCategories,
                Sliders = sliders,
                BestSellingProducts = bestSellingProducts.Items,
                NewProducts = newProducts.Items,
                Sales = sales
            };

            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View();
        }
    }
}
