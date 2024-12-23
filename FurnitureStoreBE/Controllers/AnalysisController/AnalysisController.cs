﻿using FurnitureStoreBE.Constants;
using FurnitureStoreBE.Services.AnalyticsService;
using FurnitureStoreBE.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FurnitureStoreBE.Controllers.AnalyticsController
{
    [ApiController]
    [Route(Routes.ANALYSIS)]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analysisService;
        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }
        [HttpGet("summary")]
        public async Task<IActionResult> OrderAnalyticDataSummary()
        {
            return new SuccessfulResponse<object>(await _analysisService.Summary(), (int)HttpStatusCode.OK, "Get data successfully").GetResponse();
        }
        [HttpGet("order-analytics")]
        public async Task<IActionResult> OrderAnalyticDataByMonth([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return new SuccessfulResponse<object>(await _analysisService.OrderAnalyticData(startDate, endDate), (int)HttpStatusCode.OK, "Get data successfully").GetResponse();
        }
    }
}
