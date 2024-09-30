using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.ReviewRequest;
using FurnitureStoreBE.DTOs.Response.ReplyResponses;
using FurnitureStoreBE.DTOs.Response.ReviewResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FurnitureStoreBE.Services.ReviewService
{
    public class ReviewServiceImp : IReviewService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public ReviewServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        private async Task<List<Asset>> UploadReviewImages(List<IFormFile> images)
        {
            var reviewImagesUploadResult = await _fileUploadService.UploadFilesAsync(images, EUploadFileFolder.Review.ToString());
            return reviewImagesUploadResult.Select(img => new Asset
            {
                Name = img.OriginalFilename,
                URL = img.Url.ToString(),
                CloudinaryId = img.PublicId,
                FolderName = EUploadFileFolder.Review.ToString()
            }).ToList();
        }

        // Phương thức tính toán rating mới
        private (int newRatingCount, float newRatingValue) CalculateNewRating(int currentCount, float currentValue, int newRate)
        {
            var newRatingCount = currentCount + 1;
            float newRatingValue = (currentCount * currentValue + newRate) / newRatingCount;
            newRatingValue = (float)Math.Round(newRatingValue, 1);
            return (newRatingCount, newRatingValue);
        }

        // Phương thức để lấy ReviewResponse
        private async Task<ReviewResponse> GetReviewResponseAsync(IQueryable<Review> reviewQuery)
        {
            var createdReview = await reviewQuery
                .Select(r => new Review
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Content = r.Content,
                    UpdatedDate = r.UpdatedDate,
                    User = new User
                    {
                        FullName = r.User.FullName,
                        Role = r.User.Role
                    },
                    Asset = r.Asset.Select(a => new Asset
                    {
                        URL = a.URL
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return _mapper.Map<ReviewResponse>(createdReview);
        }
        public async Task<ReviewResponse> GetReviewById(Guid reviewId)
        {
            if(!await _dbContext.Reviews.AnyAsync(r => r.Id == reviewId))
            {
                throw new ObjectNotFoundException("Review not found");
            }
            var reviewQuery = _dbContext.Reviews.Where(r => r.Id == reviewId);
            return await GetReviewResponseAsync(reviewQuery);
        }
        private async Task<PaginatedList<ReviewResponse>> GetReviews(PageInfo pageInfo, Expression<Func<Review, bool>> predicate = null)
        {
            predicate ??= r => true; // Nếu predicate là null, sử dụng một điều kiện luôn đúng
            var reviewQuery = _dbContext.Reviews
                .Include(r => r.Reply)
                .Where(predicate)
                .OrderByDescending(c => c.CreatedDate)
                .ProjectTo<ReviewResponse>(_mapper.ConfigurationProvider);

            var count = await _dbContext.Reviews.CountAsync(predicate);
            return await Task.FromResult(PaginatedList<ReviewResponse>.ToPagedList(reviewQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
        public async Task<PaginatedList<ReviewResponse>> GetReviews(PageInfo pageInfo, string keyword)
        {
            return await GetReviews(pageInfo); 
        }

        public async Task<PaginatedList<ReviewResponse>> GetReviewsByCustomer(PageInfo pageInfo, string userId)
        {
            return await GetReviews(pageInfo, (r => r.UserId == userId && !r.IsDeleted));
        }

        public async Task<PaginatedList<ReviewResponse>> GetReviewsByProduct(PageInfo pageInfo, Guid productId)
        {
            return await GetReviews(pageInfo, r => r.ProductId == productId && !r.IsDeleted);
        }

        public async Task<PaginatedList<ReviewResponse>> GetReviewsByProductAndCustomer(PageInfo pageInfo, Guid productId, string userId)
        {
            return await GetReviews(pageInfo, r => r.ProductId == productId && r.UserId == userId && !r.IsDeleted);
        }
        public async Task<ReplyResponse> ReplyReview(Guid reviewId, string userId, string content)
        {

            if (!await _dbContext.Reviews.AnyAsync(r => r.Id == reviewId))
            {
                throw new ObjectNotFoundException("Review not found");
            }
            var reply = new Reply
            { 
                Content = content,
                UserId = userId,
                ReviewId = reviewId
            };
            reply.setCommonCreate(UserSession.GetUserId());
            await _dbContext.Replies.AddAsync(reply);
            await _dbContext.SaveChangesAsync();
            var replyResponse = await (from r in _dbContext.Replies
                                       join u in _dbContext.Users on r.UserId equals u.Id
                                       where r.Id == reply.Id
                                       select new ReplyResponse
                                       {
                                           Id = r.Id,
                                           Content = r.Content,
                                           UserId = r.UserId,
                                           FullName = u.FullName,
                                           UpdatedDate = r.UpdatedDate,
                                           Role = u.Role
                                       }).FirstOrDefaultAsync();

            return replyResponse;
        }
        public async Task<ReviewResponse> CreateReview(ReviewRequest reviewRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await (from p in _dbContext.Products
                                     select new { p.Id, p.RatingCount, p.RatingValue })
                                     .SingleOrDefaultAsync();

                if (product == null)
                {
                    throw new ObjectNotFoundException("Product not found");
                }
                var hasOrderItem = await (from ot in _dbContext.OrderItems
                                          join o in _dbContext.Orders on ot.OrderId equals o.Id
                                          where ot.ProductId == reviewRequest.ProductId
                                          && o.UserId == reviewRequest.UserId
                                          select o.Id).AnyAsync();
                if (!hasOrderItem)
                {
                    throw new BusinessException("The user has never purchased this product");
                }

                var review = new Review
                {
                    ProductId = reviewRequest.ProductId,
                    UserId = reviewRequest.UserId,
                    Content = reviewRequest.Content,
                    Rate = reviewRequest.Rate,
                };
                if (reviewRequest.ReviewImage != null)
                {
                    review.Asset = await UploadReviewImages(reviewRequest.ReviewImage);
                }
                review.setCommonCreate(UserSession.GetUserId());

                //var newRatingCount = product.RatingCount + 1;
                //var newRatingValue = (product.RatingCount * product.RatingValue + reviewRequest.Rate) / newRatingCount;
                var (newRatingCount, newRatingValue) = CalculateNewRating(product.RatingCount, product.RatingValue, reviewRequest.Rate);

                var productToUpdate = new Product
                {
                    Id = product.Id,
                    RatingCount = newRatingCount,
                    RatingValue = newRatingValue
                };

                _dbContext.Products.Attach(productToUpdate);

                _dbContext.Entry(productToUpdate).Property(p => p.RatingCount).IsModified = true;
                _dbContext.Entry(productToUpdate).Property(p => p.RatingValue).IsModified = true;

                await _dbContext.Reviews.AddAsync(review);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                var reviewQuery = _dbContext.Reviews.Where(r => r.Id == review.Id);
                return _mapper.Map<ReviewResponse>(await GetReviewResponseAsync(reviewQuery));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ReviewResponse> UpdateReview(Guid reviewId, ReviewRequest reviewRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra sự tồn tại của review và lấy sản phẩm trong cùng một truy vấn
                var review = await _dbContext.Reviews
                    .Where(r => r.Id == reviewId)
                    .Select(r => new
                    {
                        Review = r,
                        Product = _dbContext.Products
                            .Select(p => new { p.Id, p.RatingCount, p.RatingValue })
                            .FirstOrDefault()
                    })
                    .SingleOrDefaultAsync();

                if (review == null || review.Product == null)
                {
                    throw new ObjectNotFoundException("Review or Product not found");
                }

                // Cập nhật nội dung review
                review.Review.Content = reviewRequest.Content;
                review.Review.setCommonCreate(UserSession.GetUserId());

                // Cập nhật hình ảnh nếu có
                if (reviewRequest.ReviewImage != null)
                {
                    review.Review.Asset = await UploadReviewImages(reviewRequest.ReviewImage);
                }

                // Cập nhật rating
                var (newRatingCount, newRatingValue) = CalculateNewRating(review.Product.RatingCount, review.Product.RatingValue, reviewRequest.Rate);

                // Cập nhật sản phẩm
                var productToUpdate = new Product
                {
                    Id = review.Product.Id,
                    RatingCount = newRatingCount,
                    RatingValue = newRatingValue
                };

                _dbContext.Products.Attach(productToUpdate);
                _dbContext.Entry(productToUpdate).Property(p => p.RatingCount).IsModified = true;
                _dbContext.Entry(productToUpdate).Property(p => p.RatingValue).IsModified = true;

                // Gắn review vào ngữ cảnh
                _dbContext.Reviews.Update(review.Review); // Update toàn bộ review

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ReviewResponse>(review.Review);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Ghi log lỗi nếu cần
                throw new ApplicationException("An error occurred while updating the review", ex);
            }
        }
        public async Task DeleteAllReviews()
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reviews = await _dbContext.Reviews
                    .Include(r => r.Reply) 
                    .Include(r => r.Asset)
                    .ToListAsync();

                if (reviews.Any())
                {
                    var assetIds = reviews
                        .SelectMany(r => r.Asset)
                        .Select(a => a.Id)
                        .ToList();

                    var repliesToDelete = reviews
                        .SelectMany(r => r.Reply)
                        .ToList();

                    if (repliesToDelete.Any())
                    {
                        _dbContext.Replies.RemoveRange(repliesToDelete);
                    }
                    if (assetIds.Any())
                    {
                        await _fileUploadService.DestroyFilesByAssetIdsAsync(assetIds);
                    }
                    _dbContext.Reviews.RemoveRange(reviews);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the reviews");
            }
        }

        public async Task DeleteReviewForCustomer(Guid reviewId, string userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if(!await _dbContext.Reviews.AnyAsync(r => r.Id == reviewId && r.UserId == userId))
                {
                    throw new ObjectNotFoundException("This review is not yours.");
                }
                var reviews = await _dbContext.Reviews
                    .Include(r => r.Reply)
                    .Include(r => r.Asset)
                    .Where(r => r.Id == reviewId && r.UserId == userId)
                    .ToListAsync();

                if (reviews.Any())
                {
                    var assetIds = reviews
                        .SelectMany(r => r.Asset)
                        .Select(a => a.Id)
                        .ToList();

                    var repliesToDelete = reviews
                        .SelectMany(r => r.Reply)
                        .ToList();

                    if (repliesToDelete.Any())
                    {
                        _dbContext.Replies.RemoveRange(repliesToDelete);
                    }
                    if (assetIds.Any())
                    {
                        await _fileUploadService.DestroyFilesByAssetIdsAsync(assetIds);
                    }
                    _dbContext.Reviews.RemoveRange(reviews);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the reviews");
            }
        }
        public async Task DeleteReply(Guid replyId, string userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reply = await _dbContext.Replies
                    .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId);
                if (reply == null)
                {
                    throw new ObjectNotFoundException("This reply is not yours.");
                }
                _dbContext.Replies.Remove(reply);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the reply");
            }
        }
        public async Task DeleteReviewsByIds(List<Guid> reviewIds)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reviews = await _dbContext.Reviews
                    .Include(r => r.Reply)  
                    .Include(r => r.Asset)
                    .Where(r => reviewIds.Contains(r.Id))
                    .ToListAsync();
                if (!reviews.Any())
                {
                    throw new ObjectNotFoundException("No reviews found for the given IDs.");
                }
                var assetIds = reviews
                    .SelectMany(r => r.Asset)
                    .Select(a => a.Id)
                    .ToList();
                var repliesToDelete = reviews
                    .SelectMany(r => r.Reply)
                    .ToList();

                if (repliesToDelete.Any())
                {
                    _dbContext.Replies.RemoveRange(repliesToDelete);
                }
                if (assetIds.Any())
                {
                    await _fileUploadService.DestroyFilesByAssetIdsAsync(assetIds);
                }
                _dbContext.Reviews.RemoveRange(reviews);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the reviews");
            }
        }
    }
}
