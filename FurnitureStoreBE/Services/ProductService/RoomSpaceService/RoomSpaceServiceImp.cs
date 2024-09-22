using AutoMapper;
using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.Enums;
using FurnitureStoreBE.Exceptions;
using FurnitureStoreBE.Models;
using FurnitureStoreBE.Services.FileUploadService;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace FurnitureStoreBE.Services.ProductService.RoomSpaceService
{
    public class RoomSpaceServiceImp : IRoomSpaceService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        public RoomSpaceServiceImp(ApplicationDBContext dbContext, IFileUploadService fileUploadService, IMapper mapper)
        {
            _dbContext = dbContext;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        public async Task<PaginatedList<RoomSpaceResponse>> GetAllRoomSpaces(PageInfo pageInfo)
        {
            var roomSpaceQuery = _dbContext.RoomSpaces
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedDate)
                .ProjectTo<RoomSpaceResponse>(_mapper.ConfigurationProvider);
            var count = await _dbContext.RoomSpaces.CountAsync();
            return await Task.FromResult(PaginatedList<RoomSpaceResponse>.ToPagedList(roomSpaceQuery, pageInfo.PageNumber, pageInfo.PageSize));
        }

        public async Task ChangeRoomSpaceImage(Guid id, IFormFile formFile)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var RoomSpace = await _dbContext.RoomSpaces.FirstOrDefaultAsync(b => b.Id == id);
                if (RoomSpace == null) throw new ObjectNotFoundException("RoomSpace not found");
                Asset RoomSpaceImage = new Asset();
                if (RoomSpace.AssetId == null)
                {
                    RoomSpaceImage.RoomSpace = RoomSpace;
                }
                else
                {
                    RoomSpaceImage.Id = (Guid)RoomSpace.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(RoomSpaceImage.Id);
                }

                var RoomSpaceImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.RoomSpace.ToString());
                RoomSpaceImage.Name = RoomSpaceImageUploadResult.OriginalFilename;
                RoomSpaceImage.URL = RoomSpaceImageUploadResult.Url.ToString();
                RoomSpaceImage.CloudinaryId = RoomSpaceImageUploadResult.PublicId;
                RoomSpaceImage.FolderName = EUploadFileFolder.RoomSpace.ToString();
                if (RoomSpace.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(RoomSpaceImage);
                }
                else
                {
                    _dbContext.Assets.Update(RoomSpaceImage);
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

        public async Task<RoomSpaceResponse> CreateRoomSpace(RoomSpaceRequest RoomSpaceRequest, IFormFile file)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var roomSpaceImageUploadResult = await _fileUploadService.UploadFileAsync(file, EUploadFileFolder.RoomSpace.ToString());
                var asset = new Asset
                {
                    Name = roomSpaceImageUploadResult.OriginalFilename,
                    URL = roomSpaceImageUploadResult.Url.ToString(),
                    CloudinaryId = roomSpaceImageUploadResult.PublicId,
                    FolderName = EUploadFileFolder.RoomSpace.ToString(),
                };
                var RoomSpace = new RoomSpace { RoomSpaceName = RoomSpaceRequest.RoomSpaceName, Description = RoomSpaceRequest.Description, Asset = asset };
                RoomSpace.setCommonCreate(UserSession.GetUserId());
                await _dbContext.RoomSpaces.AddAsync(RoomSpace);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<RoomSpaceResponse>(RoomSpace);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteRoomSpace(Guid id)
        {
            try
            {
                if (!await _dbContext.RoomSpaces.AnyAsync(b => b.Id == id)) throw new ObjectNotFoundException("RoomSpace not found");
                var sql = "DELETE FROM RoomSpace WHERE Id = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE RoomSpace SET IsDeleted = @p0 WHERE Id = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }        
            }
            catch
            {
                throw new BusinessException("RoomSpace removal failed");
            }
        }
        public async Task<RoomSpaceResponse> UpdateRoomSpace(Guid id, RoomSpaceRequest RoomSpaceRequest)
        {
            var roomSpace = await _dbContext.RoomSpaces.FirstAsync(b => b.Id == id);
            if (roomSpace == null) throw new ObjectNotFoundException("RoomSpace not found");
            roomSpace.RoomSpaceName = RoomSpaceRequest.RoomSpaceName;
            roomSpace.Description = RoomSpaceRequest.Description;
            roomSpace.setCommonUpdate(UserSession.GetUserId());
            _dbContext.RoomSpaces.Update(roomSpace);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<RoomSpaceResponse>(roomSpace);
        }
    }
}
