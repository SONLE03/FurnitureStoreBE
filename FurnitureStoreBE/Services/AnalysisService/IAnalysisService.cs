using FurnitureStoreBE.DTOs.Response.AnalyticsResponse;

namespace FurnitureStoreBE.Services.AnalyticsService
{
    public interface IAnalysisService
    {
        Task<List<OrderAnalyticData>> OrderAnalyticDataByMonth(DateTime startDate, DateTime endDate);

    }
}
