using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }   
        //public Order Order { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Dimension { get; set; }
        public Guid ColorId { get; set; }
        public Color Color { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        public Guid? CartId { get; set; }
        public Cart? Cart { get; set; }
        public Guid? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
