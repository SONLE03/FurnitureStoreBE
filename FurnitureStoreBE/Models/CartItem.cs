using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("CartItem")]
    public class CartItem
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }
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
