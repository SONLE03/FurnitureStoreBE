using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Response.AnalyticsResponse;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStoreBE.Services.AnalyticsService
{
    public class AnalysisServiceImp : IAnalysisService
    {
        private readonly ApplicationDBContext _dbContext;
        public AnalysisServiceImp(ApplicationDBContext applicationDBContext)
        {
            _dbContext = applicationDBContext;
        }
        public async Task<List<OrderAnalyticData>> OrderAnalyticDataByMonth(DateTime startDate, DateTime endDate)
        {
            var report = await _dbContext.OrderItems
                .Where(o => o.Order.OrderStatus == Enums.EOrderStatus.Completed)
                .GroupBy(o => new
                {
                    Year = o.Order.CompletedAt.Value.Year,
                    Month = o.Order.CompletedAt.Value.Month
                })
                .Select(g => new OrderAnalyticData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalOrders = g.Select(o => o.OrderId).Distinct().Count(),
                    TotalRevenue = g.Sum(o => o.Order.Total)
                })
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ToListAsync();

            return report;
        }

    }
}
