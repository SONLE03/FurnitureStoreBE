using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("UserUsedCoupon")]
    public class UserUsedCoupon
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public long Quantity { get; set; }
    }
}
