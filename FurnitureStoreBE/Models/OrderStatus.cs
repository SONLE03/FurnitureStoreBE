using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("OrderStatus")]
    public class OrderStatus : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }    
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public string? ShipperId { get; set; }
        public User User { get; set; }
        public EOrderStatus Status { get; set; } = EOrderStatus.Pending;
        public string? Note { get; set; }
        public ICollection<Asset>? Asset { get; set; }
    }
}
