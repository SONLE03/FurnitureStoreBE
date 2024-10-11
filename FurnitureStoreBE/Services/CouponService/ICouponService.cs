using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.CouponRequest;
using FurnitureStoreBE.DTOs.Response.CouponResponse;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.CouponService
{
    public interface ICouponService
    {
        Task<CouponResponse> CreateCoupon(CouponRequest couponRequest);
        Task<CouponResponse> UpdateCoupon(Guid couponId, CouponRequest couponRequest);
        Task DeleteCoupon(Guid couponId);
        Task<List<CouponResponse>> GetCouponsForCustomer();
        Task<PaginatedList<CouponResponse>> GetCoupons(PageInfo pageInfo);
        Task<CouponResponse> GetCoupon(Guid couponId);
        Task<CouponResponse> ActiveCoupon(Guid couponId);
        Task<CouponResponse> DisableCoupon(Guid couponId);
        Task<Coupon> UseCoupon(Guid couponId, string code = null);

    }
}
