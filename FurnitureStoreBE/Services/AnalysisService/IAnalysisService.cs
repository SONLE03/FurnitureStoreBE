using FurnitureStoreBE.DTOs.Response.AnalyticsResponse;

namespace FurnitureStoreBE.Services.AnalyticsService
{
    public interface IAnalysisService
    {
        Task<SummaryAnalytics> Summary();
        Task<List<OrderAnalyticData>> OrderAnalyticData(DateTime startDate, DateTime endDate);

    }
}
