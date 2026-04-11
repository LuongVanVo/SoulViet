using SoulViet.Modules.Social.Social.Domain.Entities;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Post post, CancellationToken cancellationToken);
    void Update(Post post);
    Task SoftDeleteAsync(Post post, CancellationToken cancellationToken);
}
