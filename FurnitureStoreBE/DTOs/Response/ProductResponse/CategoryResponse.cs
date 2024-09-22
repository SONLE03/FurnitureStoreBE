namespace FurnitureStoreBE.DTOs.Response.ProductResponse
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string ImageSource { get; set; }
        public string Description { get; set; }
        public Guid FurnitureTypeId { get; set; }
    }
}
