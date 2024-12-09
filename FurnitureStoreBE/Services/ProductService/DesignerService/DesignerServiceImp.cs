using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using AutoMapper;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.Services.FileUploadService;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using Microsoft.AspNetCore.Http;

namespace FurnitureStoreBE.Services.ProductService.DesignerService
{
    public class DesignerServiceImp : IDesignerService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public DesignerServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        public async Task ChangeDesignerImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var designer = await _dbContext.Designer.FirstOrDefaultAsync(b => b.Id == id);
                if (designer == null) throw new ObjectNotFoundException("Designer not found");
                Asset designerImage = new Asset();
                if (designer.AssetId == null)
                {
                    designerImage.Designer = designer;
                }
                else
                {
                    designerImage.Id = (Guid)designer.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(designerImage.Id);
                }

                var designerImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.Designer.ToString());
                designerImage.Name = designerImageUploadResult.OriginalFilename;
                designerImage.URL = designerImageUploadResult.Url.ToString();
                designerImage.CloudinaryId = designerImageUploadResult.PublicId;
                designerImage.FolderName = EUploadFileFolder.Designer.ToString();
                if (designer.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(designerImage);
                }
                else
                {
                    _dbContext.Assets.Update(designerImage);
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

        public async Task<DesignerResponse> CreateDesigner(DesignerRequest designerRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                Asset asset = null;
                if (designerRequest.Image != null)
                {
                    var designerImageUploadResult = await _fileUploadService.UploadFileAsync(designerRequest.Image, EUploadFileFolder.Designer.ToString());
                    asset = new Asset
                    {
                        Name = designerImageUploadResult.OriginalFilename,
                        URL = designerImageUploadResult.Url.ToString(),
                        CloudinaryId = designerImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.Designer.ToString(),
                    };
                }
                var designer = new Designer { DesignerName = designerRequest.DesignerName, Description = designerRequest.Description, Asset = asset };
                designer.setCommonCreate(UserSession.GetUserId());
                await _dbContext.Designer.AddAsync(designer);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<DesignerResponse>(designer);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteDesigner(Guid id)
        {
            //try
            //{
                if (!await _dbContext.Designer.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("Designer not found");
                var sql = "DELETE FROM \"Designer\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"Designer\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);                 
                }
            //}
            //catch
            //{
            //    throw new BusinessException("Designer removal failed");
            //}
        }
        public async Task<PaginatedList<DesignerResponse>> GetAllDesigners(PageInfo pageInfo)
        {
            var designerQuery = _dbContext.Designer
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<DesignerResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.Designer.CountAsync();
            return await Task.FromResult(PaginatedList<DesignerResponse>.ToPagedList(designerQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task<DesignerResponse> UpdateDesigner(Guid id, DesignerRequest designerRequest)
        {
            var desiner = await _dbContext.Designer.FirstAsync(b => b.Id == id);
            if (desiner == null) throw new ObjectNotFoundException("Designer not found");
            desiner.DesignerName = designerRequest.DesignerName;
            desiner.Description = designerRequest.Description;
            desiner.setCommonUpdate(UserSession.GetUserId());
            _dbContext.Designer.Update(desiner);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<DesignerResponse>(desiner);
        }
    }
}
