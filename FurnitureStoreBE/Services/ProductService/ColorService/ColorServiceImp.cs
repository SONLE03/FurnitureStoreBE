using AutoMapper;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.ProductService.ColorService
{
    public class ColorServiceImp : IColorService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public ColorServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<ColorResponse>> GetAllColors(PageInfo pageInfo)
        {
            var colorQuery = _dbContext.Colors
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<ColorResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Colors.CountAsync();
            return await Task.FromResult(PaginatedList<ColorResponse>.ToPagedList(colorQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }
        public async Task<ColorResponse> CreateColor(ColorRequest colorRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var color = new Color { ColorName = colorRequest.ColorName, ColorCode = colorRequest.ColorCode};
                color.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Colors.AddAsync(color);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<ColorResponse>(color);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ColorResponse> UpdateColor(Guid id, ColorRequest colorRequest)
        {
            var color = await _dbContext.Colors.FirstAsync(b => b.Id == id);
            if (color == null) throw new ObjectNotFoundException("Color not found");
            color.ColorName = colorRequest.ColorName;
            color.ColorCode = colorRequest.ColorCode;
            color.setCommonUpdate(UserSession.GetUserId());
            _dbContext.Colors.Update(color);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ColorResponse>(color);
        }
        public async Task DeleteColor(Guid id)
        {
            try
            {
                if (!await _dbContext.Colors.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("Color not found");
                var sql = "DELETE FROM \"Color\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"Color\" SET \"IsDeleted\" = @p0 WHERE \"Id\"  = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }
            }
            catch
            {
                throw new BusinessException("Color removal failed");
            }
        }

    }
}
