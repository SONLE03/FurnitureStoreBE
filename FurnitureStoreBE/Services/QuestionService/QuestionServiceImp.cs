using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.QuestionRequest;
using FurnitureStoreBE.DTOs.Request.ReviewRequest;
using FurnitureStoreBE.DTOs.Response.QuestionResponse;
using FurnitureStoreBE.DTOs.Response.ReplyResponses;
using FurnitureStoreBE.DTOs.Response.ReviewResponse;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FurnitureStoreBE.Services.QuestionService
{
    public class QuestionServiceImp : IQuestionService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public QuestionServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        // Phương thức để lấy ReviewResponse
        private async Task<QuestionResponse> GetQuestionResponseAsync(IQueryable<Question> questionQuery)
        {
            var createdQuestion = await questionQuery
                .Select(r => new Question
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Content = r.Content,
                    UpdatedDate = r.UpdatedDate,
                    User = new User
                    {
                        FullName = r.User.FullName,
                        Role = r.User.Role
                    }
                })
                .FirstOrDefaultAsync();

            return _mapper.Map<QuestionResponse>(createdQuestion);
        }
        public async Task<QuestionResponse> GetQuestionById(Guid questionId)
        {
            if (!await _dbContext.Question.AnyAsync(r => r.Id == questionId))
            {
                throw new ObjectNotFoundException("Question not found");
            }
            var questionQuery = _dbContext.Question.Where(r => r.Id == questionId);
            return await GetQuestionResponseAsync(questionQuery);
        }
        private async Task<PaginatedList<QuestionResponse>> GetQuestion(PageInfo pageInfo, Expression<Func<Question, bool>> predicate = null)
        {
            predicate ??= r => true; // Nếu predicate là null, sử dụng một điều kiện luôn đúng
            var questionQuery = _dbContext.Question
                .Include(r => r.Reply)
                .Where(predicate)
                .OrderByDescending(c => c.CreatedDate)
                .ProjectTo<QuestionResponse>(_mapper.ConfigurationProvider);

            var count = await _dbContext.Question.CountAsync(predicate);
            return await Task.FromResult(PaginatedList<QuestionResponse>.ToPagedList(questionQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
        public async Task<PaginatedList<QuestionResponse>> GetQuestion(PageInfo pageInfo, string keyword)
        {
            return await GetQuestion(pageInfo);
        }
        public async Task<PaginatedList<QuestionResponse>> GetQuestionByCustomer(PageInfo pageInfo, string userId)
        {
            return await GetQuestion(pageInfo, (r => r.UserId == userId && !r.IsDeleted));
        }


        public async Task<PaginatedList<QuestionResponse>> GetQuestionByProduct(PageInfo pageInfo, Guid productId)
        {
            return await GetQuestion(pageInfo, r => r.ProductId == productId && !r.IsDeleted);
        }

        public async  Task<PaginatedList<QuestionResponse>> GetQuestionByProductAndCustomer(PageInfo pageInfo, Guid productId, string userId)
        {
            return await GetQuestion(pageInfo, r => r.ProductId == productId && r.UserId == userId && !r.IsDeleted);
        }

        public async Task<ReplyResponse> ReplyQuestion(Guid questionId, string userId, string content)
        {
            if (!await _dbContext.Question.AnyAsync(r => r.Id == questionId))
            {
                throw new ObjectNotFoundException("Question not found");
            }
            var reply = new Reply
            {
                Content = content,
                UserId = userId,
                QuestionId = questionId
            };
            reply.setCommonCreate(UserSession.GetUserId());
            await _dbContext.Replies.AddAsync(reply);
            await _dbContext.SaveChangesAsync();
            var replyResponse = await (from r in _dbContext.Replies
                                       join u in _dbContext.Users on r.UserId equals u.Id
                                       where r.Id == reply.Id
                                       select new ReplyResponse
                                       {
                                           Id = r.Id,
                                           Content = r.Content,
                                           UserId = r.UserId,
                                           FullName = u.FullName,
                                           UpdatedDate = r.UpdatedDate,
                                           Role = u.Role
                                       }).FirstOrDefaultAsync();

            return replyResponse;
        }
        public async Task<QuestionResponse> CreateQuestion(QuestionRequest questionRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!await _dbContext.Products.AnyAsync(p => p.Id == questionRequest.ProductId))
                {
                    throw new ObjectNotFoundException("Product not found");
                }
               
                var question = new Question
                {
                    ProductId = questionRequest.ProductId,
                    UserId = questionRequest.UserId,
                    Content = questionRequest.Content,
                };

                question.setCommonCreate(UserSession.GetUserId());

                await _dbContext.Question.AddAsync(question);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                var questionQuery = _dbContext.Question.Where(r => r.Id == question.Id);
                return _mapper.Map<QuestionResponse>(await GetQuestionResponseAsync(questionQuery));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<QuestionResponse> UpdateQuestion(Guid questionId, QuestionRequest questionRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var question = await _dbContext.Question
                    .Where(q => q.Id == questionId)
                    .SingleOrDefaultAsync();

                if (question == null)
                {
                    throw new ObjectNotFoundException("Question not found");
                }

                question.Content = questionRequest.Content;
                question.setCommonCreate(UserSession.GetUserId());

                _dbContext.Question.Update(question); 

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<QuestionResponse>(question);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Ghi log lỗi nếu cần
                throw new ApplicationException("An error occurred while updating the question", ex);
            }
        }

        public async Task DeleteAllQuestions()
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var question = await _dbContext.Question
                    .Include(r => r.Reply)
                    .ToListAsync();

                if (question.Any())
                {
                    var repliesToDelete = question
                        .SelectMany(r => r.Reply)
                        .ToList();

                    if (repliesToDelete.Any())
                    {
                        _dbContext.Replies.RemoveRange(repliesToDelete);
                    }
                    _dbContext.Question.RemoveRange(question);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the question");
            }
        }

        public async Task DeleteReply(Guid replyId, string userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var reply = await _dbContext.Replies
                    .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId);
                if (reply == null)
                {
                    throw new ObjectNotFoundException("This reply is not yours.");
                }
                _dbContext.Replies.Remove(reply);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the reply");
            }
        }

        public async Task DeleteQuestionForCustomer(Guid questionId, string userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (!await _dbContext.Question.AnyAsync(r => r.Id == questionId && r.UserId == userId))
                {
                    throw new ObjectNotFoundException("This question is not yours.");
                }
                var question = await _dbContext.Question
                    .Include(r => r.Reply)
                    .Where(r => r.Id == questionId && r.UserId == userId)
                    .ToListAsync();

                if (question.Any())
                {
                    
                    var repliesToDelete = question
                        .SelectMany(r => r.Reply)
                        .ToList();

                    if (repliesToDelete.Any())
                    {
                        _dbContext.Replies.RemoveRange(repliesToDelete);
                    }
                    
                    _dbContext.Question.RemoveRange(question);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the question");
            }
        }

        public async Task DeleteQuestionsByIds(List<Guid> questionIds)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var question = await _dbContext.Question
                    .Include(r => r.Reply)
                    .Where(r => questionIds.Contains(r.Id))
                    .ToListAsync();
                if (!question.Any())
                {
                    throw new ObjectNotFoundException("No question found for the given IDs.");
                }
                
                var repliesToDelete = question
                    .SelectMany(r => r.Reply)
                    .ToList();

                if (repliesToDelete.Any())
                {
                    _dbContext.Replies.RemoveRange(repliesToDelete);
                }
                
                _dbContext.Question.RemoveRange(question);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new BusinessException("An error occurred while deleting the question");
            }
        }

       

       
    }
}
