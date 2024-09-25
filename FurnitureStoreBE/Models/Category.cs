using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Category")]
    public class Category : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public Guid? AssetId { get; set; }
        public Asset? Asset { get; set; }
        public Guid FurnitureTypeId { get; set; }
        public FurnitureType FurnitureType { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
