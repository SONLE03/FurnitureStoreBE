using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.Services.BrandService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("brand")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet]
        public async Task<IActionResult> GetBrands([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _brandService.GetAllBrands(pageInfo), (int)HttpStatusCode.OK, "Get brand successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateBrand([FromForm] BrandRequest brandRequest, [FromForm] IFormFile brandImage)
        {
            return new SuccessfulResponse<object>(await _brandService.CreateBrand(brandRequest, brandImage), (int)HttpStatusCode.Created, "Brand created successfully").GetResponse();
        }
        [HttpPut("{brandId}")]
        public async Task<IActionResult> UpdateBrand(Guid brandId, [FromBody] BrandRequest brandRequest)
        {
            return new SuccessfulResponse<object>(await _brandService.UpdateBrand(brandId, brandRequest), (int)HttpStatusCode.OK, "Brand modified successfully").GetResponse();
        }
        [HttpPost("image/{brandId}")]
        public async Task<IActionResult> ChangeBrandImage(Guid brandId, [FromForm] IFormFile brandImage)
        {
            await _brandService.ChangeBrandImage(brandId, brandImage);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Brand image changed successfully").GetResponse();
        }
        [HttpDelete("{brandId}")]
        public async Task<IActionResult> DeleteBrand(Guid brandId)
        {
            await _brandService.DeleteBrand(brandId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Brand deleted successfully").GetResponse();

        }
    }
}
