using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class FurnitureTypeRequest
    {
        [Required(ErrorMessage = "Furniture type name is required.")]
        public string FurnitureTypeName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

        public Guid RoomSpaceId { get; set; }
    }
}
