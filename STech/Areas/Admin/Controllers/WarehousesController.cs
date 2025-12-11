using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ISupplierService _supplierService;
        private readonly IEmployeeService _employeeService;

        public WarehousesController(IWarehouseService warehouseService,
            ISupplierService supplierService,
            IEmployeeService employeeService)
        {
            _warehouseService = warehouseService;
            _supplierService = supplierService;
            _employeeService = employeeService;
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
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return Unauthorized();
            }
            
            Employee? employee = await _employeeService.GetEmployeeByUserId(userId);
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();
            IEnumerable<Supplier> suppliers = await _supplierService.GetSuppliers();

            ViewBag.ActiveSidebar = "warehouses";
            return View(
                new Tuple<Employee?, IEnumerable<Warehouse>, 
                    IEnumerable<Supplier>, WarehouseImportVM>
                    (employee, warehouses, suppliers, new WarehouseImportVM()));
        }

        [AdminAuthorize(Code = Functions.ExportWarehouse)]
        public async Task<IActionResult> Export()
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return Unauthorized();
            }
            
            Employee? employee = await _employeeService.GetEmployeeByUserId(userId);
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();

            ViewBag.ActiveSidebar = "warehouses";
            return View(
                new Tuple<Employee?, IEnumerable<Warehouse>, WarehouseImportVM>
                    (employee, warehouses, new WarehouseImportVM()));
        }
    }
}
