namespace FurnitureStoreBE.DTOs.Response.ReplyResponses
{
    public class ReplyResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}