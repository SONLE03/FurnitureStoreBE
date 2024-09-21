using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class DesignerRequest
    {
        [Required(ErrorMessage = "Designer name is required.")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
