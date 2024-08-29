using NuGet.Packaging.Signing;
using System;
using System.Reflection.Metadata;

namespace FurnitureStoreBE.Models
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime? DeleteDate { get; set; }

        public void setCommonCreate(Guid currentLoginId)
        {
            this.CreatedDate = resultTimestamp();
            this.CreatedBy = currentLoginId;
            this.UpdatedDate = resultTimestamp();
            this.UpdatedBy = currentLoginId;
        }
        public void setCommonUpdate(Guid currentLoginId)
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
