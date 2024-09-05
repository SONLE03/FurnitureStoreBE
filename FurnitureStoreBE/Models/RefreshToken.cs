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
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
