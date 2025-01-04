using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Services.Services;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly AddressService _addressService;
        private readonly IAzureMapsService _azureMapsService;

        public WarehousesController(IWarehouseService warehouseService,
            AddressService addressService,
            IAzureMapsService azureMapsService)
        {
            _warehouseService = warehouseService;
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
    }
}
