using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Order")]
    public class Order : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public EPaymentMethod PaymentMethod { get; set; }
        public DateTime CanceledAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public string? Note { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal ShippingFee { get; set; }
        public EOrderStatus OrderStatus { get; set; }
        public Guid? CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal TaxFee { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Total { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
