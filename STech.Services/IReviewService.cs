using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IReviewService
    {
        Task<(PagedList<Review>, ReviewOverview)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, 
            string? sortBy, string? filterBy, string? currentUser, int page = 1);
        Task<(PagedList<Review>, ReviewOverview)> GetApprovedReviews(string productId, int reviewsPerPage, 
            int numOfReplies, string? sortBy, string? filterBy, string? currentUser, int page = 1);
        Task<IEnumerable<Review>> GetReviews(string productId, string? sortBy);
        Task<PagedList<Review>> GetReviewsWithProduct(int reviewsPerPage, string? sortBy, 
            string? status, string? filterBy, int page = 1);
        Task<(PagedList<Review>, ReviewOverview)> GetProductReviews(string productId, int reviewsPerPage, 
            string? sortBy, string? status, string? filterBy, int page = 1);
        Task<PagedList<Review>> SearchReviewsWithProduct(string query, int reviewsPerPage, 
            string? sortBy, string? status, string? filterBy, int page = 1);
        Task<Review?> GetReview(int reviewId);
        Task<PagedList<ReviewReply>> GetReviewReplies(int reviewId, int page, int repliesPerPage);
        Task<IEnumerable<ReviewReply>> GetReviewReplies(int reviewId);

        Task<bool> CreateReview(Review review);
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(int reviewId);

        Task<bool> CreateReviewReply(ReviewReply reviewReply);

        Task<bool> LikeReview(int reviewId, string userId);
        Task<bool> UnLikeReview(int reviewId, string userId);

        Task<bool> ApproveReview(int reviewId);

        Task<bool> MarkAllRepliesAsRead(int reviewId);
    }
}
