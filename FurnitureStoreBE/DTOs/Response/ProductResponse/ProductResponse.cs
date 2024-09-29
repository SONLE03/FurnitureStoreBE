using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.DTOs.Response.ProductResponse
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ImageSource {  get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string DisplayPrice { get; set; }
        public decimal Discount { get; set; } = 0;
        public List<string> Materials { get; set; }
        public List<string> Designers { get; set; }
        public List<ProductVariantResponse> ProductVariants { get; set; }
    }
    public class ProductVariantResponse
    {
        public Guid Id { get; set; }
        public Guid ColorId { get; set; }
        public string ColorName { get; set; }
        public string DisplayDimension { get; set; }
        public long Quantity { get; set; }
        public decimal Price { get; set; }
        public List<string> ImageSource { get; set; }
    }
}

