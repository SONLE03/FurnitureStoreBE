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
            //var report = await _dbContext.OrderItems
            //    .Include(o => o.Order)
            //    .Where(o => o.Order.OrderStatus == Enums.EOrderStatus.Completed)
            //    .GroupBy(o => new
            //    {
            //        Year = o.Order.CompletedAt.Value.Year,
            //        Month = o.Order.CompletedAt.Value.Month
            //    })
            //    .Select(g => new OrderAnalyticData
            //    {
            //        Year = g.Key.Year,
            //        Month = g.Key.Month,
            //        TotalOrders = g.Select(o => o.OrderId).Distinct().Count(),
            //        TotalRevenue = g.Sum(o => o.Order.Total)
            //    })
            //    .OrderBy(r => r.Year)
            //    .ThenBy(r => r.Month)
            //    .ToListAsync();                
            //return report;
            return await _dbContext.Orders
    .Where(o => o.OrderStatus == Enums.EOrderStatus.Completed && o.CompletedAt != null)
    .Where(o => o.CompletedAt >= startDate && o.CompletedAt <= endDate)
    .GroupBy(o => new { o.CompletedAt.Value.Year, o.CompletedAt.Value.Month })
    .Select(g => new OrderAnalyticData
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        TotalOrders = g.Count(), // Không cần Distinct() nếu đảm bảo đơn hàng là duy nhất
        TotalRevenue = g.Sum(o => o.Total) // Kiểm tra cột `Total` hợp lệ
    })
    .OrderBy(data => data.Year)
    .ThenBy(data => data.Month)
    .ToListAsync();

        }
    }
}
