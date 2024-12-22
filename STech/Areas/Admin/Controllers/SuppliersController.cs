using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [AdminAuthorize]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetSuppliers();

            ViewBag.ActiveSidebar = "warehouses";
            return View(suppliers);
        }
    }
}
