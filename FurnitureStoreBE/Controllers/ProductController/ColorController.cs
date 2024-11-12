using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.ProductService.ColorService;
using FurnitureStoreBE.Services.ProductService.ColorService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ProductController
{
    [ApiController]
    [Route(Routes.COLOR)]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;
        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetColors([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _colorService.GetAllColors(pageInfo), (int)HttpStatusCode.OK, "Get Color successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateColor([FromForm] ColorRequest colorRequest)
        {
            return new SuccessfulResponse<object>(await _colorService.CreateColor(colorRequest), (int)HttpStatusCode.Created, "Color created successfully").GetResponse();
        }
        [HttpPut("{colorId}")]
        public async Task<IActionResult> UpdateColor(Guid colorId, [FromForm] ColorRequest colorRequest)
        {
            return new SuccessfulResponse<object>(await _colorService.UpdateColor(colorId, colorRequest), (int)HttpStatusCode.OK, "Color modified successfully").GetResponse();
        }
        [HttpDelete("{colorId}")]
        public async Task<IActionResult> DeleteColor(Guid colorId)
        {
            await _colorService.DeleteColor(colorId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Color deleted successfully").GetResponse();

        }
    }
}
