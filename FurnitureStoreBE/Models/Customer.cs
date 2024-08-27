using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public Guid id { get; set; }
    }
}
