using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Asset")]
    public class Asset
    {
        [Key] 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string CloudinaryId { get; set; }
        public string FolderName { get; set; }

        public User? User { get; set; }
        public Brand? Brand { get; set; }
        public Designer? Designer { get; set; }
        public RoomSpace? RoomSpace { get; set; }
        public FurnitureType? FurnitureType { get; set; }
        public Category? Category { get; set; }
        public Material? Material { get; set; }
        public Guid? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public Guid? ReviewId { get; set; }
        public Review? Review { get; set; }
        public Coupon? Coupon { get; set; }
        public Product? Product { get; set; }

    }
}
