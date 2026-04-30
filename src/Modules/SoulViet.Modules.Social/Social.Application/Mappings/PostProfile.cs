using AutoMapper;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Features.PostComments.Results;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Mappings;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostResponse>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.OriginalPost, opt => opt.MapFrom(src => src.OriginalPost));
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media));
        CreateMap<PostMedia, MediaItemDto>();
        CreateMap<PostMedia, MediaItemResponse>();
        CreateMap<PostComment, PostCommentResponse>();
    }
}
