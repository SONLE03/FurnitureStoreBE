﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("ProductVariant")]
    public class ProductVariant : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid ColorId { get; set; }
        public Color Color { get; set; }
        public required string DisplayDimension { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Length { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Width { get; set; } = 0;
     
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Height { get; set; } = 0;

        public required long Quantity { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; } = 0;
        public ICollection<Asset>? Assets { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
