using FurnitureStoreBE.Data;
using FurnitureStoreBE.DTOs.Response.AnalyticsResponse;
using FurnitureStoreBE.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace FurnitureStoreBE.Services.AnalyticsService
{
    public class AnalysisServiceImp : IAnalysisService
    {
        private readonly ApplicationDBContext _dbContext;
        public AnalysisServiceImp(ApplicationDBContext applicationDBContext)
        {
            _dbContext = applicationDBContext;
        }
        public async Task<List<OrderAnalyticData>> OrderAnalyticData(DateTime startDate, DateTime endDate)
        {
            // Validate input dates
            if (startDate == default || endDate == default)
                throw new ArgumentException("Start date and end date are required");

            // Ensure that the start date is before the end date
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date");

            // Calculate the difference in days between start and end date
            int diffDays = (endDate - startDate).Days;

            // Fetch analytics based on the difference in days
            if (diffDays < 30)
            {
                return await GetOrderAnalyticsByDay(startDate, endDate);
            }
            else if (diffDays < 180)
            {
                return await GetOrderAnalyticsByWeek(startDate, endDate);
            }
            else
            {
                return await GetOrderAnalyticsByMonth(startDate, endDate);
            }
        }
        private int GetWeekOfYear(DateTime date)
        {
            var dayOfYear = date.DayOfYear;
            return (int)Math.Ceiling(dayOfYear / 7.0);
        }
        public async Task<List<OrderAnalyticData>> GetOrderAnalyticsByDay(DateTime startDate, DateTime endDate)
        {
            var orderData = await _dbContext.OrderItems
               .Include(o => o.Order)
               .Where(o => o.Order.CreatedDate >= startDate && o.Order.CreatedDate <= endDate)
               .Select(o => new
               {
                   o.ProductId,
                   o.Product,
                   o.Quantity,
                   o.Order.CreatedDate,
                   o.Order.Total,
               })
               .ToListAsync();

            // Group the data by day
            var groupedData = orderData
                .GroupBy(o => o.CreatedDate.Value.Date)
                .Select(g => new OrderAnalyticData
                {
                    Key = g.Key.ToString("yyyy-MM-dd"),
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    TotalProductsSold = g.Sum(o => o.Quantity),  // Sum quantities of order items
                    ProductAnalyticDatas = g
                        .GroupBy(oi => oi.ProductId) // Group by ProductId
                        .Select(oiGroup => new ProductAnalyticData
                        {
                            // Null check for Product before accessing Name
                            ProductName = oiGroup.FirstOrDefault()?.Product?.ProductName ?? "Unknown Product", // Default to "Unknown Product" if Product is null
                            TotalProductSold = oiGroup.Sum(oi => oi.Quantity) // Summing quantities for each product
                        })
                        .Take(10)
                        .OrderByDescending(p => p.TotalProductSold)
                        .ToList()
                })
                .OrderBy(data => data.Key)
                .ToList();

            return groupedData;
        }
        private async Task<List<OrderAnalyticData>> GetOrderAnalyticsByWeek(DateTime startDate, DateTime endDate)
        {
             var orderData = await _dbContext.OrderItems
               .Include(o => o.Order)
               .Where(o => o.Order.CreatedDate >= startDate && o.Order.CreatedDate <= endDate)
               .Select(o => new
               {
                   o.ProductId,
                   o.Product,
                   o.Quantity,
                   o.Order.CreatedDate,
                   o.Order.Total,
               })
               .ToListAsync();
            var groupedData = orderData
                .GroupBy(o => new
                {
                    Week = GetWeekOfYear(o.CreatedDate.Value),
                    Year = o.CreatedDate.Value.Year
                })
                .Select(g => new OrderAnalyticData
                {
                    Key = $"Week {g.Key.Week} - {g.Key.Year}",
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    TotalProductsSold = g.Sum(o => o.Quantity),
                    ProductAnalyticDatas = g
                        .GroupBy(oi => oi.ProductId) // Group by ProductId
                        .Select(oiGroup => new ProductAnalyticData
                        {
                            // Null check for Product before accessing Name
                            ProductName = oiGroup.FirstOrDefault()?.Product?.ProductName ?? "Unknown Product", // Default to "Unknown Product" if Product is null
                            TotalProductSold = oiGroup.Sum(oi => oi.Quantity) // Summing quantities for each product
                        })
                        .Take(10)
                        .OrderByDescending(p => p.TotalProductSold)
                        .ToList()
                })
                .OrderBy(data => data.Key)
                .ToList();

            return groupedData;
        }

        private async Task<List<OrderAnalyticData>> GetOrderAnalyticsByMonth(DateTime startDate, DateTime endDate)
        {
            var orderData = await _dbContext.OrderItems
              .Include(o => o.Order)
              .Where(o => o.Order.CreatedDate >= startDate && o.Order.CreatedDate <= endDate)
              .Select(o => new
              {
                  o.ProductId,
                  o.Product,
                  o.Quantity,
                  o.Order.CreatedDate,
                  o.Order.Total,
              })
              .ToListAsync();
            var groupedData = orderData
                .GroupBy(o => new
                {
                    Month = o.CreatedDate.Value.Month,
                    Year = o.CreatedDate.Value.Year
                })
                .Select(g => new OrderAnalyticData
                {
                    Key = $"Month {g.Key.Month} - {g.Key.Year}",
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    TotalProductsSold = g.Sum(o => o.Quantity),
                    ProductAnalyticDatas = g
                        .GroupBy(oi => oi.ProductId) // Group by ProductId
                        .Select(oiGroup => new ProductAnalyticData
                        {
                            // Null check for Product before accessing Name
                            ProductName = oiGroup.FirstOrDefault()?.Product?.ProductName ?? "Unknown Product", // Default to "Unknown Product" if Product is null
                            TotalProductSold = oiGroup.Sum(oi => oi.Quantity) // Summing quantities for each product
                        })
                        .Take(10)
                        .OrderByDescending(p => p.TotalProductSold)
                        .ToList()
                })
                .OrderBy(data => data.Key)
                .ToList();

            return groupedData;
        }


        public async Task<SummaryAnalytics> Summary()
        {
            var summaryAnalytics = new SummaryAnalytics();
            summaryAnalytics.TotalProducts = await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .CountAsync();
            summaryAnalytics.TotalCustomers = await _dbContext.Users
                .Where(p => p.IsDeleted == false && p.Role == ERole.Customer.ToString())
                .CountAsync();
            summaryAnalytics.TotalOrders = await _dbContext.Orders
                .Where(p => p.OrderStatus == EOrderStatus.Completed)
                .CountAsync();
            summaryAnalytics.TotalRevenue = await _dbContext.Orders
                .Where(p => p.OrderStatus == EOrderStatus.Completed)
                .SumAsync(p => p.Total);
            return summaryAnalytics;
        }
    }
}
