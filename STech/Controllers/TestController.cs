using Microsoft.AspNetCore.Mvc;
using STech.Services;

namespace STech.Controllers
{
    public class TestController : Controller
    {
        private readonly IProductService _productService;

        public TestController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
           

            return Ok();
        }
    }
}
