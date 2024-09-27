using FurnitureStoreBE.DTOs.Request.CouponRequest;
using FurnitureStoreBE.DTOs.Response.CouponResponse;

namespace FurnitureStoreBE.Services.CouponService
{
    public interface ICouponService
    {
        Task<CouponResponse> CreateCoupon(CouponRequest couponRequest);
        Task<CouponResponse> UpdateCoupon(Guid couponId, CouponRequest couponRequest);
        Task DeleteCoupon(Guid couponId);
        Task<List<CouponResponse>> GetCoupons();
        Task<CouponResponse> GetCoupon(Guid couponId);
    }
}
