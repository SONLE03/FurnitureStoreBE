using AutoMapper;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using FurnitureStoreBE.DTOs.Request.ProductRequest;

namespace FurnitureStoreBE.Services.ProductService.CategoryService
{
    public class CategoryServiceImp : ICategoryService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public CategoryServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<CategoryResponse>> GetAllCategories(PageInfo pageInfo)
        {
            var categoryQuery = _dbContext.Categories
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<CategoryResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Categories.CountAsync();
            return await Task.FromResult(PaginatedList<CategoryResponse>.ToPagedList(categoryQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task ChangeCategoryImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var category = await _dbContext.Categories.FirstOrDefaultAsync(b => b.Id == id);
                if (category == null) throw new ObjectNotFoundException("Category not found");
                Asset categoryImage = new Asset();
                if (category.AssetId == null)
                {
                    categoryImage.Category = category;
                }
                else
                {
                    categoryImage.Id = (Guid)category.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(categoryImage.Id);
                }

                var categoryImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.Category.ToString());
                categoryImage.Name = categoryImageUploadResult.OriginalFilename;
                categoryImage.URL = categoryImageUploadResult.Url.ToString();
                categoryImage.CloudinaryId = categoryImageUploadResult.PublicId;
                categoryImage.FolderName = EUploadFileFolder.Category.ToString();
                if (category.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(categoryImage);
                }
                else
                {
                    _dbContext.Assets.Update(categoryImage);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CategoryResponse> CreateCategory(CategoryRequest categoryRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if(!await _dbContext.FurnitureTypes.AnyAsync(ft => categoryRequest.FurnitureTypeId == ft.Id))
                {
                    throw new ObjectNotFoundException("Furniture type not found");
                }
                Asset asset = null;
                if (categoryRequest.Image != null)
                {
                    var categoryImageUploadResult = await _fileUploadService.UploadFileAsync(categoryRequest.Image, EUploadFileFolder.Category.ToString());
                    asset = new Asset
                    {
                        Name = categoryImageUploadResult.OriginalFilename,
                        URL = categoryImageUploadResult.Url.ToString(),
                        CloudinaryId = categoryImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.Category.ToString(),
                    };
                }
                var category = new Category 
                { 
                    CategoryName = categoryRequest.CategoryName, 
                    Description = categoryRequest.Description, 
                    Asset = asset,
                    FurnitureTypeId = categoryRequest.FurnitureTypeId,
                };
                category.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<CategoryResponse>(category);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteCategory(Guid id)
        {
            try
            {
                if (!await _dbContext.Categories.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("Category not found");
                var sql = "DELETE FROM \"Category\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"Category\" SET \"IsDeleted\" = @p0 WHERE \"Id\"  = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }
            }
            catch
            {
                throw new BusinessException("Category removal failed");
            }
           
        }

        public async Task<CategoryResponse> UpdateCategory(Guid id, CategoryRequest categoryRequest)
        {
            var category = await _dbContext.Categories.FirstAsync(b => b.Id == id);
            if (category == null) throw new ObjectNotFoundException("Category not found");
            category.CategoryName = categoryRequest.CategoryName;
            category.Description = categoryRequest.Description;
            category.setCommonUpdate(UserSession.GetUserId());
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
