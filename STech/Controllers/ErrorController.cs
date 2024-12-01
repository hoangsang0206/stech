using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{code:int}")]
        public IActionResult Index(int code)
        {
            if(code == 404)
            {
                return View("NotFound");
            }

            if(code == 401)
            {
                return View("Unauthorized");
            }

            return View();
        }
    }
}
