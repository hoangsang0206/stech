using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services;

namespace STech.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ISliderService _sliderService;

        public HomeController(IProductService productService, ICategoryService categoryService, IBrandService brandService, ISliderService sliderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _sliderService = sliderService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll(true);
            IEnumerable<Brand> brands = await _brandService.GetAll(true);

            IEnumerable<Category> randomCategories = await _categoryService.GetRandomWithProducts(8, 15);

            IEnumerable<Slider> sliders = await _sliderService.GetAll();

            Tuple<IEnumerable<Category>, IEnumerable<Brand>, IEnumerable<Category>, IEnumerable<Slider>> data
                = new Tuple<IEnumerable<Category>, IEnumerable<Brand>, IEnumerable<Category>, IEnumerable<Slider>>(categories, brands, randomCategories, sliders);

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
