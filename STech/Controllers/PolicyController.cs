using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class PolicyController : Controller
    {
        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult Delivery()
        {
            return View();
        }

        public IActionResult Warranty()
        {
            return View();
        }
    }
}
