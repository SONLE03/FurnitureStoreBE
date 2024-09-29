using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; } = 0;
        public long Sold { set; get; } = 0;
        public string Unit { set; get; } 

        public EProductStatus Status { get; set; } = EProductStatus.ACTIVE;
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Designer>? Designers { get; set; }
        public ICollection<Material>? Materials { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

    }
}
