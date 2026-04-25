using AutoMapper;
using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using System.Linq;

namespace SoulViet.Modules.Social.Social.Application.Features.Posts.Queries.GetPostByUserId
{
    public class GetPostsByUserIdQueryHandler : IRequestHandler<GetPostByUserIdQuery, Connection<PostResponse>?>
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public GetPostsByUserIdQueryHandler(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<Connection<PostResponse>?> Handle(GetPostByUserIdQuery request, CancellationToken cancellationToken)
        {
            Guid? cursorId = null;
            DateTime? cursorCreatedAt = null;

            var decodedCursor = CursorHelper.Decode(request.After);
            if (decodedCursor.HasValue)
            {
                if (string.Equals(decodedCursor.Value.SortBy, request.SortBy, StringComparison.OrdinalIgnoreCase))
                {
                    cursorId = decodedCursor.Value.Id;
                    cursorCreatedAt = decodedCursor.Value.CreatedAt;
                }
            }

            var (items, totalCount) = await _postRepository.GetPagedByUserIdAsync(
                request.UserId,
                request.SortBy.ToLowerInvariant(),
                cursorId,
                cursorCreatedAt,
                request.First,
                cancellationToken);

            var hasNextPage = items.Count > request.First;
            var postsToReturn = items.Take(request.First).ToList();

            var edges = postsToReturn.Select(p => new Edge<PostResponse>
            {
                Cursor = CursorHelper.Encode(p.Id, p.CreatedAt, request.SortBy, null),
                Node = _mapper.Map<PostResponse>(p)
            }).ToList();

            var pageInfo = new PageInfo
            {
                HasNextPage = hasNextPage,
                HasPreviousPage = false,
                StartCursor = edges.FirstOrDefault()?.Cursor,
                EndCursor = edges.LastOrDefault()?.Cursor
            };

            return new Connection<PostResponse>
            {
                Edges = edges,
                PageInfo = pageInfo,
                TotalCount = totalCount
            };
        }
    }
}
