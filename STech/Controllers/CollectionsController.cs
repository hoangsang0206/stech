using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
