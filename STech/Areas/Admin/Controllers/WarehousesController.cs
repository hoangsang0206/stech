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
        [Route("[area]/[controller]/import/create")]
        public async Task<IActionResult> CreateImport()
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
            return View("Import",
                new Tuple<Employee?, IEnumerable<Warehouse>, 
                    IEnumerable<Supplier>, WarehouseImportVM>
                    (employee, warehouses, suppliers, new WarehouseImportVM()));
        }

        [AdminAuthorize(Code = Functions.ImportWarehouse)]
        [Route("[area]/[controller]/import/list")]
        public async Task<IActionResult> ImportList(string? id, string? wId, string? pId,
            string? sId, string? eId,
            string? date,
            string? sort, string? status, int page = 1)
        {
            WarehouseFilterVM filterVm = new WarehouseFilterVM()
            {
                Warehouses = await _warehouseService.GetWarehouses(),
                Suppliers = await _supplierService.GetSuppliers(),
                WarehouseId = wId,
                SupplierId = sId,
                EmployeeId = eId,
                ProductId = pId,
                ItemId = id,
                DateRange = date,
            };
            
            PagedList<WarehouseImport> importList = await _warehouseService.GetWarehouseImports(id, wId, pId, sId, eId, date, sort, status, page);
            
            ViewBag.ActiveSidebar = "warehouses";
            return View(new Tuple<WarehouseFilterVM, PagedList<WarehouseImport>>(filterVm, importList));
        }

        [AdminAuthorize(Code = Functions.ExportWarehouse)]
        [Route("[area]/[controller]/export/create")]
        public async Task<IActionResult> CreateExport()
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return Unauthorized();
            }
            
            Employee? employee = await _employeeService.GetEmployeeByUserId(userId);
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();

            ViewBag.ActiveSidebar = "warehouses";
            return View("Export",
                new Tuple<Employee?, IEnumerable<Warehouse>, WarehouseImportVM>
                    (employee, warehouses, new WarehouseImportVM()));
        }
    }
}
