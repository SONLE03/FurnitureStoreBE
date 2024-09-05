using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public string id { get; set; }
    }
}
