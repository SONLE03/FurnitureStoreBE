using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.DTOs.Response.ProductResponse
{
    public class ColorResponse
    {
        public Guid Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}
