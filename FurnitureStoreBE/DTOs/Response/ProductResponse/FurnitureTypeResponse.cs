namespace FurnitureStoreBE.DTOs.Response.ProductResponse
{
    public class FurnitureTypeResponse
    {
        public Guid Id { get; set; }
        public string FurnitureTypeName { get; set; }
        public string ImageSource { get; set; }
        public string Description { get; set; }
        public Guid RoomSpaceId { get; set; }
    }
}
