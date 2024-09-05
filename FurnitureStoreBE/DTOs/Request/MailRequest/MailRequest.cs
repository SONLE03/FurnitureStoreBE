using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.MailRequest
{
    public class MailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? ToEmail { get; set; }
        [Required(ErrorMessage = "Subject is required.")]
        public string? Subject { get; set; }
        [Required(ErrorMessage = "Body is required.")]
        public string? Body { get; set; }
    }
}
