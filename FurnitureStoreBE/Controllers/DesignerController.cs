﻿using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Request.ProductRequest;
using FurnitureStoreBE.Services.BrandService;
using FurnitureStoreBE.Services.DesignerService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("designer")]
    public class DesignerController : ControllerBase
    {
        private readonly IDesignerService _designerService;
        public DesignerController(IDesignerService designerService)
        {
            _designerService = designerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetDesigners([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _designerService.GetAllDesigners(pageInfo), (int)HttpStatusCode.OK, "Get designer successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateDesigner([FromForm] DesignerRequest designerRequest, [FromForm] IFormFile designerImage)
        {
            return new SuccessfulResponse<object>(await _designerService.CreateDesigner(designerRequest, designerImage), (int)HttpStatusCode.Created, "Designer created successfully").GetResponse();
        }
        [HttpPut("{designerId}")]
        public async Task<IActionResult> UpdateDesigner(Guid designerId, [FromBody] DesignerRequest designerRequest)
        {
            return new SuccessfulResponse<object>(await _designerService.UpdateDesigner(designerId, designerRequest), (int)HttpStatusCode.OK, "Designer modified successfully").GetResponse();
        }
        [HttpPost("image/{designerId}")]
        public async Task<IActionResult> ChangeDesignerImage(Guid designerId, [FromForm] IFormFile designerImage)
        {
            await _designerService.ChangeDesignerImage(designerId, designerImage);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Designer image changed successfully").GetResponse();
        }
        [HttpDelete("{designerId}")]
        public async Task<IActionResult> DeleteDesigner(Guid designerId)
        {
            await _designerService.DeleteDesigner(designerId);
            return new SuccessfulResponse<object>(null, (int)HttpStatusCode.OK, "Designer deleted successfully").GetResponse();

        }
    }
}
