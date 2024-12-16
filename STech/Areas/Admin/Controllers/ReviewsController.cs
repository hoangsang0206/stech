using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReviewsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly IProductService _productService;

        private readonly int _reviewsPerPage = 40;

        public ReviewsController(IUserService userService, IReviewService reviewService, IProductService productService)
        {
            _userService = userService;
            _reviewService = reviewService;
            _productService = productService;
        }

        [AdminAuthorize(Code = Functions.ViewReviews)]
        public async Task<IActionResult> Index(string? search, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            if(page <= 1)
            {
                page = 1;          
            }

            PagedList<Review> reviews = search != null 
                ? await _reviewService.SearchReviewsWithProduct(search, _reviewsPerPage, sort_by, status, filter_by, page)
                : await _reviewService.GetReviewsWithProduct(_reviewsPerPage, sort_by, status, filter_by, page);
            
            ViewBag.ActiveSidebar = "reviews";
            return View(reviews);
        }

        [Route("/admin/reviews/1/{rId}")]
        [AdminAuthorize(Code = Functions.ViewReviews)]
        public async Task<IActionResult> Detail(int rId)
        {
            Review? review = await _reviewService.GetReview(rId);
           
            if(review == null)
            {
                return LocalRedirect("/admin/reviews");
            }

            Product? product = await _productService.GetProduct(review.ProductId);

            ViewBag.ActiveSidebar = "reviews";
            return View(new Tuple<Review, Product?>(review, product));
        }

        [Route("/admin/reviews/product/{pId}")]
        [AdminAuthorize(Code = Functions.ViewReviews)]
        public async Task<IActionResult> ProductReviews(string pId, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            Product? product = await _productService.GetProductWithBasicInfo(pId);

            if (product == null)
            {
                return LocalRedirect($"/admin/reviews?search={pId}");
            }

            if (page < 1)
            {
                page = 1;
            }

            var (reviews, overview) = await _reviewService.GetProductReviews(pId, _reviewsPerPage, sort_by, status, filter_by, page);
            
            ViewBag.ActiveSidebar = "reviews";
            return View(new Tuple<Product, PagedList<Review>, ReviewOverview>(product, reviews, overview));
        }
    }
}
