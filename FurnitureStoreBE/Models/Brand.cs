using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Brand")]
    public class Brand : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public required string BrandName { get; set; }
        public required string Description { get; set; }

        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
