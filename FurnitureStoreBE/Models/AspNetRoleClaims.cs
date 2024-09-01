using Microsoft.AspNetCore.Identity;

namespace FurnitureStoreBE.Models
{
    public class AspNetRoleClaims<TKey> : IdentityRoleClaim<TKey> where TKey : IEquatable<TKey>
    {
        // Add a foreign key property to the AspNetTypeClaims entity
        public int? AspNetTypeClaimsId { get; set; }

        // Navigation property for related AspNetTypeClaims entry
        public AspNetTypeClaims AspNetTypeClaims { get; set; }
    }
}
