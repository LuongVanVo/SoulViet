using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;

    public GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _mapper = mapper;
    }

    public async Task<PostResponse> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException($"Post with id '{request.Id}' was not found.");
        }

        var response = _mapper.Map<PostResponse>(post);
        response.Success = true;
        response.Message = "Post retrieved successfully.";
        return response;
    }
}

