using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.QuestionRequest;
using FurnitureStoreBE.Services.QuestionService;
using FurnitureStoreBE.Services.QuestionService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.QuestionController
{
    [ApiController]
    [Route(Routes.QUESTION)]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestionById(Guid questionId)
        {
            return new SuccessfulResponse<object>(await _questionService.GetQuestionById(questionId), (int)HttpStatusCode.OK, "Get question product by id successfully").GetResponse();
        }
        [HttpGet()]
        public async Task<IActionResult> GetQuestions([FromQuery] PageInfo pageInfo, [FromQuery] string keyword)
        {
            return new SuccessfulResponse<object>(await _questionService.GetQuestion(pageInfo, keyword), (int)HttpStatusCode.OK, "Get question successfully").GetResponse();
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetQuestionsByProduct([FromQuery] PageInfo pageInfo, Guid productId)
        {
            return new SuccessfulResponse<object>(await _questionService.GetQuestionByProduct(pageInfo, productId), (int)HttpStatusCode.OK, "Get question by product successfully").GetResponse();
        }
        [HttpGet("user/{productId}")]
        public async Task<IActionResult> GetQuestionsByCustomer([FromQuery] PageInfo pageInfo, string userId)
        {
            return new SuccessfulResponse<object>(await _questionService.GetQuestionByCustomer(pageInfo, userId), (int)HttpStatusCode.OK, "Get question by customer successfully").GetResponse();
        }
        [HttpGet("{userId}/{productId}")]
        public async Task<IActionResult> GetQuestionsByProductAndCustomer([FromQuery] PageInfo pageInfo, string userId, Guid productId)
        {
            return new SuccessfulResponse<object>(await _questionService.GetQuestionByProductAndCustomer(pageInfo, productId, userId), (int)HttpStatusCode.OK, "Get question by product and customer successfully").GetResponse();
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromForm] QuestionRequest questionRequest)
        {
            return new SuccessfulResponse<object>(await _questionService.CreateQuestion(questionRequest), (int)HttpStatusCode.OK, "Your question product has been successfully added").GetResponse();
        }
        [HttpPost("reply/{userId}/{questionId}")]
        public async Task<IActionResult> ReplyQuestion(Guid questionId, string userId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Content cannot be null or empty.");
            }
            return new SuccessfulResponse<object>(await _questionService.ReplyQuestion(questionId, userId, content), (int)HttpStatusCode.OK, "Your question product has been successfully added").GetResponse();
        }
        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(Guid questionId, [FromForm] QuestionRequest QuestionRequest)
        {
            return new SuccessfulResponse<object>(await _questionService.UpdateQuestion(questionId, QuestionRequest), (int)HttpStatusCode.OK, "Your question product has been successfully modified").GetResponse();
        }
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllQuestions()
        {
            await _questionService.DeleteAllQuestions();
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "All question product has been successfully removed").GetResponse();
        }
        [HttpDelete("by-ids")]
        public async Task<IActionResult> DeleteQuestionsByIds(List<Guid> questionId)
        {
            await _questionService.DeleteQuestionsByIds(questionId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Question product has been successfully removed").GetResponse();
        }
        [HttpDelete("{questionId}/{customerId}")]
        public async Task<IActionResult> DeleteQuestionForCustomer(Guid questionId, string customerId)
        {
            await _questionService.DeleteQuestionForCustomer(questionId, customerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Question product has been successfully removed").GetResponse();
        }
        [HttpDelete("reply/{userId}/{replyId}")]
        public async Task<IActionResult> DeleteReply(Guid replyId, string userId)
        {
            await _questionService.DeleteReply(replyId, userId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Reply question has been successfully removed").GetResponse();
        }
    }
}
