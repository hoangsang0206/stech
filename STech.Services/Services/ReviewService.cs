using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
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
                .Select(r => new Review
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreateAt = r.CreateAt,
                    ReviewerName = r.ReviewerName,
                    TotalLike = r.TotalLike,
                    TotalDislike = r.TotalDislike,
                    IsPurchased = r.IsPurchased,
                    IsProceeded = r.IsProceeded,
                    User = r.User == null ? null : new User
                    {
                        UserId = r.User.UserId,
                        FullName = r.User.FullName,
                        Avatar = r.User.Avatar,
                        RoleId = r.User.RoleId,
                    },
                    ReviewImages = r.ReviewImages,
                    ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReply
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
                    }).OrderBy(rp => rp.ReplyDate).ToList(),
                })
                .OrderByDescending(r => r.CreateAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Review>, ReviewOverview, int, int, int)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, 
            string? sort_by, string? filter_by, string? current_user, int page = 1)
        {
            IEnumerable<Review> reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => new Review
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreateAt = r.CreateAt,
                    ReviewerName = r.ReviewerName,
                    TotalLike = r.TotalLike,
                    TotalDislike = r.TotalDislike,
                    IsPurchased = r.IsPurchased,
                    IsProceeded = r.IsProceeded,
                    IsLiked = r.ReviewLikes.Any(rl => rl.UserId == current_user),
                    IsDisliked = r.ReviewDislikes.Any(rd => rd.UserId == current_user),
                    User = r.User == null ? null : new User
                    {
                        UserId = r.User.UserId,
                        FullName = r.User.FullName,
                        Avatar = r.User.Avatar,
                        RoleId = r.User.RoleId,
                    },
                    ReviewImages = r.ReviewImages,
                    ReviewReplies = r.ReviewReplies.Take(numOfReplies).Select(rp => new ReviewReply
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
                    }).OrderBy(rp => rp.ReplyDate).ToList(),
                })
                .OrderByDescending(r => r.CreateAt)
                .ToListAsync();

            int totalReviews = reviews.Count();
            double averageRating = totalReviews > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;
            int total5Star = reviews.Count(r => r.Rating == 5);
            int total4Star = reviews.Count(r => r.Rating == 4);
            int total3Star = reviews.Count(r => r.Rating == 3);
            int total2Star = reviews.Count(r => r.Rating == 2);
            int total1Star = reviews.Count(r => r.Rating == 1);

            reviews = reviews.Filter(filter_by).Sort(sort_by);
            int filteredCount = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)filteredCount / reviewsPerPage);

            int remainingReviews = filteredCount - reviewsPerPage * (page > totalPages ? totalPages : page);
                
            return (
                reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by), 
                new ReviewOverview
                {
                    AverageRating = averageRating,
                    Total5StarReviews = total5Star,
                    Total4StarReviews = total4Star,
                    Total3StarReviews = total3Star,
                    Total2StarReviews = total2Star,
                    Total1StarReviews = total1Star,
                },
                totalPages,
                totalReviews,
                remainingReviews > 0 ? remainingReviews : 0
            );
        }

        public async Task<Review?> GetReview(int reviewId)
        {
            return await _context.Reviews
                .Where(r => r.Id == reviewId)
                .Select(r => new Review
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreateAt = r.CreateAt,
                    ReviewerName = r.ReviewerName,
                    TotalLike = r.TotalLike,
                    TotalDislike = r.TotalDislike,
                    IsPurchased = r.IsPurchased,
                    IsProceeded = r.IsProceeded,
                    User = r.User == null ? null : new User
                    {
                        UserId = r.User.UserId,
                        FullName = r.User.FullName,
                        Avatar = r.User.Avatar,
                        RoleId = r.User.RoleId,
                    },
                    ReviewImages = r.ReviewImages,
                    ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReply
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
                    }).OrderBy(rp => rp.ReplyDate).ToList(),
                })
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

            return (replies.Paginate(page, repliesPerPage), totalPages, totalReplies, remainingReplies > 0 ? remainingReplies : 0);
        }


        public async Task<bool> CreateReview(Review review)
        {
            await _context.Reviews.AddAsync(review);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateReview(Review review)
        {
            Review? oldReview = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == review.Id);
            if (oldReview == null)
            {
                return false;
            }

            oldReview.TotalLike = review.TotalLike;
            oldReview.TotalDislike = review.TotalDislike;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null)
            {
                return false;
            }

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

            ReviewDislike? reviewDislike = await _context.ReviewDislikes.FirstOrDefaultAsync(rdl => rdl.ReviewId == reviewId && rdl.UserId == userId);
            if (reviewDislike != null)
            {
                _context.ReviewDislikes.Remove(reviewDislike);
                review.TotalDislike--;
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

        public async Task<bool> DislikeReview(int reviewId, string userId)
        {
            Review? review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null)
            {
                return false;
            }

            ReviewDislike? reviewDislike = await _context.ReviewDislikes.FirstOrDefaultAsync(rdl => rdl.ReviewId == reviewId && rdl.UserId == userId);
            if (reviewDislike != null)
            {
                return false;
            }

            ReviewLike? reviewLike = await _context.ReviewLikes.FirstOrDefaultAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);
            if (reviewLike != null)
            {
                _context.ReviewLikes.Remove(reviewLike);
                review.TotalLike--;
            }

            ReviewDislike newReviewDislike = new ReviewDislike
            {
                ReviewId = reviewId,
                UserId = userId,
                LikeDate = DateTime.Now,
            };

            review.TotalDislike++;

            await _context.ReviewDislikes.AddAsync(newReviewDislike);
            bool result = await _context.SaveChangesAsync() > 0;

            return result;
        }

        public async Task<bool> UnDislikeReview(int reviewId, string userId)
        {
            return false;
        }
    }
}
