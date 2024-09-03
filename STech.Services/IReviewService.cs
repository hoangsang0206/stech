using STech.Data.Models;

namespace STech.Services
{
    public interface IReviewService
    {
        Task<(IEnumerable<Review>, int, int)> GetReviews(string productId, string? sort_by, int page, int reviewsPerPage);
        Task<IEnumerable<Review>> GetReviews(string productId, string? sort_by);
        Task<Review?> GetReview(int reviewId);

        Task<bool> CreateReview(Review review);
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(int reviewId);

        Task<bool> CreateReviewReply(ReviewReply reviewReply);
    }
}
