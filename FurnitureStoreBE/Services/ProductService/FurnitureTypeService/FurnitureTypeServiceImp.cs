using AutoMapper;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Services.FileUploadService;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FurnitureStoreBE.Models;

namespace FurnitureStoreBE.Services.ProductService.FurnitureTypeService
{
    public class FurnitureTypeServiceImp : IFurnitureTypeService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public FurnitureTypeServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<FurnitureTypeResponse>> GetAllFurnitureTypes(PageInfo pageInfo)
        {
            var furnitureTypeQuery = _dbContext.FurnitureTypes
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<FurnitureTypeResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.FurnitureTypes.CountAsync();
            return await Task.FromResult(PaginatedList<FurnitureTypeResponse>.ToPagedList(furnitureTypeQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task ChangeFurnitureTypeImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var furnitureType = await _dbContext.FurnitureTypes.FirstOrDefaultAsync(b => b.Id == id);
                if (furnitureType == null) throw new ObjectNotFoundException("FurnitureType not found");
                Asset furnitureTypeImage = new Asset();
                if (furnitureType.AssetId == null)
                {
                    furnitureTypeImage.FurnitureType = furnitureType;
                }
                else
                {
                    furnitureTypeImage.Id = (Guid)furnitureType.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(furnitureTypeImage.Id);
                }

                var furnitureTypeImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.FurnitureType.ToString());
                furnitureTypeImage.Name = furnitureTypeImageUploadResult.OriginalFilename;
                furnitureTypeImage.URL = furnitureTypeImageUploadResult.Url.ToString();
                furnitureTypeImage.CloudinaryId = furnitureTypeImageUploadResult.PublicId;
                furnitureTypeImage.FolderName = EUploadFileFolder.FurnitureType.ToString();
                if (furnitureType.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(furnitureTypeImage);
                }
                else
                {
                    _dbContext.Assets.Update(furnitureTypeImage);
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

        public async Task<FurnitureTypeResponse> CreateFurnitureType(FurnitureTypeRequest furnitureTypeRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if(!await _dbContext.RoomSpaces.AnyAsync(r => furnitureTypeRequest.RoomSpaceId == r.Id))
                {
                    throw new ObjectNotFoundException("Room space not found");
                }
                Asset asset = null;
                if(furnitureTypeRequest.Image != null)
                {
                    var furnitureTypeImageUploadResult = await _fileUploadService.UploadFileAsync(furnitureTypeRequest.Image, EUploadFileFolder.FurnitureType.ToString());
                    asset = new Asset
                    {
                        Name = furnitureTypeImageUploadResult.OriginalFilename,
                        URL = furnitureTypeImageUploadResult.Url.ToString(),
                        CloudinaryId = furnitureTypeImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.FurnitureType.ToString(),
                    };
                }
                var furnitureType = new FurnitureType 
                { 
                    FurnitureTypeName = furnitureTypeRequest.FurnitureTypeName, 
                    Description = furnitureTypeRequest.Description, 
                    Asset = asset,
                    RoomSpaceId = furnitureTypeRequest.RoomSpaceId,
                };

                furnitureType.setCommonCreate(UserSession.GetUserId());
                await _dbContext.FurnitureTypes.AddAsync(furnitureType);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<FurnitureTypeResponse>(furnitureType);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteFurnitureType(Guid id)
        {
            try
            {
                if (!await _dbContext.FurnitureTypes.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("FurnitureType not found");
                var sql = "DELETE FROM \"FurnitureType\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"FurnitureType\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }
            }
            catch
            {
                throw new BusinessException("FurnitureType removal failed");
            } 
        }
        public async Task<FurnitureTypeResponse> UpdateFurnitureType(Guid id, FurnitureTypeRequest furnitureTypeRequest)
        {
            var furnitureType = await _dbContext.FurnitureTypes.FirstAsync(b => b.Id == id);
            if (furnitureType == null) throw new ObjectNotFoundException("FurnitureType not found");
            furnitureType.FurnitureTypeName = furnitureTypeRequest.FurnitureTypeName;
            furnitureType.Description = furnitureTypeRequest.Description;
            furnitureType.setCommonUpdate(UserSession.GetUserId());
            _dbContext.FurnitureTypes.Update(furnitureType);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<FurnitureTypeResponse>(furnitureType);
        }
    }
}
