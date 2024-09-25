using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;
using System.Security.Claims;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly int REVIEWS_PER_PAGE = 7;
        private readonly int REVIEW_REPLIES_PER_PAGE = 3;

        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IAzureService _azureService;

        public ReviewsController(IProductService productService, IReviewService reviewService, 
            IUserService userService, IOrderService orderService, IAzureService azureService)
        {
            _productService = productService;
            _reviewService = reviewService;
            _userService = userService;
            _orderService = orderService;
            _azureService = azureService;
        }

        #region GET

        [HttpGet("get-reviews")]
        public async Task<IActionResult> GetProductReviews(string pId, string? sort_by, string? filter_by, int page = 1)
        {
            if(page <= 0)
            {
                page = 1;
            }

            var (reviews, reviewOverview, totalPages, totalReviews, remainingReviews) = await _reviewService.GetReviews(pId, REVIEWS_PER_PAGE, REVIEW_REPLIES_PER_PAGE,
                sort_by, filter_by, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    reviews,
                    reviewOverview,
                    currentPage = page,
                    totalPages,
                    totalReviews,
                    remainingReviews
                }
            });
        }

        [HttpGet("get-review-replies")]
        public async Task<IActionResult> GetReviewReplies(int rId, int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }

            var (reviewReplies, totalPages, totalReplies, remainingReplies) = await _reviewService.GetReviewReplies(rId, page, REVIEW_REPLIES_PER_PAGE);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    reviewReplies,
                    currentPage = page,
                    totalPages,
                    totalReplies,
                    remainingReplies
                }
            });
        }

        #endregion GET


        #region POST

        [HttpPost("post-review")]
        public async Task<IActionResult> PostReview([FromForm] ReviewVM review, [FromForm] List<IFormFile>? files)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(review.Content.Trim().Length <= 0)
            {
                return BadRequest();
            }

            Product? product = await _productService.GetProductWithBasicInfo(review.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            Review newReview = new Review
            {
                ProductId = review.ProductId,
                Rating = review.Rating,
                Content = review.Content,
                CreateAt = DateTime.Now,
                TotalLike = 0,
                TotalDislike = 0
            };

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null) {
                    return Unauthorized();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                newReview.UserId = userId;
                newReview.IsPurchased = await _orderService.CheckIsPurchased(userId, review.ProductId);
            }
            else
            {
                if(review.ReviewerInfo == null || review.ReviewerInfo.ReviewerEmail == null || review.ReviewerInfo.ReviewerName == null)
                {
                    return BadRequest();
                }

                newReview.ReviewerEmail = review.ReviewerInfo.ReviewerEmail;
                newReview.ReviewerName = review.ReviewerInfo.ReviewerName;
                newReview.ReviewerPhone = review.ReviewerInfo.ReviewerPhone;

                newReview.IsPurchased = await _orderService.CheckIsPurchased(newReview.ReviewerEmail, newReview.ReviewerPhone, review.ProductId);
            }

            if(files != null && files.Count > 0)
            {
                if(files.Any(file => !ALLOWED_IMAGE_EXTENSIONS.Contains(Path.GetExtension(file.FileName).ToLower()))) {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Có hình ảnh không hợp lệ"
                    });
                }

                foreach (IFormFile file in files)
                {
                    string imagePath = $"reviews-images/{review.ProductId}/{RandomUtils.GenerateRandomString(10) + Path.GetExtension(file.FileName)}";
                    string? imageUrl = await _azureService.UploadImage(imagePath, ConvertFile.ConvertIFormFileToByteArray(file));

                    if (imageUrl != null)
                    {
                        newReview.ReviewImages.Add(new ReviewImage
                        {
                            ImageUrl = imageUrl
                        });
                    }
                }
            }

            bool result = await _reviewService.CreateReview(newReview);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Đánh giá của bạn đã được gửi thành công" : "Không thể gửi đánh giá"
            });
        }

        [Authorize]
        [HttpPost("post-review-reply")]
        public async Task<IActionResult> PostReviewReply([FromBody] ReviewReplyVM reviewReply)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(reviewReply.Content.Trim().Length <= 0)
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

            Review? review = await _reviewService.GetReview(reviewReply.ReviewId);
            if (review == null)
            {
                return NotFound();
            }

            ReviewReply newReviewReply = new ReviewReply
            {
                ReviewId = reviewReply.ReviewId,
                Content = reviewReply.Content,
                ReplyDate = DateTime.Now,
                UserReplyId = user.UserId
            };

            bool result = await _reviewService.CreateReviewReply(newReviewReply);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Phản hồi của bạn đã được gửi thành công" : "Không thể gửi phản hồi"
            });
        }

        #endregion POST
    }
}
