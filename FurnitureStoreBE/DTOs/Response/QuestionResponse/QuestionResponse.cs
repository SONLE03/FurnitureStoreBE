using FurnitureStoreBE.DTOs.Response.ReplyResponses;

namespace FurnitureStoreBE.DTOs.Response.QuestionResponse
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid ProductId { get; set; }
        public string UserId { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ICollection<ReplyResponse>? ReplyResponses { get; set; }
    }
}