using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FurnitureStoreBE.Models
{
    [Table("AspNetTypeClaims")]
    public class AspNetTypeClaims
    {
        [Key]
        public int Id { get; set; }
        public string Name {  get; set; }
        public ICollection<AspNetRoleClaims<string>> RoleClaims { get; set; }
    }
}
