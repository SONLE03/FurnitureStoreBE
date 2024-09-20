using AutoMapper;
using FurnitureStoreBE.DTOs.Response.BrandResponse;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;
namespace FurnitureStoreBE.Mapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<AspNetTypeClaims, TypeClaimsResponse>();
            CreateMap<User,  UserResponse>();
            CreateMap<Address, AddressResponse>();
            CreateMap<Brand, BrandResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));


        }
    }
}
