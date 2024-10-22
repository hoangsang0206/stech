using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
