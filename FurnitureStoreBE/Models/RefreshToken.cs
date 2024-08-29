using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.Models
{
    [Table("Token")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiredDate { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
