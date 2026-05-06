using AutoMapper;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Mappings;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<TouristAttraction, MapMarkerDto>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))  // Y là Vĩ độ (Lat)
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X)) // X là Kinh độ (Lng)
            .ForMember(dest => dest.MarkerType, opt => opt.MapFrom(src => "TouristAttraction"))
            .ForMember(dest => dest.SubType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Media.MainImage))
            .ForMember(dest => dest.PriceInfo, opt => opt.MapFrom(src => src.ReferencePrice));

        CreateMap<Accommodation, MapMarkerDto>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
            .ForMember(dest => dest.MarkerType, opt => opt.MapFrom(src => "Accommodation"))
            .ForMember(dest => dest.SubType, opt => opt.MapFrom(src => src.Type.ToString())) // Enum to string
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Media.MainImage))
            .ForMember(dest => dest.PriceInfo, opt => opt.MapFrom(src => src.PriceSegment));

        CreateMap<TouristAttraction, TouristAttractionDetailDto>()
            // Xử lý tọa độ từ PostGIS
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))

            // Làm phẳng (Flatten) Category và Province
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.CategoryIconUrl, opt => opt.MapFrom(src => src.Category != null ? src.Category.IconUrl : string.Empty))
            .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province != null ? src.Province.Name : string.Empty))

            // Làm phẳng MediaInfo
            .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.Media.MainImage))
            .ForMember(dest => dest.LandImages, opt => opt.MapFrom(src => src.Media.LandImages))
            .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Media.VideoUrl))

            // Ép kiểu Enum sang String cho UI dễ hiển thị
            .ForMember(dest => dest.VibeTag, opt => opt.MapFrom(src => src.VibeTag.ToString()));

        CreateMap<Accommodation, AccommodationDetailDto>()
            // Đảo ngược tọa độ X/Y của PostGIS thành Lng/Lat
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))

            // Làm phẳng Province
            .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Province != null ? src.Province.Name : string.Empty))

            // Ép Enum sang Chuỗi
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.VibeTag, opt => opt.MapFrom(src => src.VibeTag.ToString()))

            // Làm phẳng MediaInfo
            .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.Media.MainImage))
            .ForMember(dest => dest.LandImages, opt => opt.MapFrom(src => src.Media.LandImages))
            .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Media.VideoUrl));
    }
}