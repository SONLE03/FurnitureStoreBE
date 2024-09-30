using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Request.ReviewRequest;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.ProductService.RoomSpaceService;
using FurnitureStoreBE.Services.ReviewService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ReviewController
{
    [ApiController]
    [Route(Routes.REVIEW)]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewById(Guid reviewId)
        {
            return new SuccessfulResponse<object>(await _reviewService.GetReviewById(reviewId), (int)HttpStatusCode.OK, "Get review product by id successfully").GetResponse();
        }
        [HttpGet()]
        public async Task<IActionResult> GetReviews([FromQuery] PageInfo pageInfo, [FromQuery] string keyword)
        {
            return new SuccessfulResponse<object>(await _reviewService.GetReviews(pageInfo, keyword), (int)HttpStatusCode.OK, "Get reviews successfully").GetResponse();
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProduct([FromQuery] PageInfo pageInfo, Guid productId)
        {
            return new SuccessfulResponse<object>(await _reviewService.GetReviewsByProduct(pageInfo, productId), (int)HttpStatusCode.OK, "Get reviews by product successfully").GetResponse();
        }
        [HttpGet("user/{productId}")]
        public async Task<IActionResult> GetReviewsByCustomer([FromQuery] PageInfo pageInfo, string userId)
        {
            return new SuccessfulResponse<object>(await _reviewService.GetReviewsByCustomer(pageInfo, userId), (int)HttpStatusCode.OK, "Get reviews by customer successfully").GetResponse();
        }
        [HttpGet("{userId}/{productId}")]
        public async Task<IActionResult> GetReviewsByProductAndCustomer([FromQuery] PageInfo pageInfo, string userId, Guid productId)
        {
            return new SuccessfulResponse<object>(await _reviewService.GetReviewsByProductAndCustomer(pageInfo, productId, userId), (int)HttpStatusCode.OK, "Get reviews by product and customer successfully").GetResponse();
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromForm] ReviewRequest reviewRequest)
        {
            return new SuccessfulResponse<object>(await _reviewService.CreateReview(reviewRequest), (int)HttpStatusCode.OK, "Your review product has been successfully added").GetResponse();
        }
        [HttpPost("reply/{userId}/{reviewId}")]
        public async Task<IActionResult> ReplyReview(Guid reviewId, string userId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Content cannot be null or empty.");
            }
            return new SuccessfulResponse<object>(await _reviewService.ReplyReview(reviewId, userId, content), (int)HttpStatusCode.OK, "Your review product has been successfully added").GetResponse();
        }
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromForm] ReviewRequest reviewRequest)
        {
            return new SuccessfulResponse<object>(await _reviewService.UpdateReview(reviewId, reviewRequest), (int)HttpStatusCode.OK, "Your review product has been successfully modified").GetResponse();
        }
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllReviews()
        {
            await _reviewService.DeleteAllReviews();
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "All review product has been successfully removed").GetResponse();
        }
        [HttpDelete("by-ids")]
        public async Task<IActionResult> DeleteReviewsByIds(List<Guid> reviewId)
        {
            await _reviewService.DeleteReviewsByIds(reviewId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Review product has been successfully removed").GetResponse();
        }
        [HttpDelete("{reviewId}/{customerId}")]
        public async Task<IActionResult> DeleteReviewForCustomer(Guid reviewId, string customerId)
        {
            await _reviewService.DeleteReviewForCustomer(reviewId, customerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Review product has been successfully removed").GetResponse();
        }
        [HttpDelete("reply/{userId}/{replyId}")]
        public async Task<IActionResult> DeleteReply(Guid replyId, string userId)
        {
            await _reviewService.DeleteReply(replyId, userId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Reply review has been successfully removed").GetResponse();
        }
    }
}

