using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
