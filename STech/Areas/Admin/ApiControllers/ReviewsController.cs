using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using System.Security.Claims;
using STech.Contants;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAzureStorageService _azureService;
        private readonly IUserService _userService;

        private readonly int _reviewsPerPage = 40;

        public ReviewsController(IReviewService reviewService, IAzureStorageService azureService, IUserService userService)
        {
            _reviewService = reviewService;
            _azureService = azureService;
            _userService = userService;
        }

        [HttpGet]
        [AdminAuthorize(Code = Functions.ViewReviews)]
        public async Task<IActionResult> GetReviews(string? search, string? productId, string? sort_by, string? status, string? filter_by, int page = 1) {

            if (page <= 1)
            {
                page = 1;
            }

            PagedList<Review> reviews = search != null
                ? await _reviewService.SearchReviewsWithProduct(search, _reviewsPerPage, sort_by, status, filter_by, page)
                : productId != null
                ? _reviewService.GetProductReviews(productId, _reviewsPerPage, sort_by, status, filter_by, page).Result.Item1
                : await _reviewService.GetReviewsWithProduct(_reviewsPerPage, sort_by, status, filter_by, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    reviews = reviews.Items,
                    totalPages = reviews.TotalPages,
                    currentPage = reviews.CurrentPage
                }
            });
        }

        [HttpGet("1/{id}")]
        [AdminAuthorize(Code = Functions.ViewReviews)]
        public async Task<IActionResult> GetReview(int id)
        {
            Review? review = await _reviewService.GetReview(id);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = review
            });
        }

        [HttpPatch("approve/{id}")]
        [AdminAuthorize(Code = Functions.ManageReview)]
        public async Task<IActionResult> ApproveReview(int id)
        {
            bool result = await _reviewService.ApproveReview(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Phê duyệt đánh giá thành công" : "Không thể phê duyệt đánh giá này"
            });
        }

        [HttpPatch("mark-all-read/{id}")]
        [AdminAuthorize(Code = Functions.ManageReview)]
        public async Task<IActionResult> MarkAllRepliesAsRead(int id)
        {
            return Ok(new ApiResponse
            {
                Status = await _reviewService.MarkAllRepliesAsRead(id)
            });
        }

        [HttpPost("post-reply")]
        [AdminAuthorize(Code = Functions.ManageReview)]
        public async Task<IActionResult> PostReply(ReviewReplyVM reply)
        {
            if(ModelState.IsValid)
            {
                if (reply.Content.Trim().Length <= 0)
                {
                    return BadRequest();
                }

                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return Unauthorized();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                ReviewReply newReviewReply = new ReviewReply
                {
                    ReviewId = reply.ReviewId,
                    Content = reply.Content,
                    ReplyDate = DateTime.Now,
                    IsRead = true,
                    UserReplyId = user.UserId
                };

                bool result = await _reviewService.CreateReviewReply(newReviewReply);

                return Ok(new ApiResponse
                {
                    Status = result,
                    Data = result ? new ReviewReply
                    {
                        Content = newReviewReply.Content,
                        UserReply = new User
                        {
                            FullName = user.FullName,
                            Avatar = user.Avatar
                        }
                    } : null
                });
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [AdminAuthorize(Code = Functions.ManageReview)]
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
