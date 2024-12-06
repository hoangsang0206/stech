using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.ApiControllers
{
    [AdminAuthorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet("1/{id}")]
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

        [HttpPost]
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



            return Ok();
        }
    }
}
