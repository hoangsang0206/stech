using Microsoft.AspNetCore.Mvc;

namespace STech.Areas.Admin.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
