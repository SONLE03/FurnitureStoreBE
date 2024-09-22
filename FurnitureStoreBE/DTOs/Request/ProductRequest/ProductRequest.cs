using FurnitureStoreBE.Models;
using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "Brand name is required.")]

        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Thumbnails is required.")]

        public Guid AssetId { get; set; }
        [Required(ErrorMessage = "Brand is required.")]

        public Guid BrandId { get; set; }
        [Required(ErrorMessage = "Category is required.")]

        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Designers are required.")]

        public List<Guid>? DesignersId { get; set; }
        [Required(ErrorMessage = "Materials are required.")]

        public List<Guid>? MaterialsId { get; set; }
        [Required(ErrorMessage = "Product variants are required.")]

        public List<ProductVariantRequest>? ProductVariants { get; set; }
    }
    public class ProductVariantRequest
    {
        [Required(ErrorMessage = "Color is required.")]

        public Guid ColorId { get; set; }
        [Required(ErrorMessage = "Length is required.")]

        public decimal Length { get; set; }
        [Required(ErrorMessage = "Width is required.")]

        public decimal Width { get; set; }
        [Required(ErrorMessage = "Heights is required.")]

        public decimal Height { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]

        public long Quantity { get; set; }
        [Required(ErrorMessage = "Price is required.")]

        public decimal Price { get; set; }
        [Required(ErrorMessage = "Images are required.")]

        List<IFormFile> Images { get; set; }
    }
}
