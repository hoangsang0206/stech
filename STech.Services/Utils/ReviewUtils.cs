using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class ReviewUtils
    {
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

        public static IEnumerable<Review> Sort(this IEnumerable<Review> reviews, string sort_by)
        {
            switch (sort_by)
            {
                case "newest":
                    return reviews.OrderByDescending(r => r.CreateAt);
                case "oldest":
                    return reviews.OrderBy(r => r.CreateAt);
                case "most-liked":
                    return reviews.OrderByDescending(r => r.TotalLike);
                case "most-disliked":
                    return reviews.OrderByDescending(r => r.TotalDislike);
                default:
                    return reviews;
            }
        }
    }
}
