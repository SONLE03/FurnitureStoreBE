using NuGet.Packaging.Signing;
using System;
using System.Reflection.Metadata;

namespace FurnitureStoreBE.Models
{
    public class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeleteDate { get; set; }

        public void setCommonCreate(string currentLoginId)
        {
            this.CreatedDate = resultTimestamp();
            this.CreatedBy = currentLoginId;
            this.UpdatedDate = resultTimestamp();
            this.UpdatedBy = currentLoginId;
        }
        public void setCommonUpdate(string currentLoginId)
        {
            this.UpdatedDate = resultTimestamp();
            this.UpdatedBy = currentLoginId;
        }

        public static DateTime resultTimestamp()
        {
            return DateTime.UtcNow;
        }
    }
}
