using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Coupon")]
    public class Coupon : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        // is unique
        public string Code { get; set; }
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public string? Description { get; set; }
        public long Quantity {  get; set; }
        public long UsageCount { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderValue { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        public ECouponType ECouponType { get; set; }
        public ECouponStatus ECouponStatus { get; set; } = ECouponStatus.Active;
        public ICollection<Order>? Orders { get; set; }
        public ICollection<UserUsedCoupon>? UserUsedCoupon { get; set; }
    }
}
