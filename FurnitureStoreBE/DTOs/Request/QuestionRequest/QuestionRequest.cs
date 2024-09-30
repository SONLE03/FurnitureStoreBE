namespace FurnitureStoreBE.DTOs.Request.QuestionRequest
{
    public class QuestionRequest
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
