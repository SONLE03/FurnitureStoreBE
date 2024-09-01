using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        public Guid Id { get; set; }
        // is unique
        public required string Code { get; set; }
        public Guid? AssetId { get; set; }
        public Asset Asset { get; set; }
        public long Quantity {  get; set; }
        public long UsageCount { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public required decimal MinOrderValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ECouponType ECouponType { get; set; }
        public ECouponStatus ECouponStatus { get; set; }
        public ECouponApplyType ECouponApplyType { get; set; }

        public ICollection<Product> ProductApplied { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<UserUsedCoupon>? UserUsedCoupon { get; set; }

    }
}
