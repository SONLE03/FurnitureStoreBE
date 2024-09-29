using AutoMapper;
using FurnitureStoreBE.DTOs.Response.OrderResponse;
using FurnitureStoreBE.DTOs.Response.BrandResponse;
using FurnitureStoreBE.DTOs.Response.ProductResponse;
using FurnitureStoreBE.DTOs.Response.UserResponse;
using FurnitureStoreBE.Models;
using Microsoft.CodeAnalysis;
using FurnitureStoreBE.DTOs.Response.CouponResponse;
namespace FurnitureStoreBE.Mapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<AspNetTypeClaims, TypeClaimsResponse>();
            CreateMap<User,  UserResponse>();
            CreateMap<Address, AddressResponse>();
            CreateMap<Color, ColorResponse>();
            CreateMap<Brand, BrandResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
            CreateMap<Designer, DesignerResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
            CreateMap<RoomSpace, RoomSpaceResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
            CreateMap<FurnitureType, FurnitureTypeResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
            CreateMap<Material, MaterialResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
            CreateMap<Category, CategoryResponse>().ForMember(dest => dest.ImageSource, opt => opt.MapFrom(src => src.Asset.URL));
         

            CreateMap<ProductVariant, ProductVariantResponse>()
               .ForMember(dest => dest.ColorName, otp => otp.MapFrom(src => src.Color.ColorName))
               .ForMember(dest => dest.DisplayDimension, otp => otp.MapFrom(src => src.DisplayDimension))
               .ForMember(dest => dest.Quantity, otp => otp.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.Price, otp => otp.MapFrom(src => src.Price))
               .ForMember(dest => dest.ImageSource, otp => otp.MapFrom(src => src.Assets.Select(a => a.URL).ToList()));

            CreateMap<Product, ProductResponse>().ForMember(dest => dest.ImageSource, otp => otp.MapFrom(src => src.Asset.URL))
                .ForMember(dest => dest.Designers, otp => otp.MapFrom(src => src.Designers.Select(d => d.DesignerName)))
                .ForMember(dest => dest.Materials, otp => otp.MapFrom(src => src.Materials.Select(m => m.MaterialName)))
                .ForMember(dest => dest.CategoryName, otp => otp.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.BrandName, otp => otp.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.DisplayPrice, otp => otp.MapFrom(src => $"{src.MinPrice} - {src.MaxPrice}"))
                .ForMember(dest => dest.ProductVariants, otp => otp.MapFrom(src => src.ProductVariants));

            CreateMap<OrderItem, OrderItemResponse>().ForMember(dest => dest.ColorName, otp => otp.MapFrom(src => src.Color.ColorName))
                .ForMember(dest => dest.ProductName, otp => otp.MapFrom(src => src.Product.ProductName));

            CreateMap<Coupon, CouponResponse>()
                .ForMember(dest => dest.ImageSource, otp => otp.MapFrom(src => src.Asset.URL))
                .ForMember(dest => dest.ECouponStatus, otp => otp.MapFrom(src => src.ECouponStatus.ToString()))
                .ForMember(dest => dest.ECouponType, otp => otp.MapFrom(src => src.ECouponType.ToString()));
        }
    }
}
