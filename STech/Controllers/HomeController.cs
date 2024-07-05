using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
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

        public IActionResult Index()
        {
            return View();
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
