using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.BrandRequest
{
    public class BrandRequest
    {
        [Required(ErrorMessage = "Brand name is required.")]
        public string BrandName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
