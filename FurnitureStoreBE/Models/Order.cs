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
        [Column(TypeName = "timestamp without time zone")]

        public DateTime? CanceledAt { get; set; } = null;
        [Column(TypeName = "timestamp without time zone")]

        public DateTime? CompletedAt { get; set; } = null;
        [Column(TypeName = "timestamp without time zone")]

        public DateTime? DeliveredAt { get; set; } = null;
        public string? Note { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 30000;
        public EOrderStatus OrderStatus { get; set; } = EOrderStatus.Pending;
        public Guid? CouponId { get; set; }
        public Coupon? Coupon { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxFee { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public decimal AccountsReceivable { get; set; } = 0;
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<OrderStatus> OrderStatuses { get; set; }
    }
    public class CacheOrder
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
