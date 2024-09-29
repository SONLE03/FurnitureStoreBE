using FurnitureStoreBE.Constants;
using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.CouponRequest
{
    public class CouponRequest
    {
        public string? Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "The quantity must be greater than or equal to {1}.")]
        public long Quantity { get; set; }
        [Required(ErrorMessage = "Min order value is required.")]
        [MinValue(0.0)]
        public decimal MinOrderValue { get; set; }
        [Required(ErrorMessage = "Discount value is required.")]
        [MinValue(0.0)]
        public decimal DiscountValue { get; set; }
        [Required(ErrorMessage = "Start date is required.")]

        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]

        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Coupon type is required.")]
        public string ECouponType { get; set; }

        public IFormFile? Image { get; set; }
    }
}
