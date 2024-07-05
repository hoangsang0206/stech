using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System.Collections;
using System.Diagnostics;

namespace STech.Controllers
{
    public class HomeController : Controller
    {
        private readonly StechDbContext _dbContext;
        public HomeController(StechDbContext db)
        {
            _dbContext = db;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _dbContext.Categories.Where(c => c.CategoryId != "khac").ToListAsync();
            IEnumerable<Brand> brands = await _dbContext.Brands.ToListAsync();

            IEnumerable<Category> randomCategories = await _dbContext.Categories
                .Where(c => c.Products.Count > 10 && c.CategoryId != "khac")
                .OrderBy(c => Guid.NewGuid())
                .Select(c => new Category()
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Products = c.Products.OrderBy(p => Guid.NewGuid()).Select(p => new Product()
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        OriginalPrice = p.OriginalPrice,
                        Price = p.Price,
                        ProductImages = p.ProductImages,
                        WarehouseProducts = p.WarehouseProducts,

                    }).Take(15).ToList(),
                })
                .Take(8)
                .ToListAsync();

            IEnumerable<Slider> sliders = await _dbContext.Sliders.ToArrayAsync();

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
