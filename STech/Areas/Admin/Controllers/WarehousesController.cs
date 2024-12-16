using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [AdminAuthorize(Code = Functions.ViewWarehouses)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehousesWithMostRecentImportAndExport();

            ViewBag.ActiveSidebar = "warehouses";
            return View(warehouses);
        }

        [AdminAuthorize(Code = Functions.ImportWarehouse)]
        public async Task<IActionResult> Import()
        {
           

            ViewBag.ActiveSidebar = "warehouses";
            return View();
        }

        [AdminAuthorize(Code = Functions.ExportWarehouse)]
        public async Task<IActionResult> Export()
        {
            

            ViewBag.ActiveSidebar = "warehouses";
            return View();
        }
    }
}
