using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ReviewRequest
{
    public class ReviewRequest
    {
        public string Content { get; set; }
        public List<IFormFile>? ReviewImage { get; set; }
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        public int Rate { get; set; }
    }
}
