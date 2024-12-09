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
                var roomSpace = await _dbContext.RoomSpaces.FirstOrDefaultAsync(b => b.Id == id);
                if (roomSpace == null) throw new ObjectNotFoundException("RoomSpace not found");
                Asset roomSpaceImage = new Asset();
                if (roomSpace.AssetId == null)
                {
                    roomSpaceImage.RoomSpace = roomSpace;
                }
                else
                {
                    roomSpaceImage.Id = (Guid)roomSpace.AssetId;
                    await _fileUploadService.DestroyFileByAssetIdAsync(roomSpaceImage.Id);
                }

                var roomSpaceImageUploadResult = await _fileUploadService.UploadFileAsync(formFile, EUploadFileFolder.RoomSpace.ToString());
                roomSpaceImage.Name = roomSpaceImageUploadResult.OriginalFilename;
                roomSpaceImage.URL = roomSpaceImageUploadResult.Url.ToString();
                roomSpaceImage.CloudinaryId = roomSpaceImageUploadResult.PublicId;
                roomSpaceImage.FolderName = EUploadFileFolder.RoomSpace.ToString();
                if (roomSpace.AssetId == null)
                {
                    await _dbContext.Assets.AddAsync(roomSpaceImage);
                }
                else
                {
                    _dbContext.Assets.Update(roomSpaceImage);
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

        public async Task<RoomSpaceResponse> CreateRoomSpace(RoomSpaceRequest roomSpaceRequest)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                Asset asset = null;
                if(roomSpaceRequest.Image != null)
                {
                    var roomSpaceImageUploadResult = await _fileUploadService.UploadFileAsync(roomSpaceRequest.Image, EUploadFileFolder.RoomSpace.ToString());
                    asset = new Asset
                    {
                        Name = roomSpaceImageUploadResult.OriginalFilename,
                        URL = roomSpaceImageUploadResult.Url.ToString(),
                        CloudinaryId = roomSpaceImageUploadResult.PublicId,
                        FolderName = EUploadFileFolder.RoomSpace.ToString(),
                    };
                }
                var roomSpace = new RoomSpace { RoomSpaceName = roomSpaceRequest.RoomSpaceName, Description = roomSpaceRequest.Description, Asset = asset };
                roomSpace.setCommonCreate(UserSession.GetUserId());
                await _dbContext.RoomSpaces.AddAsync(roomSpace);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<RoomSpaceResponse>(roomSpace);
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
                var sql = "DELETE FROM \"RoomSpace\" WHERE \"Id\" = @p0";
                int affectedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql, id);
                if (affectedRows == 0)
                {
                    sql = "UPDATE \"RoomSpace\" SET \"IsDeleted\" = @p0 WHERE \"Id\" = @p1";
                    await _dbContext.Database.ExecuteSqlRawAsync(sql, true, id);
                }        
            }
            catch
            {
                throw new BusinessException("RoomSpace removal failed");
            }
        }
        public async Task<RoomSpaceResponse> UpdateRoomSpace(Guid id, RoomSpaceRequest roomSpaceRequest)
        {
            var roomSpace = await _dbContext.RoomSpaces.FirstAsync(b => b.Id == id);
            if (roomSpace == null) throw new ObjectNotFoundException("RoomSpace not found");
            roomSpace.RoomSpaceName = roomSpaceRequest.RoomSpaceName;
            roomSpace.Description = roomSpaceRequest.Description;
            roomSpace.setCommonUpdate(UserSession.GetUserId());
            _dbContext.RoomSpaces.Update(roomSpace);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<RoomSpaceResponse>(roomSpace);
        }
    }
}
