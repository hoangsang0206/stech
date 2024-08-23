using Microsoft.AspNetCore.Http;
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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        #region GET

        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchProducts(string query, string? warehouse_id, string? sort, int page = 1)
        {
            var (products, totalPages) = await _productService.SearchProducts(query, page, sort, warehouse_id);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    products,
                    totalPages,
                    currentPage = page
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? brands, string? categories, string? status, string? price_range, string? warehouse_id, string? sort, int page = 1)
        {
            var (products, totalPages) = await _productService.GetProducts(brands, categories, status, price_range, warehouse_id, sort, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new {
                    products,
                    totalPages,
                    currentPage = page
                }
            });
        }

        [HttpGet("1/{id}")]
        public async Task<IActionResult> GetProduct(string id, string? warehouseId)
        {
            Product? product = !string.IsNullOrEmpty(warehouseId)
                ? await _productService.GetProductWithBasicInfo(id, warehouseId)
                : await _productService.GetProductWithBasicInfo(id);

            return Ok(new ApiResponse
            {
                Status = product != null,
                Data = product
            });
        }

        #endregion GET

        #region PATCH

        [HttpPatch("delete/1/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            bool result = await _productService.DeleteProduct(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa sản phẩm thành công" : "Xóa sản phẩm thất bại"
            });
        }

        [HttpPatch("delete/range")]
        public async Task<IActionResult> DeleteProducts([FromBody] string[] ids)
        {
            bool result = await _productService.DeleteProducts(ids);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa sản phẩm thành công" : "Xóa sản phẩm thất bại"
            });
        }

        [HttpPatch("restore/1/{id}")]
        public async Task<IActionResult> RestoreProduct(string id)
        {
            bool result = await _productService.RestoreProduct(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Khôi phục sản phẩm thành công" : "Khôi phục sản phẩm thất bại"
            });
        }

        [HttpPatch("restore/range")]
        public async Task<IActionResult> RestoreProducts([FromBody] string[] ids)
        {
            bool result = await _productService.RestoreProducts(ids);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Khôi phục sản phẩm thành công" : "Khôi phục sản phẩm thất bại"
            });
        }

        [HttpPatch("activate/1/{id}")]
        public async Task<IActionResult> ActivateProduct(string id)
        {
            bool result = await _productService.ActivateProduct(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Kích hoạt sản phẩm thành công" : "Kích hoạt sản phẩm thất bại"
            });
        }

        [HttpPatch("activate/range")]
        public async Task<IActionResult> ActivateProducts([FromBody] string[] ids)
        {
            bool result = await _productService.ActivateProducts(ids);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Kích hoạt sản phẩm thành công" : "Kích hoạt sản phẩm thất bại"
            });
        }

        [HttpPatch("deactivate/1/{id}")]
        public async Task<IActionResult> DeActivateProduct(string id)
        {
            bool result = await _productService.DeActivateProduct(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Ngưng kích hoạt sản phẩm thành công" : "Ngưng kích hoạt sản phẩm thất bại"
            });
        }

        [HttpPatch("deactivate/range")]
        public async Task<IActionResult> DeActivateProducts([FromBody] string[] ids)
        {
            bool result = await _productService.DeActivateProducts(ids);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Ngưng kích hoạt sản phẩm thành công" : "Ngưng kích hoạt sản phẩm thất bại"
            });
        }

        #endregion PATCH

        #region DELETE
        [HttpDelete("permanently-delete/1/{id}")]
        public async Task<IActionResult> PermanentlyDeleteProduct(string id)
        {
            bool result = await _productService.PermanentlyDeleteProduct(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Đã xóa vĩnh viễn sản phẩm này" : "Không thể xóa sản phẩm này"
            });
        }

        [HttpDelete("permanently-delete/range")]
        public async Task<IActionResult> PermanentlyDeleteProducts(string[] ids)
        {
            bool result = await _productService.PermanentlyDeleteProducts(ids);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Đã xóa vĩnh viễn các sản phẩm này" : "Không thể xóa các sản phẩm này"
            });
        }

        #endregion DELETE
    }
}
