using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IReviewService
    {
        Task<(IEnumerable<Review>, ReviewOverview, int, int, int)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, string? sort_by, string? filter_by, string? current_user, int page = 1);
        Task<IEnumerable<Review>> GetReviews(string productId, string? sort_by);
        Task<Review?> GetReview(int reviewId);
        Task<(IEnumerable<ReviewReply>, int, int, int)> GetReviewReplies(int reviewId, int page, int repliesPerPage);

        Task<bool> CreateReview(Review review);
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(int reviewId);

        Task<bool> CreateReviewReply(ReviewReply reviewReply);

        Task<bool> LikeReview(int reviewId, string userId);
        Task<bool> UnLikeReview(int reviewId, string userId);

        Task<bool> DislikeReview(int reviewId, string userId);
        Task<bool> UnDislikeReview(int reviewId, string userId);
    }
}
