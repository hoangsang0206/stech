using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly StechDbContext _context;

        public ReviewService(StechDbContext context)
        {
            _context = context;
        }
 

        public async Task<IEnumerable<Review>> GetReviews(string productId, string? sort_by)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .SelectReview()
                .ToListAsync();
                
        }

        public async Task<(IEnumerable<Review>, ReviewOverview, int, int, int)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, 
            string? sort_by, string? filter_by, string? current_user, int page = 1)
        {
            IEnumerable<Review> reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .SelectReview(numOfReplies)
                .ToListAsync();

            int totalReviews = reviews.Count();
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filter_by).Sort(sort_by);
            int filteredCount = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)filteredCount / reviewsPerPage);

            int remainingReviews = filteredCount - reviewsPerPage * (page > totalPages ? totalPages : page);
                
            return (
                reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by), 
                overview,
                totalPages,
                totalReviews,
                remainingReviews > 0 ? remainingReviews : 0
            );
        }

        public async Task<(IEnumerable<Review>, int)> GetReviewsWithProduct(int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            IEnumerable<Review> reviews = await _context.Reviews
                .Include(r => r.Product)
                .SelectReviewWithProduct()
                .ToListAsync();

            if(!string.IsNullOrEmpty(status))
            {
                if(status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true);
                } 
                else if(status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true);
                }
            }

            reviews = reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by);

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews,
                totalPages
            );
        }

        public async Task<(IEnumerable<Review>, int)> GetProductReviews(string productId, int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            IEnumerable<Review> reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .SelectReviewWithProduct()
                .ToListAsync();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true).ToList();
                }
                else if (status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true).ToList();
                }
            }

            reviews = reviews.Filter(filter_by).Sort(sort_by).ToList();

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews.Paginate(page, reviewsPerPage),
                totalPages
            );
        }

        public async Task<(IEnumerable<Review>, int)> SearchReviewsWithProduct(string query, int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            string[] keywords = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<Review> reviews = await _context.Reviews
                .Where(r => r.Product.ProductId == query || keywords.All(key => r.Product.ProductName.Contains(key)))
                .Include(r => r.Product)
                .SelectReviewWithProduct()
                .ToListAsync();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true);
                }
                else if (status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true);
                }
            }

            reviews = reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by);

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews,
                totalPages
            );
        }

        public async Task<(IEnumerable<Review>, ReviewOverview, int, int, int)> GetApprovedReviews(string productId, int reviewsPerPage, int numOfReplies, string? sort_by, string? filter_by, string? current_user, int page = 1)
        {
            IEnumerable<Review> reviews = await _context.Reviews
                 .Where(r => r.ProductId == productId && r.IsProceeded == true)
                 .SelectReview(numOfReplies, current_user)
                 .ToListAsync();

            int totalReviews = reviews.Count();
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filter_by).Sort(sort_by);
            int filteredCount = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)filteredCount / reviewsPerPage);

            int remainingReviews = filteredCount - reviewsPerPage * (page > totalPages ? totalPages : page);

            return (
                reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by),
                overview,
                totalPages,
                totalReviews,
                remainingReviews > 0 ? remainingReviews : 0
            );
        }

        

        public async Task<Review?> GetReview(int reviewId)
        {
            return await _context.Reviews
                .Where(r => r.Id == reviewId)
                .SelectReviewDetail()
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<ReviewReply>, int, int, int)> GetReviewReplies(int reviewId, int page, int repliesPerPage)
        {
            IEnumerable<ReviewReply> replies = await _context.ReviewReplies
                .Where(rp => rp.ReviewId == reviewId)
                .Select(rp => new ReviewReply
                {
                    Id = rp.Id,
                    ReviewId = rp.ReviewId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                })
                .OrderBy(rp => rp.ReplyDate)
                .ToListAsync();

            int totalReplies = replies.Count();
            int totalPages = (int)Math.Ceiling((double)totalReplies / repliesPerPage);
            int remainingReplies = totalReplies - (page > totalPages ? totalPages : page) * repliesPerPage;

            return (
                    replies.Paginate(page, repliesPerPage), 
                    totalPages, 
                    totalReplies, 
                    remainingReplies > 0 ? remainingReplies : 0
                );
        }

        public async Task<IEnumerable<ReviewReply>> GetReviewReplies(int reviewId)
        {
            return await _context.ReviewReplies
                .Where(rp => rp.ReviewId == reviewId)
                .Select(rp => new ReviewReply
                {
                    Id = rp.Id,
                    ReviewId = rp.ReviewId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                })
                .OrderByDescending(rp => rp.ReplyDate)
                .ToListAsync();
        }

        public async Task<bool> CreateReview(Review review)
        {
            await _context.Reviews.AddAsync(review);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            Review? review = await _context.Reviews
                .Include(r => r.ReviewLikes)
                .Include(r => r.ReviewReplies)
                .Include(r => r.ReviewImages)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
            {
                return false;
            }

            _context.ReviewLikes.RemoveRange(review.ReviewLikes);
            _context.ReviewReplies.RemoveRange(review.ReviewReplies);
            _context.ReviewImages.RemoveRange(review.ReviewImages);
            _context.Reviews.Remove(review);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateReviewReply(ReviewReply reviewReply)
        {
            await _context.ReviewReplies.AddAsync(reviewReply);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LikeReview(int reviewId, string userId)
        {
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null)
            {
                return false;
            }

            ReviewLike? reviewLike = await _context.ReviewLikes.FirstOrDefaultAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);
            if (reviewLike != null)
            {
                return false;
            } 

            ReviewLike newReviewLike = new ReviewLike
            {
                ReviewId = reviewId,
                UserId = userId,
                LikeDate = DateTime.Now,
            };

            review.TotalLike++;

            await _context.ReviewLikes.AddAsync(newReviewLike);

            bool result = await _context.SaveChangesAsync() > 0;

            return result;
        }

        public async Task<bool> UnLikeReview(int reviewId, string userId)
        {
            return false;
        }

        public async Task<bool> ApproveReview(int reviewId)
        {
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null)
            {
                return false;
            }

            review.IsProceeded = true;
            _context.Reviews.Update(review);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAllRepliesAsRead(int reviewId)
        {
            IEnumerable<ReviewReply> replies = await _context.ReviewReplies
                .Where(rp => rp.ReviewId == reviewId && rp.IsRead != true)
                .ToListAsync();

            foreach (ReviewReply reply in replies)
            {
                reply.IsRead = true;
            }

            _context.ReviewReplies.UpdateRange(replies);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
