using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FurnitureStoreBE.Models
{
    [Table("Token")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredDate { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
