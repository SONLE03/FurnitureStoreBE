using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.ReviewRequest;
using FurnitureStoreBE.DTOs.Response.ReplyResponses;
using FurnitureStoreBE.DTOs.Response.ReviewResponse;

namespace FurnitureStoreBE.Services.ReviewService
{
    public interface IReviewService
    {
        Task<ReviewResponse> GetReviewById(Guid reviewId);
        Task<PaginatedList<ReviewResponse>> GetReviews(PageInfo pageInfo, string keyword);
        Task<PaginatedList<ReviewResponse>> GetReviewsByCustomer(PageInfo pageInfo, string userId);
        Task<PaginatedList<ReviewResponse>> GetReviewsByProduct(PageInfo pageInfo, Guid productId);
        Task<PaginatedList<ReviewResponse>> GetReviewsByProductAndCustomer(PageInfo pageInfo, Guid productId, string userId);   
        Task<ReviewResponse> CreateReview(ReviewRequest reviewRequest);
        Task<ReviewResponse> UpdateReview(Guid reviewId, ReviewRequest reviewRequest);
        Task DeleteReviewForCustomer(Guid reviewId, string userId);
        Task DeleteReviewsByIds(List<Guid> reviewsIds);
        Task DeleteAllReviews();
        Task<ReplyResponse> ReplyReview(Guid reviewId, string userId, string content);
        Task DeleteReply(Guid replyId, string userId);
    }
}
