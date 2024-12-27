using FurnitureStoreBE.Common.Pagination;
using FurnitureStoreBE.Constants;
using FurnitureStoreBE.DTOs.Request.BrandRequest;
using FurnitureStoreBE.DTOs.Request.ImportRequest;
using FurnitureStoreBE.Services.ImportService;
using FurnitureStoreBE.Services.ProductService.BrandService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.ImportController
{
    [ApiController]
    [Route(Routes.IMPORT)]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;
        public ImportController(IImportService importService)
        {
            _importService = importService;
        }
        [HttpGet]
        public async Task<IActionResult> GetImports([FromQuery] PageInfo pageInfo)
        {
            return new SuccessfulResponse<object>(await _importService.GetImports(pageInfo), (int)HttpStatusCode.OK, "Get imports successfully").GetResponse();
        }
        [HttpGet("{importId}")]
        public async Task<IActionResult> GetImportById(Guid importId)
        {
            return new SuccessfulResponse<object>(await _importService.GetImportById(importId), (int)HttpStatusCode.OK, "Get import successfully").GetResponse();
        }
        [HttpPost()]
        public async Task<IActionResult> CreateImport([FromBody] List<ImportRequest> importRequests)
        {
            if (importRequests == null || !importRequests.Any())
            {
                return BadRequest("Import request cannot be null or empty.");
            }
            return new SuccessfulResponse<object>(await _importService.CreateImport(importRequests), (int)HttpStatusCode.Created, "Import invoice created successfully").GetResponse();
        }
    }
}
