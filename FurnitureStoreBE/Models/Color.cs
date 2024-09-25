using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Color")]
    public class Color : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ColorName {  get; set; }
        public string? ColorCode { get; set; }
        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }
}
