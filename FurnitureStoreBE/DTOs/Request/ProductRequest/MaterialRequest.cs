using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class MaterialRequest
    {
        [Required(ErrorMessage = "Material name is required.")]
        public string MaterialName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

    }
}
