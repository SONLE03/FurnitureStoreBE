using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class ColorRequest
    {
        [Required(ErrorMessage = "Color name is required.")]

        public string ColorName { get; set; }
        public string? ColorCode { get; set; }
    }
}
