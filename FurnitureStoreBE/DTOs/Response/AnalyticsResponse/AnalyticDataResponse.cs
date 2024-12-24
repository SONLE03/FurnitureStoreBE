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
        //public int Year { get; set; } // Make these non-nullable if possible
        //public int Month { get; set; } // Same here
        public string Key { get; set; }
        public int TotalOrders { get; set; }
        public long TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProductAnalyticData> ProductAnalyticDatas { get; set; }
    }
    public class ProductAnalyticData
    {
        public string ProductName { get; set; }
        public long TotalProductSold { get; set; }
    }
}
