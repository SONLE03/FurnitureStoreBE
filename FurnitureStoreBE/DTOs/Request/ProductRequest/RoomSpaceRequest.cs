using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class RoomSpaceRequest
    {
        [Required(ErrorMessage = "Room space name is required.")]
        public string RoomSpaceName { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }

    }
}
