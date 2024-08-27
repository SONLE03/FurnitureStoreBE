using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        public Guid OrderId { get; set; }   
        public Order Order { get; set; }
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public string Dimension { get; set; }
        public string Color { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal SubTotal { get; set; }
    }
}
