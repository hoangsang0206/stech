using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        
        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        
        [HttpGet("1/{id}")]
        [AdminAuthorize(Code = Functions.ViewSuppliers)]
        public async Task<IActionResult> GetSupplier(string id)
        {
            Supplier? supplier = await _supplierService.GetSupplierById(id);
            
            if (supplier == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Nhà cung cấp không tồn tại"
                });
            }
            
            return Ok(new ApiResponse
            {
                Status = true,
                Data = supplier
            });
        }
        
        [HttpPost("create")]
        [AdminAuthorize(Code = Functions.CreateSupplier)]
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierVM supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier? existingSupplier = await _supplierService.GetSupplierById(supplier.SupplierId);
                
                if (existingSupplier != null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Nhà cung cấp này đã tồn tại"
                    });
                }

                bool result = await _supplierService.CreateSupplier(new Supplier
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    Address = supplier.Address,
                    Phone = supplier.Phone,
                });
                
                return Ok(new ApiResponse
                {
                    Status = result,
                    Message = result ? "Thêm nhà cung cấp thành công" : "Thêm nhà cung cấp thất bại"
                });
            }
            
            return BadRequest();
        }
        
        [HttpPut("update")]
        [AdminAuthorize(Code = Functions.EditSupplier)]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierVM supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier? existingSupplier = await _supplierService.GetSupplierById(supplier.SupplierId);
                
                if (existingSupplier == null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Nhà cung cấp không tồn tại"
                    });
                }

                bool result = await _supplierService.UpdateSupplier(new Supplier
                {
                    SupplierId = supplier.SupplierId,
                    SupplierName = supplier.SupplierName,
                    Address = supplier.Address,
                    Phone = supplier.Phone,
                });
                
                return Ok(new ApiResponse
                {
                    Status = result,
                    Message = result ? "Cập nhật nhà cung cấp thành công" : "Cập nhật nhà cung cấp thất bại"
                });
            }
            
            return BadRequest();
        }
        
        [HttpDelete("{id}")]
        [AdminAuthorize(Code = Functions.DeleteSupplier)]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            Supplier? existingSupplier = await _supplierService.GetSupplierById(id);
            
            if (existingSupplier == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Nhà cung cấp này không tồn tại"
                });
            }

            if (existingSupplier.WarehouseImports.Any())
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Nhà cung cấp này đã cung cấp hàng hóa cho kho hàng, không thể xóa"
                });
            }

            bool result = await _supplierService.DeleteSupplier(id);
            
            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa nhà cung cấp thành công" : "Xóa nhà cung cấp thất bại"
            });
        }
    }
}
