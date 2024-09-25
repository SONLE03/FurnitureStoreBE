using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Response.UserResponse
{
    public class AddressResponse
    {
        public Guid Id { get; set; }
        public string Province { get; set; }
        public string District { get; set; } 
        public string Ward { get; set; }  
        public string SpecificAddress { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }
    }
}
