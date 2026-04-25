using AutoMapper;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Mappings;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostDto>();
        CreateMap<Post, PostResponse>();
        CreateMap<PostMedia, MediaItemDto>();
        CreateMap<PostMedia, MediaItemResponse>();
    }
}
