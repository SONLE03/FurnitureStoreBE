using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.MaterialService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.MATERIAL)]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMaterials([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _materialService.GetAllMaterials(pageInfo), (int)HttpStatusCode.OK, "Get Material successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateMaterial([FromForm] MaterialRequest materialRequest)
        {
            return new SuccessfulResponse<object>(await _materialService.CreateMaterial(materialRequest), (int)HttpStatusCode.Created, "Material created successfully").GetResponse();
        }
        [HttpPut("{materialId}")]
        public async Task<IActionResult> UpdateMaterial(Guid materialId, [FromForm] MaterialRequest materialRequest)
        {
            return new SuccessfulResponse<object>(await _materialService.UpdateMaterial(materialId, materialRequest), (int)HttpStatusCode.OK, "Material modified successfully").GetResponse();
        }
        [HttpPost("image/{materialId}")]
        public async Task<IActionResult> ChangeMaterialImage(Guid materialId, [FromForm] IFormFile materialImage)
        {
            await _materialService.ChangeMaterialImage(materialId, materialImage);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Material image changed successfully").GetResponse();
        }
        [HttpDelete("{materialId}")]
        public async Task<IActionResult> DeleteMaterial(Guid materialId)
        {
            await _materialService.DeleteMaterial(materialId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Material deleted successfully").GetResponse();

        }
    }
}
