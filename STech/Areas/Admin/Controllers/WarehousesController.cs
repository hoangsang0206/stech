using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class WarehousesController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehousesWithMostRecentImportAndExport();

            ViewBag.ActiveSidebar = "warehouses";
            return View(warehouses);
        }

        public async Task<IActionResult> Import()
        {
           

            ViewBag.ActiveSidebar = "warehouses";
            return View();
        }

        public async Task<IActionResult> Export()
        {
            

            ViewBag.ActiveSidebar = "warehouses";
            return View();
        }
    }
}
