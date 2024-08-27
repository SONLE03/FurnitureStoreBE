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
        public required string Name { get; set; }
        public required string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal MinPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal MaxPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Discount { get; set; } = 0;
        public required long Sold { set; get; } = 0;

        public EProductStatus Status { get; set; } = EProductStatus.ACTIVE;
        public Guid AssetId { get; set; }
        public required Asset Asset { get; set; }
        public Guid BrandId { get; set; }
        public required Brand Brand { get; set; }
        public Guid CategoryId { get; set; }
        public required Category Category { get; set; }
        public ICollection<Designer>? Designers { get; set; }
        public ICollection<Material>? Materials { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<Coupon>? Coupons { get; set; }
    }
}
