﻿using Microsoft.AspNetCore.Mvc;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ReviewsController : Controller
    {
        public IUserService _userService;
        public IReviewService _reviewService;

        public ReviewsController(IUserService userService, IReviewService reviewService)
        {
            _userService = userService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(string? search, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            if(page <= 1)
            {
                page = 1;          
            }

            var (reviews, totalPages) = search != null 
                ? await _reviewService.SearchReviewsWithProduct(search, 40, sort_by, status, filter_by, page)
                : await _reviewService.GetReviewsWithProduct( 40, sort_by, status, filter_by, page);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            ViewBag.ActiveSidebar = "reviews";
            return View(reviews);
        }
    }
}
