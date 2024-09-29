using FurnitureStoreBE.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStoreBE.DTOs.Response.CouponResponse
{
    public class CouponResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ImageSource { get; set; }
        public string Description { get; set; }
        public long Quantity { get; set; }
        public long UsageCount { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ECouponType { get; set; }
        public string ECouponStatus { get; set; }
    }
}
