using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Shared.Domain.Enums;
using SoulViet.Modules.Social.Social.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PostResponse> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            Media = request.Media.Select(m => new PostMedia
            {
                Url = m.Url,
                MediaType = m.MediaType,
                ObjectKey = m.ObjectKey,
                Width = m.Width,
                Height = m.Height,
                FileSizeBytes = m.FileSizeBytes,
                SortOrder = m.SortOrder
            }).ToList(),
            TaggedProductIds = request.TaggedProductIds ?? new List<Guid>(),
            VibeTag = request.VibeTag,
            CheckinLocationId = request.CheckinLocationId,
            LikesCount = 0,
            CommentsCount = 0,
            SharesCount = 0,
            Status = PostStatus.Published
        };

        await _postRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<PostResponse>(post);
        response.Success = true;
        response.Message = "Post created successfully.";
        return response;
    }
}
