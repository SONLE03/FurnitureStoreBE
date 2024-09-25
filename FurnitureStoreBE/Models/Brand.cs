using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Brand")]
    public class Brand : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string? Description { get; set; }

        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
