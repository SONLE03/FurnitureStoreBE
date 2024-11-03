namespace FurnitureStoreBE.DTOs.Response.AnalyticsResponse
{
    public class SummaryAnalytics {
        public decimal TotalRevenue { set; get; }
        public long TotalOrders { set; get; }
        public long TotalCustomers { set; get; }
        public long TotalProducts { set; get; }
    }
    public class OrderAnalyticData
    {
        public int Year { get; set; } // Make these non-nullable if possible
        public int Month { get; set; } // Same here
        public long TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
