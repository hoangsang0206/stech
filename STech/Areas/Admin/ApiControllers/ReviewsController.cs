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

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
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
    }
}
