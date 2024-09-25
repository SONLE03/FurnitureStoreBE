using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

        public Guid FurnitureTypeId { get; set; }
    }
}
