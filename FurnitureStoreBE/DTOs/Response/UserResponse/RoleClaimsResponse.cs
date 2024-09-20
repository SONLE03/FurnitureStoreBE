namespace FurnitureStoreBE.DTOs.Response.UserResponse
{
    public class TypeClaimsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RoleClaimsResponse
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public int? AspNetTypeClaimsId { get; set; }
    }
    public class UserClaimsResponse
    {
        public int Id { get; set; }
        public string ClaimValue { get; set; }
    }
    public class ClaimsResult
    {
        public List<TypeClaimsResponse>? TypeClaims { get; set; } 
        public List<RoleClaimsResponse>? RoleClaims { get; set; }
    }
}
