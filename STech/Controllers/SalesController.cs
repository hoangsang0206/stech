using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        public IActionResult Index()
        {


            return NotFound();
        }

        [Route("[controller]/{id}")]
        public async Task<IActionResult> Detail(string id, string? sort)
        {
            Sale? sale = await _saleService.GetSale(id, sort);

            if (sale == null)
            {
                return NotFound();
            }

            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Khuyến mãi", "/sales"),
                new Breadcrumb(sale.SaleName, "")
            };

            return View(new Tuple<Sale, List<Breadcrumb>>(sale, breadcrumbs));
        }
    }
}
