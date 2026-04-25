using MediatR;
using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Application.Features.PostComments.Queries.GetPostCommentById
{
    public class GetPostCommentByPostIdHandler : IRequestHandler<GetPostCommentByPostIdQuery, List<PostCommentDto>>
    {
        private readonly IPostCommentRepository _repository; 
        private readonly IUserService _userService; 

        public GetPostCommentByPostIdHandler(
            IPostCommentRepository repository, 
            IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        public async Task<List<PostCommentDto>> Handle(GetPostCommentByPostIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repository.GetPostCommentsByPostIdAsync(request.PostId, cancellationToken);
            
            if (!comments.Any()) return new List<PostCommentDto>();
            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var usersInfo = await _userService.GetUsersMinimalInfoAsync(userIds, cancellationToken);
            var commentDtos = comments.Select(c => new PostCommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                FullName = usersInfo.TryGetValue(c.UserId, out var user) ? user.FullName : "Ngu?i dùng ?n danh",
                AvatarUrl = usersInfo.TryGetValue(c.UserId, out var u2) ? u2.AvatarUrl : null,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                ParentCommentId = c.ParentCommentId,
                Replies = new List<PostCommentDto>() 
            }).ToList();

            var lookup = commentDtos.ToDictionary(c => c.Id);
            var rootComments = new List<PostCommentDto>();

            foreach (var dto in commentDtos)
            {
                if (dto.ParentCommentId.HasValue && lookup.TryGetValue(dto.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(dto);
                }
                else
                {
                    rootComments.Add(dto);
                }
            }

            return rootComments;
        }
    }
}
