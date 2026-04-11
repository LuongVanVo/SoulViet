using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IPostCommentRepository
    {
        Task<List<PostComment>> GetPostCommentsByPostIdAsync(Guid postId, CancellationToken cancellationToken);
        Task<PostComment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(PostComment comment, CancellationToken cancellationToken);
        void Update(PostComment comment);
        Task Delete(PostComment comment, CancellationToken cancellationToken);
    }
}
