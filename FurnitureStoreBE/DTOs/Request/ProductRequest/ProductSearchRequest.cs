namespace FurnitureStoreBE.DTOs.Request.ProductRequest
{
    public class ProductSearchRequest
    {
        public List<Guid>? BrandIds { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public decimal? FromPrice {  get; set; }
        public decimal? ToPrice { get; set; }
        public string? SortCase { get; set; }
        public bool? IsAscSort { get; set; }   
    }
}
