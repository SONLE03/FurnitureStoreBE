using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Request.CouponRequest;
using FurnitureStoreBE.Services.CouponService;
using FurnitureStoreBE.Services.ProductService.BrandService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.CouponController
{
    [ApiController]
    [Route(Routes.COUPON)]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetCoupons([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _couponService.GetCoupons(pageInfo), (int)HttpStatusCode.OK, "Get coupons successfully").GetResponse();
        }
        [HttpGet("{couponId}")]
        public async Task<IActionResult> GetCouponById(Guid couponId)
        {
            return new SuccessfulResponse<object>(await _couponService.GetCoupon(couponId), (int)HttpStatusCode.OK, "Get coupon successfully").GetResponse();
        }
        [HttpGet("/customer")]
        public async Task<IActionResult> GetCouponForCustomers()
        {
            return new SuccessfulResponse<object>(await _couponService.GetCouponsForCustomer(), (int)HttpStatusCode.OK, "Get coupons for customer successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateCoupon([FromForm] CouponRequest couponRequest)
        {
            return new SuccessfulResponse<object>(await _couponService.CreateCoupon(couponRequest), (int)HttpStatusCode.Created, "Coupon created successfully").GetResponse();
        }
        [HttpPut("{couponId}")]
        public async Task<IActionResult> UpdateCoupon(Guid couponId, [FromForm] CouponRequest couponRequest)
        {
            return new SuccessfulResponse<object>(await _couponService.UpdateCoupon(couponId, couponRequest), (int)HttpStatusCode.OK, "Coupon modified successfully").GetResponse();
        }
        [HttpPut("/active/{couponId}")]
        public async Task<IActionResult> ActiveCoupon(Guid couponId)
        {
            return new SuccessfulResponse<object>(await _couponService.ActiveCoupon(couponId), (int)HttpStatusCode.OK, "Active coupon successfully").GetResponse();
        }
        [HttpPut("/disable/{couponId}")]
        public async Task<IActionResult> DisableCoupon(Guid couponId)
        {
            return new SuccessfulResponse<object>(await _couponService.DisableCoupon(couponId), (int)HttpStatusCode.OK, "Disable coupon successfully").GetResponse();
        }
        [HttpDelete("{couponId}")]
        public async Task<IActionResult> DeleteBrand(Guid couponId)
        {
            await _couponService.DeleteCoupon(couponId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Coupon deleted successfully").GetResponse();

        }
    }
}
