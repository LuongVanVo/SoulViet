using SoulViet.Shared.Domain.Enums;
using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Exceptions;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using SoulViet.Modules.Social.Social.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Commands.UpdatePost;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, PostResponse>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PostResponse> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException($"Post with id '{request.Id}' was not found.");
        }

        if (post.UserId != request.UserId)
        {
            throw new ForbiddenException("You are not allowed to update this post.");
        }

        post.Content = request.Content;
        post.VibeTag = request.VibeTag;
        post.CheckinLocationId = request.CheckinLocationId;
        post.AspectRatio = request.AspectRatio;
        post.TaggedProductIds = request.TaggedProductIds ?? new List<Guid>();
        
        if (request.Media != null)
        {
            var requestedUrls = request.Media.Where(m => !string.IsNullOrWhiteSpace(m.Url)).Select(m => m.Url).ToList();
            var mediaToRemove = post.Media.Where(m => !requestedUrls.Contains(m.Url)).ToList();
            
            if (mediaToRemove.Any())
            {
                foreach (var m in mediaToRemove)
                {
                    post.Media.Remove(m);
                }
            }

            foreach (var m in request.Media)
            {
                if (string.IsNullOrWhiteSpace(m.Url)) continue;

                var existingMedia = post.Media.FirstOrDefault(x => x.Url == m.Url);
                if (existingMedia != null)
                {
                    existingMedia.SortOrder = m.SortOrder;
                    existingMedia.MediaType = m.MediaType;
                    existingMedia.ObjectKey = string.IsNullOrWhiteSpace(m.ObjectKey) ? existingMedia.ObjectKey : m.ObjectKey;
                    existingMedia.Width = m.Width;
                    existingMedia.Height = m.Height;
                    existingMedia.FileSizeBytes = m.FileSizeBytes;
                }
                else
                {
                    // Add new
                    post.Media.Add(new PostMedia
                    {
                        Id = Guid.Empty, 
                        PostId = post.Id,
                        Url = m.Url,
                        MediaType = m.MediaType,
                        ObjectKey = m.ObjectKey ?? string.Empty,
                        Width = m.Width,
                        Height = m.Height,
                        FileSizeBytes = m.FileSizeBytes,
                        SortOrder = m.SortOrder
                    });
                }
            }
        }
        else
        {
            if (post.Media.Any())
            {
                post.Media.Clear();
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<PostResponse>(post);
        response.Success = true;
        response.Message = "Post updated successfully.";
        return response;
    }
}
