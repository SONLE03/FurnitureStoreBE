using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Request.ProductRequest;


namespace FurnitureStoreBE.Services.ProductService.RoomSpaceService
{
    public interface IRoomSpaceService
    {
        Task<PaginatedList<RoomSpaceResponse>> GetAllRoomSpaces(PageInfo pageInfo);
        Task<RoomSpaceResponse> CreateRoomSpace(RoomSpaceRequest roomSpaceRequest, IFormFile formFile);
        Task<RoomSpaceResponse> UpdateRoomSpace(Guid id, RoomSpaceRequest RoomSpaceRequest);
        Task DeleteRoomSpace(Guid id);
        Task ChangeRoomSpaceImage(Guid id, IFormFile formFile);
    }
}
