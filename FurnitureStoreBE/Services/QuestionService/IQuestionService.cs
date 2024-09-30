using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Response.QuestionResponse;
using FurnitureStoreBE.DTOs.Request.QuestionRequest;
using FurnitureStoreBE.DTOs.Response.ReplyResponses;

namespace FurnitureStoreBE.Services.QuestionService
{
    public interface IQuestionService
    {
        Task<QuestionResponse> GetQuestionById(Guid questionId);
        Task<PaginatedList<QuestionResponse>> GetQuestion(PageInfo pageInfo, string keyword);
        Task<PaginatedList<QuestionResponse>> GetQuestionByCustomer(PageInfo pageInfo, string userId);
        Task<PaginatedList<QuestionResponse>> GetQuestionByProduct(PageInfo pageInfo, Guid productId);
        Task<PaginatedList<QuestionResponse>> GetQuestionByProductAndCustomer(PageInfo pageInfo, Guid productId, string userId);
        Task<QuestionResponse> CreateQuestion(QuestionRequest questionRequest);
        Task<QuestionResponse> UpdateQuestion(Guid questionId, QuestionRequest questionRequest);
        Task DeleteQuestionForCustomer(Guid questionId, string userId);
        Task DeleteQuestionsByIds(List<Guid> questionIds);
        Task DeleteAllQuestions();
        Task<ReplyResponse> ReplyQuestion(Guid questionId, string userId, string content);
        Task DeleteReply(Guid replyId, string userId);
    }
}
