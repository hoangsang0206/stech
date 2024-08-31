using Azure.Storage.Blobs.Models;
using Azure;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Utils;
using System.Text.RegularExpressions;
using STech.Areas.Admin.Utils;

namespace STech.Areas.Admin.ApiControllers
{
    [AdminAuthorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IAzureService _azureService;

        private readonly string BlobDiscriptionPath = "product-discription-images/";

        public ProductsController(IProductService productService, IAzureService azureService)
        {
            _productService = productService;
            _azureService = azureService;
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



        #region POST

        #endregion POST



        #region PUT

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState
                });
            }

            string dataPattern = @"data:image/(?<type>.+?);base64,(?<data>[A-Za-z0-9+/=]+)";

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(productVM.Description);

            List<string> srcList = new List<string>();

            HtmlNodeCollection imgNodes = htmlDocument.DocumentNode.SelectNodes("//img");
            if(imgNodes != null)
            {
                foreach (HtmlNode imgNode in imgNodes)
                {
                    string? src = imgNode.GetAttributeValue("src", null);
                    Match match = Regex.Match(src, dataPattern);

                    if (match.Success)
                    {
                        string base64data = match.Groups["data"].Value;
                        string extension = match.Groups["type"].Value;

                        if (!string.IsNullOrEmpty(base64data))
                        {
                            byte[] imageBytes = Convert.FromBase64String(base64data);
                            string path = $"{BlobDiscriptionPath}{productVM.ProductId}/{RandomUtils.GenerateRandomString(20)}.{extension}";

                            string? imageUrl = await _azureService.UploadImage(path, imageBytes);
                            if (imageUrl != null)
                            {
                                imgNode.SetAttributeValue("src", imageUrl);
                                imgNode.SetAttributeValue("style", "width: 100%");
                            }
                        }
                    }

                    srcList.Add(new Uri(imgNode.GetAttributeValue("src", null)).AbsolutePath.TrimStart('/'));
                }
            }

            AsyncPageable<BlobItem> blobs = _azureService.GetBlobs($"{BlobDiscriptionPath}{productVM.ProductId}/");

            await foreach (BlobItem blob in blobs)
            {
                if (!srcList.Contains($"{_azureService.GetContainerName()}/{blob.Name}"))
                {
                    await _azureService.DeleteImage(blob.Name);
                }
            }

            productVM.Description = htmlDocument.DocumentNode.OuterHtml;
            productVM.Description = productVM.Description?
                .Replace("ql-size-large", "heading-size-large")
                .Replace("ql-size-huge", "heading-size-huge")
                .Replace("ql-align-center", "text-center")
                .Replace("ql-align-right", "text-end")
                .Replace("ql-align-justify", "text-justify");

            productVM.ShortDescription = productVM.ShortDescription?
                .Replace("ql-size-large", "heading-size-large")
                .Replace("ql-size-huge", "heading-size-huge")
                .Replace("ql-align-center", "text-center")
                .Replace("ql-align-right", "text-end")
                .Replace("ql-align-justify", "text-justify");

            if(productVM.Images != null && productVM.Images.Count > 0)
            {
                foreach (ProductVM.Image image in productVM.Images)
                {
                    if(image.Id != null)
                    {
                        if(image.Status == "deleted")
                        {
                            await _azureService.DeleteImage(image.ImageSrc);
                        }
                    }
                    else
                    {
                        Match match = Regex.Match(image.ImageSrc, dataPattern);
                        if (match.Success) {
                            string base64data = match.Groups["data"].Value;
                            string extension = match.Groups["type"].Value;

                            if (!string.IsNullOrEmpty(base64data))
                            {
                                byte[] imageBytes = Convert.FromBase64String(base64data);
                                string path = $"products/{productVM.ProductId}/{FormatString.ToSlug(productVM.ProductName)}-{RandomUtils.GenerateRandomString(10)}.{extension}";

                                string? imageUrl = await _azureService.UploadImage(path, imageBytes);
                                if (imageUrl != null)
                                {
                                    image.ImageSrc = imageUrl;
                                }
                            }
                        }
                    }
                }
            }


            bool result = await _productService.UpdateProduct(productVM);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Cập nhật sản phẩm thành công" : "Cập nhật sản phẩm thất bại"
            });
        }

        #endregion PUT



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
