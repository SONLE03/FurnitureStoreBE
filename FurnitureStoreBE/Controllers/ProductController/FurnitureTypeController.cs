using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.FurnitureTypeService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.FURNITURETYPE)]
    public class FurnitureTypeController : ControllerBase
    {
        private readonly IFurnitureTypeService _furnitureTypeService;
        public FurnitureTypeController(IFurnitureTypeService furnitureTypeService)
        {
            _furnitureTypeService = furnitureTypeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetFurnitureTypes([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _furnitureTypeService.GetAllFurnitureTypes(pageInfo), (int)HttpStatusCode.OK, "Get FurnitureType successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateFurnitureType([FromForm] FurnitureTypeRequest furnitureTypeRequest)
        {
            return new SuccessfulResponse<object>(await _furnitureTypeService.CreateFurnitureType(furnitureTypeRequest), (int)HttpStatusCode.Created, "FurnitureType created successfully").GetResponse();
        }
        [HttpPut("{furnitureTypeId}")]
        public async Task<IActionResult> UpdateFurnitureType(Guid furnitureTypeId, [FromForm] FurnitureTypeRequest furnitureTypeRequest)
        {
            return new SuccessfulResponse<object>(await _furnitureTypeService.UpdateFurnitureType(furnitureTypeId, furnitureTypeRequest), (int)HttpStatusCode.OK, "FurnitureType modified successfully").GetResponse();
        }
        [HttpPost("image/{furnitureTypeId}")]
        public async Task<IActionResult> ChangeFurnitureTypeImage(Guid furnitureTypeId, [FromForm] IFormFile furnitureTypeImage)
        {
            await _furnitureTypeService.ChangeFurnitureTypeImage(furnitureTypeId, furnitureTypeImage);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "FurnitureType image changed successfully").GetResponse();
        }
        [HttpDelete("{furnitureTypeId}")]
        public async Task<IActionResult> DeleteFurnitureType(Guid furnitureTypeId)
        {
            await _furnitureTypeService.DeleteFurnitureType(furnitureTypeId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "FurnitureType deleted successfully").GetResponse();

        }
    }
}
