using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;

namespace FurnitureStoreBE.Services.ProductService.FurnitureTypeService
{
    public interface IFurnitureTypeService
    {
        Task<PaginatedList<FurnitureTypeResponse>> GetAllFurnitureTypes(PageInfo pageInfo);
        Task<FurnitureTypeResponse> CreateFurnitureType(FurnitureTypeRequest furnitureTypeRequest);
        Task<FurnitureTypeResponse> UpdateFurnitureType(Guid id, FurnitureTypeRequest furnitureTypeRequest);
        Task DeleteFurnitureType(Guid id);
        Task ChangeFurnitureTypeImage(Guid id, IFormFile formFile);
    }
}
