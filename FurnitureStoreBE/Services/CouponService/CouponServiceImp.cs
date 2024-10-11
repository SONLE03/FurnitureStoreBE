using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.CouponRequest;
using FurnitureStoreBE.DTOs.Response.CouponResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.CouponService
{
    public class CouponServiceImp : ICouponService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public CouponServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<CouponResponse> GetCoupon(Guid couponId)
        {
            var coupon = await _dbContext.Coupons
                .Include(a => a.Asset)
                .Where(c => c.Id == couponId).SingleOrDefaultAsync();
            if (coupon == null)
            {
                throw new ObjectNotFoundException("Coupon not found");
            }
            return _mapper.Map<CouponResponse>(coupon); 
        }
        public async Task<List<CouponResponse>> GetCouponsForCustomer()
        {
            var coupons = await _dbContext.Coupons
                 .Include(a => a.Asset)
                 .Where(c => !c.IsDeleted && c.ECouponStatus == ECouponStatus.Active && c.Quantity > c.UsageCount) 
                 .OrderByDescending(c => c.CreatedDate)
                 .ToListAsync();
            return _mapper.Map<List<CouponResponse>>(coupons);
        }
        public async Task<PaginatedList<CouponResponse>> GetCoupons(PageInfo pageInfo)
        {
            var couponsQuery = _dbContext.Coupons
                .Include(a => a.Asset)
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CreatedDate)
                .ProjectTo<CouponResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Coupons.CountAsync();
            return await Task.FromResult(PaginatedList<CouponResponse>.ToPagedList(couponsQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        private async Task<string> GenerateCouponCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string couponCode;
            do
            {
                couponCode = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (await _dbContext.Coupons.AnyAsync(c => c.Code == couponCode));
            return couponCode;
            
        }
        public async Task<CouponResponse> CreateCoupon(CouponRequest couponRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var startDate = couponRequest.StartDate;
                var endDate = couponRequest.EndDate;
                var discountValue = couponRequest.DiscountValue;
                ECouponType couponType;
                if (startDate < DateTime.Now)
                {
                    throw new BusinessException("Start date cannot be in the past.");
                }
                if (startDate > endDate)
                {
                    throw new BusinessException("Start date cannot be greater than the end date.");
                }
                if (couponRequest.ECouponType == ECouponType.Percentage.ToString())
                {
                    if (discountValue >= 100)
                    {
                        throw new BusinessException("The percentage discount value cannot be greater than 100");
                    }
                    couponType = ECouponType.Percentage;
                }
                else
                {
                    couponType = ECouponType.Value;
                }
                Asset asset = null;
                if (couponRequest.Image != null)
                {
                    var couponImageUploadResult = await _fileUploadService.UploadFileAsync(couponRequest.Image, EUploadFileFolder.Coupon.ToString());
                    asset = new Asset
                    {
                        Name = couponImageUploadResult.OriginalFilename,
                        URL = couponImageUploadResult.Url.ToString(),
                        CloudinaryId = couponImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.Coupon.ToString(),
                    };
                }
                var coupon = new Coupon
                {
                    Code = await GenerateCouponCode(),
                    StartDate = startDate,
                    EndDate = endDate,
                    DiscountValue = discountValue,
                    ECouponType = couponType,
                    Description = couponRequest.Description,
                    MinOrderValue = couponRequest.MinOrderValue,
                    Quantity = couponRequest.Quantity,
                    Asset = asset
                };
                coupon.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Coupons.AddAsync(coupon);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<CouponResponse>(coupon);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
     
        public async Task<CouponResponse> UpdateCoupon(Guid couponId, CouponRequest couponRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var startDate = couponRequest.StartDate;
                var endDate = couponRequest.EndDate;
                var discountValue = couponRequest.DiscountValue;
                ECouponType couponType;
                var dateTimeNow = DateTime.Now;
                if (startDate < dateTimeNow)
                {
                    throw new BusinessException("Start date cannot be in the past.");
                }
                if (startDate > endDate)
                {
                    throw new BusinessException("Start date cannot be greater than the end date.");
                }
                if (couponRequest.ECouponType == ECouponType.Percentage.ToString())
                {
                    if (discountValue >= 100)
                    {
                        throw new BusinessException("The percentage discount value cannot be greater than 100");
                    }
                    couponType = ECouponType.Percentage;
                }
                else
                {
                    couponType = ECouponType.Value;
                }
                var coupon = await _dbContext.Coupons
                    .Include(a => a.Asset)
                    .Where(c => c.Id == couponId)
                    .SingleOrDefaultAsync();
                if(coupon == null)
                {
                    throw new ObjectNotFoundException("Coupon not found");
                }

                if(endDate < dateTimeNow)
                {
                    coupon.ECouponStatus = ECouponStatus.Expired;
                }
                coupon.StartDate = startDate;
                coupon.EndDate = endDate;
                coupon.DiscountValue = discountValue;
                coupon.ECouponType = couponType;
                coupon.Description = couponRequest.Description;
                coupon.MinOrderValue = couponRequest.MinOrderValue;
                coupon.Quantity = couponRequest.Quantity;
                coupon.setCommonUpdate(UserSession.GetUserId());
                _dbContext.Coupons.Update(coupon);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<CouponResponse>(coupon);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Coupon> UseCoupon(Guid couponId, string code = null)
        {
            var coupon = await _dbContext.Coupons.Where(c => c.Id == couponId || c.Code == code).SingleOrDefaultAsync();
            if (coupon == null)
            {
                throw new ObjectNotFoundException("Coupon not found");
            }
            if(coupon.Quantity == coupon.UsageCount)
            {
                throw new ObjectNotFoundException("Coupon has been used up");
            }
            coupon.UsageCount++;
            if(coupon.Quantity == coupon.UsageCount)
            {
                coupon.ECouponStatus = ECouponStatus.Disable;
            }
            _dbContext.Coupons.Update(coupon);
            await _dbContext.SaveChangesAsync();
            return coupon;
        }

        private async Task<CouponResponse> ModifyCouponStatus(Guid couponId, ECouponStatus eCouponStatus)
        {
            var coupon = await _dbContext.Coupons
                .Include(a => a.Asset)
                .Where(c => c.Id == couponId).SingleOrDefaultAsync();
            if (coupon == null)
            {
                throw new ObjectNotFoundException("Coupon not found");

            }
            coupon.ECouponStatus = eCouponStatus;
            _dbContext.Coupons.Update(coupon);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CouponResponse>(coupon);
        }
        public async Task<CouponResponse> ActiveCoupon(Guid couponId)
        {
            return await ModifyCouponStatus(couponId, ECouponStatus.Active);
        }

        public async Task<CouponResponse> DisableCoupon(Guid couponId)
        {
            return await ModifyCouponStatus(couponId, ECouponStatus.Disable);
        }

        public async Task DeleteCoupon(Guid couponId)
        {
            try
            {
                if (!await _dbContext.Coupons.AnyAsync(c => c.Id == couponId))
                {
                    throw new ObjectNotFoundException("Coupon not found");
                }
                var sqlDelete = "DELETE FROM \"Coupon\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sqlDelete, couponId);
                if (affectedRows == 0)
                {
                    var sqlUpdate = "UPDATE \"Coupon\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1"; // Sử dụng dấu ngoặc kép
                    await _dbContext.Database.ExecuteSqlRawAsync(sqlUpdate, true, couponId);
                }
            }
            catch
            {
                throw new BusinessException("Coupon removal failed");

            }
        }
    }
}
