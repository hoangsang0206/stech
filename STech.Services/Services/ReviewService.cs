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
 

        public async Task<IEnumerable<Review>> GetReviews(string productId, string? sortBy)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .SelectReview()
                .ToListAsync();
                
        }

        public async Task<(PagedList<Review>, ReviewOverview)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, 
            string? sortBy, string? filterBy, string? currentUser, int page = 1)
        {
            IQueryable<Review> reviews = _context.Reviews
                .Where(r => r.ProductId == productId)
                .SelectReview(numOfReplies, currentUser);
            
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filterBy).Sort(sortBy);
           
            return (
                await reviews.ToPagedListAsync(page, reviewsPerPage), 
                overview
            );
        }

        public async Task<PagedList<Review>> GetReviewsWithProduct(int reviewsPerPage, string? sortBy, string? status, string? filterBy, int page = 1)
        {
            IQueryable<Review> reviews = _context.Reviews
                .Include(r => r.Product)
                .Filter(filterBy)
                .Sort(sortBy)
                .SelectReviewWithProduct();

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
            
            return await reviews.ToPagedListAsync(page, reviewsPerPage);
        }

        public async Task<(PagedList<Review>, ReviewOverview)> GetProductReviews(string productId, int reviewsPerPage, string? sortBy, 
            string? status, string? filterBy, int page = 1)
        {
            IQueryable<Review> reviews = _context.Reviews
                .Where(r => r.ProductId == productId)
                .Filter(filterBy)
                .Sort(sortBy)
                .SelectReviewWithProduct();

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
            
            return (
                await reviews.ToPagedListAsync(page, reviewsPerPage),
                reviews.GetReviewOverview()
            );
        }

        public async Task<PagedList<Review>> SearchReviewsWithProduct(string query, int reviewsPerPage, 
            string? sortBy, string? status, string? filterBy, int page = 1)
        {
            string[] keywords = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Review> reviews = _context.Reviews
                .Where(r => r.Product.ProductId == query || keywords.All(key => r.Product.ProductName.Contains(key)))
                .Include(r => r.Product)
                .Filter(filterBy)
                .Sort(sortBy)
                .SelectReviewWithProduct();

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
            
            return await reviews.ToPagedListAsync(page, reviewsPerPage);
        }

        public async Task<(PagedList<Review>, ReviewOverview)> GetApprovedReviews(string productId, int reviewsPerPage, 
            int numOfReplies, string? sortBy, string? filterBy, string? currentUser, int page = 1)
        {
            IQueryable<Review> reviews = _context.Reviews
                 .Where(r => r.ProductId == productId && r.IsProceeded == true)
                 .SelectReview(numOfReplies, currentUser);
            
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filterBy).Sort(sortBy);
            
            return (
                await reviews.ToPagedListAsync(page, reviewsPerPage),
                overview
            );
        }

        

        public async Task<Review?> GetReview(int reviewId)
        {
            return await _context.Reviews
                .Where(r => r.Id == reviewId)
                .SelectReviewDetail()
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<ReviewReply>> GetReviewReplies(int reviewId, int page, int repliesPerPage)
        {
            IQueryable<ReviewReply> replies = _context.ReviewReplies
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
                .OrderBy(rp => rp.ReplyDate);
            
            return await replies.ToPagedListAsync(page, repliesPerPage);
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
