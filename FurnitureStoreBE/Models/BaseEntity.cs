using NuGet.Packaging.Signing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace FurnitureStoreBE.Models
{
    public class BaseEntity
    {
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "timestamp without time zone")]
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
            return DateTime.Now;
        }
    }
}
