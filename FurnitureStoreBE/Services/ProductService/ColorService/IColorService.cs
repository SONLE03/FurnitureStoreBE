using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Common;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.DTOs.Response.ProductResponse;

namespace FurnitureStoreBE.Services.ProductService.ColorService
{
    public interface IColorService
    {
        Task<PaginatedList<ColorResponse>> GetAllColors(PageInfo pageInfo);
        Task<ColorResponse> CreateColor(ColorRequest colorRequest);
        Task<ColorResponse> UpdateColor(Guid id, ColorRequest colorRequest);
        Task DeleteColor(Guid id);
    }
}
