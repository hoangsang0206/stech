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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAzureService _azureService;

        public ReviewsController(IReviewService reviewService, IAzureService azureService)
        {
            _reviewService = reviewService;
            _azureService = azureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(string? search, string? sort_by, string? status, string? filter_by, int page = 1) {

            if (page <= 1)
            {
                page = 1;
            }

            var (reviews, totalPages) = search != null
                ? await _reviewService.SearchReviewsWithProduct(search, 40, sort_by, status, filter_by, page)
                : await _reviewService.GetReviewsWithProduct(40, sort_by, status, filter_by, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    reviews,
                    totalPages,
                    currentPage = page
                }
            });
        }

        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            Review? review = await _reviewService.GetReview(id);

            if (review == null)
            {
                return NotFound(new ApiResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đánh giá"
                });
            }

            review.IsProceeded = true;

            bool result = await _reviewService.UpdateReview(review);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Phê duyệt đánh giá thành công" : "Không thể phê duyệt đánh giá này"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            Review? review = await _reviewService.GetReview(id);
            if (review == null)
            {
                return NotFound(new ApiResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đánh giá"
                });
            }

            bool result = await _reviewService.DeleteReview(id);
            if (result)
            {
                foreach(ReviewImage img in review.ReviewImages)
                {
                    await _azureService.DeleteImage(img.ImageUrl);
                }
            }

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa đánh giá thành công" : "Không thể xóa đánh giá này"
            });
        }
    }
}
