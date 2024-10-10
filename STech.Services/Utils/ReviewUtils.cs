using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services.Utils
{
    public static class ReviewUtils
    {
        public static IQueryable<Review> SelectReview(this IQueryable<Review> reviews)
        {
            return reviews.Select(r => new Review
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
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
                    IsRead = rp.IsRead,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                }).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IQueryable<Review> SelectReviewDetail(this IQueryable<Review> reviews)
        {
            return reviews.Select(r => new Review
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                ReviewerEmail = r.ReviewerEmail,
                ReviewerPhone = r.ReviewerPhone,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                User = r.User == null ? null : new User
                {
                    UserId = r.User.UserId,
                    FullName = r.User.FullName,
                    Avatar = r.User.Avatar,
                    RoleId = r.User.RoleId,
                    Email = r.User.Email,
                    Phone = r.User.Phone
                },
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReply
                {
                    Id = rp.Id,
                    ReviewId = rp.ReviewId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                }).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IQueryable<Review> SelectReview(this IQueryable<Review> reviews, int numOfReplies)
        {
            return reviews.Select(r => new Review
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
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
                    IsRead = rp.IsRead,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                }).Take(numOfReplies).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IQueryable<Review> SelectReview(this IQueryable<Review> reviews, int numOfReplies, string? userId)
        {
            return reviews.Select(r => new Review
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                IsLiked = r.ReviewLikes.Any(rl => rl.UserId == userId),
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
                    IsRead = rp.IsRead,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                }).Take(numOfReplies).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IQueryable<Review> SelectReviewWithProduct(this IQueryable<Review> reviews)
        {
            return reviews.Select(r => new Review
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
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
                    IsRead = rp.IsRead,
                    UserReply = new User
                    {
                        UserId = rp.UserReply.UserId,
                        FullName = rp.UserReply.FullName,
                        Avatar = rp.UserReply.Avatar,
                        RoleId = rp.UserReply.RoleId,
                    },
                }).OrderBy(rp => rp.ReplyDate).ToList(),
                Product = new Product
                {
                    ProductId = r.Product.ProductId,
                    ProductName = r.Product.ProductName,
                    ProductImages = r.Product.ProductImages.OrderBy(p => p.Id).Take(1).ToList(),
                }
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static ReviewOverview GetReviewOverview(this IEnumerable<Review> reviews)
        {
            int totalReviews = reviews.Count();
            double averageRating = totalReviews > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;
            int total5Star = reviews.Count(r => r.Rating == 5);
            int total4Star = reviews.Count(r => r.Rating == 4);
            int total3Star = reviews.Count(r => r.Rating == 3);
            int total2Star = reviews.Count(r => r.Rating == 2);
            int total1Star = reviews.Count(r => r.Rating == 1);

            return new ReviewOverview
            {
                AverageRating = averageRating,
                Total5StarReviews = total5Star,
                Total4StarReviews = total4Star,
                Total3StarReviews = total3Star,
                Total2StarReviews = total2Star,
                Total1StarReviews = total1Star,
            };
        }


        public static IEnumerable<Review> Paginate(this IEnumerable<Review> reviews, int page, int reviewsPerpage)
        {
            if(page <= 0)
            {
                page = 1;
            }

            return reviews.Skip((page - 1) * reviewsPerpage).Take(reviewsPerpage);
        }

        public static IEnumerable<ReviewReply> Paginate(this IEnumerable<ReviewReply> replies, int page, int repliesPerPage)
        {
            if(page <= 0)
            {
                page = 1;
            }

            return replies.Skip((page - 1) * repliesPerPage).Take(repliesPerPage);
        }

        public static IEnumerable<Review> Sort(this IEnumerable<Review> reviews, string? sort_by)
        {
            switch (sort_by)
            {
                case "newest":
                    return reviews.OrderByDescending(r => r.CreateAt);
                case "oldest":
                    return reviews.OrderBy(r => r.CreateAt);
                case "rating-ascending":
                    return reviews.OrderBy(r => r.Rating);
                case "rating-descending":
                    return reviews.OrderByDescending(r => r.Rating);
                case "most-liked":
                    return reviews.OrderByDescending(r => r.TotalLike);
                default:
                    return reviews;
            }
        }

        public static IEnumerable<Review> Filter(this IEnumerable<Review> reviews, string? filter_by)
        {
            switch (filter_by)
            {
                case "purchased":
                    return reviews.Where(r => r.IsPurchased == true);
                case "not-purchased":
                    return reviews.Where(r => r.IsPurchased != true);
                case "with-images":
                    return reviews.Where(r => r.ReviewImages.Count > 0);
                case "5-stars":
                    return reviews.Where(r => r.Rating == 5);
                case "4-stars":
                    return reviews.Where(r => r.Rating == 4);
                case "3-stars":
                    return reviews.Where(r => r.Rating == 3);
                case "2-stars":
                    return reviews.Where(r => r.Rating == 2);
                case "1-star":
                    return reviews.Where(r => r.Rating == 1);
                default:
                    return reviews;
            }
        }
    }
}
