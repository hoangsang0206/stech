using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Services.Constants;
using STech.Services.Services;
using STech.Utils;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        
        private readonly AddressService _addressService;
        private readonly IAzureMapsService _azureMapsService;

        public WarehousesController(IWarehouseService warehouseService,
            ISupplierService supplierService,
            IProductService productService,
            IEmployeeService employeeService,
            AddressService addressService,
            IAzureMapsService azureMapsService)
        {
            _warehouseService = warehouseService;
            _supplierService = supplierService;
            _productService = productService;
            _employeeService = employeeService;
            _addressService = addressService;
            _azureMapsService = azureMapsService;
        }

        [HttpGet("1/{id}")]
        [AdminAuthorize(Code = Functions.ViewWarehouses)]
        public async Task<IActionResult> GetWarehouse(string id)
        {
            Warehouse? warehouse = await _warehouseService.GetWarehouseById(id);

            if (warehouse == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy kho hàng"
                });
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = warehouse
            });
        }

        [HttpPost("create")]
        [AdminAuthorize(Code = Functions.CreateWarehouse)]
        public async Task<IActionResult> CreateWarehouse([FromBody] WarehouseVM warehouse)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            Warehouse? existedWarehouse = await _warehouseService.GetWarehouseById(warehouse.WarehouseId);
            if (existedWarehouse != null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Kho hàng đã tồn tại"
                });
            }

            AddressVM address = new AddressVM();
            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == warehouse.ProvinceCode);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == warehouse.DistrictCode);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == warehouse.WardCode);
            
            string addressStr = $"{warehouse.Address}, {address._Ward?.name_with_type}, " +
                                $"{address._District?.name_with_type}, {address._City?.name_with_type}";

            var (lat, lon) = await _azureMapsService.GetLocation(addressStr);

            bool result = await _warehouseService.CreateWarehouse(new Warehouse
            {
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.WarehouseName,
                Address = warehouse.Address,
                Ward = address._Ward?.name_with_type ?? "",
                WardCode = warehouse.WardCode,
                District = address._District?.name_with_type ?? "",
                DistrictCode = warehouse.DistrictCode,
                Province = address._City?.name_with_type ?? "",
                ProvinceCode = warehouse.ProvinceCode,
                Latitude = lat.HasValue ? Convert.ToDecimal(lat) : null,
                Longtitude = lon.HasValue ? Convert.ToDecimal(lon) : null
            });
            
            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Tạo kho hàng thành công" : "Tạo kho hàng thất bại"
            });
        }

        [HttpPut("update")]
        [AdminAuthorize(Code = Functions.EditWarehouse)]
        public async Task<IActionResult> UpdateWarehouse([FromBody] WarehouseVM warehouse)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            Warehouse? existedWarehouse = await _warehouseService.GetWarehouseById(warehouse.WarehouseId);
            if (existedWarehouse == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Kho hàng này không đã tồn tại"
                });
            }

            AddressVM address = new AddressVM();
            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == warehouse.ProvinceCode);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == warehouse.DistrictCode);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == warehouse.WardCode);

            string addressStr = $"{warehouse.Address}, {address._Ward?.name_with_type}, " +
                                $"{address._District?.name_with_type}, {address._City?.name_with_type}";

            var (lat, lon) = await _azureMapsService.GetLocation(addressStr);

            bool result = await _warehouseService.UpdateWarehouse(new Warehouse
            {
                WarehouseId = warehouse.WarehouseId,
                WarehouseName = warehouse.WarehouseName,
                Address = warehouse.Address,
                Ward = address._Ward?.name_with_type ?? "",
                WardCode = warehouse.WardCode,
                District = address._District?.name_with_type ?? "",
                DistrictCode = warehouse.DistrictCode,
                Province = address._City?.name_with_type ?? "",
                ProvinceCode = warehouse.ProvinceCode,
                Latitude = lat.HasValue ? Convert.ToDecimal(lat) : null,
                Longtitude = lon.HasValue ? Convert.ToDecimal(lon) : null
            });

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Tạo kho hàng thành công" : "Tạo kho hàng thất bại"
            });
        }
        
        [HttpDelete("{id}")]
        [AdminAuthorize(Code = Functions.DeleteWarehouse)]
        public async Task<IActionResult> DeleteWarehouse(string id)
        {
            Warehouse? wh = await _warehouseService.GetWarehouseByIdWithStockInfo(id);
            
            if (wh == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Kho hàng không tồn tại"
                });
            }

            if (wh.WarehouseProducts.Any())
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Kho hàng đang chứa sản phẩm, không thể xóa"
                });
            }

            if (wh.WarehouseImports.Any())
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Đã nhập sản phẩm vào kho này, không thể xóa"
                });
            }

            if (wh.WarehouseExports.Any())
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Đã xuất sản phẩm từ kho này, không thể xóa"
                });
            }
            
            bool result = await _warehouseService.DeleteWarehouse(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa kho hàng thành công" : "Xóa kho hàng thất bại"
            });
        }

        #region Import

        // Get import list
        [HttpGet("import-list")]
        [AdminAuthorize(Code = Functions.ImportWarehouse)]
        public async Task<IActionResult> GetImportList(string? id, string? wId, string? pId,
            string? sId, string? eId,
            string? date,
            string? sort, string? status, int page = 1)
        {
            PagedList<WarehouseImport> importList = await _warehouseService.GetWarehouseImports(id, wId, pId, sId, eId, date, sort, status, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = importList
            });
        }
        
        
        // Create import
        [HttpPost("import/create")]
        [AdminAuthorize(Code = Functions.ImportWarehouse)]
        public async Task<IActionResult> ImportWarehouse([FromBody] WarehouseImportVM model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    StatusCode = 400,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            Warehouse? warehouse = await _warehouseService.GetWarehouseById(model.WarehouseId);
            if (warehouse == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Kho hàng không tồn tại"
                });
            }
            
            Supplier? supplier = await _supplierService.GetSupplierById(model.SupplierId);
            if (supplier == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Nhà cung cấp không tồn tại"
                });
            }
            if (model.WarehouseImportDetails.Count == 0)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Vui lòng chọn ít nhất 1 sản phẩm"
                });
            }

            foreach (var product in model.WarehouseImportDetails)
            {
                Product? existedProduct = await _productService.GetProduct(product.ProductId);
                
                if (existedProduct == null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = $"Sản phẩm {product.ProductId} không tồn tại"
                    });
                }
            }
            
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return Unauthorized();
            }
            
            Employee? employee = await _employeeService.GetEmployeeByUserId(userId);
            if (employee == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Nhân viên không tồn tại"
                });
            }

            DateTime now = DateTime.Now;
            WarehouseImport import = new WarehouseImport
            {
                Wiid = now.ToString("yyyyMMdd") + RandomUtils.GenerateRandomString(8).ToUpper(),
                WarehouseId = model.WarehouseId,
                SupplierId = model.SupplierId,
                DateImport = now,
                EmployeeId = employee.EmployeeId,
                Note = model.Note,
                DateCreate = now,
                Status = StatusConstant.Waiting,
            };

            import.WarehouseImportDetails =  model.WarehouseImportDetails.Select(d => new WarehouseImportDetail
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
            }).ToList();
            
            bool result = await _warehouseService.CreateWarehouseImport(import);
           
            return Ok(new ApiResponse
            {
               Status = result,
               Message = result ? "Tạo phiếu nhập kho thành công" : "Tạo phiếu nhập kho thất bại"
            });
        }

        [HttpPatch("import/accept/{id}")]
        public async Task<IActionResult> AcceptImport(string id)
        {
            WarehouseImport? import = await _warehouseService.GetWarehouseImport(id);
            if (import == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy phiếu nhập này."
                });
            }

            return Ok(new ApiResponse
            {
                Status = await _warehouseService.ConfirmWarehouseImport(id),
                Message = "Đã xác nhận hoàn thành phiếu nhập này."
            });
        }

        #endregion
    }
}
