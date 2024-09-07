using AutoMapper;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;
namespace FurnitureStoreBE.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AspNetTypeClaims, TypeClaimsReponse>();
        }
    }
}
